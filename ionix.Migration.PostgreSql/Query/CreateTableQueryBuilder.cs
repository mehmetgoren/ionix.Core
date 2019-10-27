namespace Ionix.Migration.PostgreSql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;

    internal sealed class CreateTableQueryBuilder : ISqlQueryProvider
    {
        public string TableName { get; }
        public IEnumerable<Column> Columns { get; }

        public bool OIDS { get; set; }


        internal CreateTableQueryBuilder(string tableName, IEnumerable<Column> columns)
        {
            this.TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
            this.Columns = columns ?? throw new ArgumentNullException(nameof(columns));
            if (!columns.Any())
                throw new ArgumentException($"{nameof(columns)}' length must be greater than zero");
            this.OIDS = false;
        }

        public SqlQuery ToQuery()
        {
            SqlQuery q = "CREATE TABLE ".ToQuery().Sql(this.TableName).Sql(" (").Sql(Environment.NewLine);
            Column primaryKey = null;
            foreach (var column in this.Columns)
            {
                if (column.IsPrimaryKey)
                {
                    if (null == primaryKey)
                        primaryKey = column;
                    else
                        throw new MultipleIdentityColumnFoundException($"{this.TableName} has more than one primary key.");
                }

                q.Combine(column.ToQuery()).Sql(",").Sql(Environment.NewLine);
            }
            if (null != primaryKey)
            {
                q.Sql("PRIMARY KEY (").
                    Sql(primaryKey.Name).
                    Sql(")");
            }
            else
            {
                q.Text.Remove(q.Text.Length - 3, 3);
            }
            q.Text.AppendLine();
            if (!this.OIDS)
            {
                q.Sql(") WITH (OIDS=FALSE");
            }
            q.Sql(");");
            return q;
        }
    }
}
