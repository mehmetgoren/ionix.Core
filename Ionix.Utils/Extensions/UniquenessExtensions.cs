namespace Ionix.Utils.Extensions
{
    using System;
    using System.Threading;

    public static class UniquenessExtensions
    {
        private static readonly object syncRoot = new object();
        public static string GenerateUniqueName()
        {
            try
            {
                Monitor.Enter(syncRoot);
                byte[] arr = Guid.NewGuid().ToByteArray();
                long i = 1;
                foreach (byte b in arr)
                {
                    i *= (((int)b) + 1);
                }
                return string.Format("{0:x}", i - DateTime.Now.Ticks);
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }

        private static readonly object syncRoot2 = new object();
        public static int GenerateUniqueHashCode(params object[] args)
        {
            try
            {
                Monitor.Enter(syncRoot2);
                if (null == args || 0 == args.Length)
                    throw new ArgumentNullException(nameof(args));

                int hash = 17;
                foreach (object item in args)
                {
                    if (null == item)
                        throw new NullReferenceException("args.Item");

                    unchecked
                    {
                        hash = hash * 23 + item.GetHashCode();
                    }
                }

                return hash;
            }
            finally
            {
                Monitor.Exit(syncRoot2);
            }
        }

    }
}
