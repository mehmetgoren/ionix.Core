namespace ionix.Data.PostgreSql
{
    using System;
    using System.Collections;
    using System.Data.Common;
    using System.Globalization;

    //proxy
    public sealed class PostgreDataReader : DbDataReader
    {
        private readonly DbDataReader concrete;

        public PostgreDataReader(DbDataReader concrete)
        {
            if (null == concrete)
                throw new ArgumentNullException(nameof(concrete));

            this.concrete = concrete;
        }


        public override object this[int ordinal] => this.concrete[ordinal];

        private static readonly CultureInfo EnUs = new CultureInfo("en-US");
        public override object this[string name]
        {
            get
            {
                if (null != name)
                    name = name.ToLower(EnUs);

                return this.concrete[name];
            }
        }

        public override int Depth => this.concrete.Depth;

        public override int FieldCount => this.concrete.FieldCount;

        public override bool HasRows => this.concrete.HasRows;

        public override bool IsClosed => this.concrete.IsClosed;

        public override int RecordsAffected => this.concrete.RecordsAffected;

        public override bool GetBoolean(int ordinal)
        {
            return this.concrete.GetBoolean(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return this.concrete.GetByte(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return this.concrete.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal)
        {
            return this.concrete.GetChar(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return this.concrete.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return this.concrete.GetDataTypeName(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return this.concrete.GetDateTime(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return this.concrete.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return this.concrete.GetDouble(ordinal);
        }

        public override IEnumerator GetEnumerator()
        {
            return this.concrete.GetEnumerator();
        }

        public override Type GetFieldType(int ordinal)
        {
            return this.concrete.GetFieldType(ordinal);
        }

        public override float GetFloat(int ordinal)
        {
            return this.concrete.GetFloat(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return this.concrete.GetGuid(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return this.concrete.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return this.concrete.GetInt32(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return this.concrete.GetInt64(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return this.concrete.GetName(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            return this.concrete.GetOrdinal(name);
        }

        public override string GetString(int ordinal)
        {
            return this.concrete.GetString(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            return this.concrete.GetValue(ordinal);
        }

        public override int GetValues(object[] values)
        {
            return this.concrete.GetValues(values);
        }

        public override bool IsDBNull(int ordinal)
        {
            return this.concrete.IsDBNull(ordinal);
        }

        public override bool NextResult()
        {
            return this.concrete.NextResult();
        }

        public override bool Read()
        {
            return this.concrete.Read();
        }
    }
}
