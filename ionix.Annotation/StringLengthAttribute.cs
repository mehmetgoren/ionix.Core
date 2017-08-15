namespace ionix.Annotation
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class StringLengthAttribute : ValidationAttribute
    {
        public StringLengthAttribute(int length)
        {
            this.MaximumLength = length;

            this.ErrorMessage = "Input data is not in range";
        }

        public int MaximumLength { get;  }

        public int MinimumLength { get; set; }

        public override bool IsValid(object value)
        {
            int num = (value == null) ? 0 : value.ToString().Length;
            if (num <= this.MinimumLength || num >= this.MaximumLength)
                return false;

            return true;
        }
    }
}
