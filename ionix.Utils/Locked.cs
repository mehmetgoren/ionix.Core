namespace Ionix.Utils
{
    using System;

    public struct Locked<T> : ILockable, IEquatable<Locked<T>>
    {
        private T value;
        private bool isLocked;

        public Locked(T value)
        {
            this.value = value;
            this.isLocked = false;
        }

        public T Value
        {
            get { return this.value; }
            set
            {
                if (this.isLocked)
                    throw new ObjectIsLockedException("Locked<{0}>.Value is locked.", typeof(T).Name);
                this.value = value;
            }
        }
        public bool IsLocked => this.isLocked;

        public void Lock()
        {
            this.isLocked = true;
        }
        public void Unlock()
        {
            this.isLocked = false;
        }
        public static bool operator ==(Locked<T> lhs, Locked<T> rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(Locked<T> lhs, Locked<T> rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(Locked<T> other)
        {
            if (this.value != null)
            {
                return this.value.Equals(other.value);
            }
            return other.value == null;
        }

        public override bool Equals(object obj)
        {
            if (obj is Locked<T>)
            {
                this.Equals((Locked<T>)obj);
            }
            return false;
        }
        public override int GetHashCode()
        {
            if (null != this.value)
                return this.value.GetHashCode();
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.value == null ? string.Empty : this.value.ToString();
        }
    }

    public sealed class LockScope : IDisposable
    {
        private readonly ILockable lockable;
        public LockScope(ILockable lockable)
        {
            if (null == lockable)
                throw new ArgumentNullException(nameof(lockable));

            this.lockable = lockable;
            this.lockable.Lock();
        }
        public void Dispose()
        {
            this.lockable.Unlock();
        }
    }

    // like C++ dtor pattern
    public sealed class FunctionScope : IDisposable
    {
        private readonly Action<object> lockMethod;
        private readonly Action<object> unlockMethod;

        private readonly object unlockObjectParameter;

        public FunctionScope(Action<object> lockMethod, object lockObjectParameter, Action<object> unlockMethod, object unlockObjectParameter)
        {
            if (null == lockMethod)
                throw new ArgumentNullException(nameof(lockMethod));
            if (null == unlockMethod)
                throw new ArgumentNullException(nameof(unlockMethod));

            this.lockMethod = lockMethod;
            this.unlockMethod = unlockMethod;

            this.unlockObjectParameter = unlockObjectParameter;

            this.lockMethod(lockObjectParameter);
        }
        public FunctionScope(Action<object> lockMethod, Action<object> unlockMethod)
            : this(lockMethod, null, unlockMethod, null)
        { }
        public void Dispose()
        {
            this.unlockMethod(this.unlockObjectParameter);
        }
    }
}
