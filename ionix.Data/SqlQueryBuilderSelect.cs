namespace Ionix.Data
{
    using System.Text;

    public class SqlQueryBuilderSelect : ISqlQueryProvider
    {
        public SqlQueryBuilderSelect(IEntityMetaData metaData)
        {
            metaData.EnsureEntityMetaData();

            this.MetaData = metaData;
        }

        public IEntityMetaData MetaData { get; }

        public SqlQuery ToQuery()
        {
            string tableName = this.MetaData.TableName;

            SqlQuery query = new SqlQuery();
            StringBuilder text = query.Text;
            text.Append("SELECT ");

            foreach (PropertyMetaData property in this.MetaData.Properties)
            {
                string columnName = property.Schema.ColumnName;

                text.Append(columnName);
                text.Append(", ");
            }
            text.Remove(text.Length - 2, 1);

            text.Append("FROM ");
            text.Append(tableName);

            return query;
        }

        public SqlQuery ToQuery(string tableAlias)
        {
            string tableName = this.MetaData.TableName;

            SqlQuery query = new SqlQuery();
            StringBuilder text = query.Text;
            text.Append("SELECT ");

            string tableNameOp = tableAlias + ".";

            foreach (PropertyMetaData property in this.MetaData.Properties)
            {
                string columnName = property.Schema.ColumnName;

                text.Append(tableNameOp);
                text.Append(columnName);
                text.Append(" AS ");
                text.Append(columnName);
                text.Append(", ");
            }
            text.Remove(text.Length - 2, 1);

            text.Append("FROM ");
            text.Append(tableName);
            text.Append(' ');
            text.Append(tableAlias);

            return query;
        }
    }
}
