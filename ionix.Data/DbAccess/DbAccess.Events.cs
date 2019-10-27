namespace Ionix.Data
{
    using System;

    partial class DbAccess
    {
        private static readonly object PreExecuteSqlEvent = new object();
        public event PreExecuteSqlEventHandler PreExecuteSql
        {
            add
            {
                lock (this)
                {
                    this.events.AddHandler(DbAccess.PreExecuteSqlEvent, value);
                }
            }
            remove
            {
                lock (this)
                {
                    this.events.RemoveHandler(DbAccess.PreExecuteSqlEvent, value);
                }
            }
        }
        protected virtual void OnPreExecuteSql(SqlQuery query)
        {
            PreExecuteSqlEventHandler fPtr = (PreExecuteSqlEventHandler)this.events[DbAccess.PreExecuteSqlEvent];
            if (fPtr != null)
            {
                PreExecuteSqlEventArgs e = new PreExecuteSqlEventArgs(this, query);
                fPtr(e);
            }
        }

        //executionStart sadece execution değil genel olarak alınmalı. Yabi Fonksiyonun başında. Böylece provider lardan kaynaklanan zaman kaybıda görülebilir.
        private static readonly object ExecuteSqlCompleteEvent = new object();
        public event ExecuteSqlCompleteEventHandler ExecuteSqlComplete
        {
            add
            {
                lock (this)
                {
                    this.events.AddHandler(DbAccess.ExecuteSqlCompleteEvent, value);
                }
            }
            remove
            {
                lock (this)
                {
                    this.events.RemoveHandler(DbAccess.ExecuteSqlCompleteEvent, value);
                }
            }
        }
        protected virtual void OnExecuteSqlComplete(SqlQuery query, DateTime executionStart, Exception executingException)
        {
            ExecuteSqlCompleteEventHandler fPtr = (ExecuteSqlCompleteEventHandler)this.events[DbAccess.ExecuteSqlCompleteEvent];
            if (fPtr != null)
            {
                ExecuteSqlCompleteEventArgs e = new ExecuteSqlCompleteEventArgs(this, query, executionStart, DateTime.Now, executingException);
                fPtr(e);
            }
        }
    }
}
