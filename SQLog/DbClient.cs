namespace SQLog
{
    using System;
    using System.Data;
    using ionix.Data;

    internal abstract class DbClient<TDbAccess> : IDisposable
        where TDbAccess : IDbAccess
    {
        public TDbAccess DataAccess { get; }

        protected DbClient(TDbAccess dataAccess)
        {
            this.DataAccess = dataAccess;
        }

        //  public ICommandFactory Factory => ionixFactory.CreateFactory(this.DataAccess);

        public ICommandAdapter Cmd => ionixFactory.CreateCommand(this.DataAccess);

        public virtual void Dispose()
        {
            if (null != this.DataAccess)
                this.DataAccess.Dispose();
        }
    }

    internal sealed class DbClient : DbClient<IDbAccess>
    {
        internal DbClient(IDbAccess dbAccess)
            : base(dbAccess)
        {
        }
    }

    internal sealed class TransactionalDbClient : DbClient<ITransactionalDbAccess>, IDbTransaction
    {
        internal TransactionalDbClient(ITransactionalDbAccess transactionalDbAccess)
            : base(transactionalDbAccess)
        {
        }

        public void Commit()
        {
            this.DataAccess.Commit(); ;
        }

        public IsolationLevel IsolationLevel => this.DataAccess.IsolationLevel;

        public void Rollback()
        {
            this.DataAccess.Rollback();
        }

        IDbConnection IDbTransaction.Connection => this.DataAccess.Connection;
    }
}