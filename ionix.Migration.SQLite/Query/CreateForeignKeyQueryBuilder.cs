namespace Ionix.Migration.SQLite
{
    using Ionix.Data;
    using System;

    // Tablonun içinde çalışacak.
    internal sealed class CreateForeignKeyQueryBuilder : ISqlQueryProvider
    {
        private readonly TableForeignKeyAttribute attr;

        public CreateForeignKeyQueryBuilder(TableForeignKeyAttribute attr)
        {
            this.attr = attr ?? throw new ArgumentNullException(nameof(attr));

            this.CheckAttribute();
        }

        private void CheckAttribute()
        {
            if (String.IsNullOrEmpty(attr.Columns))
                throw new ArgumentException("TableForeignKey.Columns can not be null or empty");


            if (String.IsNullOrEmpty(attr.ReferenceTable))
                throw new NullReferenceException("TableForeignKey.ReferenceTable can not be null or empty");


            if (String.IsNullOrEmpty(attr.ReferenceColumns))
                throw new ArgumentException("TableForeignKey.ReferenceColumns can not be null or empty");
        }

        public SqlQuery ToQuery()
        {
            SqlQuery query = "FOREIGN KEY (".ToQuery()
                .Sql(this.attr.Columns).Sql(") ")
                .Sql("REFERENCES ").Sql(this.attr.ReferenceTable).Sql(" (").Sql(this.attr.ReferenceColumns).Sql(")");

            return query;
        }
    }
}
