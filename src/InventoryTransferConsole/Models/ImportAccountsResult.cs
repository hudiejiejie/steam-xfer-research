namespace InventoryTransferConsole.Models;

public sealed class ImportAccountsResult
{
    public List<string> RawLines { get; set; } = [];
    public int ParsedCount { get; set; }
    public int InvalidCount { get; set; }
}
