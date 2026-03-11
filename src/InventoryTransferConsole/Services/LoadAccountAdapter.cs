using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public sealed class LoadAccountAdapter : ILoadAccountAdapter
{
    public List<AccountAggregate> Convert(ImportAccountsResult importResult)
    {
        var result = new List<AccountAggregate>();
        var index = 1;

        foreach (var line in importResult.RawLines)
        {
            var parsed = SplitLine(line);
            if (parsed is null) continue;

            result.Add(new AccountAggregate
            {
                IsMaster = false,
                Profile = new AccountProfile
                {
                    AccountName = parsed.Value.account,
                    AccountPassword = parsed.Value.password,
                    SteamId = $"7656119...IMP{index:000}"
                },
                Guard = new SteamGuardAsset
                {
                    MaFileName = string.Empty,
                    MaFilePath = string.Empty,
                    SharedSecret = string.Empty,
                    IdentitySecret = string.Empty
                },
                Runtime = new AccountRuntimeState
                {
                    IsLoggedIn = false,
                    LoginState = "Imported",
                    Session = string.Empty,
                    TradeToken = string.Empty,
                    TradeOfferUrl = string.Empty
                },
                Trade = new TradeState
                {
                    SentCount = 0,
                    PendingReceiveCount = 0,
                    RecentOfferId = "-",
                    TaskState = "Awaiting Login"
                },
                Inventory = new InventorySnapshot
                {
                    TotalCount = 0,
                    TradableCount = 0,
                    CooldownCount = 0
                },
                RecentError = "None"
            });

            index++;
        }

        return result;
    }

    private static (string account, string password)? SplitLine(string line)
    {
        foreach (var sep in new[] { "----", ":", ",", "|" })
        {
            var parts = line.Split(sep, StringSplitOptions.TrimEntries);
            if (parts.Length >= 2 && !string.IsNullOrWhiteSpace(parts[0]) && !string.IsNullOrWhiteSpace(parts[1]))
                return (parts[0], parts[1]);
        }
        return null;
    }
}
