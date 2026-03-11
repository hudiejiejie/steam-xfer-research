using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public interface ITransferController
{
    DashboardSnapshot LoadSnapshot();
    DashboardSnapshot StartTransfer(RuntimeSettings settings, ILogSink logSink);
    DashboardSnapshot StopTransfer(ILogSink logSink);
    DashboardSnapshot ImportAccounts(ImportAccountsResult importResult, ILogSink logSink);
    DashboardSnapshot ImportMaFiles(ILogSink logSink);
    void ViewInventory(ILogSink logSink);
    void ExportResults(ILogSink logSink);
}
