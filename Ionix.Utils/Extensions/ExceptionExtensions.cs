namespace Ionix.Utils.Extensions
{
    using System;

    public static class ExceptionExtensions
    {
        public static Exception FindRoot(this Exception ex)
        {
            if (null != ex)
            {
                if (null != ex.InnerException)
                    return FindRoot(ex.InnerException);
            }
            return ex;
        }
    }
}
