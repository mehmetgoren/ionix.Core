namespace ionix.Rest
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    public static class ResultExtensions
    {
        public static Result<T> ResultSingle<T>(this ControllerBase controller, Func<T> data)
        {
            Response<T> response = new Response<T>();
            try
            {
                T item = data();
                response.Data = new List<T>() { item };
                response.Total = 0;
            }
            catch (Exception ex)
            {
                response.Error = ex.FindRoot();
            }

            return new Result<T>(response);
        }
        public static async Task<Result<T>> ResultSingleAsync<T>(this ControllerBase controller, Func<Task<T>> data)
        {
            Response<T> response = new Response<T>();
            try
            {
                T item = await data();
                response.Data = new List<T>() { item };
                response.Total = 0;
            }
            catch (Exception ex)
            {
                response.Error = ex.FindRoot();
            }

            return new Result<T>(response);
        }

        public static Result<T> ResultSingle<T>(this ControllerBase controller, Func<Data<T>> data)
        {
            Response<T> response = new Response<T>();
            try
            {
                Data<T> info = data();
                response.Data = new List<T>() { info.Entity };
                response.Total = info.Total;
            }
            catch (Exception ex)
            {
                response.Error = ex.FindRoot();
            }

            return new Result<T>(response);
        }


        public static Result<T> ResultList<T>(this ControllerBase controller, Func<IEnumerable<T>> data)
        {
            Response<T> response = new Response<T>();
            try
            {
                response.Data = data();
                response.Total = 0;
            }
            catch (Exception ex)
            {
                response.Error = ex.FindRoot();
            }

            return new Result<T>(response);
        }
        public static async Task<Result<T>> ResultListAsync<T>(this ControllerBase controller, Func<Task<IEnumerable<T>>> data)
        {
            Response<T> response = new Response<T>();
            try
            {
                response.Data = await data();
                response.Total = 0;
            }
            catch (Exception ex)
            {
                response.Error = ex.FindRoot();
            }

            return new Result<T>(response);
        }

        public static Result<T> ResultList<T>(this ControllerBase controller, Func<Datas<T>> datas)
        {
            Response<T> response = new Response<T>();
            try
            {
                Datas<T> infos = datas();
                response.Data = infos.EntityList;
                response.Total = infos.Total;
            }
            catch (Exception ex)
            {
                response.Error = ex.FindRoot();
            }

            return new Result<T>(response);
        }


        public static IActionResult ResultSingle(this ControllerBase controller, string message)
        {
            Response<object> response = new Response<object>();
            response.Message = message;

            return new Result<object>(response);
        }
    }


    //Total için
    public struct Data<T>
    {
        public T Entity { get; set; }
        public int Total { get; set; }
    }
    public struct Datas<T> //Batch total lar için.
    {
        public IEnumerable<T> EntityList { get; set; }
        public int Total { get; set; }
    }
}
