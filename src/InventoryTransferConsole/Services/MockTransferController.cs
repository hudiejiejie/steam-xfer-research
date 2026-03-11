using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public sealed class MockTransferController : ITransferController
{
    private readonly ILoadAccountAdapter loadAccountAdapter;
    private DashboardSnapshot snapshot;

    public MockTransferController()
        : this(new LoadAccountAdapter())
    {
    }

    public MockTransferController(ILoadAccountAdapter loadAccountAdapter)
    {
        this.loadAccountAdapter = loadAccountAdapter;
        snapshot = BuildDefaultSnapshot();
    }

    public DashboardSnapshot LoadSnapshot() => Clone(snapshot);

    public DashboardSnapshot StartTransfer(TransferConfig config, ILogSink logSink)
    {
        snapshot.ThreadCount = config.Runtime.ThreadCount;
        snapshot.ModeSummary = config.Runtime.TransferTypeIndex == 0 ? "边发边接" : "全发后统一接受";
        snapshot.RunState = "运行中";

        foreach (var worker in snapshot.Workers)
        {
            if (worker.LoginState == "Online" || worker.LoginState == "Imported")
                worker.TaskState = worker.Sent > 0 ? "Offer Sent" : "Queued";
        }

        SyncAggregatesFromRows();
        logSink.Info($"Transfer started. Threads={config.Runtime.ThreadCount}, Mode={snapshot.ModeSummary}, Filter={config.Runtime.ItemFilterValue}, Masters={config.Masters.Count}, Workers={config.Workers.Count}");
        return Clone(snapshot);
    }

    public DashboardSnapshot StopTransfer(ILogSink logSink)
    {
        snapshot.RunState = "停止中";
        logSink.Warn("Transfer stop requested.");
        return Clone(snapshot);
    }

    public DashboardSnapshot ImportAccounts(ImportAccountsResult importResult, ILogSink logSink)
    {
        var imported = loadAccountAdapter.Convert(importResult);
        foreach (var aggregate in imported)
        {
            snapshot.Workers.Add(new WorkerAccountRow
            {
                Account = aggregate.Profile.AccountName,
                LoginState = aggregate.Runtime.LoginState,
                Inventory = aggregate.Inventory.TotalCount,
                Tradable = aggregate.Inventory.TradableCount,
                Cooldown = aggregate.Inventory.CooldownCount,
                Sent = aggregate.Trade.SentCount,
                TaskState = aggregate.Trade.TaskState,
                MaFile = aggregate.Guard.IsBound ? "Bound" : "Missing",
                SteamId = aggregate.Profile.SteamId,
                RecentOfferId = aggregate.Trade.RecentOfferId,
                RecentError = aggregate.RecentError
            });
        }
        SyncAggregatesFromRows();
        logSink.Info($"Imported {importResult.ParsedCount} account(s). Invalid={importResult.InvalidCount}");
        return Clone(snapshot);
    }

    public DashboardSnapshot ImportMaFiles(ILogSink logSink)
    {
        var matched = 0;
        foreach (var worker in snapshot.Workers.Where(w => w.MaFile == "Missing").Take(2))
        {
            worker.MaFile = "Bound";
            matched++;
        }
        foreach (var master in snapshot.Masters.Where(m => m.MaFile == "Missing").Take(1))
        {
            master.MaFile = "Bound";
            matched++;
        }
        SyncAggregatesFromRows();
        logSink.Info($"Mock bound maFiles for {matched} account(s).");
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
        var snapshot = new DashboardSnapshot
        {
            ThreadCount = 8,
            ModeSummary = "边发边接",
            RunState = "空闲",
            Masters =
            [
                new MasterAccountRow { Account = "master_01", SteamId = "7656119...001", LoginState = "Online", Pending = 3, Assigned = 87, Limit = 200, MaFile = "Bound" },
                new MasterAccountRow { Account = "master_02", SteamId = "7656119...002", LoginState = "Offline", Pending = 0, Assigned = 0, Limit = 200, MaFile = "Missing" }
            ],
            Workers =
            [
                new WorkerAccountRow { Account = "worker_01", LoginState = "Online", Inventory = 154, Tradable = 147, Cooldown = 7, Sent = 2, TaskState = "Offer Sent", MaFile = "Bound", SteamId = "7656119...003", RecentOfferId = "#1039284", RecentError = "None" },
                new WorkerAccountRow { Account = "worker_02", LoginState = "Online", Inventory = 91, Tradable = 88, Cooldown = 3, Sent = 0, TaskState = "Inventory Ready", MaFile = "Bound", SteamId = "7656119...004", RecentOfferId = "#1039285", RecentError = "None" },
                new WorkerAccountRow { Account = "worker_03", LoginState = "Failed", Inventory = 0, Tradable = 0, Cooldown = 0, Sent = 0, TaskState = "Login Failed", MaFile = "Missing", SteamId = "7656119...005", RecentOfferId = "-", RecentError = "Login Failed" }
            ]
        };
        snapshot.Accounts = BuildAggregates(snapshot.Masters, snapshot.Workers);
        return snapshot;
    }

    private void SyncAggregatesFromRows()
    {
        snapshot.Accounts = BuildAggregates(snapshot.Masters, snapshot.Workers);
    }

    private static List<AccountAggregate> BuildAggregates(List<MasterAccountRow> masters, List<WorkerAccountRow> workers)
    {
        var result = new List<AccountAggregate>();

        result.AddRange(masters.Select(m => new AccountAggregate
        {
            IsMaster = true,
            Profile = new AccountProfile { AccountName = m.Account, SteamId = m.SteamId },
            Guard = new SteamGuardAsset { MaFileName = m.MaFile == "Bound" ? m.Account + ".maFile" : string.Empty, MaFilePath = m.MaFile == "Bound" ? $"C:/mafiles/{m.Account}.maFile" : string.Empty },
            Runtime = new AccountRuntimeState { IsLoggedIn = m.LoginState == "Online", LoginState = m.LoginState, TradeOfferUrl = m.SteamId },
            Trade = new TradeState { PendingReceiveCount = m.Pending, TaskState = "Receiver" },
            Inventory = new InventorySnapshot(),
            RecentError = m.LoginState == "Offline" ? "Offline" : string.Empty
        }));

        result.AddRange(workers.Select(w => new AccountAggregate
        {
            IsMaster = false,
            Profile = new AccountProfile { AccountName = w.Account, SteamId = w.SteamId },
            Guard = new SteamGuardAsset { MaFileName = w.MaFile == "Bound" ? w.Account + ".maFile" : string.Empty, MaFilePath = w.MaFile == "Bound" ? $"C:/mafiles/{w.Account}.maFile" : string.Empty },
            Runtime = new AccountRuntimeState { IsLoggedIn = w.LoginState == "Online" || w.LoginState == "Imported", LoginState = w.LoginState, TradeOfferUrl = w.RecentOfferId },
            Trade = new TradeState { SentCount = w.Sent, RecentOfferId = w.RecentOfferId, TaskState = w.TaskState },
            Inventory = new InventorySnapshot { TotalCount = w.Inventory, TradableCount = w.Tradable, CooldownCount = w.Cooldown },
            RecentError = w.RecentError
        }));

        return result;
    }

    private static DashboardSnapshot Clone(DashboardSnapshot src)
    {
        return new DashboardSnapshot
        {
            ThreadCount = src.ThreadCount,
            ModeSummary = src.ModeSummary,
            RunState = src.RunState,
            Masters = src.Masters.Select(m => new MasterAccountRow { Account = m.Account, SteamId = m.SteamId, LoginState = m.LoginState, Pending = m.Pending, Assigned = m.Assigned, Limit = m.Limit, MaFile = m.MaFile }).ToList(),
            Workers = src.Workers.Select(w => new WorkerAccountRow { Account = w.Account, LoginState = w.LoginState, Inventory = w.Inventory, Tradable = w.Tradable, Cooldown = w.Cooldown, Sent = w.Sent, TaskState = w.TaskState, MaFile = w.MaFile, SteamId = w.SteamId, RecentOfferId = w.RecentOfferId, RecentError = w.RecentError }).ToList(),
            Accounts = src.Accounts.Select(a => new AccountAggregate
            {
                IsMaster = a.IsMaster,
                Profile = new AccountProfile { AccountName = a.Profile.AccountName, AccountPassword = a.Profile.AccountPassword, SteamId = a.Profile.SteamId },
                Guard = new SteamGuardAsset { MaFileName = a.Guard.MaFileName, MaFilePath = a.Guard.MaFilePath, SharedSecret = a.Guard.SharedSecret, IdentitySecret = a.Guard.IdentitySecret },
                Runtime = new AccountRuntimeState { IsLoggedIn = a.Runtime.IsLoggedIn, LoginState = a.Runtime.LoginState, Session = a.Runtime.Session, TradeToken = a.Runtime.TradeToken, TradeOfferUrl = a.Runtime.TradeOfferUrl },
                Trade = new TradeState { SentCount = a.Trade.SentCount, PendingReceiveCount = a.Trade.PendingReceiveCount, RecentOfferId = a.Trade.RecentOfferId, TaskState = a.Trade.TaskState },
                Inventory = new InventorySnapshot { TotalCount = a.Inventory.TotalCount, TradableCount = a.Inventory.TradableCount, CooldownCount = a.Inventory.CooldownCount },
                RecentError = a.RecentError
            }).ToList()
        };
    }
}
