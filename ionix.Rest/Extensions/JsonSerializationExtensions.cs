namespace Ionix.Rest
{
    using Newtonsoft.Json;
    using System;
    using System.Text;

    public static class JsonSerializationExtensions
    {
        public static string Json(this object obj)
        {
            if (null != obj)
            {
                return JsonConvert.SerializeObject(obj);
            }
            return String.Empty;
        }


        public static object FromJson(this string json, Type type)
        {
            if (!String.IsNullOrEmpty(json) && null != type)
            {
                return JsonConvert.DeserializeObject(json, type);
            }
            return null;
        }
        public static T FromJson<T>(this string json)
        {
            if (!String.IsNullOrEmpty(json))
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default(T);
        }


        public static string ToQueryString(this object item)
        {
            if (null != item)
            {
                string json = item.Json();
                byte[] utf8String = Encoding.UTF8.GetBytes(json);
                return Convert.ToBase64String(utf8String);
            }
            return String.Empty;
        }
        public static T FromQueryString<T>(this string base64)
        {
            if (!String.IsNullOrEmpty(base64))
            {
                byte[] utf8String = Convert.FromBase64String(base64);
                string json = Encoding.UTF8.GetString(utf8String, 0, utf8String.Length);
                return json.FromJson<T>();
            }
            return default(T);
        }

        public static T DeepCopy<T>(T obj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }
    }
}
