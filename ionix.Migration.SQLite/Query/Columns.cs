namespace Ionix.Migration.SQLite
{
    using Ionix.Data;
    using System;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// For more information: https://www.sqlite.org/datatype3.html
    /// </summary>
    internal static class Columns
    {

        internal abstract class SQLiteColumn : Column
        {
            public sealed override SqlQuery ToQuery()
            {
                return $"{this.Name} {this.DataType}{(this.IsPrimaryKey? " PRIMARY KEY":"")}{this.GetNullStatement()}{this.GetDefaultStatement()}".ToQuery();
            }
        }

        internal sealed class Integer : SQLiteColumn
        {
            internal const string DbType = "INTEGER";
            public override string DataType => DbType;
        }

        internal sealed class Varchar : SQLiteColumn
        {
            internal const string DbType = "VARCHAR";
            public override string DataType => DbType + (this.Length > 0 ? $"({this.Length})" : null);

            public int Length { get; set; }

            public override void CopyPropertiesFrom(PropertyMetaData metaData)
            {
                base.CopyPropertiesFrom(metaData);

                if (metaData.Schema.MaxLength > 0)
                    this.Length = metaData.Schema.MaxLength;
            }
        }

        internal sealed class Text : SQLiteColumn
        {
            internal const string DbType = "TEXT";
            public override string DataType => DbType;
        }

        internal sealed class Int : SQLiteColumn
        {
            internal const string DbType = "INT";
            public override string DataType => DbType;
        }

        internal sealed class Blob : SQLiteColumn
        {
            internal const string DbType = "BLOB";
            public override string DataType => DbType;
        }

        internal sealed class Boolean : SQLiteColumn
        {
            internal const string DbType = "BOOLEAN";
            public override string DataType => DbType;
        }

        internal sealed class DateTime : SQLiteColumn
        {
            internal const string DbType = "DATETIME";
            public override string DataType => DbType;
        }

        internal class Numeric : SQLiteColumn
        {
            internal const string DbType = "NUMERIC";

            public int? Length { get; set; }

            public int? Precision { get; set; }

            public override void CopyPropertiesFrom(PropertyMetaData metaData)
            {
                base.CopyPropertiesFrom(metaData);

                PrecisionAttribute attr = metaData.Property.GetCustomAttribute<PrecisionAttribute>();
                if (null != attr)
                {
                    this.Length = attr.Length;
                    this.Precision = attr.Precision;
                }
            }

            protected void SetPrecisionScript(StringBuilder sb)
            {
                if (this.Length.HasValue)
                {
                    sb.Append("(").Append(this.Length);
                    if (this.Precision.HasValue)
                    {
                        sb.Append(',').Append(this.Precision);
                    }
                    sb.Append(")");
                }
            }

            public override string DataType
            {
                get
                {
                    StringBuilder sb = new StringBuilder(DbType);
                    this.SetPrecisionScript(sb);
                    return sb.ToString();
                }
            }
        }

        internal sealed class Real : SQLiteColumn
        {
            internal const string DbType = "REAL";
            public override string DataType => DbType;
        }

        internal sealed class WhatYouWrite : SQLiteColumn
        {
            public override string DataType { get; }

            public WhatYouWrite(string dbType)
            {
                this.DataType = dbType ?? throw new ArgumentNullException(nameof(dbType));
            }
        }
    }
}
