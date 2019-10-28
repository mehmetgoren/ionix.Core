namespace Ionix.Migration.SQLite
{
    using Ionix.Utils.Extensions;
    using System;
    using System.Text;
    using Data;

    //test et ancak, tablo oluştuktan sonra çalışmalı sanki.
    internal sealed class CreateIndexQueryBuilder : ISqlQueryProvider
    {
        private readonly string tableName;
        private readonly TableIndexAttribute attr;

        public CreateIndexQueryBuilder(string tableName, TableIndexAttribute attr)
        {
            this.tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
            this.attr = attr ?? throw new ArgumentNullException(nameof(attr));
            if (attr.Fields.IsNullOrEmpty())
            {
                throw new ArgumentException("TableIndex.Fields can not be null or empty");
            }

        }

        public SqlQuery ToQuery()
        {
            const int pgMaxNameLinegth = 31;
            string indexName = this.attr.Name;
            if (String.IsNullOrEmpty(indexName))
            {
                StringBuilder sb = new StringBuilder("Ix_").Append(this.tableName).Append('_');
                foreach (string field in attr.Fields)
                {
                    sb.Append(field.Trim()).Append('_');
                }
                sb.Remove(sb.Length - 1, 1);
                if (sb.Length > pgMaxNameLinegth)
                    sb.Remove(pgMaxNameLinegth, sb.Length - pgMaxNameLinegth);
                indexName = sb.ToString();
            }

            SqlQuery query = $"CREATE{(this.attr.Unique ? " UNIQUE" : "")} INDEX ".ToQuery();
            query.Sql(indexName).Sql(" ON ").Sql(this.tableName).Sql(" (");
            foreach (string field in attr.Fields)
            {
                query.Sql(field).Sql(", ");
            }
            query.Text.Remove(query.Text.Length - 2, 2);
            query.Sql(");");

            return query;
        }
    }
}
