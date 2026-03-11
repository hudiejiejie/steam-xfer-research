# Steam转库存V1.0 复现与改造全流程（Spider版）

> 目标：能够从头解包、反编译、修改并重打包出功能相同的程序

---

## 一、环境准备

### 必要工具
- **.NET SDK**（≥ 8.0，自带 Microsoft.NET.HostModel.dll）
- **dnSpyEx** 或 **ILSpy**（带命令行导出功能，如 `ilspycmd`）
- **7-Zip**（备用：直接查看 Inside 文件结构）
- **Git**（版本管理）
- **Optional**: `dotnet-bundle` 相关探针项目（用于精确提取）

---

## 二、提取主程序集

### 步骤 1：复制目标 exe 到纯英文工作区
```
Steam转库存V1.0.exe  →  C:\work\steam_xfer\original\Steam转库存V1.0.exe
```
原因：避免中文路径导致工具解析异常。

### 步骤 2：使用 HostModel 提取器（推荐）
```csharp
// C# 探针程序 (ExtractBundle.cs)
using System;
using Microsoft.NET.HostModel.Bundle;
using System.IO;
class Program {
    static int Main(string[] args) {
        if (args.Length < 2) {
            Console.WriteLine("Usage: ExtractBundle <bundlePath> <outputDir>");
            return 1;
        }
        var bundlePath = args[0];
        var outputDir = args[1];
        BundleExtractor.Extract(bundlePath, outputDir);
        return 0;
    }
}
```
编译为控制台程序，然后运行：
```cmd
dotnet run --project ExtractBundle.csproj "C:\work\steam_xfer\original\Steam转库存V1.0.exe" "C:\work\steam_xfer\extracted"
```
成功后你将得到：
- `SteamInventoryManager.dll`
- `SteamInventoryManager.deps.json`
- `SteamInventoryManager.runtimeconfig.json`
- 所有依赖 DLL（SteamAuth.dll, SteamKit2.dll, Newtonsoft.Json.dll, WPF 运行时等）

### 步骤 3：备选提取方式（如果 HostModel 不工作）
- 用 7-Zip 打开 exe，内部可能有 `apphost` + 托管文件段；直接提取所有文件
- 或将 exe 复制到 `%TEMP%\.net\` 运行后查看是否生成 dll（但 Spider 版可能仍在内存加载，不一定落地）

---

## 三、反编译查看源码

### 使用 ILSpy 命令行
```bash
ilspycmd -p "C:\work\steam_xfer\extracted\SteamInventoryManager.dll" -o "C:\work\steam_xfer\decompiled"
```
或使用 dnSpyEx 图形界面打开 DLL。

### 关键命名空间与类（根据已有结论）
- `SteamInventoryManager.Program` — 入口
- `SteamAutoLogin.TransferInventoryForm` — 主窗体（WinForms）
- `SteamInventoryManager.Model`:
  - `Setting` — 配置（单例，读取 Setting.json）
  - `Account` — 账号模型
  - `DataController` — 核心调度
  - `UserController` — 用户会话
  - `SteamGuardController` — 2FA/maFile
  - `TradeOfferAction` — 报价操作
- `SteamInventoryManager.Form.LoginForm`
- `OutlookUpdateEmail.LoadAccount` — 账号导入

---

## 四、配置与资源

- `Setting.json` 位置：与 exe 同目录（或 `%APPDATA%`）
  - 包含：线程数、模式、代理、上限、默认路径等
- `gloabl.db`（如果有）：SQLite 或本地数据库，存放账号缓存

建议：将这两个文件一并复制到工作区进行分析。

---

## 五、改造与重打包

### 改造路径
- 第一层（UI/配置）：`TransferInventoryForm`、`Setting`、`LoadAccount`
- 第二层（业务流）：`DataController`、`TradeOfferAction`、`UserController`、`SteamGuardController`
- 第三层（登录/授权）：`LoginForm`、`AccountAction`

### 修改后编译
1. 用 ILSpy 导出为 **项目**（而非仅反编译代码）：
   ```bash
   ilspycmd -p -t csproj "SteamInventoryManager.dll" -o "C:\work\steam_xfer\project"
   ```
2. 用 Visual Studio 或 `dotnet build` 加载项目并编译
3. 将原 exe 中的资源（图标、配置文件）复制到输出目录
4. 如有混淆，可能需要清理或重命名后再编译
5. 发布为单文件：
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
   ```

### 注意事项
- 原版可能使用内部签名或授权检查，需移除或模拟
- `Setting.json` 的字段必须与代码期望一致
- 若依赖强名称或数字签名，复现需相应密钥（无则只能打未签名包）

---

## 六、验证与测试

1. 在新机器/沙箱中运行生成的 exe
2. 导入测试账号（使用测试 maFile 或无价值账号）
3. 观察日志（`logs/`）与界面行为是否与原版一致
4. 逐步启用改造功能（如备份、上传、改线程逻辑）

---

## 七、自动化脚本（示例）

```bat
:: 1. 提取
dotnet ExtractBundle.dll "original\Steam转库存V1.0.exe" "extracted"

:: 2. 反编译导出项目
ilspycmd -p -t csproj "extracted\SteamInventoryManager.dll" -o "project"

:: 3. 编译
dotnet build "project\SteamInventoryManager.csproj" -c Release

:: 4. 发布单文件
dotnet publish "project\SteamInventoryManager.csproj" -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true

:: 5. 复制资源
copy "original\Setting.json" "publish\"
copy "original\gloabl.db" "publish\"  (如果存在)
```

---

## 八、下一步深挖（按优先级）

1. **精确定位 DataController 方法签名**：反编译后列出所有 `public`/`internal` 方法，与上文“深挖笔记”比对
2. **识别 Config 读取位置**：找到 `Setting.json` 的加载路径与默认值
3. **定位 AccountAction.Login**：查看登录服务器 URL 与参数，评估是否需要更换端点
4. **设计备份接口**：在 `RefreshAccountAsync` 结束处调用本地/远程 HTTP 备份
5. **测试最小改造**：先改一个 UI 文案，编译验证能正常运行

---

完成以上步骤后，您就拥有了一个 **可构建、可调试、可定制** 的 Steam 转库存工具基础。后续可逐步加入会员系统、云端激活、分发平台等功能。
