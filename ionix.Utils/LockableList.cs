namespace Ionix.Utils
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public sealed class LockableList<T> : IList<T>, ILockable
    {
        private IList<T> list;
        public LockableList()
        {
            this.list = new List<T>();
        }
        public LockableList(int capacity)
        {
            this.list = new List<T>(capacity);
        }
        public LockableList(IEnumerable<T> strings)
        {
            this.list = new List<T>(strings);
        }

        #region |   IList<T>   |
        public int IndexOf(T item)
        {
            return this.list.IndexOf(item);
        }
        public void Insert(int index, T item)
        {
            this.list.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }
        public T this[int index]
        {
            get => this.list[index];
            set => this.list[index] = value;
        }
        public void Add(T item)
        {
            this.list.Add(item);
        }
        public void Clear()
        {
            this.list.Clear();
        }
        public bool Contains(T item)
        {
            return this.list.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }
        public int Count => this.list.Count;

        public bool IsReadOnly => this.list.IsReadOnly;

        public bool Remove(T item)
        {
            return this.list.Remove(item);
        }
        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }
        #endregion


        #region |   ILockable   |
        public void Lock()
        {
            if (!this.list.IsReadOnly)
                this.list = new ReadOnlyCollection<T>(this.list);
        }
        public void Unlock()
        {
            if (this.IsReadOnly)
                this.list = new List<T>(this.list);
        }
        bool ILockable.IsLocked => this.IsReadOnly;

        #endregion
    }
}
