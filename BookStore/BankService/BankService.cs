using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace BankService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class BankService : StatefulService, IBank, Common.Interfaces.ITransaction
    {
        private IReliableDictionary<long, Client>? _clientDictionary;
        public BankService(StatefulServiceContext context)
            : base(context)
        { }

        private async Task InitializeClientDictionaryAsync()
        {
            _clientDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<long, Client>>("clientDictionary");
        }


        public async Task<string> EnlistMoneyTransfer(long? userSend, long? userReceive, double? amount)
        {
            using (var transaction = StateManager.CreateTransaction())
            {
                ConditionalValue<Client> clientToSend = await _clientDictionary!.TryGetValueAsync(transaction, userSend!.Value);
                ConditionalValue<Client> clientToReceive = await _clientDictionary!.TryGetValueAsync(transaction, userReceive!.Value);

                var transactionContext = new TransactionContext { ClientToSend = clientToSend, ClientToReceive = clientToReceive };

                if (!await Prepare(transactionContext, amount!.Value))
                {
                    return null!;
                }

                var clientToSendUpdate = clientToSend.Value;
                var clientToReceiveUpdate = clientToReceive.Value;

                clientToSendUpdate.BankAccount -= amount;
                clientToReceiveUpdate.BankAccount += amount;

                await _clientDictionary.TryUpdateAsync(transaction, userSend!.Value, clientToSendUpdate, clientToSend.Value);
                await _clientDictionary.TryUpdateAsync(transaction, userReceive!.Value, clientToReceiveUpdate, clientToReceive.Value);

                //await transaction.CommitAsync();

                //return string.Empty;

                return await FinishTransaction(transaction);
            }
        }
        public async Task<string> FinishTransaction(Microsoft.ServiceFabric.Data.ITransaction transaction)
        {
            try
            {
                await Commit(transaction);
                return string.Empty;
            }
            catch
            {
                await RollBack(transaction);
                return null!;
            }
        }

        public async Task<List<string>> ListClients()
        {
            await InitializeClientDictionaryAsync();

            var clients = new List<Client>()
            {
                new Client() { Id = 1, FirstName = "Milorad", LastName = "Maksic", DateOfBirth = new DateTime(1990, 1, 1), BankName = "OTP Bank", BankAccount = 1000.00, BankMembership = BankMembership.First.GetDescription() },
                new Client() { Id = 2, FirstName = "Ime", LastName = "Prezime", DateOfBirth = new DateTime(1985, 5, 15), BankName = "UniCredit", BankAccount = 750.50, BankMembership = BankMembership.Second.GetDescription() },
                new Client() { Id = 3, FirstName = "Mike", LastName = "Tyson", DateOfBirth = new DateTime(1995, 10, 8), BankName = "Banka", BankAccount = 500.75, BankMembership = BankMembership.Third.GetDescription() },
                new Client() { Id = 4, FirstName = "Jake", LastName = "Paul", DateOfBirth = new DateTime(1982, 3, 20), BankName = "Banka postanske stedionice", BankAccount = 2000.25, BankMembership = BankMembership.Fourth.GetDescription() },
                new Client() { Id = 5, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1998, 7, 3), BankName = "Jos neka banka", BankAccount = 1200.90, BankMembership = BankMembership.Second.GetDescription() }
            };

            using (var transaction = StateManager.CreateTransaction())
            {
                foreach (Client client in clients)
                    await _clientDictionary!.AddOrUpdateAsync(transaction, client.Id!.Value, client, (k, v) => v);

                //await transaction.CommitAsync();
                await FinishTransaction(transaction);
            }

            var clientsJson = new List<string>();

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerator = (await _clientDictionary!.CreateEnumerableAsync(transaction)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var client = enumerator.Current.Value;
                    // clientsJson.Add(JsonConvert.SerializeObject(client));
                    clientsJson.Add(JsonSerializer.Serialize(client));
                }
            }

            return clientsJson;
        }

        public async Task Commit(Microsoft.ServiceFabric.Data.ITransaction transaction)
        {
            await transaction.CommitAsync();
        }
        public async Task<bool> Prepare(TransactionContext context, object value)
        {
            if (!(value is double doubleParameter)) //value/amount
            {
                return false;
            }

            if (!context.ClientToSend.HasValue || !context.ClientToReceive.HasValue)
            {
                return false;
            }

            if (context.ClientToSend.Value.BankAccount < doubleParameter)
            {
                return false;
            }

            return true;
        }

        public async Task RollBack(Microsoft.ServiceFabric.Data.ITransaction transaction)
        {
             transaction.Abort();
            //await
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
            //return new ServiceReplicaListener[0];
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
