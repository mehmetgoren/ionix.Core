namespace Ionix.Utils.Extensions
{
    using System;

    public static class ConversionExtensions
    {
        public static T Cast<T>(this object obj)
        {
            return null != obj ? (T)obj : default(T);
        }

        public static T Convert<T>(this object value)
        {
            if (null != value)
            {
                return (T)System.Convert.ChangeType(value, typeof(T));
            }
            return default(T);
        }

        public static T ConvertSafely<T>(this object value)
        {
            if (null != value)
            {
                try
                {
                    return Convert<T>(value);
                }
                catch { }
            }
            return default(T);
        }
        public static TDest? ConvertSafely<TSource, TDest>(this TSource? value)
            where TSource : struct
            where TDest : struct
        {
            if (value.HasValue)
            {
                return ConvertSafely<TDest>(value.Value);
            }
            return null;
        }

        public static object ConvertSafely(this object value, TypeCode type)
        {
            if (null != value)
            {
                try
                {
                    return System.Convert.ChangeType(value, type, null);
                }
                catch { }
            }
            return null;
        }

        public static T? ToNullable<T>(this object value)
            where T : struct
        {
            if (null != value)
            {
                string stringValue = value.ToString();
                if (stringValue.Length != 0)
                    return Convert<T>(value);
            }
            return null;
        }

        public static T? ToNullableSafely<T>(this object value)
            where T : struct
        {
            if (null != value)
            {
                string stringValue = value.ToString();
                if (stringValue.Length != 0)
                    return ConvertSafely<T>(value);
            }
            return null;
        }

        public static T ToValue<T>(this T? n)
            where T : struct
        {
            return n ?? default(T);
        }

        public static bool IsNull(this object obj)
        {
            return (null == obj || obj.ToString().Length == 0);
        }
    }
}
