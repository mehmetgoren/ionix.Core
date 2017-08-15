namespace ionix.Rest
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    internal static class ResultConst
    {
        internal static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings();
    }

    public class Result<T> : JsonResult
    {
        public Result(Response<T> data)
            : base(data, ResultConst.DefaultJsonSerializerSettings)
        {
            
        }
    }
}
