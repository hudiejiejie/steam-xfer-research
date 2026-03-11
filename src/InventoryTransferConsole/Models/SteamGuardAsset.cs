namespace InventoryTransferConsole.Models;

public sealed class SteamGuardAsset
{
    public string MaFileName { get; set; } = string.Empty;
    public string MaFilePath { get; set; } = string.Empty;
    public string SharedSecret { get; set; } = string.Empty;
    public string IdentitySecret { get; set; } = string.Empty;
    public bool IsBound => !string.IsNullOrWhiteSpace(MaFilePath) || !string.IsNullOrWhiteSpace(MaFileName);
}
