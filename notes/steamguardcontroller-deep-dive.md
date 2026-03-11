# SteamGuardController 深挖：maFile / 2FA / 手机确认链

> 目标：搞清楚 Spider 版工具如何依赖 `.maFile`、如何生成 Steam Guard 验证码、以及如何完成报价确认
> 说明：基于现有 48 份逆向材料推导，方法签名与具体类型名后续需用反编译结果校正

---

## 一、为什么这层是“硬门槛”

如果你的目标是做一个“一样的”：
- UI 可以重画
- 配置可以照搬
- DataController 可以慢慢重构

但 **SteamGuardController 跑不通，整个工具就不能稳定登录，也无法自动确认报价。**

也就是说：
- `AccountAction` 决定登录入口
- `SteamGuardController` 决定登录能不能过 2FA
- `TradeOfferAction` 决定发出去的报价能不能被真正确认生效

这层是整个项目里最像“基础设施”的部分。

---

## 二、现有材料已经确认的事实

### 直接确认
1. `SteamGuardController` 存在于 `SteamInventoryManager.Model`
2. 它会读取 `account.MafilesPath`
3. 它会把 `.maFile` 反序列化为 `SteamGuardAccount`
4. 它会调用 `GenerateSteamGuardCodeForTime()` 生成验证码
5. 在主库号接受报价后，如果需要确认，会调用：
   `HandleTwoFactorAuthenticationConfirmations(accept: true)`

### 这意味着什么
- `.maFile` 不是附属功能，而是主路径的一部分
- 工具作者并不是简单靠网页 cookie 混过去，而是接入了 Steam Guard 的标准资产
- 2FA 与报价确认都已经内建在业务链路里

---

## 三、`account` 与 SteamGuard 的关系

`Account` 已知字段里有：
- `MafilesPath`
- `MafilesName`
- `identity_secret`
- `shared_secret`

这几个字段正好对应 `.maFile` 的典型结构。

### 高概率的数据流
```text
LoadAccount / 导入 maFile
  -> 账号名 + ".maFile" 匹配文件
  -> 保存到 account.MafilesPath
  -> SteamGuardController.Load(account)
      -> 读取 JSON
      -> 解析 identity_secret / shared_secret / Session
      -> 写回 account
```

### 这说明两件事
1. `.maFile` 很可能既是登录验证码来源，也是确认来源
2. `Account` 对象本身已经被设计成 “maFile + 登录态 + 交易态” 的统一容器

---

## 四、SteamGuardController 最可能的方法结构

### 1. 加载 maFile
```csharp
SteamGuardAccount LoadMaFile(Account account)
// 或
SteamGuardAccount GetSteamGuardAccount(Account account)
```

#### 功能
- 从 `account.MafilesPath` 读取 JSON
- 反序列化为 `SteamGuardAccount`
- 校验关键字段是否存在

#### 输入
- `account.MafilesPath`

#### 输出
- `SteamGuardAccount`

---

### 2. 生成验证码
```csharp
string GenerateCode(Account account)
// 或
string GenerateSteamGuardCodeForTime(Account account)
```

#### 内部逻辑
- 读取 `shared_secret`
- 基于当前时间窗口生成 2FA code

#### 作用点
- 登录时需要输入 Steam Guard 验证码
- 有些高风险操作也可能需要再次验证

---

### 3. 获取确认列表
```csharp
Task<List<Confirmation>> GetConfirmationsAsync(Account account)
```

#### 功能
- 登录后访问确认接口
- 获取待确认的交易、市场上架、设备绑定等确认项

#### 重点
对于 Spider 版转库存项目，这里最关注的是：
- 报价相关确认
- 接受报价相关确认

---

### 4. 批量确认
```csharp
Task<bool> HandleTwoFactorAuthenticationConfirmations(bool accept)
// 或
Task<int> AcceptAllTradeConfirmations(Account account)
```

#### 功能
- 扫描当前待确认项
- 根据条件过滤出 trade offers
- 如果 `accept == true`：逐个确认
- 如果 `accept == false`：可能忽略/撤销

#### 调用场景
- `DataController.AcceptBeTradeOffers(master)` 接受报价后
- 可能在 `SendTradeOffer` 后也会做一次确认清理

---

### 5. 针对单个报价确认
```csharp
Task<bool> ConfirmTradeOfferAsync(Account account, string offerId)
```

#### 功能
- 找到与 `offerId` 对应的确认项
- 提交 mobileconf 接口确认

如果工具里有“按单确认”能力，这个方法大概率存在。

---

## 五、最可能的完整确认链

### 场景 A：待转号发出报价后，需要手机确认
```text
DataController.StartTransfer(...)
  -> TradeOfferAction.SendTradeOffer(...)
  -> 返回 offerId
  -> SteamGuardController.GetConfirmations(account)
  -> 找到对应 offerId 的确认项
  -> ConfirmTradeOfferAsync(account, offerId)
```

### 场景 B：主库号接受报价后，需要再次确认
```text
DataController.AcceptBeTradeOffers(master)
  -> AcceptTradeOfferAsync(master, offerId)
  -> if need 2FA:
       SteamGuardController.HandleTwoFactorAuthenticationConfirmations(true)
```

### 这说明
Spider 版很可能做了“**操作后扫确认列表**”的模式，而不是完全依赖同步返回。

这很关键，因为你后面重写时要决定：
- 是按 `offerId` 精确确认
- 还是统一扫待确认列表后批量确认

如果目标是“像原版”，更可能是 **统一扫列表**。

---

## 六、`.maFile` 最可能依赖的字段

标准 `.maFile` 常见字段：
- `shared_secret`
- `identity_secret`
- `device_id`
- `Session` / Web cookies
- `SteamID`

### 这里的作用拆解

#### `shared_secret`
用于：
- 生成 Steam Guard 登录验证码

#### `identity_secret`
用于：
- 生成确认签名
- 处理 mobile confirmations

#### `device_id`
用于：
- 确认接口签名或设备标识

#### `SteamID`
用于：
- 绑定当前账号与确认请求

### 结论
如果你自己做一个一样的：
**至少必须兼容 `shared_secret + identity_secret + SteamID`。**

---

## 七、如果你要重写，这层应该怎么拆

不要直接把所有逻辑塞进一个 `SteamGuardController`，建议拆成 3 个服务：

### 1. `MaFileRepository`
负责：
- 读取 `.maFile`
- JSON 解析
- 字段校验
- 缓存到 `Account`

### 2. `TwoFactorCodeService`
负责：
- 基于 `shared_secret` 生成验证码
- 提供 `GenerateCode(account)`

### 3. `ConfirmationService`
负责：
- 拉取待确认项
- 匹配 `offerId`
- 提交确认
- 批量确认

这样拆的好处：
- 登录逻辑和确认逻辑分离
- 后面切换 maFile 存储方式更容易
- 可以把确认重试单独做成后台任务

---

## 八、你如果要做一个“一样的”，最小复刻清单

### 必须实现
1. `.maFile` 导入与匹配
2. 从 `.maFile` 读取 `shared_secret`
3. 生成 Steam Guard 验证码
4. 从 `.maFile` 读取 `identity_secret`
5. 拉取待确认列表
6. 自动确认 trade offer

### 可以后补
- `.maFile` 可视化管理
- 多账号资产加密仓库
- 自动刷新设备授权
- 更优雅的确认日志 UI

---

## 九、如果不想依赖 `.maFile`，能不能做？

### 理论上可以，但不建议作为第一版

如果目标是“做一个一样的”，不建议第一版去掉 `.maFile`。

因为一旦你去掉它，就要自己补：
- 验证码来源
- 确认签名机制
- 设备标识
- 登录恢复策略

这会让项目复杂度直接上一个量级。

### 最佳策略
- **第一版：兼容原 `.maFile`**
- **第二版：把 `.maFile` 封装到自己的账号资产层里**
- **第三版：再考虑无 maFile 的新资产格式**

---

## 十、这层最值得验证的 4 个问题

### 1. `HandleTwoFactorAuthenticationConfirmations(true)` 是确认所有项，还是只确认交易项？
这个会影响误确认风险。

### 2. 确认是同步阻塞执行，还是后台轮询？
这个会影响 UI 卡顿和并发调度。

### 3. `.maFile` 的内容有没有被再次写回本地？
如果会写回，说明项目可能也在维护 Session 或 token 缓存。

### 4. 确认失败时怎么处理？
- 重试几次？
- 跳过？
- 记录失败并继续？

这决定了项目的稳定性上限。

---

## 十一、对你当前最值钱的结论

如果你的目标是做一个“一样的”，
那你现在已经可以把这条链抽象成：

```text
Account
  -> MaFileRepository
  -> TwoFactorCodeService
  -> SteamLoginService
  -> ConfirmationService
  -> TradeOfferAction
  -> DataController
```

也就是说，`SteamGuardController` 已经可以被你拆成“可重写模块”，而不再是黑盒。

---

## 十二、下一步最应该挖什么

现在最值钱的下一步不是 UI，而是：

### A. `TradeOfferAction`
确认：
- 发报价最小参数集
- 拆单逻辑
- 撤回/接受参数
- 是否直接依赖 `ArchiWebHandler`

### B. `Setting`
确认：
- 所有配置字段
- 默认值
- UI 控件如何映射到配置

### 我建议优先：`TradeOfferAction`
因为：
- 登录链已经被拆成可理解结构
- 2FA/确认链也基本明确
- 下一步真正决定“能不能发货”的就是报价动作层

**也就是：再挖完 `TradeOfferAction`，你就已经接近把最难的 3 层都拆清楚了。**
