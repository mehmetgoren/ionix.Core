namespace Ionix.Migration.SQLite
{
    using Ionix.Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Data;

    public class MigrationSqlQueryBuilder : IMigrationSqlQueryBuilder
    {
        public static readonly MigrationSqlQueryBuilder Instance = new MigrationSqlQueryBuilder();
        private MigrationSqlQueryBuilder() { }

        //tableattribute check edilip gönderiliyor types' a
        public virtual SqlQuery CreateTable(IEnumerable<Type> types, DbSchemaMetaDataProvider provider, IColumnDbTypeResolver typeResolver)
        {
            SqlQuery query = new SqlQuery();
            if (!types.IsNullOrEmpty() && null != typeResolver)
            {
                foreach (Type type in types)
                {
                    IEntityMetaData metaData = provider.CreateEntityMetaData(type);
                    List<Column> columns = new List<Column>();
                    foreach (PropertyMetaData prop in metaData.Properties)
                    {
                        var column = typeResolver.GetColumn(prop);
                        columns.Add(column);
                    }

                    CreateTableQueryBuilder item = new CreateTableQueryBuilder(metaData.TableName, columns, type.GetCustomAttributes<TableForeignKeyAttribute>());
                    query.Combine(item.ToQuery());
                    query.Text.AppendLine();

                    //indexes
                    IEnumerable<TableIndexAttribute> indexAttrs = type.GetCustomAttributes<TableIndexAttribute>();
                    if (!indexAttrs.IsNullOrEmpty())
                    {
                        foreach (TableIndexAttribute indexAttr in indexAttrs)
                        {
                            CreateIndexQueryBuilder ci = new CreateIndexQueryBuilder(metaData.TableName, indexAttr);
                            query.Combine(ci.ToQuery());
                            query.Text.AppendLine();
                        }
                    }

                    query.Text.AppendLine();
                }
            }

            return query;
        }
    }
}
