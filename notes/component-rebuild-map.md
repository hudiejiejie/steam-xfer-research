# Spider版 Steam转库存：组件重建图（继续深挖）

> 目标：不是只“看懂”，而是拆到可以按模块重写

---

## 一、如果要做出一个一样的，最少需要哪些模块？

把现有成品拆开后，本质上是 7 个模块：

1. **启动器 / 主界面层**
2. **配置层 (`Setting`)**
3. **账号导入层 (`LoadAccount` + `Account`)**
4. **登录与会话层 (`AccountAction` + `UserController`)**
5. **Steam Guard / maFile 层 (`SteamGuardController`)**
6. **交易层 (`TradeOfferAction`)**
7. **总调度层 (`DataController`)**

这 7 层里，真正决定“像不像原版”的，不是 UI，而是后 4 层。

---

## 二、TradeOfferAction 深挖

### 角色定位
`TradeOfferAction` 不是总调度，而是 **“交易动作原子层”**：
- 拉报价
- 发报价
- 撤回报价
- 接受报价
- 可能还负责读取报价详情

也就是说：
- `DataController` 决定 **什么时候做**
- `TradeOfferAction` 决定 **怎么做**

---

### 推测的核心方法

```csharp
Task<List<TradeOffer>> GetTradeOffersAsync(Account account)
Task<string> SendTradeOfferAsync(Account sender, string receiverTradeUrl, List<InventoryItem> items)
Task<bool> CancelTradeOfferAsync(Account sender, string offerId)
Task<bool> AcceptTradeOfferAsync(Account receiver, string offerId)
Task<TradeOfferDetail> GetTradeOfferDetailAsync(Account account, string offerId)
```

### 输入依赖
它至少依赖这些字段：
- `account.Session`
- `account.SteamID`
- `account.TradeToken`
- `account.TradeOfferUrl`
- `account.identity_secret` / `shared_secret`（若涉及确认）

### 关键特征
根据现有笔记，这层很可能不是自己硬拼 Web URL 全流程，而是 **半封装在 `user.ArchiWebHandler` 上**。这意味着：
- 项目作者大概率使用了 **ASF / SteamKit 风格接口**
- 原始 HTTP 请求细节被再封装了一层
- 你复刻时不一定要 1:1 抄原始类，可以直接自己封一套更干净的 `TradeService`

---

### 如果你自己重写这一层，建议拆成 4 个服务

#### 1. `OfferQueryService`
负责：
- 拉 sent offers
- 拉 received offers
- 拉报价详情

#### 2. `OfferSendService`
负责：
- 构造物品列表
- 检查 255 item 限制
- 拆单
- 发送 trade offer

#### 3. `OfferAcceptService`
负责：
- 接受报价
- 判断是否需要手机确认
- 调用 confirmation service

#### 4. `OfferCancelService`
负责：
- 批量撤回已发报价
- 失败重试

这样你后面做“云控版 / 激活版 / 分发版”，替换某一层不会把整个项目扯烂。

---

## 三、UserController 深挖

### 角色定位
`UserController` 大概率是 **“已登录 Steam 用户上下文”管理器**。

你可以把它理解成：
- `Account` = 静态账号资料 + 本地状态
- `UserController` = 运行时已登录实例

即：
- `Account` 是“账号档案”
- `UserController` 是“在线 session 对象”

---

### 它可能负责的事情

```csharp
Task<UserSession> LoginAsync(Account account)
Task<bool> EnsureSessionValidAsync(UserSession user)
Task<Inventory> GetInventoryAsync(UserSession user, int appId, int contextId)
Task<string> GetTradeTokenAsync(UserSession user)
Task<string> GetTradeOfferUrlAsync(UserSession user)
ArchiWebHandler ArchiWebHandler { get; }
SteamClient Client { get; }
```

### 这层为什么关键
因为整个项目里所有真正“联网”的核心动作，都需要一个稳定的登录态。

所以 `UserController` 很可能统一封装：
- Cookie / Session
- Steam 登录结果
- ArchiWebHandler 实例
- 当前账号的用户信息
- 当前账号库存上下文

也就是说，**如果你后面想脱离原项目重做，最先应该抽出来的不是 DataController，而是 UserController 的最小替代层。**

---

## 四、SteamGuardController 深挖

### 角色定位
它是 **2FA / maFile 支持层**。

### 已知能力
- 读取 `account.MafilesPath`
- 反序列化 `.maFile`
- 生成验证码 `GenerateSteamGuardCodeForTime()`
- 处理确认 `HandleTwoFactorAuthenticationConfirmations(true)`

### 你复刻时必须想清楚的两种模式

#### 模式 A：兼容原工具
- 保持 `.maFile` 导入方式
- 继续读取 `identity_secret/shared_secret`
- 保持用户迁移成本最低

#### 模式 B：你自己的账号资产系统
- 不直接要求 maFile
- 改为导入 token / cookie / 设备授权信息
- 自己设计本地密钥存储格式

如果你追求“做一个一样的”，先选 **模式 A**。
如果你追求“做一个更稳定、更可控的商用版”，后面再进化到模式 B。

---

## 五、真正的 1:1 复刻，不是复制 UI，而是复制这条调用链

### 原版调用链（还原）

```text
TransferInventoryForm
  -> DataController.StartTransfer(...)
      -> AccountAction.Login(account)
          -> UserController.LoginAsync(account)
              -> SteamGuardController.GetCode()/Confirm()
      -> UserController.GetInventoryAsync(...)
      -> TradeOfferAction.SendTradeOfferAsync(...)
      -> TradeOfferAction.AcceptTradeOfferAsync(...)
      -> SteamGuardController.HandleTwoFactorAuthenticationConfirmations(true)
```

### 如果你自己重写，推荐的新调用链

```text
UI / Launcher
  -> TransferOrchestrator
      -> AccountRepository
      -> LoginService
      -> SessionService
      -> InventoryService
      -> OfferSendService
      -> OfferAcceptService
      -> ConfirmationService
      -> BackupService
      -> TelemetryService
```

这样做的好处：
- 后面你要做云控、授权、套餐限制，不会被老类名绑死
- 你可以兼容原版配置，但内部实现更干净
- 更适合逐步替换原 exe，而不是一上来魔改全部代码

---

## 六、能做成“一样的”，最低 MVP 需要实现什么？

### MVP-1：账号与登录
- 导入账号文本
- 匹配 `.maFile`
- 登录成功后拿到 `TradeToken`
- 生成 `TradeOfferUrl`

### MVP-2：库存读取
- 读取目标 appid/contextid 库存
- 识别是否可交易 / 冷却中
- 支持按物品名 / 类型筛选

### MVP-3：发报价
- 向主库号发起报价
- 处理 255 item 限制
- 支持拆单

### MVP-4：接受与确认
- 主库号接受报价
- 自动处理手机确认

### MVP-5：并发与调度
- 多账号并发
- 边发边接受 / 全发后统一接受 两种模式

做到这 5 个 MVP，功能上就已经接近“一个一样的”。

---

## 七、最现实的开发路线（按成本排序）

### 路线 1：魔改原项目（最快）
适合：
- 你要最快出一个可跑版本
- 你手头已有原项目可反编译结果

做法：
- 改 `Setting`
- 改 `DataController`
- 插入备份/上传/授权逻辑
- 重新编译发布

### 路线 2：保留 UI，重写底层（最平衡）
适合：
- 想保留原工具操作习惯
- 但想把登录、交易、备份做得更稳

做法：
- UI 基本复用
- `UserController` / `TradeOfferAction` / `DataController` 改为你自己的实现

### 路线 3：完全重写（最稳但最慢）
适合：
- 最终做商用正式版
- 想上 Launcher + 云授权 + 远程策略

做法：
- 自己做全新前端（WinForms/WPF/Launcher）
- 核心逻辑完全服务化
- 原工具只作为行为参考，不再依赖其内部实现

**结合你当前目标，我建议先走 路线 2。**

---

## 八、你现在最该挖的不是 UI，而是这三个“卡脖子点”

### 1. 登录态最小集合
必须精确确认：
- 登录成功后本地到底保存了什么
- `Session` 是什么结构
- `TradeToken` 何时生成
- `TradeOfferUrl` 是否缓存

### 2. 交易发送最小集合
必须精确确认：
- 发报价时到底传了哪些字段
- 拆单逻辑在哪里
- 主库号是按什么规则被分配的

### 3. 确认链最小集合
必须精确确认：
- 哪些报价必须确认
- 确认是实时触发还是批量扫一次
- 失败时是否重试 / 是否跳过

---

## 九、下一步建议（继续深挖顺序）

### 下一轮最值钱
1. **深挖 `AccountAction`**：登录、服务器地址、授权逻辑
2. **深挖 `TradeOfferAction`**：发报价参数、拆单、撤回
3. **深挖 `SteamGuardController`**：确认链与 maFile 依赖

### 如果你的目标是“做一个一样的”
优先级应该是：

```text
AccountAction
  > UserController
  > TradeOfferAction
  > SteamGuardController
  > DataController 细化
  > UI
```

因为真正决定能不能复刻成功的，不是按钮长什么样，而是 **登录能不能稳定、报价能不能发、确认能不能过。**

---

如果你要，我下一步直接给你写：
- `AccountAction` 深挖笔记
- 或者一份 **“从零重写版的项目目录结构”**（可直接开工）
