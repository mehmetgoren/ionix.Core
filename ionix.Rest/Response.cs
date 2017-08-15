namespace ionix.Rest
{
    using System;
    using System.Collections.Generic;

    public class Response<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int Total { get; set; }
        public Exception Error { get; set; }

        public string Message { get; set; }
    }
}
