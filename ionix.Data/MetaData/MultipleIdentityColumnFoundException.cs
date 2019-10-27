namespace Ionix.Data
{
    using System;

    public class MultipleIdentityColumnFoundException : Exception
    {
        public MultipleIdentityColumnFoundException(string message)
            : base(message)
        {

        }
        public MultipleIdentityColumnFoundException(object entity)
            : base("Multiple Identiy Column Found in " + entity.GetType().FullName)
        {

        }
    }
}
