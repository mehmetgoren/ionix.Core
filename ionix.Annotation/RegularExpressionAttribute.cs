namespace ionix.Annotation
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class RegularExpressionAttribute : ValidationAttribute
    {
        public RegularExpressionAttribute(string pattern)
        {
            this.Pattern = pattern;

            this.ErrorMessage = "Input Value is Invalid";
        }

        public string Pattern { get; }

        public override bool IsValid(object value)
        {
            string text = Convert.ToString(value, CultureInfo.CurrentCulture);
            if (!String.IsNullOrEmpty(text))
            {
                Regex regex = new Regex(this.Pattern);
                Match match = regex.Match(text);
                if (!(match.Success && match.Index == 0 && match.Length == text.Length))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
