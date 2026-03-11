namespace InventoryTransferConsole.Models;

public sealed class RuntimeSettings
{
    public int ThreadCount { get; set; } = 8;
    public int TransferTypeIndex { get; set; } = 0;
    public int AcceptModeIndex { get; set; } = 0;
    public int ItemTypeIndex { get; set; } = 0;
    public string ItemFilterValue { get; set; } = string.Empty;

    // Closer to original Setting.json direction
    public int MaxPerMaster { get; set; } = 200;
    public bool UseProxy { get; set; }
    public string ProxyAddress { get; set; } = string.Empty;
    public bool RememberPassword { get; set; }
}
