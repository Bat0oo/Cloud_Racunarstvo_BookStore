namespace Communication.TransactionCoordinator
{
    public interface ITransaction
    {
        Task<bool> Prepare(TransactionContext context, object value);

        Task Commit(Microsoft.ServiceFabric.Data.ITransaction transaction);

        Task RollBack(Microsoft.ServiceFabric.Data.ITransaction transaction);
    }
}
