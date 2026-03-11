# TransferInventoryForm WinForms 文字线框图

> 目标：把 Spider 版 Steam 转库存主界面落成可直接照着改 Designer 的结构图
> 原则：UI-only 改造，不动核心业务逻辑；保留原控件对象与事件绑定
> 风格方向：工业控制台 / 深色运维仪表盘

---

## 一、建议窗体尺寸

### 默认尺寸
- `Width = 1600`
- `Height = 980`
- `StartPosition = CenterScreen`
- `MinimumSize = 1360 x 820`

### 原因
这个工具信息密度高：
- 左边要放两张账号表
- 右边要放日志和详情
- 上面要放参数
- 下面要放操作栏

如果尺寸太小，会直接把它做成“挤作一团的旧工具感”。

---

## 二、顶层 Dock 结构

主窗体建议按这个顺序 Dock：

```text
Form
├── pnlTopStatus        Dock = Top      Height = 56
├── pnlBottomActions    Dock = Bottom   Height = 72
├── pnlMain             Dock = Fill
    ├── pnlLeftMain     Dock = Fill
    └── pnlRightSide    Dock = Right    Width = 420
```

### 说明
- 右侧固定 420 宽，专门做日志和详情
- 左侧吃剩余空间，承载参数区 + 两张大表
- 顶部状态条和底部按钮条固定高度，避免缩放时混乱

---

## 三、顶部状态条布局

### 容器
- `pnlTopStatus`
- `Dock = Top`
- `Padding = 16, 10, 16, 10`

### 内部结构
```text
pnlTopStatus
├── lblAppTitle         左对齐
├── lblRunState         左侧中段
├── lblThreadSummary    右侧摘要1
├── lblMasterSummary    右侧摘要2
├── lblBeTradeSummary   右侧摘要3
└── lblModeSummary      右侧摘要4
```

### 推荐排布
- `lblAppTitle`：左上主标题，例如 `Steam 资产转移控制台`
- `lblRunState`：紧跟标题右侧，例如 `状态：空闲`
- 右边一排摘要状态：
  - `线程：8`
  - `主库号：5`
  - `待转号：137`
  - `模式：边发边接`

### WinForms 实现建议
- 标题用 `Label`
- 摘要也都用 `Label`
- 不必复杂自绘，先靠字体/颜色做层级

---

## 四、主内容区布局

### 顶层结构
```text
pnlMain
├── pnlLeftMain   Dock = Fill
└── pnlRightSide  Dock = Right, Width = 420
```

---

## 五、左侧主工作区布局

左侧建议分成 3 段：

```text
pnlLeftMain
├── pnlParams     Dock = Top     Height = 150
├── pnlMaster     Dock = Top     Height = 250
└── pnlBeTrade    Dock = Fill
```

### 这样分的原因
- 参数区高度固定，避免压缩
- 主库号区相对较小，因为主库号数量通常少
- 待转号区最大，因为它是主工作区

---

## 六、参数区线框图

### 容器
- `pnlParams`
- `Padding = 12`

### 内部推荐布局
做成 2 行 3 列的参数栅格：

```text
┌─────────────────────────────────────────────┐
│ 运行参数                                     │
│                                             │
│ [线程数]      [转移模式]      [接受方式]     │
│ [筛选方式]    [筛选值]        [保留位/代理]  │
└─────────────────────────────────────────────┘
```

### 具体控件落位建议

#### 第一行
- 左：`lblThread` + `numThreadCount`
- 中：`lblTransferType` + `cmbTransferType`
- 右：`lblAcceptMode` + `cmbAcceptMode`

#### 第二行
- 左：`lblItemType` + `cmbItemType`
- 中：`lblItemFilter` + `txtItemFilter`
- 右：预留：`txtProxy` 或状态说明 Label

### 布局实现建议
- 每组用一个小 Panel 包住
- Label 在上，控件在下（比传统左字右框更现代）
- 每组宽度平均分配

---

## 七、主库号区线框图

### 容器
- `pnlMaster`
- `Padding = 12, 0, 12, 8`

### 结构
```text
┌─────────────────────────────────────────────┐
│ 主库号 / 接收端                              │
│ [状态说明 / 小按钮可选]                      │
│                                             │
│  dgvMaster                                  │
│                                             │
└─────────────────────────────────────────────┘
```

### 组成
- 顶部标题 Label：`主库号 / 接收端`
- 中间可选工具条（可先不做）
- 下方主表格：`dgvMaster`

### 高度建议
- 总高约 250
- 标题 28
- 表格填满剩余空间

---

## 八、待转号区线框图

### 容器
- `pnlBeTrade`
- `Padding = 12, 0, 12, 12`

### 结构
```text
┌─────────────────────────────────────────────┐
│ 待转号 / 发货端                              │
│ [状态说明 / 小按钮可选]                      │
│                                             │
│  dgvBeTrade                                 │
│                                             │
└─────────────────────────────────────────────┘
```

### 组成
- 顶部标题 Label：`待转号 / 发货端`
- 下方主表格：`dgvBeTrade`

### 高度策略
- `Dock = Fill`
- 保证这是左侧最大的区域

---

## 九、右侧日志与详情区布局

右侧固定 420 宽，建议分成上下两块：

```text
pnlRightSide
├── pnlLogs     Dock = Fill
└── pnlDetails  Dock = Bottom   Height = 220
```

### 原因
- 日志永远是主反馈区，要最大
- 详情区作为辅助，不需要太高

---

## 十、日志区线框图

### 容器
- `pnlLogs`
- `Padding = 8, 8, 12, 8`

### 结构
```text
┌──────────────────────────────┐
│ 实时日志        [清空] [滚动] │
│                              │
│  txtLog                      │
│                              │
│                              │
└──────────────────────────────┘
```

### 组成
- 顶部标题栏：
  - `lblLogTitle`
  - `btnClearLog`（可新增）
  - `chkAutoScroll`（可新增）
- 主日志控件：`txtLog`

### 重点
- `txtLog` 原对象尽量保留
- 只是放进新容器中并重设样式

---

## 十一、详情区线框图

### 容器
- `pnlDetails`
- `Padding = 8, 0, 12, 12`

### 结构
```text
┌──────────────────────────────┐
│ 当前详情                      │
│                              │
│ 账号：xxxxx                   │
│ SteamID：xxxxx               │
│ maFile：已绑定 / 缺失         │
│ 最近Offer：xxxxx             │
│ 最近错误：xxxxx              │
│ 库存摘要：总数 / 可交易 / 冷却 │
└──────────────────────────────┘
```

### 说明
- 第一阶段可以只放静态字段壳
- 后续再接选中行变化逻辑

---

## 十二、底部操作栏线框图

### 容器
- `pnlBottomActions`
- `Dock = Bottom`
- `Padding = 16, 12, 16, 12`

### 推荐按钮布局
```text
[导入账号] [导入 maFile] [查看库存]         [导出结果] [停止任务] [开始转移]
```

### WinForms 布局建议
- 左边 3 个普通按钮左对齐
- 右边 3 个按钮右对齐
- `btnStart` 最宽、最醒目
- `btnStop` 次重点

### 宽度建议
- 普通按钮：110~130
- `btnStop`：120~140
- `btnStart`：150~180

---

## 十三、具体 Dock / Anchor 建议

### 核心原则
- 顶部、底部固定高度用 `Dock`
- 左右主区用 `Dock`
- 表格、日志尽量 `Dock = Fill`
- 输入控件在小 Panel 内用 `Anchor = Left | Right`

### 不推荐
- 全靠 `Location/Size` 硬摆
- 不设置 Anchor 导致拉伸后全部错位

---

## 十四、控件接入顺序建议

### 第一步：只搭 Panel 壳
先把：
- `pnlTopStatus`
- `pnlBottomActions`
- `pnlMain`
- `pnlLeftMain`
- `pnlRightSide`
- `pnlParams`
- `pnlMaster`
- `pnlBeTrade`
- `pnlLogs`
- `pnlDetails`

加出来。

### 第二步：把旧控件塞进去
把原来的：
- `numThreadCount`
- `cmbTransferType`
- `cmbAcceptMode`
- `cmbItemType`
- `txtItemFilter`
- `dgvMaster`
- `dgvBeTrade`
- `txtLog`
- `btnImportAccounts`
- `btnImportMaFiles`
- `btnViewInventory`
- `btnExport`
- `btnStop`
- `btnStart`

重新挂进新容器。

### 第三步：再改配色和字体

### 第四步：跑回归测试
使用：
- `ui-first-pass-regression-checklist.md`
- `control-name-event-mapping.md`

---

## 十五、推荐视觉层级

### 标题
- `Bahnschrift SemiBold 12~14pt`

### 分区标题
- `Microsoft YaHei UI Semibold 10pt`

### 表格
- 正文 `9pt`
- 行高 28~32

### 日志
- `Cascadia Mono 9pt`

### 按钮
- `Microsoft YaHei UI 9pt`

---

## 十六、第一阶段不要做的事

- 不要改成多窗体导航
- 不要把 DataGridView 换成第三方控件
- 不要重写日志模型
- 不要把按钮拆成太多二级操作
- 不要加入太复杂的动画

第一阶段目标只有一个：
> **把旧工具做成新控制台壳，同时功能完全不变。**

---

## 十七、这张线框图的实际用途

你现在可以直接拿这份文档：

1. 去 WinForms Designer 里按 Panel 结构重排
2. 保留原控件对象不动
3. 逐区替换样式
4. 跑回归清单

做到这一步，基本就完成了 UI-only 改造的 70%。

---

## 十八、下一步最值钱的延伸

接下来最适合继续的是二选一：

### A. 我继续给你写
**高概率 `Setting.json` 样例**

这样你后面就能让新 UI、配置、引擎第一次完整对上。

### B. 我继续给你写
**UI 改造施工顺序手册（按 1~10 步）**

如果你要立刻开改，我建议下一步做：
# **B. UI 改造施工顺序手册**
