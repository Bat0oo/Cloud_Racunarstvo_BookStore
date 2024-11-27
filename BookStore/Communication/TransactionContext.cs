using Communication.Models;
using Microsoft.ServiceFabric.Data;

namespace Communication
{
    public class TransactionContext
    {
        public ConditionalValue<Book> Book { get; set; }

        public ConditionalValue<Client> ClientToSend { get; set; }

        public ConditionalValue<Client> ClientToReceive { get; set; }
    }
}
