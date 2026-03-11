using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public interface ISettingsService
{
    AppSettings Load();
    void Save(AppSettings settings);
    string SettingsPath { get; }
}
