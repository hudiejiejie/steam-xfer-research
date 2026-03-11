using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public sealed class NullLegacyAccountImportPort : ILegacyAccountImportPort
{
    public bool TryImport(ImportAccountsResult importResult, DashboardSnapshot current, ILogSink logSink, out DashboardSnapshot snapshot)
    {
        snapshot = current;
        return false;
    }
}

public sealed class NullLegacyMaFileBindingPort : ILegacyMaFileBindingPort
{
    public bool TryBind(DashboardSnapshot current, ILogSink logSink, out DashboardSnapshot snapshot)
    {
        snapshot = current;
        return false;
    }
}

public sealed class NullLegacyTransferExecutionPort : ILegacyTransferExecutionPort
{
    public bool TryStart(TransferConfig config, DashboardSnapshot current, ILogSink logSink, out DashboardSnapshot snapshot)
    {
        snapshot = current;
        return false;
    }

    public bool TryStop(DashboardSnapshot current, ILogSink logSink, out DashboardSnapshot snapshot)
    {
        snapshot = current;
        return false;
    }
}
