namespace InventoryTransferConsole.Models;

public sealed class RuntimeSettings
{
    public int ThreadCount { get; set; } = 8;
    public int TransferTypeIndex { get; set; } = 0;
    public int AcceptModeIndex { get; set; } = 0;
    public int ItemTypeIndex { get; set; } = 0;
    public string ItemFilterValue { get; set; } = string.Empty;
}
