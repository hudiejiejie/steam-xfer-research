# UI 改造施工顺序手册（1~10 步）

> 目标：把 Spider 版 Steam 转库存工具从“旧壳”稳定改成“新壳”，并把风险压到最低
> 范围：仅限 `TransferInventoryForm` UI-only 改造
> 原则：**每一步都可停、可测、可回滚**

---

## Step 1：冻结当前可用版本

### 目标
在动任何 Designer 之前，先把“现在能跑的状态”固定住。

### 操作
- 备份：
  - `TransferInventoryForm.cs`
  - `TransferInventoryForm.Designer.cs`
  - `TransferInventoryForm.resx`
- 做一次 git commit
- 运行原版，确认：
  - 能启动
  - 导入账号可用
  - 导入 maFile 可用
  - 启动/停止可用
  - 日志有输出

### 验收
- [ ] 存在可回滚版本
- [ ] 有原版运行截图

---

## Step 2：只加骨架 Panel，不动原控件

### 目标
先把新布局骨架搭起来，但不动旧控件逻辑。

### 操作
新增这些容器：
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

### 注意
- 这一步只加容器，不删任何旧控件
- 不改控件事件
- 不改控件名

### 验收
- [ ] 程序能编译
- [ ] 程序能打开
- [ ] 没有 Designer 崩溃

---

## Step 3：把旧控件搬进新容器

### 目标
完成“旧控件挂新壳”。

### 操作顺序
先搬这些控件：
1. 参数控件
2. 两个 DataGridView
3. 日志控件
4. 底部按钮

### 建议搬运表
- 参数控件 → `pnlParams`
- `dgvMaster` → `pnlMaster`
- `dgvBeTrade` → `pnlBeTrade`
- `txtLog` → `pnlLogs`
- 主按钮 → `pnlBottomActions`

### 注意
- 搬的是“原对象”，不是重建新对象
- 只改 `Parent` / `Location` / `Dock`

### 验收
- [ ] 所有旧控件还在
- [ ] 按钮看得见
- [ ] 表格看得见
- [ ] 日志区看得见

---

## Step 4：先做布局，不做美化

### 目标
先确认新结构能用，再谈视觉。

### 操作
- 设置 Dock / Anchor
- 调整各区块尺寸
- 保证左右、上下比例合理

### 推荐比例
- 顶部状态条：56 高
- 底部操作栏：72 高
- 右侧日志区：420 宽
- 参数区：150 高
- 主库号区：250 高
- 待转号区：填满剩余

### 验收
- [ ] 拉伸窗口时不明显错位
- [ ] 日志区不会被压缩为 0
- [ ] 两张表格高度正常
- [ ] 参数区不挤压

---

## Step 5：跑第一轮功能回归

### 目标
在完全不改视觉前，确认新布局没把功能搞断。

### 必测
- [ ] 导入账号
- [ ] 导入 maFile
- [ ] 启动任务
- [ ] 停止任务
- [ ] 日志追加
- [ ] 查看库存
- [ ] 导出结果

### 重点检查
- 按钮是否有响应
- 日志是否还能写入 `txtLog`
- 表格是否还能刷新数据

### 通过标准
只要功能都还在，就说明新壳骨架成功了。

---

## Step 6：统一主题样式

### 目标
把“旧工具感”去掉，换成工业控制台风格。

### 操作
改：
- Form 背景
- Panel 背景
- 按钮颜色
- 表格头样式
- 文本颜色
- 字体

### 推荐色板
- 背景：`#0F1115`
- 面板：`#171A21`
- 次面板：`#1D2230`
- 文本：`#E6EAF2`
- 蓝：`#4DA3FF`
- 绿：`#2ECC71`
- 橙：`#F5A524`
- 红：`#FF5D5D`

### 验收
- [ ] 所有文字可读
- [ ] DataGridView 选中行清晰
- [ ] 按钮状态明显
- [ ] 中文字体不截断

---

## Step 7：补顶部状态条

### 目标
把“工具”升级成“控制台”。

### 操作
新增展示控件：
- `lblAppTitle`
- `lblRunState`
- `lblThreadSummary`
- `lblMasterSummary`
- `lblBeTradeSummary`
- `lblModeSummary`

### 注意
这一步先做静态壳，不一定马上接真数据。

### 验收
- [ ] 顶部状态条显示正常
- [ ] 不影响下方布局

---

## Step 8：补右侧详情区

### 目标
给用户一个“当前上下文”观察面板。

### 操作
先放静态字段壳：
- 当前账号
- SteamID
- maFile 状态
- 最近 offerId
- 最近错误
- 库存摘要

### 注意
第一阶段不强求全接上数据，先把位置和结构定住。

### 验收
- [ ] 右侧详情区结构稳定
- [ ] 不压缩日志区

---

## Step 9：跑完整回归测试

### 目标
确认“新壳 + 旧芯”真成立。

### 使用文档
- `ui-first-pass-regression-checklist.md`
- `control-name-event-mapping.md`

### 完整验收项
- [ ] 程序正常启动
- [ ] 参数区正常读取
- [ ] 导入账号正常
- [ ] 导入 maFile 正常
- [ ] 启动正常
- [ ] 停止正常
- [ ] 日志正常
- [ ] 表格正常
- [ ] 导出结果正常
- [ ] 查看库存正常

---

## Step 10：提交到 git 仓库

### 目标
每完成一个稳定阶段就同步一次，不要把大量 Designer 改动攒到最后。

### 建议提交节奏
#### 提交 1
`ui: add new shell panels for TransferInventoryForm`

#### 提交 2
`ui: move existing controls into new dashboard layout`

#### 提交 3
`ui: apply dark operator-console theme`

#### 提交 4
`ui: add status bar and details panel`

### 原因
这样后面如果某一步出了问题，你可以精确回滚，而不是整坨返工。

---

## 最后的建议

如果你要我来实际落地，最优策略不是继续写分析文档，而是：

1. 先把 **可编辑工程**（反编译导出的 csproj 或现成源码）放进仓库
2. 我直接按上面 1~10 步开始改
3. 每完成一阶段就 commit / push

也就是说，

> **下一步最应该做的，不是再分析，而是把“可改项目源代码”拉进仓库。**
