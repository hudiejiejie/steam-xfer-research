# DataController 深挖：函数级入口与可修改点

> 基于已提取的 `SteamInventoryManager.dll` 与 48 份逆向材料整理
> 注意：部分方法名为推测或常见命名，真实开发需以反编译结果为准

---

## 一、DataController 概览

- 命名空间：`SteamInventoryManager.Model`
- 类：`DataController`
- 角色：核心流程总控，协调账号、库存、报价、接受的完整生命周期
- 单例或持有全局 `Setting.Instance`

---

## 二、公开 API（推测）

### 1. 刷新账号状态与库存
```csharp
Task<AccountRefreshResult> RefreshAccountAsync(Account account)
// 或
AccountStatus RefreshAccount(Account account)
```
功能：
- 登录指定 `account`
- 获取 `TradeToken` 并更新 `account.TradeToken`、`account.TradeOfferUrl`
- 拉取当前已发送报价（Sent Trade Offers）和待接受报价（Received Trade Offers）
- 拉取库存（`user.ArchiWebHandler.GetInventory(...)`）
- 统计：总物品数、可交易数、冷却数
- 更新 `account.Inventory`, `account.SentTradeIDs`, `account.BeTradeOfferIDs`

**可修改点**：
- 拉库存前插入备份逻辑
- 统计时加入自定义指标（如稀有物品标记）
- 调整 `contextId`（753/2/16）的映射规则

---

### 2. 主库号：接受报价
```csharp
Task AcceptOffersAsync(Account master, IEnumerable<string> offerIds, bool require2FA = true)
// 或
void AcceptBeTradeOffers(Account master)
```
功能：
- 登录主库号
- 从 `account.BeTradeOfferIDs` 读取待接受报价 ID
- 循环调用接受接口（可能是 `user.ArchiWebHandler.AcceptTradeOffer(...)`）
- 若触发手机确认，调用 `SteamGuardController.HandleTwoFactorAuthenticationConfirmations(accept: true)`

**可修改点**：
- 接受间隔控制（防封）
- 失败重试策略
- 2FA 确认方式替换（如自动邮件确认）
- 接受后触发自定义回调（如记录到数据库）

---

### 3. 待转号：撤回已发报价
```csharp
Task CancelSentOffersAsync(Account beTrade, IEnumerable<string> offerIds)
// 或
void WithdrawSentOffers(Account beTrade)
```
功能：
- 登录待转号
- 读取 `account.SentTradeIDs`
- 批量取消报价（调用 `user.ArchiWebHandler.CancelTradeOffer(...)`）

**可修改点**：
- 撤回条件（只撤回特定物品、保留某些报价）
- 撤回频率限制
- 撤回后清理本地记录逻辑

---

### 4. 完整转库存总流程
```csharp
Task RunTransferAsync(TransferConfig config)
// 或
void StartTransfer(TransferConfig config)
```
参数示例 (`TransferConfig`)：
- `MasterAccounts`：主库号列表
- `BeTradeAccounts`：待转号列表
- `MaxThreads`：并发数（来自 `Setting.Instance.ThreadCount`）
- `TransferType`：0=边发边接受，1=全部发完再统一接受
- `ItemFilter`：物品筛选规则（类型/名称/最小数量等）
- `PerMasterLimit`：每个主库号最大接收物品数

内部流程：
1. 并发登录待转号（`AccountAction.Login(...)`，重试最多 3 次，指数退避）
2. 为每个待转号拉库存并筛选
3. 主库号剩余额度分配算法（按可交易数量、按优先级）
4. 调 `user.ArchiWebHandler.SendInventory(...)` 发起报价
   - 同时更新 `account.SentTradeIDs`
5. 若 `TransferType == 0`：边发边接受（主库号并行接受）
   - 若 `TransferType == 1`：全部发完后统一接受
6. 最终汇总结果（成功/失败数量，错误信息）

**可修改点**：
- 并发模型（改为队列/批量）
- 分配算法（优先某些主库号、按物品类型分流）
- 超时与重试策略
- 拆单逻辑（255 item 限制）
- 上传备份时机（发起报价前/后、接受后）
- 持久化状态到 `gloabl.db` 或远程服务器

---

## 三、关键私有/内部方法（推测）

### 1. 登录与令牌
```csharp
Task EnsureLoggedInAsync(Account account)
Task<string> GetTradeTokenAsync(Account account)
Task<string> GenerateTradeOfferUrlAsync(Account account)
```
- 重用 `AccountAction.Login`
- maFile 路径：`account.MafilesPath`
- 2FA 通过 `SteamGuardController` 生成验证码

---

### 2. 库存与筛选
```csharp
Task<Inventory> FetchInventoryAsync(Account account, int appId = 753, int contextId = 6)
bool ItemMatches(Item item, ItemFilter filter)
```
- `contextId`：753→6，其他 appid 常见 2/16
- `ItemFilter`：`itemType` (0=类型, 1=名称精确), `value` (如 "匕首" 或 "AK-47")

---

### 3. 报价操作
```csharp
Task<string> SendTradeOfferAsync(Account sender, Account receiver, IEnumerable<Item> items, string message = "")
Task CancelTradeOfferAsync(Account sender, string offerId)
Task<IEnumerable<TradeOffer>> GetSentOffersAsync(Account account)
Task<IEnumerable<TradeOffer>> GetReceivedOffersAsync(Account account)
```
- 实际可能封装在 `TradeOfferAction` 或 `user.ArchiWebHandler`
- 发送前会检查 `trade token` 有效性

---

### 4. 接受与确认
```csharp
Task AcceptTradeOfferAsync(Account receiver, string offerId)
Task ConfirmTradeOfferAsync(Account receiver, string offerId)
Task HandleTwoFactorAuthenticationConfirmations(bool accept)
```
- 接受后可能需要 `mobileconf` 确认
- 该方法在 `SteamGuardController` 或类似类中

---

## 四、调用链举例

### 用户点击“启动” → 主线程启动 → 调度
```
TransferInventoryForm.btnStart_Click(...)
  -> DataController.StartTransfer(config)
      -> Parallel.ForEach(beTradeAccounts, account => LoginAndPrepare(account))
      -> ForEach master: AssignItems(master, totalItems)
      -> ForEach (master, items): SendInventoryAsync(master, items)
      -> If TransferType==0: Parallel Accept loop
         Else: AfterAllSending: AcceptAll()
```

### 接受报价（主库号）
```
DataController.AcceptBeTradeOffers(master)
  -> master.Login()
  -> var offers = GetReceivedOffers(master)
  -> foreach offer: AcceptTradeOfferAsync(master, offer.Id)
      -> if need 2FA: SteamGuardController.HandleTwoFactorAuthenticationConfirmations(true)
```

---

## 五、最值得修改的“插点”总结

| 插点位置 | 适合修改 | 风险 |
|----------|----------|------|
| `RefreshAccountAsync` 开头 | 加自动备份（账号/库存状态） | 低 |
| `RefreshAccountAsync` 结尾 | 上传统计到远程服务器 | 低 |
| `FetchInventoryAsync` 返回后 | 筛选前插入自定义过滤（黑名单/白名单） | 低 |
| `SendTradeOfferAsync` 调用前 | 修改发送内容、附加消息、控制拆单 | 中 |
| `SendInventoryAsync` 外层循环 | 插入每单发送后的回调（记录到 DB） | 低 |
| `AcceptTradeOfferAsync` 前后 | 修改接受间隔、失败重试、2FA 替代方案 | 中 |
| `StartTransfer` 开始 | 前置检查（账号可用性、库存是否充足） | 低 |
| `StartTransfer` 结束 | 发送完成通知（邮件/Telegram） | 低 |
| `AccountAction.Login` | 替换登录服务器地址、绕过授权（需逆向） | 高 |

---

## 六、建议下一步

1. 使用 dnSpyEx 或 ILSpy 打开 `SteamInventoryManager.dll`
2. 导航到 `SteamInventoryManager.Model.DataController`
3. 导出方法清单（签名）并比对本文推测
4. 重点标注 `private`/`internal` 的可覆盖方法，评估是否需要 patching 或屑接
5. 确定“自动备份”和“上传服务器”的注入位置（推荐：`RefreshAccountAsync` 结尾 和 `StartTransfer` 结束）

---

## 七、示例：在 RefreshAccountAsync 结尾注入备份

```csharp
// 伪代码：在 RefreshAccountAsync 成功返回前
try {
    var result = original_RefreshAccountAsync(account);
    // 备份点
    BackupAccountState(account, result);
    return result;
} catch (Exception ex) {
    Log.Error(ex);
    throw;
}
```

BackupAccountState 可序列化 `account` 的关键字段及 `result.InventorySnapshot` 到本地 JSON 或远程 API。

---

如需，我可以继续为 `TradeOfferAction` 和 `UserController` 做类似的深挖。
