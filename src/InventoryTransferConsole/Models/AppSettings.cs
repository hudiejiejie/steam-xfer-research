namespace InventoryTransferConsole.Models;

public sealed class AppSettings
{
    public RuntimeSettings Runtime { get; set; } = new();
    public LegacyBridgeSettings LegacyBridge { get; set; } = new();
}
