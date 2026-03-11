using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public sealed class MockTransferController : ITransferController
{
    public DashboardSnapshot LoadSnapshot()
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

    public void StartTransfer(ILogSink logSink)
    {
        logSink.Info("Start transfer clicked.");
    }

    public void StopTransfer(ILogSink logSink)
    {
        logSink.Warn("Stop transfer clicked.");
    }

    public void ImportAccounts(ILogSink logSink)
    {
        logSink.Info("Import accounts clicked.");
    }

    public void ImportMaFiles(ILogSink logSink)
    {
        logSink.Info("Import maFiles clicked.");
    }

    public void ViewInventory(ILogSink logSink)
    {
        logSink.Info("View inventory clicked.");
    }

    public void ExportResults(ILogSink logSink)
    {
        logSink.Info("Export clicked.");
    }
}
