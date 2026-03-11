using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public interface ILegacyAccountImportPort
{
    bool TryImport(ImportAccountsResult importResult, DashboardSnapshot current, ILogSink logSink, out DashboardSnapshot snapshot);
}
