namespace InventoryTransferConsole.Models;

public sealed class TransferConfig
{
    public RuntimeSettings Runtime { get; set; } = new();
    public List<AccountAggregate> Masters { get; set; } = [];
    public List<AccountAggregate> Workers { get; set; } = [];
}
