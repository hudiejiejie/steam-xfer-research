using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public sealed class LegacyPortContext
{
    public string DecompiledProjectRoot { get; init; } = string.Empty;
    public string OriginalExeRoot { get; init; } = string.Empty;
    public string MaFileDirectory { get; init; } = string.Empty;
    public string WorkingDirectory { get; init; } = string.Empty;

    public static LegacyPortContext FromSettings(AppSettings settings)
    {
        return new LegacyPortContext
        {
            DecompiledProjectRoot = settings.LegacyBridge.DecompiledProjectRoot,
            OriginalExeRoot = settings.LegacyBridge.OriginalExeRoot,
            MaFileDirectory = settings.LegacyBridge.MaFileDirectory,
            WorkingDirectory = settings.LegacyBridge.WorkingDirectory
        };
    }

    public static LegacyPortContext Empty() => new();
}
