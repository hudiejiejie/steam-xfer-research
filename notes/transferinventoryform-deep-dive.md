# TransferInventoryForm 深挖：主界面交互图 / 按钮行为 / 与后端调用链的对齐

> 目标：把 Spider 版 Steam 转库存工具的主界面拆成“可复刻的产品交互图”
> 说明：基于现有 48 份逆向材料与前序笔记推导，控件名与事件名需以后续反编译校正

---

## 一、为什么现在该挖 `TransferInventoryForm`

前面我们已经把这个项目最重要的底层几乎都拆出来了：

- `Setting` —— 配置总线
- `Account` —— 数据骨架
- `LoadAccount` —— 数据导入入口
- `AccountAction` —— 登录链
- `SteamGuardController` —— 2FA 与确认链
- `TradeOfferAction` —— 交易动作链
- `DataController` —— 总调度链

但如果要真正做出一个“一样的工具”，
还缺最后一张图：

> **用户是怎么操作这个系统的。**

这张图就藏在 `TransferInventoryForm` 里。

它决定：
- 用户先点什么
- 哪些参数从 UI 进入 `Setting`
- 哪些按钮会触发 `DataController`
- 日志怎么显示
- 列表怎么刷新
- 启动/停止如何贯穿到业务层

换句话说，`TransferInventoryForm` 不是简单窗体，
而是整个产品的 **控制面板**。

---

## 二、现有材料已确认的主界面功能区

根据已有笔记，`SteamAutoLogin.TransferInventoryForm` 主界面至少包含：

1. 主库号列表（Master）
2. 待转号列表（BeTrade）
3. 日志区
4. 物品筛选
5. 线程数
6. 转移模式
7. 主库号接受方式
8. 导入账号
9. 导入 maFiles
10. 启动 / 停止
11. 导出结果
12. 库存查看

这已经足够反推出：

- 它不是极简工具
- 而是“**批量任务控制台**”
- 用户可以在一个主窗体里完成：
  - 数据导入
  - 参数调整
  - 执行任务
  - 查看日志
  - 查看结果

---

## 三、主界面的角色定位

如果从产品架构角度看，`TransferInventoryForm` 其实承担了 4 种角色：

### 1. 配置面板
用户通过它设置：
- 线程数
- 模式
- 筛选规则
- 接受方式

### 2. 数据入口
用户通过它：
- 导入账号
- 导入 `.maFile`
- 管理主库号与待转号列表

### 3. 任务控制器
用户通过它：
- 启动转库存
- 停止任务
- 触发撤回/接受等操作

### 4. 状态看板
用户通过它：
- 观察日志
- 查看库存
- 查看任务结果

### 结论
这不是普通 CRUD 窗口，而是一个：

> **批量任务调度控制台 + 状态监控界面**

---

## 四、最可能的界面结构（可复刻）

根据功能区推测，主界面大概率可以拆成这几个区块：

---

### A. 顶部参数区

可能包含：
- `线程数` NumericUpDown
- `转移模式` ComboBox
- `主库号接受方式` ComboBox
- `物品筛选方式` ComboBox
- `物品筛选值` TextBox
- 可能还有：appId/contextId 或代理输入

#### 作用
- 初始化时从 `Setting.Instance` 读取
- 用户修改后实时更新 `Setting.Instance`
- 启动任务时，组合成 `TransferConfig`

---

### B. 左侧主库号区

可能是一个 `DataGridView` / `ListView`

显示字段可能包括：
- 账号名
- SteamID
- 当前登录状态
- 待接受报价数
- 接收上限 / 当前已分配数量

#### 作用
- 显示可作为“接收端”的账号
- 可能支持手动勾选/排序
- 供 `DataController` 做任务分配

---

### C. 右侧待转号区

可能也是 `DataGridView` / `ListView`

显示字段可能包括：
- 账号名
- 是否有 maFile
- 登录状态
- 库存数量
- 已发报价数量
- 当前任务状态

#### 作用
- 显示可作为“发货端”的账号
- 是批量转库存的主工作区

---

### D. 底部日志区

大概率是多行文本框 / ListBox

显示内容可能包括：
- 登录成功/失败
- 报价发送结果
- 接受成功/失败
- 手机确认结果
- 撤回结果
- 错误信息

#### 作用
- 是用户判断任务是否正常进行的主要反馈面板
- 很可能由 `DataController` / `TradeOfferAction` 回调写入

---

### E. 操作按钮区

已知/高概率按钮：
- 导入账号
- 导入 maFile
- 启动
- 停止
- 导出结果
- 查看库存
- 可能还有：撤回报价 / 刷新账号 / 清空日志

---

## 五、按钮行为的高概率事件链

这部分最重要，因为这是后面复刻主界面的关键蓝图。

---

### 1. 导入账号按钮

```text
btnImportAccounts_Click
  -> 打开 LoadAccount
  -> ImportFromFile / ImportFromClipboard
  -> 返回 List<Account>
  -> 更新待转号 / 主库号列表数据源
  -> 刷新 UI
```

#### 备注
有可能：
- 主库号与待转号使用同一导入入口
- 再由用户在 UI 中分配角色

也可能：
- 分成“导入主库号”和“导入待转号”两套按钮

---

### 2. 导入 maFile 按钮

```text
btnImportMaFiles_Click
  -> 选择目录
  -> 遍历 *.maFile
  -> 按 账号名 + .maFile 规则匹配
  -> 写回对应 Account.MafilesPath
  -> 刷新列表状态（是否有 maFile）
```

#### 作用
- 不是单独导入数据，而是给已有账号补安全资产
- 这是从“账号档案”进入“可登录账号”的关键一步

---

### 3. 启动按钮

```text
btnStart_Click
  -> 从 UI 读取线程数/模式/筛选条件
  -> 更新 Setting.Instance
  -> 组装 TransferConfig
  -> 调 DataController.StartTransfer(config)
  -> 按钮状态切换：开始禁用 / 停止启用
```

#### 这是整个主界面的核心按钮
它是：
> **UI → 总调度层** 的总入口

---

### 4. 停止按钮

```text
btnStop_Click
  -> 设置取消标志 / CancellationToken
  -> 请求 DataController 停止后续任务分派
  -> 可能等待当前报价发送完成
  -> 刷新 UI 状态
```

#### 注意
停止通常不是立刻 kill，而更像：
- 终止后续任务
- 当前已发动作可能继续清尾

如果原版实现得比较稳，停止后可能还会：
- 撤回未完成报价
- 记录中断原因

---

### 5. 导出结果按钮

```text
btnExport_Click
  -> 收集当前账号列表 / 结果状态
  -> 导出成 txt / csv / json
  -> 保存到 ExportPath
```

#### 可能导出的内容
- 成功转移的账号
- 失败账号
- 缺失 maFile 账号
- 当前库存统计

---

### 6. 查看库存按钮

```text
btnViewInventory_Click
  -> 读取当前选中账号
  -> 调 DataController.RefreshAccount / UserController.GetInventory
  -> 弹窗显示库存明细 或 更新某个明细面板
```

#### 作用
- 帮用户验证账号库存是否正常
- 也可能用于调试筛选规则

---

## 六、日志区与后端的关系

日志区是主界面里最能暴露真实调用链的地方。

### 高概率日志来源

#### 1. `AccountAction`
- 登录成功/失败
- 验证码生成失败
- Session 失效

#### 2. `SteamGuardController`
- maFile 缺失
- 确认成功/失败

#### 3. `TradeOfferAction`
- 报价发送成功，返回 offerId
- 报价撤回成功/失败
- 接受报价成功/失败

#### 4. `DataController`
- 当前处理哪个待转号
- 分配给哪个主库号
- 当前进度 / 任务结束

### 这意味着
主界面的日志区，其实是：
> **多个控制器共同写入的统一状态总线**

如果你后面自己做版本，建议直接抽一个：
- `ILogSink`
- `TaskEventBus`

不要让各控制器直接操作 WinForms 控件。

---

## 七、UI 与 `Setting` 的绑定关系

前一轮我们已经拆过 `Setting`，这里把它和主界面真正对齐一次：

| UI 控件 | Setting 字段 | 后端影响 |
|--------|--------------|----------|
| 线程数 | `ThreadCount` | 控制并发登录/发送 |
| 转移模式 | `TransferType` | 决定边发边接 / 全发后接 |
| 接受方式 | `AcceptMode` | 决定主库号接受策略 |
| 物品筛选方式 | `ItemType` | 决定类型筛选 / 名称筛选 |
| 物品筛选值 | `ItemFilterValue` | 控制 inventory 过滤 |
| 记住密码 | `RememberPassword` | 控制登录记忆 |
| maFile 路径 | `MaFileDirectory` | 控制资产绑定 |
| 导出路径 | `ExportPath` | 控制结果导出位置 |

这张表，基本就是你后面复刻 UI 时的绑定蓝图。

---

## 八、如果你要做一个一样的，主界面最小 MVP

### MVP-1：参数区
- 线程数
- 模式
- 筛选方式 + 值
- 接受方式

### MVP-2：账号列表区
- 主库号列表
- 待转号列表
- 状态列（有无 maFile / 登录状态 / 库存数）

### MVP-3：操作按钮
- 导入账号
- 导入 maFile
- 启动
- 停止
- 导出结果

### MVP-4：日志区
- 统一追加日志
- 区分成功/失败/警告

做到这四块，用户感知上就已经非常接近原版。

---

## 九、如果你自己重写，建议的 UI 架构

不要让 `TransferInventoryForm` 直接调用一切。

### 建议拆法

#### 1. `TransferViewModel`
负责：
- 承载 UI 当前状态
- 参数绑定
- 列表绑定

#### 2. `TransferController`
负责：
- 响应按钮事件
- 调用导入、启动、停止、导出逻辑

#### 3. `TaskLogService`
负责：
- 统一日志
- 事件分发到 UI

#### 4. `TransferOrchestrator`
负责：
- 真正串联 `DataController` / `TradeAction` / `LoginAction`

### 这样做的好处
- WinForms 版和以后 Launcher 版可以复用逻辑
- 日志不会和 UI 控件硬耦合
- 后面接云控、后台任务也更容易

---

## 十、最值得验证的 4 个问题

### 1. 主库号和待转号是两套独立导入，还是同一池账号分角色？
这影响 UI 设计。

### 2. “停止”按钮是硬停还是软停？
这影响任务中断体验。

### 3. 日志区是直接 append 文本，还是有结构化日志模型？
这影响以后做后台监控。

### 4. “库存查看”是即时调用接口，还是读取缓存 Inventory？
这影响 UI 响应速度与设计方式。

---

## 十一、对你现在最值钱的结论

到这一轮为止，这个 Spider 版工具已经能从 **三张图** 来理解：

### 1. 模块图
- `Setting`
- `LoadAccount`
- `AccountAction`
- `SteamGuardController`
- `TradeOfferAction`
- `DataController`

### 2. 数据图
- `Account`

### 3. 交互图
- `TransferInventoryForm`

也就是说：

> 你现在已经开始接近“产品级复刻”，而不只是技术逆向。

---

## 十二、下一步最佳方向

现在继续深挖，最值钱的是二选一：

### A. `Program + GlobalDatabase`
把程序启动顺序、数据库初始化、全局状态装配过程拆清楚

### B. `Setting.json` 真实字段推导版
基于现有所有结论，反向写出一个“高概率完整配置样例”

### 我建议优先：`Setting.json` 真实字段推导版
因为这样你就能开始：
- 自己造配置
- 自己启动一版“像原版”的程序
- 让 UI / 配置 / 引擎第一次真正对上

**也就是：下一步该从“结构理解”走向“可运行重建”。**
