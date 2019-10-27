namespace Ionix.Data
{
    using System.Data;
    using System.Data.Common;

    public class TransactionalDbAccess : DbAccess, ITransactionalDbAccess
    {
        private DbTransaction transaction;
        public DbTransaction Transaction
        {
            get
            {
                if (null == this.transaction)
                {
                    this.transaction = this.Connection.BeginTransaction(this.IsolationLevel);
                }
                return this.transaction;
            }
        }

        public TransactionalDbAccess(DbConnection conn, IsolationLevel isolationLevel)
            : base(conn)
        {

            this.IsolationLevel = isolationLevel;
        }
        public TransactionalDbAccess(DbConnection conn)
            : this(conn, IsolationLevel.Unspecified)
        { }


        protected override void OnCommandCreated(DbCommand cmd)
        {
            cmd.Transaction = this.Transaction;
        }

        public IsolationLevel IsolationLevel { get; set; }
        IDbConnection IDbTransaction.Connection
        {
            get { return this.Connection; }
        }


        public virtual void Commit()
        {
            if (null != this.transaction)
                this.transaction.Commit();
        }
        public virtual void Rollback()
        {
            if (null != this.transaction)
                this.transaction.Rollback();
        }

        public override void Dispose()
        {
            if (null != this.transaction)
            {
                this.transaction.Dispose();
                this.transaction = null;
            }
            base.Dispose();
        }
    }
}
