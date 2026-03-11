# LoadAccount 深挖：账号导入 / 文本格式 / maFile 匹配链

> 目标：把 Spider 版 Steam 转库存工具的“账号进入系统”这一步彻底拆清楚
> 说明：基于现有 48 份逆向材料与前序笔记推导，方法签名和具体字段以后续反编译校正

---

## 一、为什么 `LoadAccount` 很关键

前面我们已经把：
- `Setting`
- `Account`
- `AccountAction`
- `SteamGuardController`
- `TradeOfferAction`
- `DataController`

这些主干层拆得差不多了。

但如果要真正做出一个“一样的”，用户最开始接触的不是这些控制器，
而是：

> **先把账号导进去。**

也就是说，`LoadAccount` 决定的是：
- 外部账号数据怎么进入系统
- 文本怎么变成 `Account`
- `.maFile` 怎么自动匹配到账号
- 错误账号怎么过滤
- 导入后的账号怎么落到 UI 和后端状态里

如果这层没拆清楚，后面再强的登录链也没入口。

---

## 二、现有材料已确认的事实

### 已确认
1. 导入窗体：`OutlookUpdateEmail.LoadAccount`
2. 支持两种导入：
   - 文件导入
   - 剪贴板导入
3. maFile 匹配规则：
   - `账号名 + ".maFile"`
4. 匹配成功后会写入：
   - `account.MafilesPath`
5. 最终会生成/填充 `Account` 对象，供后续登录和调度使用

### 这意味着什么
- `LoadAccount` 不只是一个 UI 窗口，而是 **账号预处理层**
- 它至少要做：
  - 解析文本
  - 清洗格式
  - 去重
  - 匹配 maFile
  - 生成标准 `Account`

---

## 三、`LoadAccount` 的角色定位

如果用系统架构语言来说，`LoadAccount` 本质上是：

> **外部账号数据 → 内部 Account 模型 的转换器**

所以它不只是“导入”动作，它还承担：
- 格式标准化
- 输入容错
- 资产绑定（账号名 ↔ maFile）
- 可能还包括基础校验

---

## 四、最可能的方法结构

### 1. 从文件导入
```csharp
List<Account> ImportFromFile(string path)
```

#### 功能
- 打开账号文本文件
- 按行读取
- 过滤空行
- 解析为 `Account`

---

### 2. 从剪贴板导入
```csharp
List<Account> ImportFromClipboard(string text)
```

#### 功能
- 读取剪贴板纯文本
- 按换行拆分
- 解析为 `Account`

---

### 3. 单行解析
```csharp
Account ParseAccountLine(string line)
```

#### 这是最关键的方法之一
因为整个账号导入质量，都取决于这一步。

它可能负责：
- 切分用户名/密码
- 去空格
- 去 BOM / 不可见字符
- 忽略注释行
- 处理格式异常

---

### 4. maFile 自动匹配
```csharp
void MatchMaFile(Account account, string maFileDirectory)
```

#### 规则（已确认）
- 按 `账号名 + .maFile` 查找
- 找到后写入 `account.MafilesPath`
- 可能也会写入 `account.MafilesName`

---

### 5. 批量清洗
```csharp
List<Account> NormalizeAccounts(List<Account> accounts)
```

#### 可能做的事情
- 去重（相同账号名）
- 去除非法账号
- 合并重复导入
- 修复简单格式问题

---

### 6. 导入结果回传
```csharp
void ReturnAccountsToParent(List<Account> accounts)
```

#### 功能
- 把导入结果送回 `TransferInventoryForm`
- 更新主界面的待转号/主库号列表

---

## 五、账号文本的最可能格式

现有材料没有直接给出完整样例，但根据这种工具的常见做法，最可能格式有三种：

### 格式 A：`账号----密码`
```text
user1----pass1
user2----pass2
```

### 格式 B：`账号:密码`
```text
user1:pass1
user2:pass2
```

### 格式 C：`账号----密码----附加字段`
```text
user1----pass1----邮箱
user2----pass2----邮箱
```

### 结合现有工具风格判断
Spider 版更可能支持一种“简单粗暴”的文本格式，优先考虑：
- 固定分隔符
- 一行一个账号
- 不强调复杂 schema

所以第一版复刻时，建议至少兼容：
1. `----`
2. `:`
3. `,` 或 `|`（可选）

---

## 六、最可能的导入链

### 文件导入路径
```text
TransferInventoryForm.btnImportAccounts_Click(...)
  -> 打开 LoadAccount 窗口
      -> 选择文件
      -> LoadAccount.ImportFromFile(path)
          -> ReadAllLines
          -> ParseAccountLine(line)
          -> NormalizeAccounts(accounts)
          -> MatchMaFile(account, selectedDir)
      -> ReturnAccountsToParent(accounts)
  -> UI 列表更新
```

### 剪贴板导入路径
```text
TransferInventoryForm.btnPasteAccounts_Click(...)
  -> 打开 LoadAccount 窗口
      -> ImportFromClipboard(Clipboard.GetText())
          -> SplitLines
          -> ParseAccountLine(line)
          -> NormalizeAccounts(accounts)
          -> MatchMaFile(...)
      -> ReturnAccountsToParent(accounts)
```

### 这条链说明
`LoadAccount` 很可能是：
- 一个带 UI 的导入器
- 但内部也有一套清晰的数据转换逻辑

如果你后面自己做版本，完全可以把 UI 和解析逻辑拆开。

---

## 七、maFile 匹配规则是这个类的核心价值之一

### 已确认规则
```text
账号名 + ".maFile"
```

这其实很重要，因为它说明：
- 原版工具默认假设用户名与 maFile 文件名一一对应
- 用户体验是“你把 maFile 扔到目录里，工具自动匹配”

### 这对复刻意味着什么
如果你要做一个“一样的”，必须保留这条体验。

### 重写建议
匹配规则可以这样设计：

```csharp
string expected = account.AccountName + ".maFile";
var file = Directory.GetFiles(maFileDir, "*.maFile")
    .FirstOrDefault(f => Path.GetFileName(f).Equals(expected, StringComparison.OrdinalIgnoreCase));
if (file != null) {
    account.MafilesPath = file;
    account.MafilesName = Path.GetFileName(file);
}
```

### 第二阶段增强
后面你还可以扩展：
- 模糊匹配
- 多目录搜索
- 缺失 maFile 提示
- 批量导入报告

但第一版先别搞复杂，先 1:1 兼容原规则。

---

## 八、导入时最可能发生的校验

### 1. 去空行
```csharp
if (string.IsNullOrWhiteSpace(line)) continue;
```

### 2. 去注释/垃圾行
```csharp
if (line.StartsWith("#") || line.StartsWith("//")) continue;
```

### 3. 用户名密码是否齐全
```csharp
if (string.IsNullOrWhiteSpace(account.AccountName)) invalid;
if (string.IsNullOrWhiteSpace(account.AccountPassword)) invalid;
```

### 4. 账号是否重复
```csharp
accounts = accounts.GroupBy(a => a.AccountName).Select(g => g.First()).ToList();
```

### 5. maFile 是否匹配成功
- 成功：写入 `MafilesPath`
- 失败：保留账号，但标记为“无 maFile”或放入失败列表

---

## 九、`LoadAccount` 与 `Account` 的边界

这点很关键。

### `LoadAccount` 应该做什么
- 把原始文本解析成 `Account`
- 做初步清洗
- 做 maFile 绑定

### `LoadAccount` 不应该做什么
- 真正登录 Steam
- 获取 Session
- 获取 TradeToken
- 拉库存
- 发报价

这些属于：
- `AccountAction`
- `UserController`
- `DataController`

### 也就是说
`LoadAccount` 是：
> **数据进入系统的入口层**
而不是“业务执行层”。

---

## 十、如果你要做一个一样的，最小导入 MVP

### MVP-1：账号导入
- 支持文件导入
- 支持剪贴板导入
- 一行一个账号
- 解析用户名和密码

### MVP-2：maFile 绑定
- 指定 maFile 目录
- 自动按 `账号名 + .maFile` 绑定

### MVP-3：导入结果展示
- 成功数量
- 重复数量
- 缺失 maFile 数量
- 非法格式数量

### MVP-4：导入后进入主界面列表
- 能直接成为后续登录/转库存的数据源

做到这四步，导入体验就已经接近原版了。

---

## 十一、你如果重写，建议拆成 4 个部件

### 1. `AccountTextParser`
负责：
- 文本行解析
- 多分隔符兼容
- 基础校验

### 2. `AccountNormalizer`
负责：
- 去重
- 清洗
- 修复简单格式问题

### 3. `MaFileMatcher`
负责：
- 根据账号名绑定 `.maFile`

### 4. `ImportReportBuilder`
负责：
- 汇总导入结果
- 生成 UI 提示 / 导出报告

这样你后面做 Launcher 或 Web 后台导入时，也能复用同一套逻辑。

---

## 十二、对你当前最有价值的结论

到这一轮为止，这个项目已经可以分成两大半：

### A. 数据进入系统前半段
- `LoadAccount`
- `Account`
- `Setting`

### B. 数据进入系统后半段
- `AccountAction`
- `SteamGuardController`
- `TradeOfferAction`
- `DataController`

这意味着：

> 你现在已经不仅能重建“运行引擎”，也已经开始能重建“数据入口”。

这一步很关键，因为真正做产品时，导入体验决定了用户能不能上手。

---

## 十三、下一步最佳方向

现在继续深挖，最值钱的是二选一：

### A. `TransferInventoryForm`
把 UI 的每个按钮、列表、日志区、进度反馈、导入导出动作全部与后端链路对齐

### B. `Program + GlobalDatabase`
把程序启动顺序、数据库初始化、全局状态装配过程拆清楚

### 我建议优先：`TransferInventoryForm`
因为：
- 现在底层引擎、数据骨架、导入入口都已经很清楚
- 下一步最应该补的是“人怎么操作这个系统”
- 一旦 UI 与后端调用链对齐，你就基本拥有了原版完整产品的交互蓝图

**也就是：下一步该把“系统结构图”推进成“产品交互图”。**
