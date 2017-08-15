namespace ionix.Annotation
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class RequiredAttribute : ValidationAttribute
    {
        public bool AllowEmptyStrings { get; set; }

        public RequiredAttribute()
        {
            this.ErrorMessage = "This field is required";
        }

        public override bool IsValid(object value)
        {
            if (null == value)
                return false;

            if (!this.AllowEmptyStrings && 0 == value.ToString().Length)
                return false;

            return true;
        }
    }
}
