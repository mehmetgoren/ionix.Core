namespace Ionix.Migration.PostgreSql
{
    using System;
    using System.Reflection;
    using System.Text;
    using Data;

    internal static class Columns
    {
        internal sealed class BigSerial : Column
        {
            internal const string DbType = "bigserial";
            public override string DataType => DbType;
        }

        internal sealed class Serial : Column
        {
            internal const string DbType = "serial";
            public override string DataType => DbType;
        }

        internal sealed class SmallSerial : Column
        {
            internal const string DbType = "smallserial";
            public override string DataType => DbType;
        }

        internal sealed class CharacterVarying : Column
        {
            internal const string DbType = "character varying";

            public int Length { get; set; }

            public override void CopyPropertiesFrom(PropertyMetaData metaData)
            {
                base.CopyPropertiesFrom(metaData);

                if (metaData.Schema.MaxLength > 0)
                    this.Length = metaData.Schema.MaxLength;
            }

            public override string DataType => DbType + (this.Length > 0 ? $"({this.Length})" : null);
        }


        internal sealed class Numeric : Column
        {
            internal const string DbType = "numeric";

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

            public override string DataType
            {
                get
                {
                    StringBuilder sb = new StringBuilder(DbType);
                    if (this.Length.HasValue)
                    {
                        sb.Append("(").Append(this.Length);
                        if (this.Precision.HasValue)
                        {
                            sb.Append(',').Append(this.Precision);
                        }
                        sb.Append(")");
                    }
                    return sb.ToString();
                }
            }
        }


        internal sealed class Date : Column
        {
            internal const string DbType = "date";

            public override string DataType => DbType;
        }

        internal sealed class DateTime : Column
        {
            internal const string DbType = "timestamp";

            public override string DataType => DbType;
        }


        internal sealed class Text : Column
        {
            internal const string DbType = "text";

            public override string DataType => DbType;
        }

        internal sealed class Integer : Column
        {
            internal const string DbType = "integer";

            public override string DataType => DbType;
        }

        internal sealed class DoublePrecision : Column
        {
            internal const string DbType = "double precision";

            public override string DataType => DbType;
        }

        internal sealed class ByteA : Column
        {
            internal const string DbType = "bytea";

            public override string DataType => DbType;
        }

        internal sealed class Boolean : Column
        {
            internal const string DbType = "boolean";

            public override string DataType => DbType;
        }

        internal sealed class SmallInt : Column
        {
            internal const string DbType = "smallint";

            public override string DataType => DbType;
        }

        internal sealed class Character : Column
        {
            internal const string DbType = "character";

            public override string DataType => DbType;
        }

        internal sealed class Real : Column
        {
            internal const string DbType = "real";

            public override string DataType => DbType;
        }

        internal sealed class BigInt : Column
        {
            internal const string DbType = "bigint";

            public override string DataType => DbType;
        }

        internal sealed class Uuid : Column
        {
            internal const string DbType = "uuid";

            public override string DataType => DbType;
        }

        internal sealed class Interval : Column
        {
            internal const string DbType = "interval";

            public override string DataType => DbType;
        }

        internal sealed class Timestamp : Column
        {
            internal const string DbType = "timestamp with time zone";

            public override string DataType => DbType;
        }

        internal sealed class Jsonb : Column
        {
            internal const string DbType = "jsonb";

            public override string DataType => DbType;
        }


        internal sealed class WhatYouWrite : Column
        {
            public override string DataType { get; }

            public WhatYouWrite(string dbType)
            {
                this.DataType = dbType ?? throw new ArgumentNullException(nameof(dbType));
            }

        }
    }
}