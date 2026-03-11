# Steam转库存V1.0.exe 逆向学习笔记（按文本顺序去重整理）

> 源材料：`C:\Users\温洪杰\.openclaw\media\inbound` 下 48 个 txt（其中 47 个为连续编号文本 + 1 个未编号文本）。
> 连续合并稿：`C:\Users\温洪杰\.openclaw\workspace\steam_xfer_all_48_with_misc.txt`


## 1. 初始判断
- 先识别技术栈：排查 .NET / PyInstaller / UPX / 原生 / 易语言。
- 已确认目标不是 PyInstaller、不是 UPX，也不是传统易语言黑盒。
- 命中强 .NET 特征：
  - `mscoree.dll`
  - `_CorExeMain`
  - `.CLR_UEF`

## 2. 程序形态判断
- 该 exe 是 **.NET 8 Windows 桌面单文件程序（single-file bundle）**。
- 关键特征：
  - `.runtimeconfig.json`
  - `.deps.json`
  - `singlefile`
  - `bundle`
- 主程序集名已确认：`SteamInventoryManager.dll`

## 3. 依赖与技术栈
- 核心依赖：
  - `SteamKit2`
  - `SteamAuth`
  - `Newtonsoft.Json`
  - `Microsoft.IdentityModel.JsonWebTokens`
  - `Microsoft.Extensions.Configuration`
- UI 相关依赖：
  - `PresentationFramework.dll`
  - `System.Windows.Forms.dll`
  - `WindowsFormsIntegration.dll`
  - `System.Windows.Controls.Ribbon.dll`
- 综合判断：**WPF 为主，WinForms 为辅的 C# 桌面程序**。

## 4. 提取思路演进
1. 先看有没有现成 single-file 解包工具。
2. `dotnet-file` 不够用，它不是反编译工具。
3. 现成 NuGet 包版本太老，无法解析 .NET 8 bundle。
4. 改用 **本机 .NET SDK 自带的新版 `Microsoft.NET.HostModel.dll`**。
5. 进一步发现可直接使用官方接口：
   - `Microsoft.NET.HostModel.Bundle.Extractor`
6. 原始 exe 被占用时，先复制副本到工作区，再对副本提取。

## 5. 提取结果
已成功提取到：
- `C:\Users\温洪杰\.openclaw\workspace\extracted_steam_xfer_v6\SteamInventoryManager.dll`
- `SteamInventoryManager.deps.json`
- `SteamInventoryManager.runtimeconfig.json`
- `SteamAuth.dll`
- `SteamKit2.dll`
- `Newtonsoft.Json.dll`
- 一整套 WPF / .NET 依赖

结论：
- 原 exe 之前不能直接被 ilspycmd 打开，是因为它是 **native apphost + 内嵌托管程序集**。
- 一旦拿到 `SteamInventoryManager.dll`，后续就进入接近 C# 项目的分析模式。

## 6. 结构分析主结论
### 程序入口
- 入口类：`SteamInventoryManager.Program`
- 流程：应用初始化 → 加载 `GlobalDatabase` → 启动主窗体 `SteamAutoLogin.TransferInventoryForm`

### 主窗体
- 主 UI：`SteamAutoLogin.TransferInventoryForm`
- 是 **Material 风格 WinForms 界面**（不是 WPF 主窗体）
- 功能区包括：
  - 主库号列表（Master）
  - 待转号列表（BeTrade）
  - 日志区
  - 物品筛选
  - 线程数
  - 转移模式
  - 主库号接受方式
  - 导入账号
  - 导入 maFiles
  - 启动 / 停止
  - 导出结果
  - 库存查看

### 配置中心
- 配置类：`SteamInventoryManager.Model.Setting`
- 对应 `Setting.json`
- 可优先修改：
  - 默认线程
  - 默认模式
  - 默认上限
  - 默认代理
  - 默认登录记忆
  - 默认物品类型

### 登录系统
- 登录窗体：`SteamInventoryManager.Form.LoginForm`
- 登录调用：`AccountAction.Login(...)`
- 本地保存：用户名/密码写入 `Setting.Instance`，并用 **AES-CBC** 加密

### 账号导入
- 导入窗体：`OutlookUpdateEmail.LoadAccount`
- 支持：文件导入 / 剪贴板导入
- maFile 匹配规则：`账号名 + ".maFile"`
- 成功后写入：`account.MafilesPath`

### 账号模型
- 关键类：`SteamInventoryManager.Model.Account`
- 已确认字段/属性：
  - `account_name`
  - `account_password`
  - `MafilesName`
  - `MafilesPath`
  - `identity_secret`
  - `shared_secret`
  - `Session`
  - `TradeOfferUrl`
  - `TradeToken`
  - `SteamID`
  - `User`
  - `Inventory`
  - `SentTradeIDs`
  - `BeTradeOfferIDs`
- 这是后续改 token / trade url / 导出账号状态的关键插点。

## 7. 核心流程总控：DataController
真正总控在：`SteamInventoryManager.Model.DataController`

### 四组核心能力
1. **刷新账号状态 / 拉库存**
   - 登录账号
   - 获取 `TradeToken`
   - 生成 `TradeOfferUrl`
   - 拉当前报价列表
   - 拉库存
   - 统计总库存 / 可交易数量 / 冷却数量

2. **主库号：接受报价**
   - 登录主库号
   - 读取 `BeTradeOffers`
   - 循环接受报价
   - 需要手机确认时调用 `HandleTwoFactorAuthenticationConfirmations(accept: true)`

3. **待转号：撤回已发报价**
   - 登录待转号
   - 拉 `SentTradeOffers`
   - 批量取消报价

4. **完整转库存总流程**
   - 并发登录待转号（最多重试 3 次，指数退避）
   - 拉库存并筛选
   - 按主库号剩余额度分配
   - 调 `user.ArchiWebHandler.SendInventory(...)` 发报价
   - 根据 `Transfer_Type` 决定：
     - `0`：边发边接受
     - `1`：全部发起后统一接受

### 重要细节
- 单个 trade offer 默认最多 **255 项**
- 最多拆 **5 单**
- 库存 context：
  - `753 -> 6`
  - 其他 appid 常见 `2 / 16`
- 物品筛选：
  - `itemType == 0`：按类型
  - `itemType == 1`：按名称精确

## 8. 真正发货与确认链
### 交易动作类
- `SteamInventoryManager.Actions.TradeOfferAction`
- 方法：
  - `GetTradeOffers(...)`
  - `SendTradeOffer(...)`
  - `CancelTradeOffer(...)`
  - 接受报价相关方法（内部有混淆名）

### 更高层调用
- 实际 DataController 调的是：`user.ArchiWebHandler.SendInventory(...)`
- 说明真正“发报价”在更高层封装里，更接近 ASF 风格。

## 9. 登录与 maFile 地位
- 主路径优先使用 **maFile 登录**，不是每次都纯账号密码硬登。
- `SteamGuardController`：
  - 直接读取 `account.MafilesPath`
  - 反序列化成 `SteamGuardAccount`
  - 调 `GenerateSteamGuardCodeForTime()` 生成验证码
- 这说明 maFile 是一等公民，不是附属补丁功能。

## 10. 最值得修改的模块（分层）
### 第一层：最容易改
1. `SteamInventoryManager.Model.Setting`
2. `SteamAutoLogin.TransferInventoryForm`
3. `OutlookUpdateEmail.LoadAccount`

适合改：
- 界面文案
- 默认配置
- 导入逻辑
- maFile 目录逻辑
- 备份逻辑
- 文件路径逻辑

### 第二层：核心业务
4. `SteamInventoryManager.Model.DataController`
5. `SteamInventoryManager.Actions.TradeOfferAction`
6. `SteamInventoryManager.Model.SteamGuardController`
7. `SteamInventoryManager.Model.UserController`

适合改：
- 转移流程
- 接受/确认流程
- Steam 通信方式
- 并发行为
- 上传备份时机

### 第三层：账号系统 / 登录系统
8. `SteamInventoryManager.Form.LoginForm`
9. `SteamInventoryManager.Actions.AccountAction`

适合改：
- 登录界面
- 登录后的状态显示
- 本地记忆密码方式
- 对接自己的服务器 / 替换原授权系统

## 11. 当前稳定结论
- 这已经不是“只能补丁改字符串”的黑盒。
- 已经进入：**围绕 `SteamInventoryManager.dll` 做真实 C# 级修改** 的阶段。
- 虽然存在部分混淆，但：
  - 类名
  - 模块边界
  - 属性名
  - 主要业务骨架
  都已经清楚。

## 12. 后续最值钱的两个方向
### A. 深挖 `DataController`
适合目标：
- 加自动备份
- 加上传服务器
- 改转移流程
- 改 UI 按钮行为

### B. 深挖 `AccountAction`
适合目标：
- 查登录服务器 / API 地址 / 校验逻辑
- 改登录 / 改会员 / 去原授权

## 13. 当前推荐
如果目标是后面真正“改里面内容”，**优先深挖 `DataController`**。
因为它是核心流程总控，把它摸清楚，后面无论改备份、上传、线程、接受时机，都会顺很多。
