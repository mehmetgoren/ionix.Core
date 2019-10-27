namespace Ionix.Utils
{
    using System;

    public struct Lazy<T>
    {
        private readonly Func<T> func;

        public Lazy(Func<T> func)
        {
            if (null == func)
                throw new ArgumentNullException(nameof(func));

            this.func = func;
            this.value = default(T);
            this.init = false;
        }


        private bool init;
        private T value;
        private static readonly object syncRoot = new object();
        public T Value
        {
            get
            {
                lock (syncRoot)
                {
                    if (!this.init)
                    {

                        this.value = this.func();
                        this.init = true;
                    }
                }
                return this.value;
            }
        }
    }
}
