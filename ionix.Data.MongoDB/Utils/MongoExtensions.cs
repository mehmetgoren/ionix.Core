﻿namespace ionix.Data.Mongo
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.Reflection;

    public static class MongoExtensions
    {
        public static MongoCollectionAttribute GetNames(Type type)
        {
            MongoCollectionAttribute ret = type.GetTypeInfo().GetCustomAttribute<MongoCollectionAttribute>();
            if (null == ret)
            {
                string[] splits = type.FullName.Split('.');
                if (splits.Length != 2)
                    throw new InvalidOperationException(
                        $"{type.FullName} is not compatible with CreateCollection desing rules. Use MongoCollectionAttribute to set database and collection name correctly or set namespace as Database and class name as collection.");

                ret.Database = splits[0];
                ret.Name = splits[1];
            }

            return ret;
        }

        public static ObjectId ToObjectId(this string id)
        {
            ObjectId temp;
            if (ObjectId.TryParse(id, out temp))
            {
                return temp;
            }
            return ObjectId.Empty;
        }

        public static ObjectId? ToObjectIdNullable(this string id)
        {
            ObjectId temp;
            if (ObjectId.TryParse(id, out temp))
            {
                return temp;
            }
            return null;
        }

        public static ObjectId ToNonNullable(this ObjectId? id)
        {
            return id ?? ObjectId.Empty;
        }

        public static bool IsValidObjectId(this string id)
        {
            ObjectId temp;
            if (ObjectId.TryParse(id, out temp))
            {
                return temp != ObjectId.Empty;
            }
            return false;
        }

        public static bool IsEmpty(this ObjectId id)
        {
            return id == ObjectId.Empty;
        }

        public static bool IsNotEmpty(this ObjectId id)
        {
            return !IsEmpty(id);
        }

        public static bool IsEmpty(this ObjectId? id)
        {
            return id == null || id == ObjectId.Empty;
        }

        public static bool IsNotEmpty(this ObjectId? id)
        {
            return !IsEmpty(id);
        }

        public static PropertyInfo GetIdProperty<TEntity>(bool throwIfNotFound)
        {
            foreach (var pi in typeof(TEntity).GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.GetCustomAttribute<BsonIdAttribute>() != null)
                    return pi;
            }

            if (throwIfNotFound)
                throw new NotSupportedException($"{typeof(TEntity).Name} does not have an BsonId Property.");

            return null;
        }
    }
}
