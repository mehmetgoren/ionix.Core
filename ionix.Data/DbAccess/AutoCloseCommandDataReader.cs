namespace Ionix.Data
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Common;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// To avoid the 'Invalid attempt to call Read when reader is closed.' exception.
    /// This is a proxy type that close its creator command object automatically.
    /// </summary>
    public sealed class AutoCloseCommandDataReader : DbDataReader
    {
        public static AutoCloseCommandDataReader Create(DbCommand command, DbDataReader concrete)
        {
            return new AutoCloseCommandDataReader(command, concrete);
        }

        private readonly DbCommand command;

        private readonly DbDataReader concrete;

        /// <summary>
        /// ctor for Proxy.
        /// </summary>
        /// <param name="command">Command shouldn't be disposed.</param>
        /// <param name="concrete"></param>
        private AutoCloseCommandDataReader(DbCommand command, DbDataReader concrete)
        {
            this.command = command ?? throw new ArgumentNullException(nameof(command));
            this.concrete = concrete ?? throw new ArgumentNullException(nameof(concrete));
        }

        public DbDataReader Concrete => this.concrete;


        #region  |   virtual   |

        public override int VisibleFieldCount => this.concrete.VisibleFieldCount;

        public override void Close() => this.concrete.Close();

        public override T GetFieldValue<T>(int ordinal) => this.concrete.GetFieldValue<T>(ordinal);

        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) => this.concrete.GetFieldValueAsync<T>(ordinal, cancellationToken);

        public override Type GetProviderSpecificFieldType(int ordinal) => this.concrete.GetProviderSpecificFieldType(ordinal);

        public override object GetProviderSpecificValue(int ordinal) => this.concrete.GetProviderSpecificValue(ordinal);

        public override int GetProviderSpecificValues(object[] values) => this.concrete.GetProviderSpecificValues(values);

        public override DataTable GetSchemaTable() => this.concrete.GetSchemaTable();

        public override Stream GetStream(int ordinal) => this.concrete.GetStream(ordinal);

        public override TextReader GetTextReader(int ordinal) => this.concrete.GetTextReader(ordinal);

        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) => this.concrete.IsDBNullAsync(ordinal, cancellationToken);

        public override Task<bool> NextResultAsync(CancellationToken cancellationToken) => this.concrete.NextResultAsync(cancellationToken);

        public override Task<bool> ReadAsync(CancellationToken cancellationToken) => this.concrete.ReadAsync(cancellationToken);

        /// <summary>
        /// this why i created this proxy type.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            //base.Dispose(disposing);base.Dispose calls Close and we don't need such an action. 
    
            if (disposing)
            {
                this.concrete?.Dispose();

                this.command?.Dispose();
            }
        }


        protected override DbDataReader GetDbDataReader(int ordinal)//public DbDataReader GetData(int ordinal) method call this. many ado.net provider dos not support this.
        {
            var result = this.concrete.GetType().GetMethod(nameof(GetDbDataReader),
                BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this.concrete, new object[] { ordinal });

            return (DbDataReader)result;
        }

        #endregion


        #region  |   abstract   |

        public override object this[int ordinal] => this.concrete[ordinal];

        public override object this[string name] => this.concrete[name];


        public override int Depth => this.concrete.Depth;

        public override int FieldCount => this.concrete.FieldCount;

        public override bool HasRows => this.concrete.HasRows;

        public override bool IsClosed => this.concrete.IsClosed;

        public override int RecordsAffected => this.concrete.RecordsAffected;

        public override bool GetBoolean(int ordinal) => this.concrete.GetBoolean(ordinal);

        public override byte GetByte(int ordinal) => this.concrete.GetByte(ordinal);


        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
            => this.concrete.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);

        public override char GetChar(int ordinal) => this.concrete.GetChar(ordinal);

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
            => this.concrete.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);

        public override string GetDataTypeName(int ordinal) => this.concrete.GetDataTypeName(ordinal);


        public override DateTime GetDateTime(int ordinal) => this.concrete.GetDateTime(ordinal);

        public override decimal GetDecimal(int ordinal) => this.concrete.GetDecimal(ordinal);

        public override double GetDouble(int ordinal) => this.concrete.GetDouble(ordinal);

        public override IEnumerator GetEnumerator() => this.concrete.GetEnumerator();

        public override Type GetFieldType(int ordinal) => this.concrete.GetFieldType(ordinal);

        public override float GetFloat(int ordinal) => this.concrete.GetFloat(ordinal);

        public override Guid GetGuid(int ordinal) => this.concrete.GetGuid(ordinal);

        public override short GetInt16(int ordinal) => this.concrete.GetInt16(ordinal);

        public override int GetInt32(int ordinal) => this.concrete.GetInt32(ordinal);

        public override long GetInt64(int ordinal) => this.concrete.GetInt64(ordinal);

        public override string GetName(int ordinal) => this.concrete.GetName(ordinal);

        public override int GetOrdinal(string name) => this.concrete.GetOrdinal(name);

        public override string GetString(int ordinal) => this.concrete.GetString(ordinal);

        public override object GetValue(int ordinal) => this.concrete.GetValue(ordinal);

        public override int GetValues(object[] values) => this.concrete.GetValues(values);

        public override bool IsDBNull(int ordinal) => this.concrete.IsDBNull(ordinal);

        public override bool NextResult() => this.concrete.NextResult();

        public override bool Read() => this.concrete.Read();

        #endregion
    }
}
