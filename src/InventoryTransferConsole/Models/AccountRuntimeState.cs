namespace InventoryTransferConsole.Models;

public sealed class AccountRuntimeState
{
    public bool IsLoggedIn { get; set; }
    public string LoginState { get; set; } = string.Empty;
    public string Session { get; set; } = string.Empty;
    public string TradeToken { get; set; } = string.Empty;
    public string TradeOfferUrl { get; set; } = string.Empty;
}
