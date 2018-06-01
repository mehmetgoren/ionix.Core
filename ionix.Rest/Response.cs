namespace ionix.Rest
{
    using System;
    using System.Collections.Generic;

    public class Response<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int Total { get; set; }
        

        /// <summary>
        /// if server returns invalid operation message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// if a server exception occurs
        /// </summary>
        public Exception Error { get; set; }
    }
}
