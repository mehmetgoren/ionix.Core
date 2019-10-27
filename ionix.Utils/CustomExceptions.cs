namespace Ionix.Utils
{
    using System;

    public class ObjectIsLockedException : Exception
    {
        public ObjectIsLockedException()
            : base("Object is Locked")
        {

        }
        public ObjectIsLockedException(string message, params object[] args)
            : base(String.Format(message, args))
        {

        }

        public ObjectIsLockedException(string message) : base(message)
        {
        }

        public ObjectIsLockedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class ItemAlreadyAddedException : Exception
    {
        public ItemAlreadyAddedException()
            : base("Item has already been added")
        {

        }
        public ItemAlreadyAddedException(string message, params object[] args)
            : base(String.Format(message, args))
        {

        }

        public ItemAlreadyAddedException(string message) : base(message)
        {
        }

        public ItemAlreadyAddedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message, params object[] args)
            : base(String.Format(message, args))
        {

        }

        public NotFoundException() : base()
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class ReadOnlyException : Exception
    {
        public ReadOnlyException()
            : base("Field is Readonly")
        {

        }
        public ReadOnlyException(string message, params object[] args)
            : base(String.Format(message, args))
        {

        }

        public ReadOnlyException(string message) : base(message)
        {
        }

        public ReadOnlyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
