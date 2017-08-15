namespace ionix.Annotation
{
    using System;


    public abstract class ValidationAttribute : Attribute
    {
        public string ErrorMessage { get; set; }

        public abstract bool IsValid(object value);
    }
}
