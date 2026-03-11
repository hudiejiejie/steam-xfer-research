using InventoryTransferConsole.Models;

namespace InventoryTransferConsole.Services;

public interface ILoadAccountAdapter
{
    List<AccountAggregate> Convert(ImportAccountsResult importResult);
}
