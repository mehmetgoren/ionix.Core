namespace Ionix.Migration
{
    using System;
    using Data;

    public abstract class Column : ISqlQueryProvider
    {
        public string Name { get; set; }

        public bool NotNull { get; set; }

        public string Default { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsAutoIncrement { get; set; }

        public abstract string DataType { get; }


        protected string GetNullStatement()
        {
            return this.NotNull ? " NOT NULL" : "";//var sayılan NULL olduğu için belirtmeye gerek yok.
        }

        protected string GetDefaultStatement()
        {
            if (!String.IsNullOrEmpty(this.Default))
                return " DEFAULT " + this.Default;

            return "";
        }

        public virtual SqlQuery ToQuery()
        {
            return $"{this.Name} {this.DataType}{this.GetNullStatement()}{this.GetDefaultStatement()}".ToQuery();
        }

        public virtual void CopyPropertiesFrom(PropertyMetaData metaData)
        {
            if (null != metaData)
            {
                SchemaInfo schema = metaData.Schema;
                this.Name = schema.ColumnName;
                this.NotNull = !schema.IsNullable;
                this.Default = schema.DefaultValue;
                this.IsPrimaryKey = schema.IsKey;
                this.IsAutoIncrement = schema.DatabaseGeneratedOption == StoreGeneratedPattern.Identity;
            }
        }
    }
}
