namespace Ionix.Utils
{
    using System;

    public struct NonNullable<T> : IEquatable<NonNullable<T>>
        where T : class
    {
        private readonly T value;

        public NonNullable(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            this.value = value;
        }

        public T Value
        {
            get
            {
                if (this.value == null)
                {
                    throw new NullReferenceException("Value");
                }
                return this.value;
            }
        }
        public bool Equals(NonNullable<T> other)
        {
            return this.value.Equals(other.value);
        }
        public override bool Equals(object obj)
        {
            if (obj is NonNullable<T>)
            {
                return this.Equals((NonNullable<T>)obj);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
        public override string ToString()
        {
            return this.value.ToString();
        }

        public static bool operator ==(NonNullable<T> n1, NonNullable<T> n2)
        {
            return n1.Equals(n2);
        }
        public static bool operator !=(NonNullable<T> n1, NonNullable<T> n2)
        {
            return !(n1 == n2);
        }
        public static implicit operator NonNullable<T>(T value)
        {
            return new NonNullable<T>(value);
        }

        public static implicit operator T(NonNullable<T> wrapper)
        {
            return wrapper.value;
        }
    }
}
