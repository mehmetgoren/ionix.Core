namespace Ionix.Utils
{
    using System;

    public struct Track<T> : IEquatable<Track<T>>
    {
        public static bool operator ==(Track<T> n1, Track<T> n2)
        {
            return n1.Equals(n2);
        }
        public static bool operator !=(Track<T> n1, Track<T> n2)
        {
            return !(n1 == n2);
        }

        private T _value;

        public Track(T value)
        {
            this._value = value;
            this.IsDirty = false;
        }

        public T Value
        {
            get => this._value;
            set
            {
                this._value = value;
                this.IsDirty = true;
            }
        }

        public bool IsDirty { get; private set; }


        public bool Equals(Track<T> other)
        {
            return this._value.Equals(other._value);
        }
        public override bool Equals(object obj)
        {
            if (obj is Track<T>)
            {
                return this.Equals((Track<T>)obj);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return this._value.GetHashCode();
        }
        public override string ToString()
        {
            return this._value.ToString();
        }
    }
}
