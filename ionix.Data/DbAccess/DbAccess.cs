namespace Ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Utils.Collections;

    public partial class DbAccess : IDbAccess
    {
        private readonly EventHandlerList events;

        public DbAccess(DbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (connection.State != ConnectionState.Open)
                throw new ArgumentException("'connection' is not open.");

            this.Connection = connection;
            this.events = new EventHandlerList();
        }


        private int? commandTimeout;
        public int CommandTimeout 
        {
            get 
            {
                if (this.commandTimeout.HasValue)
                    return this.commandTimeout.Value;

                return this.Connection.ConnectionTimeout;
            }
            set { this.commandTimeout = value; }
        }

        public DbConnection Connection { get; private set; }


        public virtual void Dispose()
        {
            if (null != this.Connection)
            {
                this.Connection.Dispose();
                this.Connection = null;
            }
            if (null != this.events)
                this.events.Dispose();
        }

        private sealed class OutParameters : Dictionary<SqlQueryParameter, DbParameter>
        {
            internal void SetOutParametersValues()
            {
                foreach (KeyValuePair<SqlQueryParameter, DbParameter> kvp in this)
                    kvp.Key.Value = kvp.Value.Value;
            }
        }
    }
}
