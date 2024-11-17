using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface ITransaction
    {
        Task<bool> Prepare(TransactionContext context, object value);

        Task Commit(Microsoft.ServiceFabric.Data.ITransaction transaction);

        Task RollBack(Microsoft.ServiceFabric.Data.ITransaction transaction);
    }
}
