namespace InventoryTransferConsole.Models;

public sealed class DashboardSnapshot
{
    public int ThreadCount { get; set; }
    public string ModeSummary { get; set; } = string.Empty;
    public string RunState { get; set; } = string.Empty;
    public List<MasterAccountRow> Masters { get; set; } = [];
    public List<WorkerAccountRow> Workers { get; set; } = [];
    public List<AccountAggregate> Accounts { get; set; } = [];
}
