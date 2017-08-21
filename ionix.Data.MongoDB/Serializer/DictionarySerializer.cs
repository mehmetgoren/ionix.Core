namespace ionix.Data.Mongo.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System.Collections.Concurrent;
    using System.Linq;
    using ionix.Utils.Reflection;

    public static class DictionarySerializer
    {
        private static readonly IDictionary<PropertyInfo, string> _cacheFieldName =
    new ConcurrentDictionary<PropertyInfo, string>();

        public static string GetFieldName(PropertyInfo pi)
        {
            if (!_cacheFieldName.TryGetValue(pi ?? throw new ArgumentNullException(nameof(pi)), out var fieldName))
            {
                BsonIdAttribute bia = pi.GetCustomAttribute<BsonIdAttribute>();
                if (null != bia)
                {
                    fieldName = "_id";
                }
                else
                {
                    BsonElementAttribute bea = pi.GetCustomAttribute<BsonElementAttribute>();
                    if (null != bea)
                    {
                        fieldName = bea.ElementName;
                    }
                    else
                    {
                        fieldName = pi.Name;
                    }
                }

                _cacheFieldName[pi] = fieldName;
            }

            return fieldName;
        }

        private static readonly IDictionary<Type, IDictionary<string, PropertyInfo>> _cache =
            new ConcurrentDictionary<Type, IDictionary<string, PropertyInfo>>();
        //_id olmuyor.
        private static readonly object _syncRoot = new object();
        public static IDictionary<string, PropertyInfo> GetValidProperties(Type type)
        {
            IDictionary<string, PropertyInfo> ret;
            if (!_cache.TryGetValue(type, out ret))
            {
                lock (_syncRoot)
                {
                    if (!_cache.TryGetValue(type, out ret))
                    {
                        ret = new Dictionary<string, PropertyInfo>();
                        foreach (var pi in type.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (pi.GetCustomAttribute<BsonIgnoreAttribute>() != null)
                                continue;

                            Type propertyType = pi.PropertyType;
                            bool isObjectIdType = propertyType == typeof(ObjectId) || propertyType == typeof(ObjectId?);
                            if (!isObjectIdType)
                            {
                                if (!ReflectionExtensions.IsPrimitiveType(pi.PropertyType))
                                    continue;

                                if (ReflectionExtensions.IsEnumerable(pi.PropertyType))
                                    continue;
                            }

                            ret[GetFieldName(pi)] = pi;
                        }

                        _cache.Add(type, ret);
                    }
                }
            }

            return ret;
        }

        static ConcurrentDictionary<Type, PropertyInfo> IDColumnsDict = new ConcurrentDictionary<Type, PropertyInfo>();

        public static PropertyInfo GetBsonIdProperty(Type entityType)
        {
            if (!IDColumnsDict.TryGetValue(entityType ?? throw new ArgumentNullException(nameof(entityType)), out var result))
            {
                var pi = entityType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(p => p.GetCustomAttribute<BsonIdAttribute>() != null);
                if (null == pi)
                    throw new InvalidOperationException($"{entityType} has no bsonid property");

                IDColumnsDict[entityType] = pi;
                result = pi;
            }

            return result;
        }
        public static PropertyInfo GetBsonIdProperty<TEntity>()
        {
            return GetBsonIdProperty(typeof(TEntity));
        }

        public static IDictionary<string, object> ToDictionary(this object model)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            if (null != model)
            {
                foreach (var kvp in GetValidProperties(model.GetType()))
                {
                    ret[kvp.Key] = kvp.Value.GetValue(model);
                }
            }
            return ret;
        }

        public static object To(this IDictionary<string, object> dic, Type target)
        {
            if (null != dic && null != target)
            {
                var model = Activator.CreateInstance(target);

                foreach (var kvp in GetValidProperties(target))
                {
                    object value;
                    if (dic.TryGetValue(kvp.Key, out value))
                    {
                        var pi = kvp.Value;
                        pi.SetValueSafely(model, value);
                    }

                }

                return model;
            }

            return null;
        }

        public static T To<T>(this IDictionary<string, object> dic)
        {
            return (T)To(dic, typeof(T));
        }

        public static T Unwind<T>(this IDictionary<string, object> dic)
        {
            var name = MongoExtensions.GetCollectionInfo(typeof(T)).Name;
            var dicInner = dic[name] as IDictionary<string, object>;
            return To<T>(dicInner);
        }
    }
}
