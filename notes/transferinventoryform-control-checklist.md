# TransferInventoryForm 控件级改造清单（UI-only）

> 目标：不改核心业务逻辑，只重构主界面
> 原则：**保名不保形、保事件不保布局、保调用链不保旧视觉**
> 设计方向：**工业控制台 / 深色运维仪表盘 / 高信息密度**

---

## 一、改造总原则

### 必须保留
- 原事件函数绑定
- 原控件承载的数据绑定逻辑
- 原 `DataController` / `LoadAccount` / `Setting` 调用顺序
- 原按钮触发入口

### 可以大胆修改
- 窗口布局
- 分区结构
- 颜色、字体、图标
- 按钮大小与位置
- 表格列顺序与显示样式
- 日志区显示方式

### 尽量不要改
- 控件 `Name`
- 事件函数名
- `DataGridView` 对应的数据源字段名

---

## 二、建议主窗体结构

### 顶层容器
建议把主窗体拆成 5 个主容器：

```text
Form
├── pnlTopStatus          顶部状态条
├── pnlLeftMain           左侧主工作区
│   ├── pnlParams         参数面板
│   ├── pnlMaster         主库号面板
│   └── pnlBeTrade        待转号面板
├── pnlRightSide          右侧详情区
│   ├── pnlLogs           日志区
│   └── pnlDetails        详情/摘要区
└── pnlBottomActions      底部操作栏
```

---

## 三、控件级改造清单

## 1. 主窗体本体

### 建议修改
- `FormBorderStyle`：可保留原样，先不动
- `BackColor`：改为深色，例如 `#0F1115`
- `Font`：默认改为 `Microsoft YaHei UI, 9pt`
- `Text`：改为更产品化标题，例如：
  - `Spider Transfer Console`
  - `Steam Inventory Transfer Console`
  - `Steam资产转移控制台`

### 建议新增
- 程序图标（`.ico`）
- 顶部状态栏 Panel

---

## 2. 顶部状态条 `pnlTopStatus`

### 作用
显示全局运行状态，不承载业务逻辑。

### 建议新增控件
- `lblAppTitle`：标题
- `lblRunState`：运行状态
- `lblThreadSummary`：线程数摘要
- `lblMasterSummary`：主库号数量
- `lblBeTradeSummary`：待转号数量
- `lblModeSummary`：当前模式

### 推荐显示文案
- `状态：空闲`
- `线程：8`
- `主库号：5`
- `待转号：137`
- `模式：边发边接`

### 配色建议
- 背景：`#171A21`
- 状态空闲：灰蓝
- 运行中：绿色
- 错误：红色

---

## 3. 参数面板 `pnlParams`

### 需要保留/复用的原控件（尽量保名）
- `numThreadCount` / `numThread`
- `cmbTransferType`
- `cmbAcceptMode`
- `cmbItemType`
- `txtItemFilter`
- 可能还有：代理 / appid / contextid 输入框

### 建议布局
使用 2 列紧凑表单：

| 左列 | 右列 |
|------|------|
| 线程数 | 转移模式 |
| 接受方式 | 筛选方式 |
| 筛选值 | 代理/保留位 |

### 样式建议
- Label 统一浅灰字
- 输入框深色底 + 蓝色 focus 边框
- 下拉框保持高对比
- 参数区加标题：`运行参数`

### 不要改动的东西
- ComboBox 的 `SelectedIndexChanged` 事件
- NumericUpDown 的值读取逻辑
- 与 `Setting.Instance` 的同步逻辑

---

## 4. 主库号表格区 `pnlMaster`

### 尽量保留原控件名
- `dgvMaster`
- 或当前主库号列表控件名

### 建议显示列
- 账号
- SteamID
- 登录状态
- 待接受数
- 已分配数
- 上限
- maFile

### 样式建议
- 行高提高到 28~32
- 头部深色 + 半粗体
- 选中行高亮蓝边框/底色
- 登录失败行用红点或浅红背景
- 无 maFile 行用橙色标记

### 不建议做的事
- 不要先改成完全自定义 OwnerDraw 表格
- 先用原 DataGridView 把风格改干净

---

## 5. 待转号表格区 `pnlBeTrade`

### 尽量保留原控件名
- `dgvBeTrade`

### 建议显示列
- 账号
- 登录状态
- 库存数
- 可交易数
- 冷却数
- 已发报价数
- 当前状态
- maFile

### 当前状态建议统一枚举显示
- 未处理
- 登录成功
- 拉库存完成
- 已发报价
- 已确认
- 失败
- 已跳过

### 样式建议
- 支持按状态上色
- 支持按列排序（如果原本已有）
- 行首加状态色条（可选）

---

## 6. 日志区 `pnlLogs`

### 尽量保留原控件名
- `txtLog`
- 或原来的日志输出控件

### 最小改造方案
即使不重写日志模型，也建议：
- 字体改为 `Cascadia Mono`
- 背景改深色
- 文本分级着色（若难实现，至少统一高对比）
- 自动滚动到底部
- 日志区标题加：`实时日志`

### 如果允许小增强
可以新增：
- `btnClearLog`
- `chkAutoScroll`
- `cmbLogLevel`

但这些是 UI 辅助，不应影响原日志写入逻辑。

---

## 7. 详情区 `pnlDetails`

### 这是可以新增的辅助区
不直接改变业务逻辑，只显示当前选中项摘要。

### 建议显示
- 当前选中账号
- SteamID
- maFile 状态
- 最近 offerId
- 最近错误信息
- Inventory 摘要

### 推荐控件
- `Label`
- `ValueLabel`
- 小型只读文本框

### 注意
这一区可以先只做静态壳，后续再慢慢接数据。

---

## 8. 底部操作栏 `pnlBottomActions`

### 需要保留/复用的原按钮（尽量保名）
- `btnImportAccounts`
- `btnImportMaFiles`
- `btnStart`
- `btnStop`
- `btnExport`
- `btnViewInventory`

### 建议新的排列顺序
```text
[导入账号] [导入 maFile] [查看库存]        [导出结果] [停止任务] [开始转移]
```

### 按钮风格建议
- 主按钮：`btnStart`
  - 高亮蓝/绿色
  - 更宽、更醒目
- 危险/中断按钮：`btnStop`
  - 橙/红色
- 普通按钮：深灰底 + 亮边框

### 建议文案统一
- 导入账号
- 导入 maFile
- 查看库存
- 导出结果
- 停止任务
- 开始转移

---

## 四、控件名保留白名单（优先不改）

以下控件名，建议 **优先保留**：

```text
btnStart
btnStop
btnImportAccounts
btnImportMaFiles
btnExport
btnViewInventory

dgvMaster
dgvBeTrade

txtLog

cmbTransferType
cmbAcceptMode
cmbItemType

txtItemFilter
numThreadCount
```

如果原工程里名字略有差异，也遵循同一原则：
> **事件源控件名优先不改**

---

## 五、最小风险修改顺序

### 第 1 步：复制备份
- `TransferInventoryForm.cs`
- `TransferInventoryForm.Designer.cs`
- `TransferInventoryForm.resx`

### 第 2 步：先只新增容器 Panel
不要删原控件，先把原控件重新挂到新 Panel 里。

### 第 3 步：重排现有控件
- 把参数控件放进 `pnlParams`
- 把主库号列表放进 `pnlMaster`
- 把待转号列表放进 `pnlBeTrade`
- 把日志放进 `pnlLogs`
- 把按钮放进 `pnlBottomActions`

### 第 4 步：统一主题样式
- 背景色
- 按钮颜色
- 表格头样式
- 字体
- 边框

### 第 5 步：验证功能没断
逐个测试：
- 导入账号
- 导入 maFile
- 启动
- 停止
- 导出
- 查看库存
- 日志输出

### 第 6 步：再补状态条和详情区
这一步是增强，不是第一阶段必须。

---

## 六、第一阶段不要做的改动

### 先不要做
- 改按钮事件函数名
- 改 DataGridView 数据源模型
- 重写日志系统
- 改 `Setting` 字段名
- 改 `Account` 字段名
- 重写启动/停止链路

### 原因
第一阶段目标不是“重构系统”，而是：
> **换壳不换芯**

---

## 七、建议的视觉规格

### 颜色
- 背景：`#0F1115`
- 主面板：`#171A21`
- 次面板：`#1D2230`
- 文本：`#E6EAF2`
- 次文本：`#9AA4B2`
- 主高亮：`#4DA3FF`
- 成功：`#2ECC71`
- 警告：`#F5A524`
- 错误：`#FF5D5D`

### 字体
- 标题：`Bahnschrift SemiBold`
- 正文：`Microsoft YaHei UI`
- 日志：`Cascadia Mono`

### 间距
- Panel 内边距：12~16
- 控件间距：8~12
- 区块间距：10~14

### 角度
- 轻圆角即可（如果当前 UI 框架好做）
- 不要追求太强的“移动端感”

---

## 八、这份清单的实际用途

你现在可以直接拿这份清单做两件事：

### 1. 反编译出窗体后，按清单重排
适合最快落地。

### 2. 先画出新的 WinForms 线框图
适合先对齐视觉和操作流，再进 Designer。

---

## 九、下一步最值钱的延伸

接下来最适合继续的是二选一：

### A. 我继续给你写
**WinForms 线框版布局草图（文字版）**

会精确到：
- 每个 Panel 的 Dock
- 左右宽度比例
- 上中下布局关系

### B. 我继续给你写
**第一阶段改 UI 的测试清单**

会告诉你：
- 每改完一块怎么验
- 哪些功能必须回归测试
- 怎样判断“UI 改了但功能没断”

如果你要最快开始真改，我建议下一步做：
# **B. 第一阶段改 UI 的测试清单**
