using Common.Models;
using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class TransactionContext
    {
        public ConditionalValue<Book> Book { get; set; }

        public ConditionalValue<Client> ClientToSend { get; set; }

        public ConditionalValue<Client> ClientToReceive { get; set; }
    }
}
