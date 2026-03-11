using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public static class AccountImportService
{
    private static readonly string[] Separators = ["----", "——", "—", ":", ",", "|"];

    public static ImportAccountsResult ParseText(string text)
    {
        var lines = text.Replace("\r", string.Empty)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var result = new ImportAccountsResult { RawLines = lines };
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in lines)
        {
            var parsed = SplitLine(line);
            if (parsed is null || string.IsNullOrWhiteSpace(parsed.Value.account) || string.IsNullOrWhiteSpace(parsed.Value.password))
            {
                result.InvalidCount++;
                continue;
            }

            if (!seen.Add(parsed.Value.account))
                continue;

            result.ParsedAccounts.Add(new ImportedAccountCredential
            {
                Account = parsed.Value.account,
                Password = parsed.Value.password
            });
            result.ParsedCount++;
        }

        return result;
    }

    public static ImportAccountsResult ParseFile(string filePath)
        => ParseText(File.ReadAllText(filePath));

    public static bool HasMaFileToken(string account)
    {
        var mafDir = GetMafDirectory();
        return Directory.EnumerateFiles(mafDir)
            .Any(file => Path.GetFileNameWithoutExtension(file).Equals(account, StringComparison.OrdinalIgnoreCase));
    }

    public static string ResolveMaFileState(string account)
        => HasMaFileToken(account) ? "Bound" : "Missing";

    public static string GetMafDirectory()
    {
        var mafDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "maf"));
        Directory.CreateDirectory(mafDir);
        return mafDir;
    }

    private static (string account, string password)? SplitLine(string line)
    {
        foreach (var sep in Separators)
        {
            var parts = line.Split(sep, StringSplitOptions.TrimEntries);
            if (parts.Length >= 2)
                return (parts[0], parts[1]);
        }

        return null;
    }
}
