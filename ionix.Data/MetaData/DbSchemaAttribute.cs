namespace ionix.Data
{
    using ionix.Utils;
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DbSchemaAttribute : ValidationAttribute
    {
        public string ColumnName { get; set; }//Proprty ismi kolon ismiyle farklılık gösteriyor mu diye.

        public bool IsKey { get; set; }
        public StoreGeneratedPattern DatabaseGeneratedOption { get; set; }

        internal Track<bool> isNullable = new Track<bool>(true);
        public bool IsNullable
        {
            get => this.isNullable.Value;
            set => this.isNullable.Value = value;
        }

        public int MaxLength { get; set; }//UI Binding için.
        public string DefaultValue { get; set; }

        internal Track<bool> readOnly;
        public bool ReadOnly
        {
            get => this.readOnly.Value;
            set => this.readOnly.Value = value;
        }

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
