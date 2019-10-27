namespace Ionix.Migration.PostgreSql
{
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ColumnAttribute : Attribute
    {
        public PostgresDbType? DbType { get; set; }

        public string DbTypeName { get; set; }

        public ColumnAttribute(PostgresDbType dbType)
        {
            this.DbType = dbType;
        }
        public ColumnAttribute(string dbTypeName)
        {
            this.DbTypeName = dbTypeName;
        }

        public override string ToString()
        {
            if (this.DbType.HasValue)
                return Extensions.GetEnumDescription(this.DbType.Value);

            return DbTypeName ?? "";
        }
    }

    public enum PostgresDbType
    {
        [Description(Columns.BigSerial.DbType)]
        BigSerial,

        [Description(Columns.Serial.DbType)]
        Serial,

        [Description(Columns.SmallSerial.DbType)]
        SmallSerial,

        [Description(Columns.CharacterVarying.DbType)]
        CharacterVarying,

        [Description(Columns.Numeric.DbType)]
        Numneric,

        [Description(Columns.Date.DbType)]
        Date,

        [Description(Columns.DateTime.DbType)]
        DateTime,

        [Description(Columns.Text.DbType)]
        Text,

        [Description(Columns.Integer.DbType)]
        Integer,

        [Description(Columns.DoublePrecision.DbType)]
        DoublePrecision,

        [Description(Columns.ByteA.DbType)]
        ByteA,

        [Description(Columns.Boolean.DbType)]
        Boolean,

        [Description(Columns.SmallInt.DbType)]
        SmallInt,

        [Description(Columns.Character.DbType)]
        Character,

        [Description(Columns.Real.DbType)]
        Real,

        [Description(Columns.BigInt.DbType)]
        BigInt,

        [Description(Columns.Uuid.DbType)]
        Uuid,

        [Description(Columns.Interval.DbType)]
        Interval,

        [Description(Columns.Timestamp.DbType)]
        Timestamp,

        [Description(Columns.Jsonb.DbType)]
        Jsonb
    }
}
