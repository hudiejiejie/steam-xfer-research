using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public interface ITransferController
{
    DashboardSnapshot LoadSnapshot();
    void StartTransfer(ILogSink logSink);
    void StopTransfer(ILogSink logSink);
    void ImportAccounts(ILogSink logSink);
    void ImportMaFiles(ILogSink logSink);
    void ViewInventory(ILogSink logSink);
    void ExportResults(ILogSink logSink);
}
