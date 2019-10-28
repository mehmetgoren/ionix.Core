namespace Ionix.Utils.Extensions
{
    using Ionix.Utils.Collections;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CollectionExtensions
    {
        public static bool In<T>(this T obj, params T[] items)
        {
            if (null != obj)
            {
                foreach (T item in items)
                    if (obj.Equals(item))
                        return true;
            }
            return false;
        }


        public static bool IsNullOrEmpty<T>(this IEnumerable<T> en)
        {
            if (null != en)
            {
                ICollection<T> list = en as ICollection<T>;
                if (null != list)
                    return list.Count == 0;
                else
                {
                    using (IEnumerator<T> enumerator = en.GetEnumerator())
                    {
                        return !enumerator.MoveNext();
                    }
                }
            }
            return true;
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> source)
        {
            if (null != collection && !source.IsNullOrEmpty())
            {
                foreach (T item in source)
                {
                    collection.Add(item);
                }
            }
        }

        public static IEnumerable<T> Paging<T>(this IEnumerable<T> input, int page, int pagesize)
        {
            if (input != null)
            {
                int skip = page * pagesize;
                if (skip < input.Count())
                    return input.Skip(page * pagesize).Take(pagesize);
                else
                    return input.Take(pagesize);
            }
            return new List<T>();
        }

        public static IList<T> ElementToList<T>(this T item)
        {
            List<T> ret = new List<T>();
            if (null != item)
                ret.Add(item);
            return ret;
        }

        public static SingleItemList<T> ToSingleItemList<T>(this T item)
        {
            return new SingleItemList<T>(item);
        }

        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            if (null != values && null != action)
                foreach (T obj in values)
                    action(obj);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> collection)
        {
            return new HashSet<T>(collection);
        }
    }
}
