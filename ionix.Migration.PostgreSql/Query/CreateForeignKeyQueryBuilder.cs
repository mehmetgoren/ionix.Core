namespace Ionix.Migration.PostgreSql
{
    using System;
    using System.Text;
    using Data;

    internal sealed class CreateForeignKeyQueryBuilder : ISqlQueryProvider
    {
        private readonly string tableName;
        private readonly TableForeignKeyAttribute attr;

        public CreateForeignKeyQueryBuilder(string tableName, TableForeignKeyAttribute attr)
        {
            this.tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
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
            const int pgMaxNameLinegth = 31;
            string fkName = this.attr.Name;
            if (String.IsNullOrEmpty(fkName))
            {
                StringBuilder sb = new StringBuilder("fk_").Append(this.tableName).Append('_').Append(this.attr.ReferenceTable).Append('_')
                    .Append(this.attr.Columns.Trim().Replace(',', '_')).Append('_').Append(this.attr.ReferenceColumns.Trim().Replace(',', '_'));

                if (sb.Length > pgMaxNameLinegth)
                    sb.Remove(pgMaxNameLinegth, sb.Length - pgMaxNameLinegth);
                fkName = sb.ToString();
            }

            SqlQuery query = "ALTER TABLE ".ToQuery();
            query.Sql(this.tableName).Sql(" ADD CONSTRAINT ").Sql(fkName)
                .Sql(" FOREIGN KEY (").Sql(this.attr.Columns).Sql(") ")
                .Sql("REFERENCES ").Sql(this.attr.ReferenceTable).Sql(" (").Sql(this.attr.ReferenceColumns).Sql(");");

            return query;
        }
    }
}
