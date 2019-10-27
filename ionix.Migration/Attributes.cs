namespace Ionix.Migration
{
    using System;
    using System.Collections.Generic;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class MigrationVersionAttribute : Attribute
    {
        public string MigrationVersion { get; }

        public MigrationVersionAttribute(string version)
        {
            this.MigrationVersion = version;
        }
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class TableIndexAttribute : Attribute
    {
        public TableIndexAttribute()
        {

        }

        public TableIndexAttribute(params string[] fields)
        {
            this.Fields = fields;
        }

        public IEnumerable<string> Fields { get; set; }

        public string Name { get; set; }

        public bool Unique { get; set; }
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class TableForeignKeyAttribute : Attribute
    {
        public TableForeignKeyAttribute()
        {

        }

        public TableForeignKeyAttribute(string columns, string referenceTable, string referenceColumns)
        {
            this.Columns = columns;
            this.ReferenceTable = referenceTable;
            this.ReferenceColumns = referenceColumns;
        }

        public string Name { get; set; }

        //virgülle ayırarak çoklu kolon desteği eklenecek.
        public string Columns { get; set; }

        public string ReferenceTable { get; set; }

        public string ReferenceColumns { get; set; }
    }

    /// <summary>
    /// For column with numeric data types
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PrecisionAttribute : Attribute
    {
        public int Length { get; set; }

        public int Precision { get; set; }

        public PrecisionAttribute()
        {

        }

        public PrecisionAttribute(int length, int precision)
        {
            this.Length = length;
            this.Precision = precision;
        }
    }
}
