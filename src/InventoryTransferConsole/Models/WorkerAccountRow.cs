namespace InventoryTransferConsole.Models;

public sealed class WorkerAccountRow
{
    public string Account { get; set; } = string.Empty;
    public string LoginState { get; set; } = string.Empty;
    public int Inventory { get; set; }
    public int Tradable { get; set; }
    public int Cooldown { get; set; }
    public int Sent { get; set; }
    public string TaskState { get; set; } = string.Empty;
    public string MaFile { get; set; } = string.Empty;
    public string SteamId { get; set; } = string.Empty;
    public string RecentOfferId { get; set; } = string.Empty;
    public string RecentError { get; set; } = string.Empty;
}
