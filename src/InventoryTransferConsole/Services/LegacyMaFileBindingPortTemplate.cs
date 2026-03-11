using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

/// <summary>
/// 真实接入原版 maFile 匹配与绑定逻辑时，从这里开始替换。
/// </summary>
public sealed class LegacyMaFileBindingPortTemplate : ILegacyMaFileBindingPort
{
    private readonly LegacyPortContext context;

    public LegacyMaFileBindingPortTemplate(LegacyPortContext? context = null)
    {
        this.context = context ?? LegacyPortContext.Empty();
    }

    public bool TryBind(DashboardSnapshot current, ILogSink logSink, out DashboardSnapshot snapshot)
    {
        snapshot = current;

        // TODO 接原版逻辑时按这个顺序实现：
        // 1. 读取 context.MaFileDirectory
        // 2. 按 账号名 + ".maFile" 规则匹配
        // 3. 更新 AccountAggregate.Guard
        // 4. 回写 DashboardSnapshot 的 maFile 状态列

        if (string.IsNullOrWhiteSpace(context.MaFileDirectory))
        {
            logSink.Warn("LegacyMaFileBindingPortTemplate: MaFileDirectory not configured, fallback to mock.");
            return false;
        }

        logSink.Warn("LegacyMaFileBindingPortTemplate: not implemented yet, fallback to mock.");
        return false;
    }
}
