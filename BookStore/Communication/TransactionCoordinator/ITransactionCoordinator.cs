using Microsoft.ServiceFabric.Services.Remoting;

namespace Communication.TransactionCoordinator
{
    public interface ITransactionCoordinator : IService
    {
        Task<List<string>> ListAvailableItems();

        Task<string> EnlistPurchase(long? bookId, uint? count);

        Task<string> GetItemPrice(long? bookId);

        Task<string> GetItem(long? bookId);

        Task<List<string>> ListClients();

        Task<string> EnlistMoneyTransfer(long? userSend, long? userReceive, double? amount);
    }
}
