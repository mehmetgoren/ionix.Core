namespace Ionix.Migration.SQLite
{
    using Utils;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using Data;

    public class ColumnDbTypeResolver : IColumnDbTypeResolver
    {
        public static readonly ColumnDbTypeResolver Instance = new ColumnDbTypeResolver();
        private ColumnDbTypeResolver()
        {

        }

        private static readonly object syncRoot = new object();
        private IDictionary<Type, Type> _cache = null;
        private IDictionary<Type, Type> Cache
        {
            get
            {
                if (_cache == null)
                {
                    lock (syncRoot)
                    {
                        if (_cache == null)
                        {
                            IDictionary<Type, Type> temp = new ConcurrentDictionary<Type, Type>();

                            temp.Add(CachedTypes.Decimal, typeof(Columns.Numeric));
                            temp.Add(CachedTypes.Int32, typeof(Columns.Int));
                            temp.Add(CachedTypes.DateTime, typeof(Columns.DateTime));
                            temp.Add(CachedTypes.Double, typeof(Columns.Real));
                            temp.Add(CachedTypes.ByteArray, typeof(Columns.Blob));
                            temp.Add(CachedTypes.Boolean, typeof(Columns.Boolean));
                            temp.Add(CachedTypes.Char, typeof(Columns.Varchar));
                            temp.Add(CachedTypes.Single, typeof(Columns.Real));
                            temp.Add(CachedTypes.Int16, typeof(Columns.Int));
                            temp.Add(CachedTypes.Int64, typeof(Columns.Int));
                            temp.Add(CachedTypes.Byte, typeof(Columns.Int));
                            temp.Add(CachedTypes.SByte, typeof(Columns.Int));
                            temp.Add(CachedTypes.UInt64, typeof(Columns.Int));
                            temp.Add(CachedTypes.UInt32, typeof(Columns.Int));
                            temp.Add(CachedTypes.UInt16, typeof(Columns.Int));
                            temp.Add(CachedTypes.Guid, typeof(Columns.Varchar));
                            temp.Add(CachedTypes.TimeSpan, typeof(Columns.Int));
                            temp.Add(CachedTypes.DateTimeOffset, typeof(Columns.DateTime));

                            _cache = temp;
                        }
                    }
                }

                return _cache;
            }
        }


        private Column IfNoJsonColumnAttribute(PropertyMetaData metaData)
        {
            SchemaInfo schema = metaData.Schema;
            Type netType = schema.DataType;//pi alında nullable olabilir.
            if (netType == CachedTypes.String)
            {
                if (schema.MaxLength > 0)
                    return new Columns.Varchar();

                return new Columns.Text();
            }

            if (schema.DatabaseGeneratedOption == StoreGeneratedPattern.Identity)
            {
                return new Columns.Integer();
            }

            if (Cache.TryGetValue(netType, out Type columnType))
            {
                return (Column)Activator.CreateInstance(columnType);
            }

            return null;
        }


        public Column GetColumn(PropertyMetaData metaData)
        {
            return this.GetColumn(metaData, true);
        }

        public virtual Column GetColumn(PropertyMetaData metaData, bool throwExIfNotFound)
        {
            if (null == metaData)
                throw new ArgumentNullException(nameof(metaData));

            Column ret = null;

            ColumnAttribute jcattr = metaData.Property.GetCustomAttribute<ColumnAttribute>();
            if (null != jcattr)
            {
                string dbTypeName = jcattr.ToString();
                if (!String.IsNullOrEmpty(dbTypeName))
                {
                    switch (dbTypeName)
                    {
                        case Columns.DateTime.DbType:
                            ret = new Columns.DateTime();
                            break;
                        case Columns.Text.DbType:
                            ret = new Columns.Text();
                            break;
                        case Columns.Integer.DbType:
                            ret = new Columns.Integer();
                            break;
                        case Columns.Boolean.DbType:
                            ret = new Columns.Boolean();
                            break;
                        default:
                            ret = new Columns.WhatYouWrite(dbTypeName);
                            break;
                    }
                }
            }
            else
            {
                ret = IfNoJsonColumnAttribute(metaData);
            }

            if (null == ret && throwExIfNotFound)
                throw new NotSupportedException($"could not find a suitable sqlite type for:{metaData.Schema.DataType}");

            ret.CopyPropertiesFrom(metaData);

            return ret;
        }
    }
}
