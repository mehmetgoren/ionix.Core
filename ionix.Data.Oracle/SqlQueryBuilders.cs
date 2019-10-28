namespace Ionix.Data.Oracle
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class EntitySqlQueryBuilderUpdate : IEntitySqlQueryBuilder
    {
        public bool UseSemicolon { get; set; }

        //EntityChangeSet  Kullanımı İçin Eklendi.
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
                text.Append("=");

                SqlQueryHelper.SetColumnValue(ValueSetter.Instance, metaData, index, query, property, entity);

                text.Append(',');
            }
            text.Remove(text.Length - 1, 1);

            query.Combine(SqlQueryHelper.CreateWhereSqlByKeys(metaData, index, GlobalInternal.Prefix, entity));

            if (this.UseSemicolon)
                text.Append(';');

            return query;
        }
    }


    public class EntitySqlQueryBuilderInsert : IEntitySqlQueryBuilder
    {
        private readonly IDbAccess dataAccess;
        private readonly bool isUpsert;

        public EntitySqlQueryBuilderInsert(IDbAccess dataAccess, bool isUpsert)
        {
            if (null == dataAccess)
                throw new ArgumentNullException(nameof(dataAccess));

            this.dataAccess = dataAccess;
            this.isUpsert = isUpsert;
        }

        public bool UseSemicolon { get; set; }

        public HashSet<string> InsertFields { get; set; }

        public virtual SqlQuery CreateQuery(object entity, IEntityMetaData metaData, int index, out PropertyMetaData sequenceIdentity)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));

            sequenceIdentity = null;

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
                    case StoreGeneratedPattern.Identity://Oracle 12c ile test edilip bu kısımda çalışılabilr hale getirilecek.
                    case StoreGeneratedPattern.Computed:
                        continue;// not in insert list.

                    case StoreGeneratedPattern.AutoGenerateSequence:
                        if (null != sequenceIdentity)
                            throw new MultipleIdentityColumnFoundException(entity);

                        sequenceIdentity = property;
                        break;//Identity ise insert list e giriyor.
                    default:
                        if (insertFieldsEnabled && !this.InsertFields.Contains(schema.ColumnName))
                            continue;
                        break;
                }


                text.Append(schema.ColumnName);
                text.Append(',');

                validInfos.Add(property);
            }

            text.Remove(text.Length - 1, 1);
            text.Append(") VALUES (");

            bool addSequenceIdentity = null != sequenceIdentity;

            foreach (PropertyMetaData property in validInfos)
            {
                SchemaInfo schema = property.Schema;

                if (addSequenceIdentity && sequenceIdentity.Schema.Equals(schema))
                {
                    text.Append(SequenceManager.GetSequenceName(this.dataAccess, metaData.TableName, sequenceIdentity.Schema.ColumnName));
                    text.Append(".NEXTVAL,");
                    addSequenceIdentity = false;
                }
                else
                {
                    SqlQueryHelper.SetColumnValue(ValueSetter.Instance, metaData, index, query, property, entity);

                    text.Append(',');
                }
            }
            text.Remove(text.Length - 1, 1);
            text.Append(')');
            if (null != sequenceIdentity)
            {
                text.AppendLine();

                text.Append("RETURNING ");
                text.Append(sequenceIdentity.Schema.ColumnName);
                text.Append(" INTO :");

                string parameterName = metaData.GetParameterName(sequenceIdentity, index);
                text.Append(parameterName);

                SqlQueryParameter identityParameter = SqlQueryHelper.EnsureHasParameter(query, parameterName, sequenceIdentity, entity);
                identityParameter.Direction = this.isUpsert ? System.Data.ParameterDirection.InputOutput : System.Data.ParameterDirection.Output;
            }
            if (this.UseSemicolon)
                text.Append(';');

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
        private readonly IDbAccess dataAccess;
        private readonly bool isBatchUpsert;

        public EntitySqlQueryBuilderUpsert(IDbAccess dataAccess, bool isBatchUpsert)
        {
            if (null == dataAccess)
                throw new ArgumentNullException(nameof(dataAccess));

            this.dataAccess = dataAccess;
            this.isBatchUpsert = isBatchUpsert;
        }

        public HashSet<string> UpdatedFields { get; set; }

        public HashSet<string> InsertFields { get; set; }

        public virtual SqlQuery CreateQuery(object entity, IEntityMetaData metaData, int index, out PropertyMetaData sequenceIdentity)
        {
            EntitySqlQueryBuilderUpdate builderUpdate = new EntitySqlQueryBuilderUpdate() { UseSemicolon = true };
            builderUpdate.UpdatedFields = this.UpdatedFields;

            SqlQuery query = new SqlQuery();
            StringBuilder text = query.Text;

            if (!this.isBatchUpsert)
                text.Append(GlobalInternal.OracleBeginStatement);

            query.Combine(builderUpdate.CreateQuery(entity, metaData, index));

            text.AppendLine();
            text.Append("IF SQL%ROWCOUNT = 0 THEN ");
            text.AppendLine();

            EntitySqlQueryBuilderInsert builderInsert = new EntitySqlQueryBuilderInsert(this.dataAccess, true) { UseSemicolon = true };
            builderInsert.InsertFields = this.InsertFields;

            query.Combine(builderInsert.CreateQuery(entity, metaData, index, out sequenceIdentity));

            text.AppendLine();
            text.Append("END IF;");

            if (!this.isBatchUpsert)
                text.Append(GlobalInternal.OracleEndStatement);

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
        public bool UseSemicolon { get; set; }

        public virtual SqlQuery CreateQuery(object entity, IEntityMetaData metaData, int index)
        {
            SqlQuery query = new SqlQuery();
            StringBuilder text = query.Text;
            text.Append("DELETE FROM ");
            text.Append(metaData.TableName);

            query.Combine(SqlQueryHelper.CreateWhereSqlByKeys(metaData, index, GlobalInternal.Prefix, entity));

            if (this.UseSemicolon)
                text.Append(';');

            return query;
        }
    }
}
