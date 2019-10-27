namespace Ionix.Rest
{
    using Ionix.Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public abstract class ResponseModel
    {
        public static Action<Exception> OnException { get; set; }

        protected static void OnExceptionDoSomething(Exception ex)//for logging
        {
            OnException?.Invoke(ex);
        }
    }

    public sealed class ResponseModel<T> : ResponseModel
    {
        private IEnumerable<T> _data;
        public ResponseModel<T> Data(Func<T> func)
        {
            if (null != func)
            {
                try
                {

                    this._data = new List<T> { func() };
                }
                catch (Exception ex)
                {
                    this._error = ex.FindRoot();
                    OnExceptionDoSomething(this._error);
                }
            }

            return this;
        }
        public ResponseModel<T> Data(Func<IEnumerable<T>> func)
        {
            if (null != func)
            {
                try
                {

                    this._data = func();
                }
                catch (Exception ex)
                {
                    this._error = ex.FindRoot();
                    OnExceptionDoSomething(this._error);
                }
            }

            return this;
        }
        public async Task<ResponseModel<T>> DataAsync(Func<Task<T>> func)
        {
            if (null != func)
            {
                try
                {
                    this._data = new List<T>() { await func() };
                }
                catch (Exception ex)
                {
                    this._error = ex.FindRoot();
                    OnExceptionDoSomething(this._error);
                }
            }

            return this;
        }
        public async Task<ResponseModel<T>> DataAsync(Func<Task<IEnumerable<T>>> func)
        {
            if (null != func)
            {
                try
                {
                    this._data = await func();
                }
                catch (Exception ex)
                {
                    this._error = ex.FindRoot();
                    OnExceptionDoSomething(this._error);
                }
            }

            return this;
        }


        private int _total;
        public ResponseModel<T> Total(Func<int> func)
        {
            if (null != func)
                this._total = func();
            return this;
        }

        private string _message;
        public ResponseModel<T> Message(Func<string> func)
        {
            if (null != func)
                this._message = func();
            return this;
        }

        private sealed class Proxy
        {
            public static Proxy From(ResponseModel<T> parent)
            {
                Proxy ret = new Proxy();
                ret.Data = parent._data;
                ret.Total = parent._total;
                ret.Message = parent._message;
                ret.Error = parent._error;

                return ret;
            }

            public IEnumerable<T> Data { get; set; }
            public int Total { get; set; }
            public string Message { get; set; }
            public Exception Error { get; set; }
        }

        private Exception _error;

        public DefaultJsonResult AsJsonResult() => new DefaultJsonResult(Proxy.From(this));
    }
}
