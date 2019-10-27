namespace Ionix.Utils
{
    using System;
    using System.Dynamic;

    public static class CachedTypes
    {
        public static readonly Type String = typeof(string);
        public static readonly Type Decimal = typeof(decimal);
        public static readonly Type Int32 = typeof(int);
        public static readonly Type DateTime = typeof(DateTime);
        public static readonly Type Double = typeof(double);
        public static readonly Type ByteArray = typeof(byte[]);
        public static readonly Type Boolean = typeof(bool);
        public static readonly Type Byte = typeof(byte);
        public static readonly Type Char = typeof(char);
        public static readonly Type Single = typeof(float);
        public static readonly Type Int16 = typeof(short);
        public static readonly Type Int64 = typeof(long);
        public static readonly Type SByte = typeof(sbyte);
        public static readonly Type UInt64 = typeof(ulong);
        public static readonly Type UInt32 = typeof(uint);
        public static readonly Type UInt16 = typeof(ushort);
        public static readonly Type Guid = typeof(Guid);


        public static readonly Type PureNullableType = typeof(Nullable<>);


        public static readonly Type Nullable_Decimal = typeof(decimal?);
        public static readonly Type Nullable_Int32 = typeof(int?);
        public static readonly Type Nullable_DateTime = typeof(DateTime?);
        public static readonly Type Nullable_Double = typeof(double?);
        public static readonly Type Nullable_Boolean = typeof(bool?);
        public static readonly Type Nullable_Byte = typeof(byte?);
        public static readonly Type Nullable_Char = typeof(char?);
        public static readonly Type Nullable_Single = typeof(float?);
        public static readonly Type Nullable_Int16 = typeof(short?);
        public static readonly Type Nullable_Int64 = typeof(long?);
        public static readonly Type Nullable_SByte = typeof(sbyte?);
        public static readonly Type Nullable_UInt64 = typeof(ulong?);
        public static readonly Type Nullable_UInt32 = typeof(uint?);
        public static readonly Type Nullable_UInt16 = typeof(ushort?);
        public static readonly Type Nullable_Guid = typeof(Guid?);


        //extenden for ReflectionExtensions primitive types and SqlQueryParameter.Create
        //ReflectionExtensions.IsPrimitive
        public static readonly Type ObjectType = typeof(Object);//Dynamic İçin Eklendi
        public static readonly Type ExpandoObjectType = typeof(ExpandoObject);

        //SqlQueryParameter.Create
        public static readonly Type DateTimeOffset = typeof(DateTimeOffset);
        public static readonly Type Nullable_DateTimeOffset = typeof(DateTimeOffset?);
        public static readonly Type TimeSpan = typeof(TimeSpan);
        public static readonly Type Nullable_TimeSpan = typeof(TimeSpan?);


        //extended for SqlQuryBuilders
        public static readonly Type DBNull = typeof(DBNull);
    }
}
