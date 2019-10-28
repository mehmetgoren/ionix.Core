namespace Ionix.Data.SqlServer
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public class BulkCopyCommand : IBulkCopyCommand
    {
        private readonly SqlConnection conn;
        public BulkCopyCommand(SqlConnection conn)
        {
            if (null == conn)
                throw new ArgumentNullException(nameof(conn));

            if (conn.State != ConnectionState.Open)
                throw new ArgumentException("'connection' is not open.");

            this.conn = conn;
        }

        private SqlBulkCopy CreateSqlBulkCopy(DataTable dataTable)
        {
            SqlBulkCopy s = new SqlBulkCopy(this.conn);
            s.BulkCopyTimeout = int.MaxValue;
            s.DestinationTableName = dataTable.TableName;

            foreach (var column in dataTable.Columns)
                s.ColumnMappings.Add(column.ToString(), column.ToString());

            return s;
        }

        private static bool EnsureDataTable(DataTable dataTable)
        {
            if (null != dataTable)
            {
                if (String.IsNullOrEmpty(dataTable.TableName))
                    throw new ArgumentException("DataTable.TableName Proeprty must be set");

                return true;
            }

            return false;
        }

        public void Execute(DataTable dataTable)
        {
            if (EnsureDataTable(dataTable))
            {
                using (SqlBulkCopy s = this.CreateSqlBulkCopy(dataTable))
                {
                    s.WriteToServer(dataTable.CreateDataReader());
                }
            }
        }

        public Task ExecuteAsync(DataTable dataTable)
        {
            if (EnsureDataTable(dataTable))
            {
                using (SqlBulkCopy s = this.CreateSqlBulkCopy(dataTable))
                {
                    return s.WriteToServerAsync(dataTable.CreateDataReader());
                }
            }
            return Task.Delay(0);
        }

        private static bool EnsureEntityList<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            return !entityList.IsNullOrEmpty() && null != provider;
        }

        public void Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (EnsureEntityList(entityList, provider))
            {
                DataTable table = entityList.ToDataTable(provider);
                this.Execute(table);
            }
        }

        public Task ExecuteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (EnsureEntityList(entityList, provider))
            {
                DataTable table = entityList.ToDataTable(provider);
                return this.ExecuteAsync(table);
            }

            return Task.Delay(0);
        }

        public void Dispose()
        {
            if (null != this.conn)
                this.conn.Dispose();
        }
    }
}
