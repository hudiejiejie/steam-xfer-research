using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public sealed class MockTransferController : ITransferController
{
    private readonly AccountWorkspaceRepository repository;
    private DashboardSnapshot snapshot;

    public MockTransferController()
    {
        repository = new AccountWorkspaceRepository();
        snapshot = BuildDefaultSnapshot();
        LoadPersistedAccountsIntoSnapshot();
        SyncMaFileState(snapshot);
    }

    public DashboardSnapshot LoadSnapshot()
    {
        SyncMaFileState(snapshot);
        return Clone(snapshot);
    }

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
        var categories = settings.SelectedItemCategories.Count > 0
            ? string.Join("、", settings.SelectedItemCategories)
            : "未选择";
        logSink.Info($"Transfer started. Threads={settings.ThreadCount}, Mode={snapshot.ModeSummary}, Categories={categories}");
        return Clone(snapshot);
    }

    public DashboardSnapshot StopTransfer(ILogSink logSink)
    {
        snapshot.RunState = "Stopping";
        logSink.Warn("Transfer stop requested.");
        return Clone(snapshot);
    }

    public DashboardSnapshot ImportMasters(ImportAccountsResult importResult, ILogSink logSink)
    {
        var nextIndex = snapshot.Masters.Count + 1;
        var imported = 0;
        foreach (var parsed in importResult.ParsedAccounts)
        {
            if (snapshot.Masters.Any(x => x.Account.Equals(parsed.Account, StringComparison.OrdinalIgnoreCase)))
                continue;

            snapshot.Masters.Add(new MasterAccountRow
            {
                Account = parsed.Account,
                SteamId = $"7656119...M{nextIndex:000}",
                LoginState = "Imported",
                Pending = 0,
                Assigned = 0,
                Limit = 200,
                MaFile = AccountImportService.ResolveMaFileState(parsed.Account)
            });
            nextIndex++;
            imported++;
        }
        repository.UpsertMasters(importResult.ParsedAccounts);
        SyncMaFileState(snapshot);
        logSink.Info($"已载入主库号 {imported} 个；令牌统一从 maf 文件夹自动匹配。");
        return Clone(snapshot);
    }

    public DashboardSnapshot ImportWorkers(ImportAccountsResult importResult, ILogSink logSink)
    {
        var nextIndex = snapshot.Workers.Count + 1;
        var imported = 0;
        foreach (var parsed in importResult.ParsedAccounts)
        {
            if (snapshot.Workers.Any(x => x.Account.Equals(parsed.Account, StringComparison.OrdinalIgnoreCase)))
                continue;

            snapshot.Workers.Add(new WorkerAccountRow
            {
                Account = parsed.Account,
                LoginState = "Imported",
                Inventory = 0,
                Tradable = 0,
                Cooldown = 0,
                Sent = 0,
                TaskState = "Awaiting Login",
                MaFile = AccountImportService.ResolveMaFileState(parsed.Account),
                SteamId = $"7656119...W{nextIndex:000}",
                RecentOfferId = "-",
                RecentError = "None"
            });
            nextIndex++;
            imported++;
        }
        repository.UpsertWorkers(importResult.ParsedAccounts);
        SyncMaFileState(snapshot);
        logSink.Info($"已载入待转号 {imported} 个；令牌统一从 maf 文件夹自动匹配。");
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

    private void LoadPersistedAccountsIntoSnapshot()
    {
        var nextMasterIndex = snapshot.Masters.Count + 1;
        foreach (var account in repository.LoadMasters())
        {
            if (snapshot.Masters.Any(x => x.Account.Equals(account.Account, StringComparison.OrdinalIgnoreCase)))
                continue;

            snapshot.Masters.Add(new MasterAccountRow
            {
                Account = account.Account,
                SteamId = $"7656119...M{nextMasterIndex:000}",
                LoginState = "Imported",
                Pending = 0,
                Assigned = 0,
                Limit = 200,
                MaFile = AccountImportService.ResolveMaFileState(account.Account)
            });
            nextMasterIndex++;
        }

        var nextWorkerIndex = snapshot.Workers.Count + 1;
        foreach (var account in repository.LoadWorkers())
        {
            if (snapshot.Workers.Any(x => x.Account.Equals(account.Account, StringComparison.OrdinalIgnoreCase)))
                continue;

            snapshot.Workers.Add(new WorkerAccountRow
            {
                Account = account.Account,
                LoginState = "Imported",
                Inventory = 0,
                Tradable = 0,
                Cooldown = 0,
                Sent = 0,
                TaskState = "Awaiting Login",
                MaFile = AccountImportService.ResolveMaFileState(account.Account),
                SteamId = $"7656119...W{nextWorkerIndex:000}",
                RecentOfferId = "-",
                RecentError = "None"
            });
            nextWorkerIndex++;
        }
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

    private static void SyncMaFileState(DashboardSnapshot data)
    {
        foreach (var master in data.Masters)
            master.MaFile = AccountImportService.ResolveMaFileState(master.Account);
        foreach (var worker in data.Workers)
            worker.MaFile = AccountImportService.ResolveMaFileState(worker.Account);
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
