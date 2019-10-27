namespace Ionix.Utils
{
    using System;
    using Collections;

    //Türemiş Tipler için Singleton kontrolü. Micro Servisler için kullanılıyor.
    public abstract class Singleton
    {
        private static readonly object locker = new object();
        private static readonly ThrowingHashSet<Type> registeredTypes = new ThrowingHashSet<Type>();
        protected Singleton()
        {
            lock (locker)
            {
                registeredTypes.Add(this.GetType());
            }
        }
    }
}
