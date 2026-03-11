namespace InventoryTransferConsole.Models;

public sealed class AccountAggregate
{
    public AccountProfile Profile { get; set; } = new();
    public SteamGuardAsset Guard { get; set; } = new();
    public AccountRuntimeState Runtime { get; set; } = new();
    public TradeState Trade { get; set; } = new();
    public InventorySnapshot Inventory { get; set; } = new();
    public bool IsMaster { get; set; }
    public string RecentError { get; set; } = string.Empty;
}
