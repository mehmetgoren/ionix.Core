namespace ionix.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DbSchemaAttribute : ValidationAttribute
    {
        public string ColumnName { get; set; }//Proprty ismi kolon ismiyle farklılık gösteriyor mu diye.

        public bool IsKey { get; set; }
        public StoreGeneratedPattern DatabaseGeneratedOption { get; set; }

        public bool IsNullable { get; set; } = true;
        public int MaxLength { get; set; }//UI Binding için.
        public string DefaultValue { get; set; }

        public bool ReadOnly { get; set; }

        public SqlValueType SqlValueType { get; set; }

        public override bool IsValid(object value)
        {
            if (!this.IsNullable)//required
            {
                if (null == value)
                {
                    this.ErrorMessage = "this field is required";
                    return false;
                }
            }
            int maxLength = this.MaxLength;
            if (maxLength > 0)
            {
                if (value?.ToString().Length > maxLength)
                {
                    this.ErrorMessage = "input value is not in range";
                    return false;
                }
            }

            return true;
        }
    }
}
