namespace Ionix.Data.SqlServer
{
    using Utils;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Ionix.Utils.Extensions;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public static class SqlServerExtensions
    {
        public static string ToParameterlessQuery(this SqlQuery query)
        {
            if (null != query)
            {
                string sql = query.Text.ToString();
                if (sql.Length != 0)
                {
                    List<SqlQueryParameter> pars = new List<SqlQueryParameter>(query.Parameters);
                    for (int j = 0; j < pars.Count; ++j)
                    {
                        SqlQueryParameter par = pars[j];
                        string parameterName = par.ParameterName;
                        object value = par.Value;
                        string strValue = ToParameterlessString(value);

                        parameterName = parameterName[0] == '@' ? parameterName.Remove(0, 1) : parameterName;

                        string pattern = @"\@" + parameterName + @"\b";
                        string replace = strValue;
                        sql = Regex.Replace(sql, pattern, replace);
                    }
                }
                return sql;
            }
            return String.Empty;
        }

        private static string ToParameterlessString(object parameterValue)
        {
            if (parameterValue == null)
                return "NULL";
            Type parameterValueType = parameterValue.GetType();
            if (parameterValueType == CachedTypes.DBNull)
                return "NULL";

            else if (parameterValueType == CachedTypes.String
                || parameterValueType == CachedTypes.Guid
                || parameterValueType == CachedTypes.Char
                || parameterValueType == CachedTypes.Nullable_Char)
                return '\'' + parameterValue.ToString() + '\'';
            else if (parameterValueType == CachedTypes.DateTime || parameterValueType == CachedTypes.Nullable_DateTime)
            {
                return "CONVERT(Datetime, '" + parameterValue + "', 104)";
            }
            else if (parameterValueType == CachedTypes.Boolean || parameterValueType == CachedTypes.Nullable_Boolean)
            {
                return (Boolean)parameterValue ? "1" : "0";
            }
            else if (parameterValueType == CachedTypes.ByteArray)
                return "<Byte Array>";
            else
                return parameterValue.ToString();
        }

        private const int MaxAllowedParameterCount = 2100;

        private static int GetProCount<T>(Expression<Func<T, object>>[] fields)
        {
            int propCount = fields?.Length ?? 0;
            if (propCount == 0)
            {
                var provider = new DbSchemaMetaDataProvider();
                IEntityMetaData metaData = provider.CreateEntityMetaData(typeof(T));
                propCount = metaData.Properties.Count();
            }

            return propCount;
        }

        private static void BatchOperationLimit2100<T>(IEnumerable<T> entities, int propCount, ref int affectedCount, Expression<Func<T, object>>[] fields
            , Func<IEnumerable<T>, Expression<Func<T, object>>[],  int> fn)
        {
            int limit = MaxAllowedParameterCount / propCount;
            int entityCount = entities.Count();

            if (entityCount < limit)
                affectedCount += fn(entities, fields);
            else
            {
                List<T> entityList = entities.ToList();
                List<T> sublist = entityList.GetRange(0, limit);
                affectedCount += fn(sublist, fields);

                entityList.RemoveRange(0, limit);
                BatchOperationLimit2100(entityList, propCount, ref affectedCount, fields, fn);
            }
        }

        private static async Task BatchOperationLimit2100Async<T>(IEnumerable<T> entities, int propCount, Expression<Func<T, object>>[] fields
            , Func<IEnumerable<T>, Expression<Func<T, object>>[], Task<int>> fnAsync)
        {
            int limit = MaxAllowedParameterCount / propCount;
            int entityCount = entities.Count();

            if (entityCount < limit)
                await fnAsync(entities, fields);
            else
            {
                List<T> entityList = entities.ToList();
                List<T> sublist = entityList.GetRange(0, limit); 
                await fnAsync(sublist, fields);

                entityList.RemoveRange(0, limit);
                await BatchOperationLimit2100Async(entityList, propCount, fields, fnAsync);
            }
        }

        public static int BatchInsertLimit2100<T>(this ICommandAdapter cmd, IEnumerable<T> entities, params Expression<Func<T, object>>[] insertFields)
        {
            if (null == cmd || entities.IsNullOrEmpty())
                return 0;

            int propCount = GetProCount(insertFields);
            if (propCount == 0)
                return 0;

            int affectedCount = 0;
            BatchOperationLimit2100(entities, propCount, ref affectedCount, insertFields, cmd.BatchInsert);

            return affectedCount;
        }

        public static int BatchUpdateLimit2100<T>(this ICommandAdapter cmd, IEnumerable<T> entities, params Expression<Func<T, object>>[] updatedFields)
        {
            if (null == cmd || entities.IsNullOrEmpty())
                return 0;

            int propCount = GetProCount(updatedFields);
            if (propCount == 0)
                return 0;

            int affectedCount = 0;
            BatchOperationLimit2100(entities, propCount, ref affectedCount, updatedFields, cmd.BatchUpdate);

            return affectedCount;
        }

        public static int BatchUpsertLimit2100<T>(this ICommandAdapter cmd, IEnumerable<T> entities, params Expression<Func<T, object>>[] updatedFields)
        {
            if (null == cmd || entities.IsNullOrEmpty())
                return 0;

            int propCount = GetProCount(updatedFields);
            if (propCount == 0)
                return 0;

            int affectedCount = 0;
            BatchOperationLimit2100(entities, propCount, ref affectedCount, updatedFields, cmd.BatchUpdate);

            return affectedCount;
        }

        public static async Task BatchInsertLimit2100Async<T>(this ICommandAdapter cmd, IEnumerable<T> entities, params Expression<Func<T, object>>[] insertFields)
        {
            if (null == cmd || entities.IsNullOrEmpty())
                return;

            int propCount = GetProCount(insertFields);
            if (propCount == 0)
                return;

            await BatchOperationLimit2100Async(entities, propCount, insertFields, cmd.BatchInsertAsync);
        }

        public static async Task BatchUpdateLimit2100Async<T>(this ICommandAdapter cmd, IEnumerable<T> entities, params Expression<Func<T, object>>[] insertFields)
        {
            if (null == cmd || entities.IsNullOrEmpty())
                return;

            int propCount = GetProCount(insertFields);
            if (propCount == 0)
                return;

            await BatchOperationLimit2100Async(entities, propCount, insertFields, cmd.BatchUpdateAsync);
        }

        public static async Task BatchUpsertLimit2100Async<T>(this ICommandAdapter cmd, IEnumerable<T> entities, params Expression<Func<T, object>>[] insertFields)
        {
            if (null == cmd || entities.IsNullOrEmpty())
                return;

            int propCount = GetProCount(insertFields);
            if (propCount == 0)
                return;

            await BatchOperationLimit2100Async(entities, propCount, insertFields, cmd.BatchUpsertAsync);
        }
    }
}
