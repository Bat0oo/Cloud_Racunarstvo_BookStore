using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Interfaces;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace TransactionCoordinator
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class TransactionCoordinator : StatefulService, ITransactionCoordinator
    {
        private readonly string bookstorePath = @"fabric:/BookStore/BookstoreService";
        private readonly string bankPath = @"fabric:/BookStore/BankService";
        public TransactionCoordinator(StatefulServiceContext context)
            : base(context)
        { }

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

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            // return new ServiceReplicaListener[0];
            return this.CreateServiceRemotingReplicaListeners();
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
