using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Interfaces;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ValidationService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class ValidationService : StatelessService, IValidation
    {
        private readonly string transactionCoordinatorPath = @"fabric:/BookStore/TransactionCoordinator";
        public ValidationService(StatelessServiceContext context)
            : base(context)
        { }

        public async Task<string> EnlistMoneyTransfer(long? userSend, long? userReceive, double? amount)
        {
            if (userSend is null || userReceive is null || amount is null)
            {
                return null!;
            }

            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.EnlistMoneyTransfer(userSend!.Value, userReceive!.Value, amount!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> EnlistPurchase(long? bookId, uint? count)
        {
            if (bookId is null || count is null)
            {
                return null!;
            }

            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.EnlistPurchase(bookId!.Value, count!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> GetItem(long? bookId)
        {
            if (bookId is null)
            {
                return null!;
            }

            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.GetItem(bookId!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> GetItemPrice(long? bookId)
        {
            if (bookId is null)
            {
                return null!;
            }

            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.GetItemPrice(bookId!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<List<string>> ListAvailableItems()
        {
            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.ListAvailableItems();
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<List<string>> ListClients()
        {
            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.ListClients();
            }
            catch (Exception)
            {
                return null!;
            }
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            //return new ServiceInstanceListener[0];
            return this.CreateServiceRemotingInstanceListeners();
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
