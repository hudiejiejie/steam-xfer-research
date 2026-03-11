# Setting 深挖：配置层 / UI 映射 / 运行参数

> 目标：把 Spider 版 Steam 转库存工具的配置系统拆清楚，达到可以 1:1 复刻控制台行为
> 说明：基于现有 48 份逆向材料推导，字段名与默认值需以后续反编译结果校正

---

## 一、为什么 `Setting` 很关键

前面几轮我们已经把“发动机”拆得差不多了：
- `AccountAction`：登录链
- `SteamGuardController`：2FA / `.maFile` / 确认链
- `TradeOfferAction`：交易动作链
- `DataController`：总调度链

而 `Setting` 决定的是：

- 这个工具**怎么跑**
- UI 控件**控制了什么**
- 默认值**从哪里来**
- 状态**怎么保存**

也就是说：

> `Setting` 就是这个工具的“控制台总线”。

如果你想做一个一样的，光有底层引擎不够，
还必须把线程数、模式、筛选、代理、记忆密码、导入路径这些全部接回去。

---

## 二、现有材料已确认的事实

### 已知
1. 配置类：`SteamInventoryManager.Model.Setting`
2. 对应文件：`Setting.json`
3. `Setting` 是配置中心，很可能是单例：`Setting.Instance`
4. 登录记忆写入 `Setting.Instance`，并采用 **AES-CBC** 保存用户名/密码
5. 它至少控制这些配置域：
   - 默认线程数
   - 默认模式
   - 默认上限
   - 默认代理
   - 默认登录记忆
   - 默认物品类型

### 从主界面反推到的 UI 控件映射
主窗体 `TransferInventoryForm` 已知功能区包括：
- 主库号列表
- 待转号列表
- 物品筛选
- 线程数
- 转移模式
- 主库号接受方式
- 导入账号
- 导入 maFiles
- 启动 / 停止
- 导出结果
- 库存查看

这意味着 `Setting` 不只是简单配置文件，而是：

- 启动时加载默认 UI 状态
- 运行中提供参数给 `DataController`
- 关闭时回写部分用户偏好

---

## 三、`Setting` 的角色定位

### 它大概率是单例配置容器
最可能的结构是：

```csharp
public class Setting {
    public static Setting Instance { get; set; }

    public int ThreadCount { get; set; }
    public int TransferType { get; set; }
    public int ItemType { get; set; }
    public int MaxPerMaster { get; set; }
    public string Proxy { get; set; }
    public bool RememberPassword { get; set; }
    public string SavedUsername { get; set; }
    public string SavedPasswordCipher { get; set; }
    public string MaFileDirectory { get; set; }
    public string LastImportPath { get; set; }
}
```

### 典型用法
```csharp
// 启动时
Setting.Instance = Setting.Load("Setting.json");

// UI 初始化
threadNumeric.Value = Setting.Instance.ThreadCount;
modeCombo.SelectedIndex = Setting.Instance.TransferType;
itemTypeCombo.SelectedIndex = Setting.Instance.ItemType;

// 运行时
DataController.StartTransfer(Setting.Instance);

// 关闭时
Setting.Instance.Save();
```

这就是“配置总线”的典型模式。

---

## 四、最可能存在的字段清单（按功能分组）

下面这部分不是空猜，是根据主窗体功能区 + 已知行为反推出来的。

---

### A. 运行参数类

#### 1. 线程数
```csharp
int ThreadCount
```
用途：
- 批量登录并发
- 发货并发
- 可能影响接受报价并发

#### 2. 转移模式
```csharp
int TransferType
```
已知：
- `0` = 边发边接受
- `1` = 全部发完后统一接受

#### 3. 主库号接受方式
```csharp
int AcceptMode
```
用途：
- 主库号是否自动接受
- 是否延后接受
- 是否只手动确认

#### 4. 主库号接收上限
```csharp
int MaxPerMaster
```
用途：
- 控制每个主库号接收多少物品
- 供 `DataController` 做分配算法

---

### B. 物品筛选类

#### 5. 筛选方式
```csharp
int ItemType
```
已知：
- `0` = 按类型
- `1` = 按名称精确

#### 6. 筛选值
```csharp
string ItemFilterValue
```
用途：
- 比如“匕首”“AK-47”
- 或某一类库存标签

#### 7. AppId / ContextId
```csharp
int AppId
int ContextId
```
用途：
- 决定拉哪个库存
- 例如 `753 -> 6`

虽然材料里没直接说 `Setting` 保存了它们，
但如果 UI 有库存查看与筛选，很有可能这里有默认值。

---

### C. 登录与账号偏好类

#### 8. 记住密码开关
```csharp
bool RememberPassword
```

#### 9. 用户名
```csharp
string SavedUsername
```

#### 10. 密码密文
```csharp
string SavedPasswordCipher
```
说明：
- 使用 AES-CBC 加密
- 不排除还会保存 key/iv 派生材料

#### 11. 默认 maFile 目录
```csharp
string MaFileDirectory
```
用途：
- 导入 `.maFile` 时的默认目录
- 自动匹配 `账号名 + .maFile`

---

### D. 导入导出与路径类

#### 12. 上次导入账号文本路径
```csharp
string LastAccountImportPath
```

#### 13. 上次导入 maFile 路径
```csharp
string LastMaFileImportPath
```

#### 14. 导出结果路径
```csharp
string ExportPath
```

#### 15. 日志目录
```csharp
string LogPath
```

这些字段不一定都在 `Setting.json`，但如果原版体验较完整，至少会保存一部分路径偏好。

---

### E. 网络与代理类

#### 16. 代理开关
```csharp
bool UseProxy
```

#### 17. 代理地址
```csharp
string ProxyAddress
```

#### 18. 超时配置
```csharp
int TimeoutSeconds
```

这个工具既然要批量登录、多账号转移，很有可能配置层里有代理或超时相关字段。

---

## 五、UI 到 `Setting` 的映射关系（核心）

如果你后面要自己复刻，最重要的不是字段名，而是 **UI 控件和配置字段如何一一对应**。

### 可能的映射表

| UI 控件 | Setting 字段 | 作用 |
|--------|--------------|------|
| 线程数 NumericUpDown | `ThreadCount` | 控制并发 |
| 转移模式 ComboBox | `TransferType` | 边发边接 / 全发后接 |
| 接受方式 ComboBox | `AcceptMode` | 主库号接受策略 |
| 物品类型 ComboBox | `ItemType` | 类型筛选 / 名称筛选 |
| 物品输入框 TextBox | `ItemFilterValue` | 筛选关键词 |
| 记住密码 CheckBox | `RememberPassword` | 是否保存登录信息 |
| 用户名 TextBox | `SavedUsername` | 登录默认值 |
| maFile 路径按钮 | `MaFileDirectory` | 默认导入路径 |
| 导出路径按钮 | `ExportPath` | 导出结果位置 |
| 代理输入框 | `ProxyAddress` | 请求代理 |

这张表基本就是后续“做一个一样的”时 UI 与配置层绑定的蓝图。

---

## 六、`Setting.json` 最可能的生命周期

### 1. 启动加载
```text
Program.Main
  -> Setting.Load()
  -> Setting.Instance = result
  -> TransferInventoryForm 初始化控件默认值
```

### 2. 运行读取
```text
DataController.StartTransfer(...)
  -> 从 Setting.Instance 读取线程数 / 模式 / 筛选规则
```

### 3. 用户修改 UI
```text
TransferInventoryForm 控件变化
  -> 同步更新 Setting.Instance
```

### 4. 关闭保存
```text
FormClosing
  -> Setting.Save("Setting.json")
```

### 结论
这很可能是一个“**配置即状态**”的项目：
- 配置不是纯静态常量
- 而是用户偏好 + 当前运行参数 + 登录记忆 的混合体

所以你后面重写时，建议也保留这类“启动即恢复”的体验。

---

## 七、AES-CBC 记忆密码意味着什么

我们已知：
- 用户名/密码写入 `Setting.Instance`
- 密码采用 AES-CBC 加密

### 这层的真实意义
它不是安全系统，而是“用户体验补丁”。

也就是说，原版作者主要目标可能是：
- 让用户下次打开不用再输密码
- 而不是做非常严格的本地凭据安全模型

### 对你后面复刻的建议

#### 如果目标是“像原版”
- 兼容 AES-CBC 存储即可

#### 如果目标是“做得更稳”
- 可以改为：
  - Windows DPAPI
  - Launcher 自己管理凭据
  - 本地加密仓 + 机器绑定

### 但重要的是
**这层不应阻碍你第一版复刻。**
先跑通，再升级安全模型。

---

## 八、如果要做一个一样的，`Setting` 的最小复刻清单

### MVP 必须实现
1. 线程数
2. 转移模式
3. 接受方式
4. 物品筛选模式 + 值
5. 记住密码开关
6. 用户名/密码本地保存
7. maFile 默认目录
8. 配置文件读写

### 第二阶段增强
1. 代理配置
2. 导出路径配置
3. 日志级别
4. 多 profile 切换
5. 配置模板导入导出

### 也就是说
第一版只要这 8 项跑通，用户体验就已经很接近原版了。

---

## 九、如果你重写，建议不要把 `Setting` 做成“超大垃圾桶”

原版很可能是单例大对象，全都塞进一个类里。

### 你重写时建议拆成：

#### 1. `RuntimeSettings`
- 线程数
- 模式
- 筛选规则
- 接受方式

#### 2. `CredentialSettings`
- 记住密码
- 用户名
- 密码密文

#### 3. `PathSettings`
- maFile 目录
- 导入导出路径
- 日志路径

#### 4. `NetworkSettings`
- 代理
- 超时

然后再通过一个 `AppSettings` 聚合。

### 好处
- 后续你做 Launcher 时可以更自然地映射到 UI
- 配置校验更容易做
- 以后云端下发套餐限制时，也更容易与本地设置区分开

---

## 十、最值得验证的 4 个问题

### 1. `Setting.json` 是只在退出时保存，还是每次修改就立即保存？
这影响崩溃恢复体验。

### 2. 密码密文和普通配置是否混在同一个 json 里？
如果是，说明原版配置层耦合比较重。

### 3. `TransferType` / `AcceptMode` / `ItemType` 的枚举值是否硬编码在 UI 和业务层？
如果是，后续改枚举时要一起改。

### 4. 是否存在隐藏配置项？
例如：
- 重试次数
- 最大拆单数
- 默认 appid/contextid
- 请求超时

这些项可能没露在 UI 上，但存在于 `Setting` 里。

---

## 十一、对你现在最有价值的结论

到这一轮为止，Spider 版这个工具最重要的 5 条主线已经都基本拆出来了：

1. `Setting` —— 控制台与运行参数
2. `AccountAction` —— 登录链
3. `SteamGuardController` —— 2FA 与确认链
4. `TradeOfferAction` —— 交易动作链
5. `DataController` —— 总调度链

这意味着：

> 你现在已经不是在“研究一个工具怎么用”，而是在“逆向一个完整可重建的系统”。

---

## 十二、下一步最佳方向

现在继续深挖，最值钱的是二选一：

### A. `Account` 模型深挖
把字段、运行时状态、持久化边界、与各控制器的关系全部拆清楚

### B. `TransferInventoryForm` 深挖
把 UI 按钮、表格、日志、导入导出动作与后端调用完全对齐

### 我建议优先：`Account`
原因：
- `Account` 是所有层共用的数据核心
- 登录、配置、交易、确认、库存都围着它转
- 把它拆清楚，后面你自己重写时整个项目的数据骨架就有了

**也就是：下一步该从“模块”进入“数据模型”层了。**
