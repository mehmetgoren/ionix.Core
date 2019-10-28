namespace Ionix.Migration.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Ionix.Utils.Extensions;

    internal sealed class CreateTableQueryBuilder : ISqlQueryProvider
    {
        public string TableName { get; }
        public IEnumerable<Column> Columns { get; }
        public IEnumerable<TableForeignKeyAttribute> TableForeignKeyList { get; }

        internal CreateTableQueryBuilder(string tableName, IEnumerable<Column> columns
            , IEnumerable<TableForeignKeyAttribute> tableForeignKeyList)
        {
            this.TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
            this.Columns = columns ?? throw new ArgumentNullException(nameof(columns));
            if (!columns.Any())
                throw new ArgumentException($"{nameof(columns)}' length must be greater than zero");

            this.TableForeignKeyList = tableForeignKeyList;
        }

        public SqlQuery ToQuery()
        {
            SqlQuery q = "CREATE TABLE ".ToQuery().Sql(this.TableName).Sql(" (").Sql(Environment.NewLine);
            Column primaryKey = null;
            foreach (var column in this.Columns)
            {
                if (column.IsPrimaryKey)
                {
                    if (null != primaryKey)
                        throw new MultipleIdentityColumnFoundException($"{this.TableName} has more than one primary key.");

                    primaryKey = column;
                }

                q.Combine(column.ToQuery()).Sql(",").Sql(Environment.NewLine);
            }
            if (!this.TableForeignKeyList.IsNullOrEmpty())
            {
                foreach (var fk in this.TableForeignKeyList)
                {
                    if (null != fk)
                    {
                        q.Combine(new CreateForeignKeyQueryBuilder(fk).ToQuery()).Sql(",").Sql(Environment.NewLine);
                    }
                }
            }


            q.Text.Remove(q.Text.Length - 3, 3);

            q.Text.AppendLine();
            q.Sql(");");
            return q;
        }
    }
}
