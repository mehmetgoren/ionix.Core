namespace Ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;

    public interface IBulkCopyCommand : IDisposable
    {
        void Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
        Task ExecuteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);

        void Execute(DataTable dataTable);
        Task ExecuteAsync(DataTable dataTable);
    }
}
