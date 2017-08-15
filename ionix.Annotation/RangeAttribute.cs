namespace ionix.Annotation
{
    using ionix.Utils.Extensions;
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class RangeAttribute : ValidationAttribute
    {
        public RangeAttribute(int minimum, int maximum)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.OperandType = TypeCode.Int32;

            this.ErrorMessage = $"Value must be between {minimum} and {maximum}";
        }

        public RangeAttribute(double minimum, double maximum)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.OperandType = TypeCode.Double;

            this.ErrorMessage = $"Value must be between {minimum} and {maximum}";
        }
        public RangeAttribute(TypeCode type, string minimum, string maximum)
        {
            this.OperandType = type;
            this.Minimum = minimum;
            this.Maximum = maximum;

            this.ErrorMessage = $"Value must be between {minimum} and {maximum}";
        }


        public object Minimum { get; }

        public object Maximum { get; }

        public TypeCode OperandType { get; }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            string text = value as string;
            if (text != null && string.IsNullOrEmpty(text))
            {
                return true;
            }
            object obj = null;
            try
            {
                obj = value.ConvertTo(this.OperandType);
            }
            catch (Exception)
            {
                return false;
            }
            IComparable comparable = (IComparable)this.Minimum;
            IComparable comparable2 = (IComparable)this.Maximum;
            return comparable.CompareTo(obj) <= 0 && comparable2.CompareTo(obj) >= 0;
        }
    }
}
