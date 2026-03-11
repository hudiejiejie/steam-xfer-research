using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

/// <summary>
/// 预留给后续接入原始反编译逻辑/兼容层。
/// 当前先作为桥接占位实现，后面可替换为真正的 Legacy 控制器。
/// </summary>
public sealed class LegacyTransferController : ITransferController
{
    private readonly ITransferController fallback;

    public LegacyTransferController(ITransferController? fallback = null)
    {
        this.fallback = fallback ?? new MockTransferController();
    }

    public DashboardSnapshot LoadSnapshot() => fallback.LoadSnapshot();

    public DashboardSnapshot StartTransfer(TransferConfig config, ILogSink logSink)
    {
        logSink.Warn("LegacyTransferController not connected yet. Falling back to mock runtime.");
        return fallback.StartTransfer(config, logSink);
    }

    public DashboardSnapshot StopTransfer(ILogSink logSink)
    {
        logSink.Warn("LegacyTransferController stop fallback.");
        return fallback.StopTransfer(logSink);
    }

    public DashboardSnapshot ImportAccounts(ImportAccountsResult importResult, ILogSink logSink)
    {
        logSink.Info("LegacyTransferController import fallback.");
        return fallback.ImportAccounts(importResult, logSink);
    }

    public DashboardSnapshot ImportMaFiles(ILogSink logSink)
    {
        logSink.Info("LegacyTransferController maFile binding fallback.");
        return fallback.ImportMaFiles(logSink);
    }

    public void ViewInventory(ILogSink logSink)
    {
        fallback.ViewInventory(logSink);
    }

    public void ExportResults(ILogSink logSink)
    {
        fallback.ExportResults(logSink);
    }
}
