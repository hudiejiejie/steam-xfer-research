using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public sealed class AccountWorkspaceRepository
{
    private readonly string rootDir;
    private readonly string mastersPath;
    private readonly string workersPath;

    public AccountWorkspaceRepository(string? rootDir = null)
    {
        this.rootDir = rootDir ?? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "data"));
        Directory.CreateDirectory(this.rootDir);
        mastersPath = Path.Combine(this.rootDir, "masters.txt");
        workersPath = Path.Combine(this.rootDir, "workers.txt");
    }

    public IReadOnlyList<ImportedAccountCredential> LoadMasters()
        => LoadFile(mastersPath);

    public IReadOnlyList<ImportedAccountCredential> LoadWorkers()
        => LoadFile(workersPath);

    public void UpsertMasters(IEnumerable<ImportedAccountCredential> accounts)
        => SaveMerged(mastersPath, accounts);

    public void UpsertWorkers(IEnumerable<ImportedAccountCredential> accounts)
        => SaveMerged(workersPath, accounts);

    private static IReadOnlyList<ImportedAccountCredential> LoadFile(string path)
    {
        if (!File.Exists(path))
            return [];

        return AccountImportService.ParseFile(path).ParsedAccounts;
    }

    private static void SaveMerged(string path, IEnumerable<ImportedAccountCredential> accounts)
    {
        var merged = LoadFile(path)
            .Concat(accounts)
            .GroupBy(x => x.Account, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .OrderBy(x => x.Account, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var lines = merged.Select(x => $"{x.Account}----{x.Password}");
        File.WriteAllLines(path, lines);
    }
}
