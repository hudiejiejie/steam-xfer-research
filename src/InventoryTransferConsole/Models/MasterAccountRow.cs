namespace InventoryTransferConsole.Models;

public sealed class MasterAccountRow
{
    public string Account { get; set; } = string.Empty;
    public string SteamId { get; set; } = string.Empty;
    public string LoginState { get; set; } = string.Empty;
    public int Pending { get; set; }
    public int Assigned { get; set; }
    public int Limit { get; set; }
    public string MaFile { get; set; } = string.Empty;
}
