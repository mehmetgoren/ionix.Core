namespace Ionix.Data.PostgreSql.BulkCopy
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Text;
    using Data;
    using Utils;
    using NpgsqlTypes;
    using Npgsql;

    public class BulkCopyCommand : IBulkCopyCommand
    {
        private readonly NpgsqlConnection conn;
        public BulkCopyCommand(NpgsqlConnection conn)
        {
            if (null == conn)
                throw new ArgumentNullException(nameof(conn));

            if (conn.State != ConnectionState.Open)
                throw new ArgumentException("'connection' is not open.");

            this.conn = conn;
        }


        public void Dispose()
        {
            conn?.Dispose();
        }

        private static void EnsureParameters<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (null == entityList)
                throw new ArgumentNullException(nameof(entityList));
            if (null == provider)
                throw new ArgumentNullException(nameof(provider));
        }

        public void Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            EnsureParameters(entityList, provider);

            if (entityList.Any())
            {
                IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();
                var mappings = CreateMappingDbType(metaData);
                SqlQuery query = CreateInitQuery(metaData);

                using (var writer = this.conn.BeginBinaryImport(query.ToString()))
                {
                    foreach (TEntity entity in entityList)
                    {
                        writer.StartRow();
                        foreach (PropertyMetaData prop in metaData.Properties)
                        {
                            SchemaInfo schema = prop.Schema;
                            if (schema.DatabaseGeneratedOption != StoreGeneratedPattern.Identity)
                            {
                                object value = prop.Property.GetValue(entity);
                                if (null == value)
                                    writer.WriteNull();
                                else
                                    writer.Write(value, mappings[schema]);
                            }
                        }
                    }
                }
            }
        }

        private static SqlQuery CreateInitQuery(IEntityMetaData metaData)
        {
            SqlQuery q = "COPY ".ToQuery();
            StringBuilder sql = q.Text;
            sql.Append(metaData.TableName).Append(" (");
            foreach (PropertyMetaData md in metaData.Properties)
            {
                SchemaInfo schema = md.Schema;
                if (schema.DatabaseGeneratedOption != StoreGeneratedPattern.Identity)
                {
                    sql.Append(schema.ColumnName).Append(',');
                }
            }

            sql.Remove(sql.Length - 1, 1);
            sql.Append(") FROM STDIN (FORMAT BINARY)");

            return q;
        }


        private static IDictionary<SchemaInfo, NpgsqlDbType> CreateMappingDbType(IEntityMetaData metaData)
        {
            IDictionary<SchemaInfo, NpgsqlDbType> ret = new Dictionary<SchemaInfo, NpgsqlDbType>();
            foreach (PropertyMetaData prop in metaData.Properties)
            {
                NpgsqlDbType dbType;
                SchemaInfo scheme = prop.Schema;
                if (scheme.DataType == CachedTypes.String)
                {
                    dbType = scheme.MaxLength > 0 ? NpgsqlDbType.Varchar : NpgsqlDbType.Text;
                }
                else if (Cache.TryGetValue(scheme.DataType, out NpgsqlDbType columnType))
                {
                    dbType = columnType;
                }
                else
                {
                    throw new NotSupportedException(scheme.DataType.FullName);
                }

                ret.Add(scheme, dbType);
            }

            return ret;
        }

        private static readonly object syncRoot = new object();
        private static IDictionary<Type, NpgsqlDbType> _cache = null;
        private static IDictionary<Type, NpgsqlDbType> Cache
        {
            get
            {
                if (_cache == null)
                {
                    lock (syncRoot)
                    {
                        if (_cache == null)
                        {
                            IDictionary<Type, NpgsqlDbType> temp = new ConcurrentDictionary<Type, NpgsqlDbType>();

                            temp.Add(CachedTypes.Decimal, NpgsqlDbType.Numeric);
                            temp.Add(CachedTypes.Int32, NpgsqlDbType.Integer);
                            temp.Add(CachedTypes.DateTime, NpgsqlDbType.Timestamp);
                            temp.Add(CachedTypes.Double, NpgsqlDbType.Double);
                            temp.Add(CachedTypes.ByteArray, NpgsqlDbType.Bytea);
                            temp.Add(CachedTypes.Boolean, NpgsqlDbType.Boolean);
                            temp.Add(CachedTypes.Char, NpgsqlDbType.Char);
                            temp.Add(CachedTypes.Single, NpgsqlDbType.Real);
                            temp.Add(CachedTypes.Int16, NpgsqlDbType.Smallint);
                            temp.Add(CachedTypes.Int64, NpgsqlDbType.Bigint);
                            temp.Add(CachedTypes.Byte, NpgsqlDbType.Smallint);
                            temp.Add(CachedTypes.SByte, NpgsqlDbType.Smallint);
                            temp.Add(CachedTypes.UInt64, NpgsqlDbType.Bigint);
                            temp.Add(CachedTypes.UInt32, NpgsqlDbType.Integer);
                            temp.Add(CachedTypes.UInt16, NpgsqlDbType.Smallint);
                            temp.Add(CachedTypes.Guid, NpgsqlDbType.Uuid);
                            temp.Add(CachedTypes.TimeSpan, NpgsqlDbType.Interval);
                            temp.Add(CachedTypes.DateTimeOffset, NpgsqlDbType.Timestamp);

                            _cache = temp;
                        }
                    }
                }

                return _cache;
            }
        }



        Task IBulkCopyCommand.ExecuteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            throw new NotSupportedException("Npgsql does not support async operation when COPY method calling.");
        }

        void IBulkCopyCommand.Execute(DataTable dataTable)
        {
            throw new NotSupportedException("DataTable is not supported.");
        }

        Task IBulkCopyCommand.ExecuteAsync(DataTable dataTable)
        {
            throw new NotSupportedException("DataTable is not supported.");
        }
    }
}
