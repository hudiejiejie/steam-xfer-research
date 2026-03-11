using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

/// <summary>
/// 预留给后续接入原始反编译逻辑/兼容层。
/// 当前按“分段替换点”组织：导入账号、绑定 maFile、启动/停止任务。
/// 某一段未接入时自动回退到 fallback。
/// </summary>
public sealed class LegacyTransferController : ITransferController
{
    private readonly ILegacyAccountImportPort accountImportPort;
    private readonly ILegacyMaFileBindingPort maFileBindingPort;
    private readonly ILegacyTransferExecutionPort transferExecutionPort;
    private readonly ITransferController fallback;
    private DashboardSnapshot currentSnapshot = new();

    public LegacyTransferController(
        ILegacyAccountImportPort? accountImportPort = null,
        ILegacyMaFileBindingPort? maFileBindingPort = null,
        ILegacyTransferExecutionPort? transferExecutionPort = null,
        ITransferController? fallback = null)
    {
        this.accountImportPort = accountImportPort ?? new NullLegacyAccountImportPort();
        this.maFileBindingPort = maFileBindingPort ?? new NullLegacyMaFileBindingPort();
        this.transferExecutionPort = transferExecutionPort ?? new NullLegacyTransferExecutionPort();
        this.fallback = fallback ?? new MockTransferController();
        currentSnapshot = this.fallback.LoadSnapshot();
    }

    public DashboardSnapshot LoadSnapshot()
    {
        currentSnapshot = fallback.LoadSnapshot();
        return currentSnapshot;
    }

    public DashboardSnapshot StartTransfer(TransferConfig config, ILogSink logSink)
    {
        if (transferExecutionPort.TryStart(config, currentSnapshot, logSink, out var snapshot))
        {
            currentSnapshot = snapshot;
            return snapshot;
        }

        logSink.Warn("Legacy transfer execution port not connected. Falling back to mock runtime.");
        currentSnapshot = fallback.StartTransfer(config, logSink);
        return currentSnapshot;
    }

    public DashboardSnapshot StopTransfer(ILogSink logSink)
    {
        if (transferExecutionPort.TryStop(currentSnapshot, logSink, out var snapshot))
        {
            currentSnapshot = snapshot;
            return snapshot;
        }

        logSink.Warn("Legacy stop port not connected. Falling back to mock runtime.");
        currentSnapshot = fallback.StopTransfer(logSink);
        return currentSnapshot;
    }

    public DashboardSnapshot ImportAccounts(ImportAccountsResult importResult, ILogSink logSink)
    {
        if (accountImportPort.TryImport(importResult, currentSnapshot, logSink, out var snapshot))
        {
            currentSnapshot = snapshot;
            return snapshot;
        }

        logSink.Info("Legacy account import port not connected. Falling back to mock import.");
        currentSnapshot = fallback.ImportAccounts(importResult, logSink);
        return currentSnapshot;
    }

    public DashboardSnapshot ImportMaFiles(ILogSink logSink)
    {
        if (maFileBindingPort.TryBind(currentSnapshot, logSink, out var snapshot))
        {
            currentSnapshot = snapshot;
            return snapshot;
        }

        logSink.Info("Legacy maFile bind port not connected. Falling back to mock binding.");
        currentSnapshot = fallback.ImportMaFiles(logSink);
        return currentSnapshot;
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
