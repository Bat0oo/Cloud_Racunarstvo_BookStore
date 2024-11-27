using Microsoft.ServiceFabric.Services.Remoting;

namespace Communication.Bookstore
{
    public interface IBookstore : IService
    {
        Task<List<string>> ListAvailableItems();

        Task<string> EnlistPurchase(long? bookId, uint? count);

        Task<string> GetItemPrice(long? bookId);

        Task<string> GetItem(long? bookId);
    }
}
