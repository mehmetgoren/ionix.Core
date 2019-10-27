namespace Ionix.Utils.Collections
{
    using System;
    using System.Collections.Generic;


    public class ThrowingHashSet<T> : ICollection<T>
    {
        private readonly HashSet<T> hash;

        public ThrowingHashSet()
        {
            this.hash = new HashSet<T>();
        }
        public ThrowingHashSet(IEnumerable<T> en)
            : this()
        {
            if (null == en)
                throw new ArgumentNullException(nameof(en));

            foreach (T item in en)
                this.Add(item);
        }
        public ThrowingHashSet(IEqualityComparer<T> comparer)
        {
            this.hash = new HashSet<T>(comparer);
        }

        public void Add(T item)
        {
            if (!this.hash.Add(item))
                throw new ItemAlreadyAddedException();
        }
        public void Clear()
        {
            this.hash.Clear();
        }
        public bool Contains(T item)
        {
            return this.hash.Contains(item);
        }
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            this.hash.CopyTo(array, arrayIndex);
        }
        public int Count => this.hash.Count;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)this.hash).IsReadOnly;

        public bool Remove(T item)
        {
            return this.hash.Remove(item);
        }
        public IEnumerator<T> GetEnumerator()
        {
            return this.hash.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
