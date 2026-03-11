# TradeOfferAction 深挖：发报价 / 撤回 / 接受 / 拆单逻辑

> 目标：把 Spider 版 Steam 转库存工具的交易动作层拆到可以独立重写
> 说明：基于现有 48 份逆向材料与前序笔记推导，方法签名需以后续反编译校正

---

## 一、TradeOfferAction 的定位

`TradeOfferAction` 不是总调度，也不是登录层。
它是整个系统里的 **交易原子动作层**：

- 拉取报价列表
- 发起报价
- 撤回报价
- 接受报价
- 读取报价详情
- 可能负责部分报价状态转换

一句话：

- `DataController` 决定 **什么时候发 / 接 / 撤**
- `TradeOfferAction` 决定 **交易动作本身怎么落地**

所以如果目标是“做一个一样的”，
`TradeOfferAction` 基本就是 **交易引擎的壳层接口**。

---

## 二、现有材料已经确认的事实

### 已确认
1. 类存在：`SteamInventoryManager.Actions.TradeOfferAction`
2. 已知方法名：
   - `GetTradeOffers(...)`
   - `SendTradeOffer(...)`
   - `CancelTradeOffer(...)`
   - 接受报价相关方法（但具体名可能混淆）
3. `DataController` 真正发货时，更高层调用的是：
   - `user.ArchiWebHandler.SendInventory(...)`
4. `DataController` 接受报价后，若需要确认，会转到：
   - `SteamGuardController.HandleTwoFactorAuthenticationConfirmations(true)`
5. 单个 trade offer 默认最多 **255 项**
6. 最多拆 **5 单**

### 这说明什么
- `TradeOfferAction` 不是完全裸 HTTP 拼接口，而是 **部分借助上层封装（ArchiWebHandler）**
- 它更像“把 Steam trade offer 动作收口到一个统一类里”
- 你后面重写时，不需要执着于原类名，重点是把交易动作参数和顺序复刻出来

---

## 三、最可能的方法结构

### 1. 获取报价列表
```csharp
Task<List<TradeOffer>> GetTradeOffersAsync(Account account)
// 或
List<TradeOffer> GetTradeOffers(Account account)
```

#### 作用
- 拉当前账号的 sent offers / received offers
- 更新 `account.SentTradeIDs` / `account.BeTradeOfferIDs`

#### 输出可能包括
- `OfferId`
- `State`
- `ItemsToGive`
- `ItemsToReceive`
- `PartnerSteamId`
- `RequiresConfirmation`

#### 可修改点
- 报价过滤（只保留 active offers）
- 只同步必要字段，减少性能消耗
- 给每个报价增加本地索引，便于后续重试/确认

---

### 2. 发送报价
```csharp
Task<string> SendTradeOfferAsync(Account sender, string tradeOfferUrl, List<InventoryItem> items)
// 或
string SendTradeOffer(Account sender, Account receiver, List<InventoryItem> items)
```

#### 输入依赖
- `sender.Session`
- `sender.TradeToken`
- `sender.TradeOfferUrl`
- `receiver.TradeOfferUrl` 或由 `SteamID + token` 拼出 URL
- `items`

#### 输出
- 成功：`offerId`
- 失败：空/null 或异常

#### 高概率内部步骤
1. 校验当前 Session 是否有效
2. 解析目标 `tradeOfferUrl`
3. 把 `items` 转成 Steam 报价格式
4. 发起报价请求
5. 返回 `offerId`
6. 更新 `account.SentTradeIDs`
7. 如需确认，等待确认层处理

---

### 3. 撤回报价
```csharp
Task<bool> CancelTradeOfferAsync(Account sender, string offerId)
```

#### 功能
- 对已经发出的报价执行取消
- 场景：
  - 主库号未接受
  - 超时
  - 发错单
  - 任务中止

#### 调用来源
`DataController` 在“待转号撤回已发报价”场景中会批量调用

---

### 4. 接受报价
```csharp
Task<bool> AcceptTradeOfferAsync(Account receiver, string offerId)
```

#### 功能
- 主库号接受某个报价
- 接受成功后如果需要移动端确认，再交给 `SteamGuardController`

#### 调用链
```text
DataController.AcceptBeTradeOffers(master)
  -> TradeOfferAction.AcceptTradeOffer(master, offerId)
  -> if need 2FA:
       SteamGuardController.HandleTwoFactorAuthenticationConfirmations(true)
```

---

### 5. 获取报价详情
```csharp
Task<TradeOfferDetail> GetTradeOfferDetailAsync(Account account, string offerId)
```

#### 用途
- 核对报价里的物品
- 判断是否是刚刚发送的报价
- 对确认列表进行匹配

这类方法如果存在，会非常有价值，因为它能让你把“发出报价”与“确认对应报价”精准关联起来。

---

## 四、最关键的“发报价最小参数集”

如果你后面要自己重写，这几个参数是必须的：

### 必需参数
1. **发送者登录态**
   - `Session/Cookies`
2. **目标交易地址**
   - `TradeOfferUrl`
   - 或 `SteamID + token`
3. **物品列表**
   - `assetid`
   - `appid`
   - `contextid`
   - 数量（大多为 1）
4. **消息文本**（可为空）
5. **报价来源类型**
   - sent / received / direct / by-token

### 也就是说
如果你能自己稳定提供：
- 登录态
- 目标地址
- 资产列表

那你就已经具备“自己发 Steam 报价”的最小条件。

---

## 五、拆单逻辑：这是复刻像不像原版的关键

### 已知规则
- 单笔报价最多 **255 项**
- 最多拆 **5 单**

### 推测原逻辑
```csharp
var chunks = SplitItems(items, maxPerOffer: 255, maxOffers: 5);
foreach (var chunk in chunks) {
    var offerId = SendTradeOffer(sender, receiver, chunk);
}
```

### 重点不是“能不能拆”，而是“怎么拆”
有三种常见拆法：

#### 1. 顺序拆
按库存返回顺序每 255 个切一次

#### 2. 按价值/类型拆
同类物品优先放同一单

#### 3. 按主库号剩余额度拆
如果主库号有限额，则先按额度切，再按 255 切

### 根据现有材料判断
Spider 版更像：
- 先由 `DataController` 做“主库号剩余额度分配”
- 再在 `TradeOfferAction` 内做 **255 item 技术性拆单**

也就是说：
- **业务分配在 DataController**
- **协议拆单在 TradeOfferAction**

这个边界非常重要，后面你重写时也最好这么分。

---

## 六、最可能的内部依赖：ArchiWebHandler

现有笔记中最关键的一条是：

> 实际 DataController 调的是 `user.ArchiWebHandler.SendInventory(...)`

这说明 `TradeOfferAction` 很可能不是直接底层调用 HTTP，而是使用了某种封装好的 Steam Web Handler。

### 这意味着什么
你后面复刻时有两条路：

#### 路线 A：继续走上层封装
- 自己找/复用类似 ASF 的 `ArchiWebHandler`
- 交易、库存、确认都走同一套会话体系

#### 路线 B：自己手写 Web 层
- 手动构造 tradeoffer 请求
- 手动带 cookie/session/token
- 自己处理返回值、错误、节流

### 建议
如果目标是 **尽快做出一个一样的**，优先路线 A。
因为原版明显更接近这条路线。

---

## 七、接受报价后的确认模式

### 已知
- 主库号接受报价后，如需要 2FA，会走：
  `HandleTwoFactorAuthenticationConfirmations(true)`

### 推测
这说明 `AcceptTradeOffer` 自身可能只是“网页接受动作”，
真正最终完成要靠：
- 接受请求成功
- 再扫确认列表
- 再做移动端确认

### 这给你的启发
以后自己重写时，不要把 `AcceptTradeOffer` 设计成“成功=彻底完成”。
更合理的是：

```csharp
class AcceptResult {
    bool AcceptedOnWeb;
    bool RequiresMobileConfirmation;
    bool Confirmed;
    string OfferId;
}
```

这样逻辑会更稳定。

---

## 八、撤回逻辑也不能简单理解成“删掉报价”

撤回动作实际上是：
- 扫描当前 sent offers
- 找到仍然 active 的单
- 逐个取消
- 更新本地 `SentTradeIDs`
- 可能写日志 / 更新 UI

### 如果你重写，建议把撤回分成两步
1. `QueryCancelableOffers(account)`
2. `CancelOffers(account, offers)`

这样你可以更容易实现：
- 只撤回超时报价
- 只撤回指定主库号的报价
- 中断任务时的批量清理

---

## 九、如果要做一个一样的，TradeOfferAction 最小复刻清单

### MVP 必须实现
1. 获取 sent / received offers
2. 发送报价
3. 接受报价
4. 撤回报价
5. 拆单（255 item）
6. 返回 `offerId`
7. 与确认层联动

### 第二阶段增强
1. 报价详情读取
2. 错误分类（网络/风控/会话失效）
3. 精准确认匹配
4. 报价幂等保护（避免重复发）
5. 自定义发货顺序

---

## 十、你如果自己重写，最合理的服务拆法

### 1. `OfferBuilder`
负责：
- 把库存项组装成报价请求体
- 处理 appid/contextid/assetid

### 2. `OfferChunker`
负责：
- 255 item 技术拆单
- 最多 5 单限制

### 3. `OfferSender`
负责：
- 真正发送报价
- 返回 `offerId`

### 4. `OfferReceiver`
负责：
- 接受报价
- 返回需要确认与否

### 5. `OfferCanceler`
负责：
- 撤回已发报价

### 6. `OfferQueryService`
负责：
- 拉 sent offers / received offers / detail

这样你后面就不需要死守原版 `TradeOfferAction` 类，而是可以做一个更干净的重写版。

---

## 十一、最关键的 4 个待验证问题

### 1. `SendTradeOffer(...)` 内部是否直接调用 `ArchiWebHandler.SendInventory(...)`？
如果是，说明 TradeOfferAction 只是浅封装。

### 2. `offerId` 是发送后直接返回，还是后续从报价列表里匹配出来？
这个影响确认链的实现方式。

### 3. 拆单后是顺序发，还是并行发？
这个影响风控与速率。

### 4. 撤回和接受是否共享同一个报价查询接口？
这决定你以后服务边界怎么切。

---

## 十二、当前最值钱的结论

现在最难的四层里，你已经基本拆清楚了三层半：

- `AccountAction`：登录链
- `SteamGuardController`：2FA/确认链
- `TradeOfferAction`：交易动作链
- `DataController`：总调度链

这意味着：

> 你现在已经不再是“在猜一个黑盒”，而是在逐层拆一个可以重写的系统。

换句话说，已经开始进入 **“架构复刻”** 阶段，而不是单纯逆向观察阶段。

---

## 十三、下一步最佳方向

现在继续往下，最值钱的是二选一：

### A. `Setting` 深挖
把所有配置项、默认值、UI 映射关系拆清楚

### B. `Account` 模型深挖
把运行时状态、持久化字段、导入来源、会话缓存边界拆清楚

### 我建议优先：`Setting`
因为如果目标是做出一个“一样的”，
配置层决定了：
- 线程数
- 模式
- 代理
- 默认筛选规则
- 自动记忆密码
- 路径布局

也就是：
**你已经快拆完“发动机”了，下一步应该拆“仪表盘和控制台”。**
