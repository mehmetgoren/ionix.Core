namespace Ionix.Data.SqlServer
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class EntitySqlQueryBuilderUpdate : IEntitySqlQueryBuilder
    {
        public HashSet<string> UpdatedFields { get; set; }

        public virtual SqlQuery CreateQuery(object entity, IEntityMetaData metaData, int index)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));

            bool updatedFieldsEnabled = !this.UpdatedFields.IsNullOrEmpty();

            SqlQuery query = new SqlQuery();
            StringBuilder text = query.Text;
            text.Append("UPDATE ");
            text.Append(metaData.TableName);
            text.Append(" SET ");

            foreach (PropertyMetaData property in metaData.Properties)
            {
                SchemaInfo schema = property.Schema;

                if (schema.IsKey)
                    continue;

                if (schema.DatabaseGeneratedOption != StoreGeneratedPattern.None)
                    continue;

                if (schema.ReadOnly)
                    continue;

                if (updatedFieldsEnabled && !this.UpdatedFields.Contains(schema.ColumnName))
                    continue;

                text.Append(schema.ColumnName);
                text.Append('=');

                SqlQueryHelper.SetColumnValue(ValueSetter.Instance, metaData, index, query, property, entity);

                text.Append(',');
            }
            text.Remove(text.Length - 1, 1);

            query.Combine(SqlQueryHelper.CreateWhereSqlByKeys(metaData, index, GlobalInternal.Prefix, entity));

            return query;
        }
    }

    public class EntitySqlQueryBuilderInsert : IEntitySqlQueryBuilder
    {
        public HashSet<string> InsertFields { get; set; }

        public virtual SqlQuery CreateQuery(object entity, IEntityMetaData metaData, int index, out PropertyMetaData identity)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));

            identity = null;

            bool insertFieldsEnabled = !this.InsertFields.IsNullOrEmpty();

            SqlQuery query = new SqlQuery();
            StringBuilder text = query.Text;
            text.Append("INSERT INTO ");
            text.Append(metaData.TableName);
            text.Append(" (");

            List<PropertyMetaData> validInfos = new List<PropertyMetaData>();
            foreach (PropertyMetaData property in metaData.Properties)
            {
                SchemaInfo schema = property.Schema;

                switch (schema.DatabaseGeneratedOption)
                {
                    case StoreGeneratedPattern.Identity:
                        if (null != identity)
                            throw new MultipleIdentityColumnFoundException(entity);

                        identity = property;
                        break;

                    case StoreGeneratedPattern.Computed:
                        break;

                    default:
                        if (insertFieldsEnabled && !this.InsertFields.Contains(schema.ColumnName))
                            continue;

                        text.Append(schema.ColumnName);
                        text.Append(',');

                        validInfos.Add(property);
                        break;
                }
            }

            text.Remove(text.Length - 1, 1);
            text.Append(") VALUES (");

            foreach (PropertyMetaData property in validInfos)
            {
                SqlQueryHelper.SetColumnValue(ValueSetter.Instance, metaData, index, query, property, entity);

                text.Append(',');
            }
            text.Remove(text.Length - 1, 1);
            text.Append(')');
            if (null != identity)
            {
                text.AppendLine();

                text.Append("SELECT @");

                string parameterName = metaData.GetParameterName(identity, index);
                text.Append(parameterName);

                text.Append("=SCOPE_IDENTITY()");

                SqlQueryParameter identityParameter = SqlQueryHelper.EnsureHasParameter(query, parameterName, identity, entity);
                identityParameter.Direction = System.Data.ParameterDirection.InputOutput;
            }

            return query;
        }

        SqlQuery IEntitySqlQueryBuilder.CreateQuery(object entity, IEntityMetaData metaData, int index)
        {
            PropertyMetaData identity;
            return this.CreateQuery(entity, metaData, index, out identity);
        }
    }

    public class EntitySqlQueryBuilderUpsert : IEntitySqlQueryBuilder
    {
        public HashSet<string> UpdatedFields { get; set; }

        public HashSet<string> InsertFields { get; set; }

        public virtual SqlQuery CreateQuery(object entity, IEntityMetaData metaData, int index, out PropertyMetaData identity)
        {
            EntitySqlQueryBuilderUpdate builderUpdate = new EntitySqlQueryBuilderUpdate();
            builderUpdate.UpdatedFields = this.UpdatedFields;

            SqlQuery query = builderUpdate.CreateQuery(entity, metaData, index);
            StringBuilder text = query.Text;

            text.AppendLine();
            text.Append("IF @@ROWCOUNT = 0");
            text.AppendLine();
            text.Append("BEGIN");
            text.AppendLine();

            EntitySqlQueryBuilderInsert builderInsert = new EntitySqlQueryBuilderInsert();
            builderInsert.InsertFields = this.InsertFields;
            query.Combine(builderInsert.CreateQuery(entity, metaData, index, out identity));

            text.AppendLine();
            text.Append("END");

            return query;
        }

        SqlQuery IEntitySqlQueryBuilder.CreateQuery(object entity, IEntityMetaData metaData, int index)
        {
            PropertyMetaData identity;
            return this.CreateQuery(entity, metaData, index, out identity);
        }
    }

    public class EntitySqlQueryBuilderDelete : IEntitySqlQueryBuilder
    {
        public virtual SqlQuery CreateQuery(object entity, IEntityMetaData metaData, int index)
        {
            SqlQuery query = new SqlQuery();
            StringBuilder text = query.Text;
            text.Append("DELETE FROM ");
            text.Append(metaData.TableName);

            query.Combine(SqlQueryHelper.CreateWhereSqlByKeys(metaData, index, GlobalInternal.Prefix, entity));

            return query;
        }
    }
}
