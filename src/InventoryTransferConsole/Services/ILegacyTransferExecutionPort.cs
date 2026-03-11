using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public interface ILegacyTransferExecutionPort
{
    bool TryStart(TransferConfig config, DashboardSnapshot current, ILogSink logSink, out DashboardSnapshot snapshot);
    bool TryStop(DashboardSnapshot current, ILogSink logSink, out DashboardSnapshot snapshot);
}
