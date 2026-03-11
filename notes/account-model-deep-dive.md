# Account 模型深挖：字段分层 / 状态骨架 / 重写建议

> 目标：把 Spider 版 Steam 转库存工具里的 `Account` 拆成可复刻的数据模型
> 说明：基于现有 48 份逆向材料推导，真实字段与类型需以后续反编译结果校正

---

## 一、为什么 `Account` 是整个系统的数据骨架

前面已经拆出的关键模块有：
- `Setting`
- `AccountAction`
- `SteamGuardController`
- `TradeOfferAction`
- `DataController`

而这 5 个模块有一个共同中心：

> **`Account`**

因为：
- 登录写回它
- maFile 资产挂在它上面
- Session 挂在它上面
- TradeToken / TradeOfferUrl 挂在它上面
- Inventory 挂在它上面
- 已发报价 / 待接受报价列表也挂在它上面

也就是说，这个项目不是围绕“请求对象”设计的，
而是围绕 **账号对象 + 账号运行时状态** 设计的。

这非常关键：
如果你要做一个一样的，先把 `Account` 拆清楚，整个系统的数据结构就立住了。

---

## 二、现有材料已经确认的字段

根据前面整理的逆向结论，已明确字段/属性有：

```csharp
account_name
account_password
MafilesName
MafilesPath
identity_secret
shared_secret
Session
TradeOfferUrl
TradeToken
SteamID
User
Inventory
SentTradeIDs
BeTradeOfferIDs
```

这组字段已经足够说明：
`Account` 不是一个简单的“账号名密码 DTO”，
而是一个 **账号档案 + 运行时上下文 + 交易状态容器**。

---

## 三、最合理的字段分层（非常重要）

如果只看原字段名，容易乱。要真正做出一个一样的，必须把这些字段按职责拆层看。

---

### A. 静态身份字段（账号基本资料）

```csharp
string account_name;
string account_password;
string SteamID;
```

#### 含义
- `account_name`：Steam 登录用户名
- `account_password`：Steam 登录密码（若不是纯 maFile 模式）
- `SteamID`：登录成功后可确认的用户唯一标识

#### 特点
- `account_name` / `account_password` 偏输入资产
- `SteamID` 偏登录后确认得到的静态身份

#### 重写建议
后面你自己做版本时，这层可以叫：
- `CredentialIdentity`
或直接并入 `AccountProfile`

---

### B. Steam Guard 资产字段

```csharp
string MafilesName;
string MafilesPath;
string identity_secret;
string shared_secret;
```

#### 含义
- `MafilesName`：maFile 文件名
- `MafilesPath`：maFile 完整路径
- `identity_secret`：用于 mobile confirmation
- `shared_secret`：用于生成 Steam Guard code

#### 特点
这层就是：
> **账号安全资产层**

它并不是业务数据，而是登录与确认链的底层凭据。

#### 重写建议
这层不要继续散落在 Account 顶层，建议未来拆成：
```csharp
class SteamGuardAsset {
    string MaFileName;
    string MaFilePath;
    string SharedSecret;
    string IdentitySecret;
}
```

这样后面做加密存储会轻松很多。

---

### C. 会话字段（运行时登录态）

```csharp
object Session;
object User;
string TradeToken;
string TradeOfferUrl;
```

#### 含义
- `Session`：登录后得到的会话/ cookie / 用户上下文
- `User`：很可能是 `UserController` 或已登录用户对象
- `TradeToken`：交易 token
- `TradeOfferUrl`：完整交易链接

#### 特点
这一层最像：
> **运行时在线状态层**

这层不是静态配置，不适合长期持久化为“账号本体”。

#### 重写建议
未来你自己做版本时，建议拆成：
```csharp
class AccountRuntimeState {
    SessionInfo Session;
    object User;
    string TradeToken;
    string TradeOfferUrl;
    bool IsLoggedIn;
    DateTime? SessionExpiresAt;
}
```

因为它们本质上都属于“登录后才有”的数据。

---

### D. 库存字段

```csharp
object Inventory;
```

#### 含义
- 当前账号库存快照
- 可能包含全部 inventory items、可交易状态、分类信息

#### 特点
这是典型的：
> **派生业务数据**

库存本质不是账号身份，而是运行中临时加载的数据。

#### 重写建议
拆成：
```csharp
class InventorySnapshot {
    List<InventoryItem> Items;
    int TotalCount;
    int TradableCount;
    int CooldownCount;
    DateTime FetchedAt;
}
```

这样你后面做备份 / 差异比较 / 上传服务器时会更好用。

---

### E. 交易状态字段

```csharp
List<string> SentTradeIDs;
List<string> BeTradeOfferIDs;
```

#### 含义
- `SentTradeIDs`：当前账号已经发出的报价 ID 列表
- `BeTradeOfferIDs`：当前账号待接受的报价 ID 列表

#### 特点
这是：
> **当前任务上下文状态**

这些数据通常随着一次转库存任务变化，不应该视为长期静态档案。

#### 重写建议
拆成：
```csharp
class TradeState {
    List<string> SentOfferIds;
    List<string> PendingReceiveOfferIds;
    DateTime LastOfferSyncAt;
}
```

这样会比直接挂在 `Account` 顶层更清晰。

---

## 四、原版 `Account` 的本质：大一统状态容器

综合来看，原版 `Account` 很可能是一个“什么都往里塞”的大对象：

- 身份字段
- 登录字段
- maFile 资产
- 库存状态
- 报价状态

都放在一起。

### 这对原版有什么好处
- 写起来快
- 各层直接拿一个 `Account` 就能干活
- UI 绑定也方便

### 这对原版有什么代价
- 耦合重
- 不容易区分哪些能持久化、哪些只是运行时
- 后续扩展（云端同步/断点续跑/多端管理）会越来越乱

### 对你的启发
如果目标是：
- **先做出一个一样的** → 可以先接受这种大对象模式
- **后面做更稳的商用版** → 需要尽快拆分 Account 内部结构

---

## 五、`Account` 在各模块之间是怎么流动的

这部分特别重要，因为它决定“系统架构是不是围绕账号对象在转”。

### 1. LoadAccount → Account
```text
导入账号文本 / 剪贴板
  -> 创建 Account
  -> 填充 account_name / account_password
  -> 匹配 maFile
  -> 写入 MafilesPath / MafilesName
```

### 2. AccountAction → Account
```text
Login(account)
  -> 使用用户名/密码/maFile 登录
  -> 回填 Session / User / SteamID
```

### 3. SteamGuardController → Account
```text
LoadMaFile(account)
  -> 读取 shared_secret / identity_secret
  -> 写回 Account
```

### 4. UserController → Account
```text
获取会话 / 用户上下文
  -> 写回 User / Session / TradeToken / TradeOfferUrl
```

### 5. TradeOfferAction → Account
```text
发报价 / 拉报价 / 撤回 / 接受
  -> 更新 SentTradeIDs / BeTradeOfferIDs
```

### 6. DataController → Account
```text
拉库存 / 分配任务 / 统计结果
  -> 更新 Inventory / TradeState
```

### 结论
这个项目的真实架构可以概括成：

> **所有模块都围着 Account 做读写。**

这就是为什么 `Account` 必须先被拆透。

---

## 六、如果你要自己做一个一样的，`Account` 最小复刻模型

### 第一版（最像原版）
```csharp
class Account {
    string AccountName;
    string AccountPassword;
    string MaFilesName;
    string MaFilesPath;
    string IdentitySecret;
    string SharedSecret;

    SessionInfo Session;
    object User;
    string SteamID;
    string TradeToken;
    string TradeOfferUrl;

    InventorySnapshot Inventory;
    List<string> SentTradeIDs;
    List<string> BeTradeOfferIDs;
}
```

### 这版的优点
- 最接近原版
- 最容易直接对照旧逻辑迁移
- 各服务直接传一个 Account 就能跑

### 缺点
- 很快会变乱
- 难以做持久化边界控制

---

## 七、如果你要做更稳的版本，建议的二阶段模型

```csharp
class AccountProfile {
    string AccountName;
    string AccountPassword;
    string SteamID;
}

class SteamGuardAsset {
    string MaFileName;
    string MaFilePath;
    string SharedSecret;
    string IdentitySecret;
}

class AccountRuntimeState {
    SessionInfo Session;
    object User;
    string TradeToken;
    string TradeOfferUrl;
    bool IsLoggedIn;
}

class TradeState {
    List<string> SentOfferIds;
    List<string> PendingReceiveOfferIds;
}

class InventorySnapshot {
    List<InventoryItem> Items;
    int TotalCount;
    int TradableCount;
    int CooldownCount;
    DateTime FetchedAt;
}

class AccountAggregate {
    AccountProfile Profile;
    SteamGuardAsset Guard;
    AccountRuntimeState Runtime;
    TradeState Trade;
    InventorySnapshot Inventory;
}
```

### 这版的意义
这就是你后面做：
- Launcher
- 云端授权
- 账号资产同步
- 任务断点续跑
- 后台统一管理

时更适合的模型。

---

## 八、`Account` 持久化边界（非常关键）

如果你后面要做本地数据库 / JSON / 云端同步，必须分清：

### 适合持久化的
- `account_name`
- `account_password`（如果你选择保存）
- `MafilesPath`
- `MafilesName`
- `identity_secret/shared_secret`（若允许）
- `SteamID`

### 不适合长期持久化的
- `Session`
- `User`
- `TradeToken`（可缓存但要可过期）
- `TradeOfferUrl`（可缓存但要可重建）
- `Inventory`
- `SentTradeIDs`
- `BeTradeOfferIDs`

### 也就是说
原版 `Account` 很可能把“长期资产”和“短期状态”混在一起。
而你后面自己做时，最好尽快把这条边界拆出来。

---

## 九、这层最值得验证的 4 个问题

### 1. `SteamID` 是登录后立即写回，还是等到首次拉用户资料时才写回？

### 2. `TradeToken` / `TradeOfferUrl` 是否会缓存回 Account 并持久化？

### 3. `identity_secret/shared_secret` 是始终直接放在 Account 中，还是只在运行时展开？

### 4. `Inventory` 的真实类型是什么？
- 原始列表？
- 字典？
- 自定义类？

这些点会决定你后面重写时的模型细节。

---

## 十、对你现在最值钱的结论

到这一轮为止，Spider 版这个工具的：
- **控制台（Setting）**
- **数据骨架（Account）**
- **登录链（AccountAction）**
- **2FA 链（SteamGuardController）**
- **交易动作链（TradeOfferAction）**
- **总调度链（DataController）**

已经形成一张完整的系统图。

换句话说：

> 你现在已经接近能把它按“模块 + 数据模型”两条线独立重建了。

---

## 十一、下一步最佳方向

现在继续深挖，最值钱的是二选一：

### A. `TransferInventoryForm`
把 UI 的按钮、表格、导入导出动作、日志、线程输入、模式选择，全部和后端调用链对齐

### B. `LoadAccount`
把账号导入规则、文本格式、maFile 匹配逻辑、批量导入清洗规则拆清楚

### 我建议优先：`LoadAccount`
因为如果你的目标是做一个“一样的”，
那用户第一步最真实的入口不是“点开始”，而是：

> **先把账号和 maFile 导进去。**

把这层挖清楚，你就能把“账号来源 → Account 模型 → 登录链”前半段彻底打通。
