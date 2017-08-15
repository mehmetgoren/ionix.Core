using System;
using System.Data;
using ionix.Data;

namespace ionix.DataTests.SqlServer
{
    //Genel Anlamda using için getirildi. these are proxies
    public abstract class DbClient<TDbAccess> : IDisposable
        where TDbAccess : IDbAccess
    {
        protected abstract TDbAccess CreateDbAccess();

        private TDbAccess dataAccess;
        public TDbAccess DataAccess
        {
            get
            {
                if (null == this.dataAccess)
                    this.dataAccess = this.CreateDbAccess();
                return this.dataAccess;
            }
        }

        public ICommandFactory Factory => ionixFactory.CreateFactory(this.DataAccess);

        public ICommandAdapter Cmd => ionixFactory.CreateCommandAdapter(this.DataAccess);


        public static SqlQuery GenerateDeleteQuery<TEntity>()
        {
            SqlQuery query = new SqlQuery();
            query.Text.Append("DELETE FROM ");
            query.Text.Append(AttributeExtension.GetTableName<TEntity>());
            return query;
        }
        public static int Delete<TEntity>(IDbAccess dataAccess, IEntityMetaDataProvider provider, int id, char prefix)
        {
            if (null == provider)
                throw new ArgumentNullException(nameof(provider));

            IEntityMetaData meta = provider.CreateEntityMetaData(typeof(TEntity));
            PropertyMetaData key = meta.GetPrimaryKey();
            if (null != key)
            {
                SqlQuery query = GenerateDeleteQuery<TEntity>()
                .Sql(" WHERE ")
                .Sql(key.Schema.ColumnName)
                .Sql("=")
                .Sql(prefix.ToString())
                .Sql(key.Schema.ColumnName)
                .Parameter(key.Schema.ColumnName, id);

                return dataAccess.ExecuteNonQuery(query);
            }

            return 0;
        }

        public virtual void Dispose()
        {
            if (null != this.dataAccess)
                this.dataAccess.Dispose();
        }
    }

    public sealed class DbClient : DbClient<IDbAccess>
    {
        internal DbClient() { }

        protected override IDbAccess CreateDbAccess()
        {
            return ionixFactory.CreatDataAccess();
        }
    }

    public sealed class TransactionalDbClient : DbClient<ITransactionalDbAccess>, IDbTransaction
    {
        internal TransactionalDbClient() { }

        protected override ITransactionalDbAccess CreateDbAccess()
        {
            return ionixFactory.CreateTransactionalDataAccess();
        }

        public void Commit()
        {
            this.DataAccess.Commit(); ;
        }

        public IDbConnection Connection => this.DataAccess.Connection;

        public IsolationLevel IsolationLevel => this.DataAccess.IsolationLevel;

        public void Rollback()
        {
            this.DataAccess.Rollback();
        }
    }
}
