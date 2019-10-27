namespace Ionix.Rest
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections;
    using System.Threading;

    internal struct UserGuid
    {
        internal User User;
        internal Guid Guid;

        public UserGuid(User user, Guid guid)
        {
            this.User = user;
            this.Guid = guid;
        }
    }

    internal sealed class UserGuidDic : IEnumerable<UserGuid>, IDisposable
    {
        private readonly ConcurrentDictionary<User, Guid> users = new ConcurrentDictionary<User, Guid>(UserEqualityComparer.Instance);
        private readonly ConcurrentDictionary<Guid, User> guids = new ConcurrentDictionary<Guid, User>();

        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        private void EnsureCounts()
        {
            if (this.users.Count != this.guids.Count)
                throw new InvalidOperationException("users and guids collections does not match");
        }

        public void Add(User user, Guid guid)
        {
            this.rwLock.EnterWriteLock();

            try
            {
                if (this.users.TryAdd(user, guid))
                    if (!this.guids.TryAdd(guid, user))
                        throw new InvalidOperationException("can not add item to User Grid Dictionary");

                this.EnsureCounts();
            }
            finally
            {
                this.rwLock.ExitWriteLock();
            }
        }


        public void RemoveByUser(User user)
        {
            this.rwLock.EnterWriteLock();
            try
            {
                Guid guid;
                if (this.users.TryGetValue(user, out guid))
                {
                    if (this.guids.TryGetValue(guid, out  user))
                    {
                        User tempUser; Guid tempGuid;
                        if (!(this.users.TryRemove(user, out tempGuid) && this.guids.TryRemove(guid, out tempUser)))
                        {
                            throw new InvalidOperationException("can not remove item from User Grid Dictionary");
                        }

                        this.EnsureCounts();
                    }
                }
            }
            finally
            {
                this.rwLock.ExitWriteLock();
            }
        }

        public bool TryGetByUser(User user, out Guid guid)
        {
            this.rwLock.EnterReadLock();
            try
            {
                return this.users.TryGetValue(user, out guid);
            }
            finally
            {
                this.rwLock.ExitReadLock();
            }
        }

        public bool TryGetByGuid(Guid guid, out User user)
        {
            this.rwLock.EnterReadLock();
            try
            {
                return this.guids.TryGetValue(guid, out user);
            }
            finally
            {
                this.rwLock.ExitReadLock();
            }
        }

        public IEnumerator<UserGuid> GetEnumerator()
        {
            this.rwLock.EnterReadLock();
            try
            {
                foreach (var kvp in this.users)
                {
                    yield return new UserGuid(kvp.Key, kvp.Value);
                }
            }
            finally
            {
                this.rwLock.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Dispose()
        {
            if (null != this.rwLock)
                this.rwLock.Dispose();
        }

        //added at 06.03.2017
        public int Count => this.users.Count;
        //
    }

}
