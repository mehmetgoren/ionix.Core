namespace Ionix.Utils.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Linq;
    using System.Reflection;
    using Ionix.Utils.Extensions;

    public class IndexedEntityList<TEntity> : ICollection<TEntity>
    {
        public static IndexedEntityList<TEntity> Create(params Expression<Func<TEntity, object>>[] keys)
        {
            return new IndexedEntityList<TEntity>(new Dictionary<Key, TEntity>(), keys);
        }

        public static IndexedEntityList<TEntity> CreateConcurrent(params Expression<Func<TEntity, object>>[] keys)
        {
            return new IndexedEntityList<TEntity>(new ConcurrentDictionary<Key, TEntity>(), keys);
        }

        private readonly IDictionary<Key, TEntity> dic;
        private readonly ICollection<PropertyInfo> keys;

        private IndexedEntityList(IDictionary<Key, TEntity> seed, params Expression<Func<TEntity, object>>[] keys)
        {
            if (!keys.Any())
                throw new ArgumentNullException(nameof(keys));

            this.dic = seed;
            this.keys = new List<PropertyInfo>(keys.Length);
            foreach (Expression<Func<TEntity, object>> exp in keys)
            {
                this.keys.Add(ReflectionExtensions.GetPropertyInfo(exp));
            }
        }

        private struct Key
        {
            private readonly List<object> keys;

            public Key(ICollection<PropertyInfo> keys, TEntity entity)
            {
                this.keys = new List<object>(keys.Count);
                foreach (PropertyInfo pi in keys)
                {
                    this.keys.Add(pi.GetValue(entity));
                }
            }

            public Key(IEnumerable keys)
            {
                this.keys = new List<object>();
                foreach (object key in keys)
                {
                    this.keys.Add(key);
                }
            }

            public override int GetHashCode()
            {
                int length = this.keys.Count;
                unchecked
                {
                    int hash = 17;
                    for (int j = 0; j < length; ++j)
                    {
                        hash = hash * 23 + this.keys[j].GetHashCode();
                    }
                    return hash;
                }
            }

            public override bool Equals(object obj)
            {
                return obj.GetHashCode() == this.GetHashCode();
            }
        }

        private Key GetKey(TEntity entity)
        {
            return new Key(this.keys, entity);
        }
        public void Add(TEntity entity)
        {
            if (null != entity)
            {
                this.dic[this.GetKey(entity)] = entity;
            }
        }

        public void Clear()
        {
            this.dic.Clear();
        }

        public bool Contains(TEntity entity)
        {
            if (null != entity)
            {
                Key k = this.GetKey(entity);
                return this.dic.ContainsKey(k);
            }
            return false;
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            if (array.Any())
            {
                foreach (TEntity entity in array)
                {
                    this.Add(entity);
                }
            }
        }

        public int Count => this.dic.Count;

        public bool IsReadOnly => false;

        public bool Remove(TEntity entity)
        {
            if (null != entity)
            {
                Key k = this.GetKey(entity);
                return this.dic.Remove(k);
            }
            return false;
        }

        public bool Remove(params object[] keyValues)
        {
            if (keyValues.Any())
            {
                Key k = new Key(keyValues);
                return this.dic.Remove(k);
            }
            return false;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.dic.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public List<TEntity> ToList()
        {
            return this.dic.Values.ToList();
        }

        public TEntity Find(params object[] keyValues)
        {
            if (keyValues.Any())
            {
                Key k = new Key(keyValues);
                TEntity entity;
                this.dic.TryGetValue(k, out entity);
                return entity;
            }
            return default(TEntity);
        }

        public void AddRange(IEnumerable<TEntity> list)
        {
            if (list.Any())
            {
                foreach (TEntity entity in list)
                {
                    this.Add(entity);
                }
            }
        }

        public bool Replace(TEntity entity)
        {
            if (null != entity)
            {
                Key k = this.GetKey(entity);
                if (this.dic.ContainsKey(k))
                {
                    this.dic[k] = entity;
                    return true;
                }
            }
            return false;
        }
    }
}
