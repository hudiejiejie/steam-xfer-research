# AccountAction 深挖：登录链 / 授权链 / 可复刻方案

> 目标：搞清楚 Spider 版 Steam 转库存工具是如何完成账号登录、会话建立、密码记忆与可能存在的授权校验
> 说明：基于现有 48 份逆向材料推导，具体方法签名需以后续反编译结果校正

---

## 一、AccountAction 的定位

`AccountAction` 不像 `DataController` 那样负责总调度，
它更像 **账号动作服务层**，职责集中在：

1. 登录账号
2. 初始化 `Account` 对象运行时状态
3. 将登录结果灌入 `UserController` / `Session`
4. 处理密码记忆、失败信息、异常状态
5. 可能包含一部分授权/会员/登录前校验逻辑

一句话：

- `DataController` 决定 **何时登录**
- `AccountAction` 决定 **如何登录成功**

---

## 二、它为什么是复刻关键点

如果你要做一个“一样的”：
- UI 好做
- 配置好抄
- 库存和报价可以后补

但 **登录链如果没摸透，整个项目就起不来**。

尤其是这个项目已经确认：
- 支持账号密码登录
- 更偏向 `.maFile` / SteamGuard 登录
- 登录成功后会产生：
  - `Session`
  - `TradeToken`
  - `TradeOfferUrl`
  - `SteamID`
  - `User`

所以 `AccountAction` 是“进入系统”的闸门。

---

## 三、基于现有材料可确认的信息

### 已知事实
1. `LoginForm` 登录窗体存在
2. 登录实际调用 `AccountAction.Login(...)`
3. 登录记忆写入 `Setting.Instance`
4. 本地密码保存采用 **AES-CBC**
5. `.maFile` 是一等公民，优先级高于纯账号密码
6. 成功登录后，`Account` 会被补齐：
   - `Session`
   - `TradeToken`
   - `TradeOfferUrl`
   - `SteamID`
   - `User`

### 高概率事实
1. `AccountAction.Login(...)` 内部会转调 `UserController.LoginAsync(...)`
2. 2FA 码来自 `SteamGuardController.GenerateSteamGuardCodeForTime()`
3. 若遇到确认链，后续由 `SteamGuardController.HandleTwoFactorAuthenticationConfirmations(true)` 处理
4. 登录成功后，`Account` 会作为运行时状态容器被传给 `DataController`

---

## 四、推测的方法结构

### 1. 登录主方法
```csharp
Task<bool> LoginAsync(Account account)
// 或
bool Login(Account account)
```

#### 输入
- `account.account_name`
- `account.account_password`
- `account.MafilesPath`
- 可能还依赖：代理、验证码、登录模式

#### 输出
- 成功/失败布尔值
- 或者 `LoginResult`
- 同时写回 `account.Session`, `account.User`, `account.SteamID`

---

### 2. 带配置的登录
```csharp
Task<LoginResult> LoginAsync(Account account, bool rememberPassword, bool useMaFile)
```

这类重载常见于：
- UI 登录按钮
- DataController 批量登录
- 导入账号后立即验证

---

### 3. 记忆密码 / 加密保存
```csharp
void SaveCredentials(string username, string password)
(string username, string password) LoadCredentials()
string EncryptAesCbc(string plain)
string DecryptAesCbc(string cipher)
```

#### 这里的意义
如果你后面要自己做一个一样的，必须决定：
- 是继续兼容这套 AES-CBC
- 还是改成你自己的安全存储（Windows DPAPI / 本地证书 / Launcher 管理）

如果是“像原版”，先兼容 AES-CBC 即可。

---

### 4. 登录前检查
```csharp
bool ValidateAccountInput(Account account)
bool CheckMaFileExists(Account account)
```

可能会做：
- 用户名密码是否为空
- `.maFile` 是否存在
- 账号是否已经登录
- 当前 Session 是否仍可复用

---

### 5. 失败处理
```csharp
void HandleLoginFailure(Account account, Exception ex)
```

可能做：
- 写日志
- 更新 UI 状态
- 标记账号失败次数
- 决定是否重试

---

## 五、最可能的登录链（还原）

### 原版逻辑推测

```text
LoginForm.btnLogin_Click(...)
  -> AccountAction.Login(account)
      -> ValidateAccountInput(account)
      -> if account.MafilesPath exists:
             SteamGuardController.LoadMaFile(account)
             SteamGuardController.GenerateSteamGuardCodeForTime()
      -> UserController.LoginAsync(account)
           -> 创建 SteamClient / ArchiWebHandler / Session
           -> 获取 SteamID / User / Cookies
      -> SaveCredentials(...)  // 若勾选记住密码
      -> 回填 account.Session / account.User / account.SteamID
      -> 返回成功
```

### 批量转库存时的登录链

```text
DataController.StartTransfer(...)
  -> foreach beTradeAccount:
       AccountAction.Login(account)
         -> UserController.LoginAsync(account)
         -> 获取 Session
         -> 获取 TradeToken / TradeOfferUrl
         -> 返回给 DataController
```

关键区别：
- **UI 登录** 偏人机交互
- **批量登录** 偏无人值守

所以很可能存在两个入口：
- `LoginForm` 用的登录方法
- `DataController` 批量用的登录方法

---

## 六、Session 到底可能是什么

### 已知字段
`Account.Session`

### 高概率不是简单字符串
它更可能是：
- Cookie 集合
- 登录后用户上下文对象
- 或封装后的登录结果 DTO

可能结构示例：
```csharp
class SessionInfo {
    string SessionId;
    Dictionary<string,string> Cookies;
    DateTime ExpiresAt;
    bool IsAuthenticated;
}
```

或者直接是某个第三方库对象。

### 为什么这个点很重要
因为如果你后面要做自己的版本：
- 登录一次后是否复用 Session
- 是否支持断点续跑
- 是否支持免登录恢复

都取决于你把 `Session` 怎么落地。

---

## 七、授权链：有，还是没有？

### 已知材料里没有直接证明强授权
但有两种可能：

#### 情况 A：Spider 版免费版没有登录授权，只是本地登录 Steam
这时 `AccountAction` 主要就是 Steam 登录。

#### 情况 B：Spider 版有外围授权校验
这时会表现为：
- 登录前访问某个 API
- 校验机器码 / 会员状态 / 到期时间
- 决定功能开关或线程上限

### 结合现有规划判断
你后面的产品方案是：
- 云端激活码服务
- 本地 Launcher
- 后台套餐与功能开关

所以就算原版没有强授权，你也应该在 **新版本自己的 Launcher** 中接入授权，而不是强耦合到 Steam 登录动作本身。

**建议：授权不要硬塞进 `AccountAction`，而是放到 Launcher / 启动前校验层。**

---

## 八、如果要自己做一个一样的，AccountAction 最小复刻清单

### 必须实现的能力
1. 从 `Account` 读取用户名 / 密码 / maFile 路径
2. 判断是否优先使用 maFile
3. 生成 Steam Guard 验证码
4. 建立 Steam 登录会话
5. 获取并保存：
   - Session
   - SteamID
   - TradeToken
   - TradeOfferUrl
6. 将登录态挂到 `UserController`
7. 将错误写回 UI / 日志

### 先不要实现的能力
- 复杂授权逻辑
- 云端套餐逻辑
- 太深的本地加密体系

因为这些不是“做得像”的第一阻碍。

---

## 九、建议的新架构：不要直接复刻 AccountAction，应该拆成 4 层

### 1. `CredentialStore`
负责：
- 账号密码保存/读取
- 本地加密（兼容原版或替换）

### 2. `SteamGuardService`
负责：
- `.maFile` 读取
- 验证码生成
- 手机确认

### 3. `SteamLoginService`
负责：
- 执行真正登录
- 建立会话
- 返回 `UserSession`

### 4. `AccountBootstrapper`
负责：
- 将 `UserSession` 写回 `Account`
- 补齐 `TradeToken` / `TradeOfferUrl` / `SteamID`

### 为什么这样更好
原版的 `AccountAction` 很可能把这些东西全揉在一起。
而你如果自己做，拆开后：
- 好测试
- 好替换
- 好接授权系统
- 好做无头版本

---

## 十、最关键的验证点（下轮应该继续追）

### 验证点 1：登录时是否真的调用了外部授权 API
要确认：
- 有无固定域名/URL
- 有无机器码上传
- 有无套餐字段/线程上限下发

### 验证点 2：`TradeToken` 是登录时就拿，还是拉库存时才拿
这个会影响你重写登录服务的边界。

### 验证点 3：`TradeOfferUrl` 是由本地拼接，还是服务端返回
如果是本地拼接，复刻更容易；如果依赖接口返回，要顺着会话链继续挖。

### 验证点 4：AES-CBC 的 key/iv 从哪里来
- 常量硬编码？
- 派生于机器码？
- 派生于账号名？

这决定你是否能兼容原有“记住密码”。

---

## 十一、对你现在最有价值的结论

如果目标是：**做出一个一样的**，
那 `AccountAction` 不是要 100% 原样照抄，
而是要先确认下面这 5 件事：

1. 登录输入是什么
2. 登录输出是什么
3. maFile 参与到哪一层
4. Session 如何保存
5. TradeToken / TradeOfferUrl 在哪一步生成

只要这 5 件事确定，原版 `AccountAction` 就已经被你“功能解构”了。

---

## 十二、下一步最佳延伸

现在最应该继续挖的是两条之一：

### A. `SteamGuardController`
目的：把 maFile / 2FA / 确认链挖穿

### B. `TradeOfferAction`
目的：把发报价 / 接受 / 撤回的最小参数集确认清楚

### 我建议优先：`SteamGuardController`
因为登录要稳定跑通，2FA 是硬门槛。
登录没跑通，后面的发报价都是空谈。
