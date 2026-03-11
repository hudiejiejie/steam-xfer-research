namespace InventoryTransferConsole;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new TransferInventoryConsoleForm());
    }
}

