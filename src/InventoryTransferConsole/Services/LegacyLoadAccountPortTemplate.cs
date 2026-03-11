using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

/// <summary>
/// 真实接入原版 LoadAccount 逻辑时，从这里开始替换。
/// 当前默认返回 false，让 LegacyTransferController 自动回退到 Mock。
/// </summary>
public sealed class LegacyLoadAccountPortTemplate : ILegacyAccountImportPort
{
    private readonly LegacyPortContext context;

    public LegacyLoadAccountPortTemplate(LegacyPortContext? context = null)
    {
        this.context = context ?? LegacyPortContext.Empty();
    }

    public bool TryImport(ImportAccountsResult importResult, DashboardSnapshot current, ILogSink logSink, out DashboardSnapshot snapshot)
    {
        snapshot = current;

        // TODO 接原版逻辑时按这个顺序实现：
        // 1. 定位反编译导出的 LoadAccount 相关类/方法
        // 2. 统一账号文本分隔符（---- : , |）
        // 3. 生成 AccountAggregate / AccountProfile
        // 4. 在导入阶段做去重、非法过滤、maFile 初始绑定
        // 5. 把结果回写为 DashboardSnapshot
        
        if (string.IsNullOrWhiteSpace(context.DecompiledProjectRoot))
        {
            logSink.Warn("LegacyLoadAccountPortTemplate: DecompiledProjectRoot not configured, fallback to mock.");
            return false;
        }

        logSink.Warn("LegacyLoadAccountPortTemplate: not implemented yet, fallback to mock.");
        return false;
    }
}
