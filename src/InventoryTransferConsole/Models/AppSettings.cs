namespace InventoryTransferConsole.Models;

public sealed class AppSettings
{
    public RuntimeSettings Runtime { get; set; } = new();
}
