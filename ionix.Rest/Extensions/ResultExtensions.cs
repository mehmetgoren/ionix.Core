namespace Ionix.Rest
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    public static class ResultExtensions
    {
        public static DefaultJsonResult ResultSingle<T>(this ControllerBase controller, Func<T> data, Func<int> total = null, Func<string> message = null)
             => new ResponseModel<T>().Data(data).Message(message).Total(total).AsJsonResult();


        public static async Task<IActionResult> ResultSingleAsync<T>(this ControllerBase controller, Func<Task<T>> data, Func<int> total = null, Func<string> message = null)
             => (await new ResponseModel<T>().DataAsync(data)).Message(message).Total(total).AsJsonResult();


        public static DefaultJsonResult ResultList<T>(this ControllerBase controller, Func<IEnumerable<T>> data, Func<int> total = null, Func<string> message = null)
             => new ResponseModel<T>().Data(data).Message(message).Total(total).AsJsonResult();


        public static async Task<IActionResult> ResultListAsync<T>(this ControllerBase controller, Func<Task<IEnumerable<T>>> data, Func<int> total = null, Func<string> message = null)
             => (await new ResponseModel<T>().DataAsync(data)).Message(message).Total(total).AsJsonResult();



        public static DefaultJsonResult ResultAsMessage(this ControllerBase controller, string message)
             => new ResponseModel<object>().Message(() => message).AsJsonResult();
    }
}
