namespace Ionix.Migration.SQLite
{
    using System;
    using System.ComponentModel;


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ColumnAttribute : Attribute
    {
        public SQLiteDbType? DbType { get; set; }

        public string DbTypeName { get; set; }

        public ColumnAttribute(SQLiteDbType dbType)
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

    public enum SQLiteDbType
    {
        [Description(Columns.Integer.DbType)]
        Integer,

        [Description(Columns.Varchar.DbType)]
        Varchar,

        [Description(Columns.Text.DbType)]
        Text,

        [Description(Columns.Int.DbType)]
        Int,

        [Description(Columns.Blob.DbType)]
        Blob,

        [Description(Columns.Boolean.DbType)]
        Boolean,

        [Description(Columns.DateTime.DbType)]
        DateTime,

        [Description(Columns.Numeric.DbType)]
        Numeric,

        [Description(Columns.Real.DbType)]
        Real
    }
}
