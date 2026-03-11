# Spider版 Steam转库存：UI-only 改造蓝图

> 目标：**只改主界面 UI，不改核心业务逻辑**
> 核心原则：保留原有 `DataController / AccountAction / SteamGuardController / TradeOfferAction / UserController` 调用链不变，只重构 `TransferInventoryForm` 及其视觉层。

---

## 一、推荐改造方向

### 设计方向：**工业控制台 / 运维仪表盘**

这类工具不适合做成花哨炫技型 UI，最适合的是：
- 深色背景
- 高对比状态色
- 清晰分区
- 强信息密度
- 批量任务导向
- 一眼能看出：哪些账号正常、哪些失败、现在卡在哪一步

### 风格关键词
- industrial / utilitarian
- operator console
- mission control
- dark dashboard
- high-signal UI

### 不建议的方向
- 花里胡哨的炫彩渐变
- 太软萌的卡片风
- 过度圆角 + 大留白
- 过重的动画

因为这个工具本质上是：
> **高频、批量、状态驱动、任务执行型界面**

---

## 二、UI-only 改造的核心策略

### 原则 1：**控件名尽量不改**
如果原来 Designer 里有：
- `btnStart`
- `btnStop`
- `dgvMaster`
- `dgvBeTrade`
- `txtLog`
- `cmbTransferType`
- `numThread`

那么：
- **优先保留这些控件名**
- 只改位置、尺寸、颜色、字体、分组方式
- 不轻易改事件绑定

### 原则 2：**事件函数不动**
不要碰这些逻辑入口：
- `btnStart_Click`
- `btnStop_Click`
- `btnImportAccounts_Click`
- `btnImportMaFiles_Click`
- `btnExport_Click`
- `btnViewInventory_Click`

你可以：
- 改按钮样式
- 改按钮文案
- 改按钮位置

但不要先改它们背后的调用链。

### 原则 3：**先重排，再重做**
改造顺序应该是：
1. 先重排布局
2. 再改视觉
3. 最后再考虑新增辅助 UI

而不是一上来重写整个窗体。

---

## 三、建议改造范围（只动这些文件）

### 第一批可动文件
1. `TransferInventoryForm.cs`
2. `TransferInventoryForm.Designer.cs`
3. `TransferInventoryForm.resx`
4. 图标/图片资源文件

### 第二批谨慎改文件
5. `Setting`（仅在你要新增 UI 配置项时）
6. `LoadAccount`（仅当导入交互也要一起美化时）

### 暂时不要动
- `DataController`
- `TradeOfferAction`
- `AccountAction`
- `SteamGuardController`
- `UserController`

---

## 四、建议的新界面布局

下面这个布局是为了：
- 尽量不动原逻辑
- 让工具看起来像专业产品
- 保持批量操作的效率

---

## 顶层布局：三段式

```text
┌──────────────────────────────────────────────┐
│ 顶部工具栏 / 状态栏                           │
├──────────────────────────────────────────────┤
│ 左侧：账号与任务面板   │ 右侧：日志与详情面板 │
│                        │                    │
│ 参数区                 │ 实时日志           │
│ 主库号列表             │ 当前任务详情        │
│ 待转号列表             │ 库存 / 结果摘要     │
├──────────────────────────────────────────────┤
│ 底部操作栏：启动 / 停止 / 导入 / 导出 / 库存查看 │
└──────────────────────────────────────────────┘
```

---

## 五、具体分区设计

### A. 顶部状态条
用于显示：
- 当前模式
- 线程数
- 主库号数量
- 待转号数量
- 当前运行状态（空闲 / 运行中 / 停止中 / 错误）

#### 建议字段
- `状态：空闲`
- `线程：8`
- `主库号：5`
- `待转号：137`
- `模式：边发边接`

#### 视觉建议
- 深色背景 + 高亮状态徽标
- 状态颜色：
  - 空闲：灰
  - 运行中：绿
  - 停止中：橙
  - 错误：红

---

### B. 左上参数区
保留原参数控件，但做成统一仪表盘卡片：

#### 建议保留字段
- 线程数
- 转移模式
- 接受方式
- 筛选方式
- 筛选值
- 可能加：AppId / ContextId（如果后面确定需要）

#### UI 建议
- 2 列网格排列
- Label 靠左、控件靠右
- 用深色输入框 + 亮色 focus 边框
- 不要用传统 Windows 灰底老表单感

---

### C. 左中主库号列表
这块建议改成 **状态表格**，列更明确：

#### 建议列
- 账号
- SteamID
- 登录状态
- 待接受数
- 已分配数
- 上限
- maFile

#### 行状态颜色
- 正常：默认亮字
- 登录失败：红色标记
- 无 maFile：黄色提示
- 正在处理：蓝色高亮

---

### D. 左下待转号列表
这是最重要的主工作区，建议列更多一些：

#### 建议列
- 账号
- 登录状态
- 库存数
- 可交易数
- 冷却数
- 已发报价数
- 当前状态
- maFile

#### 当前状态可以统一成枚举显示
- 未处理
- 登录成功
- 拉库存完成
- 已发报价
- 已确认
- 失败
- 已跳过

这样用户一眼就能知道任务进度。

---

### E. 右侧日志区
不要只是一个普通多行文本框，建议拆成两层：

#### 1. 上层：实时日志
- 时间
- 级别（INFO/WARN/ERROR）
- 文本

#### 2. 下层：当前任务详情 / 选中账号详情
显示：
- 当前处理账号
- 最近 offerId
- 最近错误信息
- 最近一次确认结果

### 如果你暂时不想改太大
那就至少：
- 保留原日志控件
- 改字体为等宽字体（如 Cascadia Mono）
- 不同级别文本上色
- 自动滚动到底部

---

### F. 底部操作栏
建议固定 6 个主按钮：
- 导入账号
- 导入 maFile
- 启动任务
- 停止任务
- 导出结果
- 查看库存

#### 交互规则
- 启动后：
  - `启动` 禁用
  - `停止` 启用
- 停止中：
  - `停止` 显示“停止中...”
- 空闲时：
  - `启动` 高亮主按钮

#### 文案建议
把原来偏生硬的按钮文案改得统一一点：
- `导入账号`
- `导入 maFile`
- `开始转移`
- `停止任务`
- `导出结果`
- `查看库存`

---

## 六、建议保留不变的交互链

为了确保功能不变，这些交互链必须原样保留：

### 启动链
```text
UI参数读取
  -> Setting.Instance 更新
  -> 组装 TransferConfig
  -> DataController.StartTransfer(...)
```

### 停止链
```text
btnStop_Click
  -> 取消标志 / CancellationToken
  -> DataController 停止后续分派
```

### 导入账号链
```text
TransferInventoryForm
  -> LoadAccount
  -> List<Account>
  -> 绑定到列表
```

### 导入 maFile 链
```text
选择目录
  -> MatchMaFile
  -> 更新 Account.MafilesPath
  -> 刷新列表状态
```

### 库存查看链
```text
读取当前选中账号
  -> RefreshAccount / GetInventory
  -> 展示库存
```

---

## 七、最小风险改造步骤

### Step 1：冻结业务逻辑
先不要碰：
- `DataController`
- `TradeOfferAction`
- `AccountAction`
- `SteamGuardController`
- `UserController`

### Step 2：备份原窗体
复制：
- `TransferInventoryForm.cs`
- `TransferInventoryForm.Designer.cs`
- `TransferInventoryForm.resx`

做一个 `TransferInventoryForm.backup.*`

### Step 3：只重排布局
先做：
- Panel 分区
- 按钮位置调整
- 列表区域重排
- 日志区位置优化

### Step 4：再改视觉层
再做：
- 主题色
- 字体
- 图标
- 分组标题
- 表格样式

### Step 5：验证事件没断
重点验证：
- 所有按钮还能点
- 启动后逻辑照跑
- 导入后列表仍更新
- 日志仍输出
- 停止仍有效

### Step 6：最后再考虑补充 UI 辅助信息
比如：
- 任务统计栏
- 账号状态徽标
- 当前选中账号详情面板

---

## 八、推荐的控件保留策略

### 尽量保留控件名
例如：
- `btnStart`
- `btnStop`
- `btnImportAccounts`
- `btnImportMaFiles`
- `btnExport`
- `btnViewInventory`
- `dgvMaster`
- `dgvBeTrade`
- `txtLog`
- `cmbTransferType`
- `cmbAcceptMode`
- `cmbItemType`
- `txtItemFilter`
- `numThreadCount`

### 可以改的
- `Text`
- `BackColor`
- `ForeColor`
- `Font`
- `Dock`
- `Anchor`
- `Size`
- `Location`

### 谨慎改的
- `Name`
- `Click += ...`
- `SelectedIndexChanged += ...`
- 数据绑定字段名

---

## 九、建议的配色方案（工业控制台风）

### 主色板
- 背景：`#0F1115`
- 面板：`#171A21`
- 次级面板：`#1D2230`
- 主文本：`#E6EAF2`
- 次文本：`#9AA4B2`
- 高亮蓝：`#4DA3FF`
- 成功绿：`#2ECC71`
- 警告橙：`#F5A524`
- 错误红：`#FF5D5D`

### 字体建议
- 标题：`Bahnschrift SemiBold` / `DIN`
- 正文：`Microsoft YaHei UI` / `Segoe UI`
- 日志：`Cascadia Mono`

这样做出来会更像：
- 运维面板
- 批量控制台
- 半专业桌面工具

而不是普通小脚本工具。

---

## 十、如果要更进一步，但仍不动功能

在 UI-only 范围内，你还可以做这些升级：

### 1. 增加状态徽标系统
- 已登录
- 缺 maFile
- 有库存
- 已发单
- 确认完成
- 失败

### 2. 增加顶部摘要卡片
- 主库号数
- 待转号数
- 当前进度
- 今日成功数
- 失败数

### 3. 增加选中账号详情面板
点击账号后右侧显示：
- SteamID
- maFile 状态
- Inventory 摘要
- 最近报价 ID
- 最近错误

这些都属于 **UI 增强**，不用动核心交易逻辑。

---

## 十一、这条路线的结论

如果你的目标是：

> **先在原基础上重构 exe，主要改 UI，功能暂时不动**

那么最优路线就是：

# **保留后端 100% 不动，只改 `TransferInventoryForm` 及资源层**

这是当前：
- 成功率最高
- 成本最低
- 最快能出结果
- 最适合你现在阶段

的路线。

---

## 十二、下一步可以直接做什么

接下来最值钱的是二选一：

### A. 我继续给你写一份
**`TransferInventoryForm` 控件级改造清单**

会具体到：
- 哪些 Panel
- 哪些按钮
- 哪些表格
- 哪些控件必须保名
- 哪些地方可以换布局

### B. 我直接给你出一版
**新的主界面原型结构图（文字线框版）**

也就是按 WinForms 的实际布局给出：
- 左右分栏
- 顶部状态条
- 中部列表
- 右侧日志
- 底部操作栏

如果你要最快落地，我建议下一步直接做：
# **A. 控件级改造清单**
