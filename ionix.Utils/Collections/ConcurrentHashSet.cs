namespace Ionix.Utils.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class ConcurrentHashSet<T> : ICollection<T>, IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly HashSet<T> hashSet = new HashSet<T>();


        private sealed class Write : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;
            public Write(ConcurrentHashSet<T> parent)
            {
                this._lock = parent._lock;
                this._lock.EnterWriteLock();
            }
            public void Dispose()
            {
                if (this._lock.IsWriteLockHeld) this._lock.ExitWriteLock();
            }
        }

        private sealed class Read : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;
            public Read(ConcurrentHashSet<T> parent)
            {
                this._lock = parent._lock;
                this._lock.EnterReadLock();
            }
            public void Dispose()
            {
                if (this._lock.IsWriteLockHeld) this._lock.ExitReadLock();
            }
        }

        #region Implementation of ICollection<T> ...ish
        public bool Add(T item)
        {
            using (new Write(this))
            {
                return this.hashSet.Add(item);
            }
        }

        public void Clear()
        {
            using (new Write(this))
            {
                this.hashSet.Clear();
            }
        }

        public bool Contains(T item)
        {
            using (new Read(this))
            {
                return this.hashSet.Contains(item);
            }
        }

        public bool Remove(T item)
        {
            using (new Write(this))
            {
                return this.hashSet.Remove(item);
            }
        }

        public int Count
        {
            get
            {
                using (new Read(this))
                {
                    return this.hashSet.Count;
                }
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (this._lock != null)
                    this._lock.Dispose();
        }
        ~ConcurrentHashSet()
        {
            this.Dispose(false);
        }
        #endregion

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            using (new Write(this))
            {
                this.hashSet.CopyTo(array, arrayIndex);
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.hashSet.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
