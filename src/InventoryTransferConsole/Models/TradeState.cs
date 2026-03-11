namespace InventoryTransferConsole.Models;

public sealed class TradeState
{
    public int SentCount { get; set; }
    public int PendingReceiveCount { get; set; }
    public string RecentOfferId { get; set; } = string.Empty;
    public string TaskState { get; set; } = string.Empty;
}
