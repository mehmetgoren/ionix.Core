namespace Ionix.Data.SQLite
{
    using Utils;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Ionix.Utils.Extensions;

    public static class SQLiteExtensions
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

            else if (parameterValueType.In(CachedTypes.String, CachedTypes.Guid, CachedTypes.Nullable_Guid,
                CachedTypes.Char, CachedTypes.Nullable_Char, CachedTypes.DateTime, CachedTypes.Nullable_DateTime))
                return '\'' + parameterValue.ToString() + '\'';
            else if (parameterValueType == CachedTypes.Boolean || parameterValueType == CachedTypes.Nullable_Boolean)
                return (Boolean)parameterValue ? "true" : "false";
            else if (parameterValueType == CachedTypes.ByteArray)
                return "<Byte Array>";
            else
                return parameterValue.ToString();
        }
    }
}
