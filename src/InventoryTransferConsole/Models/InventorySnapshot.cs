namespace InventoryTransferConsole.Models;

public sealed class InventorySnapshot
{
    public int TotalCount { get; set; }
    public int TradableCount { get; set; }
    public int CooldownCount { get; set; }
}
