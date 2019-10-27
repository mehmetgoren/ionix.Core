namespace Ionix.Data
{
    using Utils;
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data;

    public sealed class SqlQueryParameter : IEquatable<SqlQueryParameter>
    {
        internal static SqlQueryParameter Create(string parameterName, PropertyMetaData pm, object value)
        {
            SqlQueryParameter ret = new SqlQueryParameter();
            ret.ParameterName = parameterName;
            ret.Value = value;

            SchemaInfo schema = pm.Schema;
            ret.IsNullable = schema.IsNullable;
            ret.dbType = TypeMap[schema.DataType];

            return ret;
        }

        private string parameterName;

        public SqlQueryParameter()
            : this(null, null, ParameterDirection.Input, true)
        { }

        public SqlQueryParameter(string parameterName, object value)
            : this(parameterName, value, ParameterDirection.Input, true)
        {

        }
        public SqlQueryParameter(string parameterName, object value, ParameterDirection direction)
            : this(parameterName, value, direction, true)
        { }

        public SqlQueryParameter(string parameterName, object value, ParameterDirection direction, bool isNullable)
        {
            this.parameterName = parameterName;
            this.Value = value;
            this.Direction = direction;
            this.IsNullable = isNullable;
        }

        public string ParameterName
        {
            get
            {
                if (null == this.parameterName)
                    return String.Empty;
                return this.parameterName;
            }
            set => this.parameterName = value;
        }

        public object Value { get; set; }

        public ParameterDirection Direction { get; set; }

        public bool IsNullable { get; set; }

        #region  IEquatable<SqlQueryParameter>
        public bool Equals(SqlQueryParameter other)
        {
            if (null != other)
                return this.ParameterName.Equals(other.ParameterName, StringComparison.OrdinalIgnoreCase);
            return false;
        }
        public override bool Equals(object obj)
        {
            SqlQueryParameter other = obj as SqlQueryParameter;
            if (null != other)
                return this.Equals(other);
            return false;
        }
        public override int GetHashCode()
        {
            return this.ParameterName.GetHashCode();
        }
        #endregion

        public override string ToString()
        {
            return this.ParameterName;
        }

        internal DbType? dbType;

        //Test Edilince Kaldır.
        public DbType DataType
        {
            get
            {
                if (!this.dbType.HasValue)
                    return 0.Cast<DbType>();
                return this.dbType.Value;
            }
            set => this.dbType = value;
        }


        private static readonly object TypeMapSync = new object();
        private static Dictionary<Type, DbType> typeMap;
        private static Dictionary<Type, DbType> TypeMap
        {
            get
            {
                if (null == typeMap)
                {
                    lock (TypeMapSync)
                    {
                        if (null == typeMap)
                        {
                            typeMap = new Dictionary<Type, DbType>();

                            typeMap.Add(CachedTypes.Byte, DbType.Byte);
                            typeMap.Add(CachedTypes.SByte, DbType.SByte);
                            typeMap.Add(CachedTypes.Int16, DbType.Int16);
                            typeMap.Add(CachedTypes.UInt16, DbType.UInt16);
                            typeMap.Add(CachedTypes.Int32, DbType.Int32);
                            typeMap.Add(CachedTypes.UInt32, DbType.UInt32);
                            typeMap.Add(CachedTypes.Int64, DbType.Int64);
                            typeMap.Add(CachedTypes.UInt64, DbType.UInt64);
                            typeMap.Add(CachedTypes.Single, DbType.Single);
                            typeMap.Add(CachedTypes.Double, DbType.Double);
                            typeMap.Add(CachedTypes.Decimal, DbType.Decimal);
                            typeMap.Add(CachedTypes.Boolean, DbType.Boolean);
                            typeMap.Add(CachedTypes.String, DbType.String);
                            typeMap.Add(CachedTypes.Char, DbType.StringFixedLength);
                            typeMap.Add(CachedTypes.Guid, DbType.Guid);
                            typeMap.Add(CachedTypes.DateTime, DbType.DateTime);
                            typeMap.Add(CachedTypes.DateTimeOffset, DbType.DateTimeOffset);
                            typeMap.Add(CachedTypes.ByteArray, DbType.Binary);
                            typeMap.Add(CachedTypes.Nullable_Byte, DbType.Byte);
                            typeMap.Add(CachedTypes.Nullable_SByte, DbType.SByte);
                            typeMap.Add(CachedTypes.Nullable_Int16, DbType.Int16);
                            typeMap.Add(CachedTypes.Nullable_UInt16, DbType.UInt16);
                            typeMap.Add(CachedTypes.Nullable_Int32, DbType.Int32);
                            typeMap.Add(CachedTypes.Nullable_UInt32, DbType.UInt32);
                            typeMap.Add(CachedTypes.Nullable_Int64, DbType.Int64);
                            typeMap.Add(CachedTypes.Nullable_UInt64, DbType.UInt64);
                            typeMap.Add(CachedTypes.Nullable_Single, DbType.Single);
                            typeMap.Add(CachedTypes.Nullable_Double, DbType.Double);
                            typeMap.Add(CachedTypes.Nullable_Decimal, DbType.Decimal);
                            typeMap.Add(CachedTypes.Nullable_Boolean, DbType.Boolean);
                            typeMap.Add(CachedTypes.Nullable_Char, DbType.StringFixedLength);
                            typeMap.Add(CachedTypes.Nullable_Guid, DbType.Guid);
                            typeMap.Add(CachedTypes.Nullable_DateTime, DbType.DateTime);
                            typeMap.Add(CachedTypes.Nullable_DateTimeOffset, DbType.DateTimeOffset);
                            TypeMap.Add(CachedTypes.TimeSpan, DbType.Time);
                            TypeMap.Add(CachedTypes.Nullable_TimeSpan, DbType.Time);

                            //typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
                        }
                    }
                }

                return typeMap;
            }
        }
    }
}
