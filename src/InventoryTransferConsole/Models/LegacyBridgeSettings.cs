namespace InventoryTransferConsole.Models;

public sealed class LegacyBridgeSettings
{
    public bool Enabled { get; set; }
    public string DecompiledProjectRoot { get; set; } = string.Empty;
    public string OriginalExeRoot { get; set; } = string.Empty;
    public string MaFileDirectory { get; set; } = string.Empty;
    public string WorkingDirectory { get; set; } = string.Empty;
}
