using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public sealed class MockTransferController : ITransferController
{
    private DashboardSnapshot snapshot;

    public MockTransferController()
    {
        snapshot = BuildDefaultSnapshot();
    }

    public DashboardSnapshot LoadSnapshot() => Clone(snapshot);

    public DashboardSnapshot StartTransfer(RuntimeSettings settings, ILogSink logSink)
    {
        snapshot.ThreadCount = settings.ThreadCount;
        snapshot.ModeSummary = settings.TransferTypeIndex == 0 ? "Send+Accept" : "Send All Then Accept";
        snapshot.RunState = "Running";
        foreach (var worker in snapshot.Workers)
        {
            if (worker.LoginState == "Online")
                worker.TaskState = worker.Sent > 0 ? "Offer Sent" : "Queued";
        }
        logSink.Info($"Transfer started. Threads={settings.ThreadCount}, Mode={snapshot.ModeSummary}, Filter={settings.ItemFilterValue}");
        return Clone(snapshot);
    }

    public DashboardSnapshot StopTransfer(ILogSink logSink)
    {
        snapshot.RunState = "Stopping";
        logSink.Warn("Transfer stop requested.");
        return Clone(snapshot);
    }

    public DashboardSnapshot ImportAccounts(ImportAccountsResult importResult, ILogSink logSink)
    {
        var nextIndex = snapshot.Workers.Count + 1;
        foreach (var line in importResult.RawLines)
        {
            var parsed = SplitLine(line);
            if (parsed is null) continue;
            snapshot.Workers.Add(new WorkerAccountRow
            {
                Account = parsed.Value.account,
                LoginState = "Imported",
                Inventory = 0,
                Tradable = 0,
                Cooldown = 0,
                Sent = 0,
                TaskState = "Awaiting Login",
                MaFile = "Missing",
                SteamId = $"7656119...{nextIndex:000}",
                RecentOfferId = "-",
                RecentError = "None"
            });
            nextIndex++;
        }
        logSink.Info($"Imported {importResult.ParsedCount} account(s). Invalid={importResult.InvalidCount}");
        return Clone(snapshot);
    }

    public DashboardSnapshot ImportMaFiles(ILogSink logSink)
    {
        var matched = 0;
        // 模拟：为主库号绑定令牌（maFile），仅修改主库号
        foreach (var master in snapshot.Masters.Where(m => m.MaFile == "Missing").Take(2))
        {
            master.MaFile = "Bound";
            matched++;
        }
        logSink.Info($"已模拟绑定主库号令牌（maFiles）{matched} 个。");
        return Clone(snapshot);
    }

    public void ViewInventory(ILogSink logSink)
    {
        logSink.Info("View inventory clicked.");
    }

    public void ExportResults(ILogSink logSink)
    {
        logSink.Info("Export clicked.");
    }

    private static DashboardSnapshot BuildDefaultSnapshot()
    {
        return new DashboardSnapshot
        {
            ThreadCount = 8,
            ModeSummary = "Send+Accept",
            RunState = "Idle",
            Masters =
            [
                new MasterAccountRow
                {
                    Account = "master_01",
                    SteamId = "7656119...001",
                    LoginState = "Online",
                    Pending = 3,
                    Assigned = 87,
                    Limit = 200,
                    MaFile = "Bound"
                },
                new MasterAccountRow
                {
                    Account = "master_02",
                    SteamId = "7656119...002",
                    LoginState = "Offline",
                    Pending = 0,
                    Assigned = 0,
                    Limit = 200,
                    MaFile = "Missing"
                }
            ],
            Workers =
            [
                new WorkerAccountRow
                {
                    Account = "worker_01",
                    LoginState = "Online",
                    Inventory = 154,
                    Tradable = 147,
                    Cooldown = 7,
                    Sent = 2,
                    TaskState = "Offer Sent",
                    MaFile = "Bound",
                    SteamId = "7656119...003",
                    RecentOfferId = "#1039284",
                    RecentError = "None"
                },
                new WorkerAccountRow
                {
                    Account = "worker_02",
                    LoginState = "Online",
                    Inventory = 91,
                    Tradable = 88,
                    Cooldown = 3,
                    Sent = 0,
                    TaskState = "Inventory Ready",
                    MaFile = "Bound",
                    SteamId = "7656119...004",
                    RecentOfferId = "#1039285",
                    RecentError = "None"
                },
                new WorkerAccountRow
                {
                    Account = "worker_03",
                    LoginState = "Failed",
                    Inventory = 0,
                    Tradable = 0,
                    Cooldown = 0,
                    Sent = 0,
                    TaskState = "Login Failed",
                    MaFile = "Missing",
                    SteamId = "7656119...005",
                    RecentOfferId = "-",
                    RecentError = "Login Failed"
                }
            ]
        };
    }

    private static (string account, string password)? SplitLine(string line)
    {
        foreach (var sep in new[] { "----", ":", ",", "|" })
        {
            var parts = line.Split(sep, StringSplitOptions.TrimEntries);
            if (parts.Length >= 2)
                return (parts[0], parts[1]);
        }
        return null;
    }

    private static DashboardSnapshot Clone(DashboardSnapshot src)
    {
        return new DashboardSnapshot
        {
            ThreadCount = src.ThreadCount,
            ModeSummary = src.ModeSummary,
            RunState = src.RunState,
            Masters = src.Masters.Select(m => new MasterAccountRow
            {
                Account = m.Account,
                SteamId = m.SteamId,
                LoginState = m.LoginState,
                Pending = m.Pending,
                Assigned = m.Assigned,
                Limit = m.Limit,
                MaFile = m.MaFile
            }).ToList(),
            Workers = src.Workers.Select(w => new WorkerAccountRow
            {
                Account = w.Account,
                LoginState = w.LoginState,
                Inventory = w.Inventory,
                Tradable = w.Tradable,
                Cooldown = w.Cooldown,
                Sent = w.Sent,
                TaskState = w.TaskState,
                MaFile = w.MaFile,
                SteamId = w.SteamId,
                RecentOfferId = w.RecentOfferId,
                RecentError = w.RecentError
            }).ToList()
        };
    }
}
