using Communication.Bank;
using Communication.Bookstore;
using Communication.Enums;
using Communication.TransactionCoordinator;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace TransactionCoordinator
{
    internal sealed class TransactionCoordinator : StatelessService, ITransactionCoordinator
    {
        private readonly string bookstorePath = @"fabric:/CloudVezbe/Bookstore";
        private readonly string bankPath = @"fabric:/CloudVezbe/Bank";

        public TransactionCoordinator(StatelessServiceContext context)
            : base(context)
        { }

        #region ITransactionCoordinatorImplementation

        public async Task<List<string>> ListAvailableItems()
        {
            IBookstore? bookstoreProxy = ServiceProxy.Create<IBookstore>(new Uri(bookstorePath), new ServicePartitionKey((int)PartiotionKeys.One));

            try
            {
                return await bookstoreProxy.ListAvailableItems();
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> EnlistPurchase(long? bookId, uint? count)
        {
            IBookstore? bookstoreProxy = ServiceProxy.Create<IBookstore>(new Uri(bookstorePath), new ServicePartitionKey((int)PartiotionKeys.One));

            try
            {
                return await bookstoreProxy.EnlistPurchase(bookId!.Value, count!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> GetItemPrice(long? bookId)
        {
            IBookstore? bookstoreProxy = ServiceProxy.Create<IBookstore>(new Uri(bookstorePath), new ServicePartitionKey((int)PartiotionKeys.One));

            try
            {
                return await bookstoreProxy.GetItemPrice(bookId!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> GetItem(long? bookId)
        {
            IBookstore? bookstoreProxy = ServiceProxy.Create<IBookstore>(new Uri(bookstorePath), new ServicePartitionKey((int)PartiotionKeys.One));

            try
            {
                return await bookstoreProxy.GetItem(bookId!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<List<string>> ListClients()
        {
            IBank? bankProxy = ServiceProxy.Create<IBank>(new Uri(bankPath), new ServicePartitionKey((int)PartiotionKeys.Two));

            try
            {
                return await bankProxy.ListClients();
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> EnlistMoneyTransfer(long? userSend, long? userReceive, double? amount)
        {
            IBank? bankProxy = ServiceProxy.Create<IBank>(new Uri(bankPath), new ServicePartitionKey((int)PartiotionKeys.Two));

            try
            {
                return await bankProxy.EnlistMoneyTransfer(userSend!.Value, userReceive!.Value, amount!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        #endregion

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }
    }
}
