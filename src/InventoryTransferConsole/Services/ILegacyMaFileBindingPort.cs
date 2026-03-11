using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public interface ILegacyMaFileBindingPort
{
    bool TryBind(DashboardSnapshot current, ILogSink logSink, out DashboardSnapshot snapshot);
}
