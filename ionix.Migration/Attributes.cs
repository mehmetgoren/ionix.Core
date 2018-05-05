namespace ionix.Migration
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
}
