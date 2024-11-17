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

namespace BookStoreService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class BookStoreService : StatefulService, IBookstore, Common.Interfaces.ITransaction
    {
        private IReliableDictionary<long, Book>? _bookDictionary;
        public BookStoreService(StatefulServiceContext context)
            : base(context)
        { }
        private async Task InitializeBookDictionaryAsync()
        {
            _bookDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<long, Book>>("bookDictionary");
        }



        public async Task<string> EnlistPurchase(long? bookId, uint? count)
        {
            using (var transaction = StateManager.CreateTransaction())
            {
                ConditionalValue<Book> book = await _bookDictionary!.TryGetValueAsync(transaction, bookId!.Value);
                var transactionContext = new TransactionContext { Book = book };

                if (!await Prepare(transactionContext, count!.Value))
                {
                    return null!;
                }

                var bookToUpdate = book.Value;

                bookToUpdate.Quantity -= Convert.ToInt32(count);

                await _bookDictionary.TryUpdateAsync(transaction, bookId!.Value, bookToUpdate, book.Value);

                //await transaction.CommitAsync();

                //return string.Empty;

                return await FinishTransaction(transaction);
            }
        }

        public async Task<string> GetItem(long? bookId)
        {
            using (var transaction = StateManager.CreateTransaction())
            {
                var book = await _bookDictionary!.TryGetValueAsync(transaction, bookId!.Value);

                return JsonSerializer.Serialize(book.Value);
            }

            throw null!;
        }

        public async Task<string> GetItemPrice(long? bookId)
        {
            using (var transaction = StateManager.CreateTransaction())
            {
                var book = await _bookDictionary!.TryGetValueAsync(transaction, bookId!.Value);

                return book.Value.Price!.Value.ToString();
            }

            throw null!;
        }

        public async Task<List<string>> ListAvailableItems()
        {

            await InitializeBookDictionaryAsync();

            var books = new List<Book>()
            {
                new Book(){ Id = 1, Title = "Seobe", Author = "Milos Crnjanski", PagesNumber = 281, PublicationYear = 1960, Price = 9.99, Quantity = 10 },
                new Book(){ Id = 2, Title = "1984", Author = "Dzordz Osvel", PagesNumber = 328, PublicationYear = 1949, Price = 10.60, Quantity = 10 },
                new Book(){ Id = 3, Title = "Na Drini Cuprija", Author = "Ivo Andric", PagesNumber = 180, PublicationYear = 1925, Price = 22.35, Quantity = 10 },
                new Book(){ Id = 4, Title = "Tvrdjava", Author = "Mesa Selimovic", PagesNumber = 310 , PublicationYear = 1937, Price = 50.00, Quantity = 10 },
                new Book(){ Id = 5, Title = "Seobe 2", Author = "Milos Crnjanski", PagesNumber = 279, PublicationYear = 1813, Price = 14.85, Quantity = 10 }
            };

            using (var transaction = StateManager.CreateTransaction())
            {
                foreach (Book book in books)
                    await _bookDictionary!.AddOrUpdateAsync(transaction, book.Id!.Value, book, (k, v) => v);

                //await transaction.CommitAsync();
                await FinishTransaction(transaction);
            }

            var booksJson = new List<string>();

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerator = (await _bookDictionary!.CreateEnumerableAsync(transaction)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var book = enumerator.Current.Value;
                    //booksJson.Add(JsonConvert.SerializeObject(book));
                    booksJson.Add(JsonSerializer.Serialize(book));
                }
            }

            return booksJson;
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
        public async Task<bool> Prepare(TransactionContext context, object value)
        {

            if (!(value is uint uintParameter))
            {
                return false;
            }

            if (!context.Book.HasValue)
            {
                return false;
            }

            if (context.Book.Value.Quantity < uintParameter)
            {
                return false;
            }

            return true;
        }

        public async Task Commit(Microsoft.ServiceFabric.Data.ITransaction transaction)
        {
            await transaction.CommitAsync();
        }

        public async Task RollBack(Microsoft.ServiceFabric.Data.ITransaction transaction)
        {
            transaction.Abort();
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
