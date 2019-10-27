namespace Ionix.Rest
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    //added to disable camelCase. 
    public class DefaultJsonResult : JsonResult
    {
        internal static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings();

        public DefaultJsonResult(object value)
            : base(value, DefaultJsonSerializerSettings)
        {

        }
    }
}
