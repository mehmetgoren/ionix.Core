namespace Ionix.Migration.PostgreSql
{
    using Utils;
    using Ionix.Utils.Extensions;
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
                            temp.Add(CachedTypes.Int32, typeof(Columns.Integer));
                            temp.Add(CachedTypes.DateTime, typeof(Columns.DateTime));
                            temp.Add(CachedTypes.Double, typeof(Columns.DoublePrecision));
                            temp.Add(CachedTypes.ByteArray, typeof(Columns.ByteA));
                            temp.Add(CachedTypes.Boolean, typeof(Columns.Boolean));
                            temp.Add(CachedTypes.Char, typeof(Columns.Character));
                            temp.Add(CachedTypes.Single, typeof(Columns.Real));
                            temp.Add(CachedTypes.Int16, typeof(Columns.SmallInt));
                            temp.Add(CachedTypes.Int64, typeof(Columns.BigInt));
                            temp.Add(CachedTypes.Byte, typeof(Columns.SmallInt));
                            temp.Add(CachedTypes.SByte, typeof(Columns.SmallInt));
                            temp.Add(CachedTypes.UInt64, typeof(Columns.BigInt));
                            temp.Add(CachedTypes.UInt32, typeof(Columns.Integer));
                            temp.Add(CachedTypes.UInt16, typeof(Columns.SmallInt));
                            temp.Add(CachedTypes.Guid, typeof(Columns.Uuid));
                            temp.Add(CachedTypes.TimeSpan, typeof(Columns.Interval));
                            temp.Add(CachedTypes.DateTimeOffset, typeof(Columns.Timestamp));

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
                    return new Columns.CharacterVarying();
                return new Columns.Text();
            }

            if (schema.DatabaseGeneratedOption == StoreGeneratedPattern.Identity)
            {
                if (netType.In(CachedTypes.Int16, CachedTypes.UInt16))
                {
                    return new Columns.SmallSerial();
                }
                else if (netType.In(CachedTypes.Int32, CachedTypes.UInt32))
                {
                    return new Columns.Serial();
                }
                else if (netType.In(CachedTypes.Int64, CachedTypes.UInt64))
                {
                    return new Columns.BigSerial();
                }
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
                        case Columns.BigSerial.DbType:
                            ret = new Columns.BigSerial();
                            break;
                        case Columns.Serial.DbType:
                            ret = new Columns.Serial();
                            break;
                        case Columns.SmallSerial.DbType:
                            ret = new Columns.SmallSerial();
                            break;
                        case Columns.CharacterVarying.DbType:
                            ret = new Columns.CharacterVarying();
                            break;
                        case Columns.Numeric.DbType:
                            ret = new Columns.Numeric();
                            break;
                        case Columns.Date.DbType:
                            ret = new Columns.Date();
                            break;
                        case Columns.DateTime.DbType:
                            ret = new Columns.DateTime();
                            break;
                        case Columns.Text.DbType:
                            ret = new Columns.Text();
                            break;
                        case Columns.Integer.DbType:
                            ret = new Columns.Integer();
                            break;
                        case Columns.DoublePrecision.DbType:
                            ret = new Columns.DoublePrecision();
                            break;
                        case Columns.ByteA.DbType:
                            ret = new Columns.ByteA();
                            break;
                        case Columns.Boolean.DbType:
                            ret = new Columns.Boolean();
                            break;
                        case Columns.SmallInt.DbType:
                            ret = new Columns.SmallInt();
                            break;
                        case Columns.Character.DbType:
                            ret = new Columns.Character();
                            break;
                        case Columns.Real.DbType:
                            ret = new Columns.Real();
                            break;
                        case Columns.BigInt.DbType:
                            ret = new Columns.BigInt();
                            break;
                        case Columns.Uuid.DbType:
                            ret = new Columns.Uuid();
                            break;
                        case Columns.Jsonb.DbType:
                            ret = new Columns.Jsonb();
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
                throw new NotSupportedException($"could not find a suitable postgres type for:{metaData.Schema.DataType}");

            ret.CopyPropertiesFrom(metaData);

            return ret;
        }
    }
}
