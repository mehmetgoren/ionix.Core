﻿namespace ionix.Migration.PostgreSql
{
    using ionix.Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Data;

    public class MigrationSqlQueryBuilder : IMigrationSqlQueryBuilder
    {
        public static readonly MigrationSqlQueryBuilder Instance = new MigrationSqlQueryBuilder();
        private MigrationSqlQueryBuilder() { }

        //tableattribute check edilip gönderiliyor types' a
        public virtual SqlQuery CreateTable(IEnumerable<Type> types, MigrationCreateTableDbSchemaMetaDataProvider provider, IColumnDbTypeResolver typeResolver)
        {
            SqlQuery query = new SqlQuery();
            if (!types.IsEmptyList() && null != typeResolver)
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

                    CreateTableQueryBuilder item = new CreateTableQueryBuilder(metaData.TableName, columns);
                    query.Combine(item.ToQuery());
                    query.Text.AppendLine();

                    //indexes
                    IEnumerable<TableIndexAttribute> indexAttrs = type.GetCustomAttributes<TableIndexAttribute>();
                    if (!indexAttrs.IsEmptyList())
                    {
                        foreach (TableIndexAttribute indexAttr in indexAttrs)
                        {
                            CreateIndexQueryBuilder ci = new CreateIndexQueryBuilder(metaData.TableName, indexAttr);
                            query.Combine(ci.ToQuery());
                            query.Text.AppendLine();
                        }
                    }

                    //fks
                    IEnumerable<TableForeignKeyAttribute> fkAttrs = type.GetCustomAttributes<TableForeignKeyAttribute>();
                    if (!fkAttrs.IsEmptyList())
                    {
                        foreach (TableForeignKeyAttribute fkAttr in fkAttrs)
                        {
                            CreateForeignKeyQueryBuilder cfk = new CreateForeignKeyQueryBuilder(metaData.TableName, fkAttr);
                            query.Combine(cfk.ToQuery());
                            query.Text.AppendLine();
                        }
                        query.Text.AppendLine();
                    }


                    query.Text.AppendLine();
                }
            }

            return query;
        }

        public SqlQuery AddColumn(IEnumerable<Type> types, MigrationAddColumnDbSchemaMetaDataProvider provider, IColumnDbTypeResolver typeResolver)
        {
            SqlQuery query = new SqlQuery();
            if (!types.IsEmptyList() && null != typeResolver)
            {
                foreach (Type type in types)
                {
                    IEntityMetaData metaData = provider.CreateEntityMetaData(type);
                    foreach (PropertyMetaData prop in metaData.Properties)
                    {
                        var column = typeResolver.GetColumn(prop);
                        query.Sql($"ALTER TABLE {metaData.TableName} ADD COLUMN ").Combine(column.ToQuery()).Sql(";").Sql(Environment.NewLine);
                    }
                }
            }

            return query;
        }
    }
}
