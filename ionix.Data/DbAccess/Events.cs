namespace Ionix.Data
{
    using System;

    public class PreExecuteSqlEventArgs : EventArgs
    {
        internal PreExecuteSqlEventArgs(DbAccess dataAccess, SqlQuery query)
        {
            this.DataAccess = dataAccess;
            this.Query = query;
        }

        public DbAccess DataAccess { get; }

        public SqlQuery Query { get; }
    }
    public delegate void PreExecuteSqlEventHandler(PreExecuteSqlEventArgs e);

    public sealed class ExecuteSqlCompleteEventArgs : PreExecuteSqlEventArgs
    {
        internal ExecuteSqlCompleteEventArgs(DbAccess dataAccess, SqlQuery query, DateTime executionStart, DateTime executionFinish, Exception executionException)
            : base(dataAccess, query)
        {
            this.ExecutionStart = executionStart;
            this.ExecutionFinish = executionFinish;
            this.ExecutionException = executionException;
        }

        public DateTime ExecutionStart { get; }

        public DateTime ExecutionFinish { get; }

        public TimeSpan Elapsed => (this.ExecutionFinish - this.ExecutionStart);

        public Exception ExecutionException { get; }

        public bool Succeeded => this.ExecutionException == null;
    }
    public delegate void ExecuteSqlCompleteEventHandler(ExecuteSqlCompleteEventArgs e);
}
