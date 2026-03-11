using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

/// <summary>
/// 真实接入原版 DataController / Transfer 执行链时，从这里开始替换。
/// </summary>
public sealed class LegacyTransferExecutionPortTemplate : ILegacyTransferExecutionPort
{
    private readonly LegacyPortContext context;

    public LegacyTransferExecutionPortTemplate(LegacyPortContext? context = null)
    {
        this.context = context ?? LegacyPortContext.Empty();
    }

    public bool TryStart(TransferConfig config, DashboardSnapshot current, ILogSink logSink, out DashboardSnapshot snapshot)
    {
        snapshot = current;

        // TODO 接原版逻辑时按这个顺序实现：
        // 1. 把 TransferConfig 映射回原项目的 Setting / Account / DataController 输入
        // 2. 调用原版 StartTransfer / RunTransfer 入口
        // 3. 把运行中状态、日志、结果同步回 DashboardSnapshot

        if (string.IsNullOrWhiteSpace(context.DecompiledProjectRoot))
        {
            logSink.Warn("LegacyTransferExecutionPortTemplate: DecompiledProjectRoot not configured, fallback to mock.");
            return false;
        }

        logSink.Warn("LegacyTransferExecutionPortTemplate: start not implemented yet, fallback to mock.");
        return false;
    }

    public bool TryStop(DashboardSnapshot current, ILogSink logSink, out DashboardSnapshot snapshot)
    {
        snapshot = current;

        // TODO 接原版逻辑时按这个顺序实现：
        // 1. 找到原版停止入口 / 取消标志
        // 2. 将 UI 的停止动作映射到原版软停语义
        // 3. 同步停止中/已停止状态到 DashboardSnapshot

        if (string.IsNullOrWhiteSpace(context.DecompiledProjectRoot))
        {
            logSink.Warn("LegacyTransferExecutionPortTemplate: DecompiledProjectRoot not configured, fallback to mock.");
            return false;
        }

        logSink.Warn("LegacyTransferExecutionPortTemplate: stop not implemented yet, fallback to mock.");
        return false;
    }
}
