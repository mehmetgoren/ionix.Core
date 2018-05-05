namespace ionix.Migration.PostgreSql
{
    using ionix.Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;
    using Data;

    public class MigrationBuilder : IMigrationBuilder
    {
        public static readonly MigrationBuilder Instance = new MigrationBuilder();
        private MigrationBuilder()
        {

        }

        public virtual SqlQuery Build(IEnumerable<Type> types, IColumnDbTypeResolver typeResolver)
        {
            SqlQuery query = new SqlQuery();
            if (!types.IsEmptyList() && null != typeResolver)
            {
                IEntityMetaDataProvider metaDataProvider = new DbSchemaMetaDataProvider();
                foreach (Type type in types)
                {
                    TableAttribute tattr = type.GetCustomAttribute<TableAttribute>();//Bu olmaz ise migration a girmeyecek.
                    if (null != tattr)
                    {
                        IEntityMetaData metaData = metaDataProvider.CreateEntityMetaData(type);
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
            }

            return query;
        }
    }
    }
