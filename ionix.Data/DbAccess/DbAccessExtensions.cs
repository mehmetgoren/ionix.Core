namespace Ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Dynamic;
    using System.Threading.Tasks;

    public static class DbAccessExtensions
    {
        private static void EnsureDbAccess(IDbAccess dataAccess)
        {
            if (null == dataAccess)
                throw new ArgumentNullException(nameof(dataAccess));
        }


        #region   |   Execute   |

        public static IList<object> ExecuteScalarList(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            List<object> ret = new List<object>();
            using (IDataReader dr = dataAccess.CreateDataReader(query, CommandBehavior.Default))
            {
                while (dr.Read())
                {
                    object value = dr[0];

                    ret.Add(value);
                }
            }
            return ret;
        }
        public static async Task<IList<object>> ExecuteScalarListAsync(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            List<object> ret = new List<object>();
            using (IDataReader dr = await dataAccess.CreateDataReaderAsync(query, CommandBehavior.Default))
            {
                while (dr.Read())
                {
                    object value = dr[0];

                    ret.Add(value);
                }
            }
            return ret;
        }

        public static IDataReader CreateDataReader(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            return dataAccess.CreateDataReader(query, CommandBehavior.Default);
        }
        public static Task<AutoCloseCommandDataReader> CreateDataReaderAsync(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            return dataAccess.CreateDataReaderAsync(query, CommandBehavior.Default);
        }


        //Generics
        public static T ExecuteScalar<T>(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            try
            {
                return (T)Convert.ChangeType(dataAccess.ExecuteScalar(query), typeof(T));
            }
            catch
            {
                return default(T);
            }
        }
        public static async Task<T> ExecuteScalarAsync<T>(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            try
            {
                return (T)Convert.ChangeType(await dataAccess.ExecuteScalarAsync(query), typeof(T));
            }
            catch
            {
                return default(T);
            }
        }


        public static IList<T> ExecuteScalarList<T>(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            List<T> ret = new List<T>();
            using (IDataReader dr = dataAccess.CreateDataReader(query, CommandBehavior.Default))
            {
                while (dr.Read())
                {
                    object value = dr[0];

                    T item;
                    try
                    {
                        item = (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch
                    {
                        item = default(T);
                    }

                    ret.Add(item);
                }
            }
            return ret;
        }
        public static async Task<IList<T>> ExecuteScalarListAsync<T>(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            List<T> ret = new List<T>();
            using (DbDataReader dr = await dataAccess.CreateDataReaderAsync(query, CommandBehavior.Default))
            {
                while (dr.Read())
                {
                    object value = dr[0];

                    T item;
                    try
                    {
                        item = (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch
                    {
                        item = default(T);
                    }

                    ret.Add(item);
                }
            }
            return ret;
        }


        #endregion

        #region   |   Dynamic   |

        public static dynamic QuerySingle(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            IDataReader dr = null;
            try
            {
                dr = dataAccess.CreateDataReader(query, CommandBehavior.SingleRow);

                Lazy<int> fieldCount = new Lazy<int>(() => dr.FieldCount);
                if (dr.Read())
                {
                    ExpandoObject expando = new ExpandoObject();
                    IDictionary<string, object> dic = expando;
                    for (int j = 0; j < fieldCount.Value; ++j)
                    {
                        object dbValue = dr.IsDBNull(j) ? null : dr[j];

                        dic.Add(dr.GetName(j), dbValue);
                    }

                    return expando;
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return null;
        }
        public static async Task<dynamic> QuerySingleAsync(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            IDataReader dr = null;
            try
            {
                dr = await dataAccess.CreateDataReaderAsync(query, CommandBehavior.SingleRow);

                int fieldCount = dr.FieldCount;
                if (dr.Read())
                {
                    ExpandoObject expando = new ExpandoObject();
                    IDictionary<string, object> dic = expando;
                    for (int j = 0; j < fieldCount; ++j)
                    {
                        object dbValue = dr.IsDBNull(j) ? null : dr[j];

                        dic.Add(dr.GetName(j), dbValue);
                    }

                    return expando;
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return null;
        }

        public static IList<dynamic> Query(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            IList<dynamic> ret = new List<dynamic>();
            IDataReader dr = null;
            try
            {
                dr = dataAccess.CreateDataReader(query, CommandBehavior.Default);

                Lazy<int> fieldCount = new Lazy<int>(() => dr.FieldCount);
                while (dr.Read())
                {
                    ExpandoObject expando = new ExpandoObject();
                    IDictionary<string, object> dic = expando;
                    for (int j = 0; j < fieldCount.Value; ++j)
                    {
                        object dbValue = dr.IsDBNull(j) ? null : dr[j];

                        dic.Add(dr.GetName(j), dbValue);
                    }

                    ret.Add(expando);
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return ret;
        }
        public static async Task<IList<dynamic>> QueryAsync(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            IList<dynamic> ret = new List<dynamic>();
            IDataReader dr = null;
            try
            {
                dr = await dataAccess.CreateDataReaderAsync(query, CommandBehavior.Default);

                Lazy<int> fieldCount = new Lazy<int>(() => dr.FieldCount);
                while (dr.Read())
                {
                    ExpandoObject expando = new ExpandoObject();
                    IDictionary<string, object> dic = expando;
                    for (int j = 0; j < fieldCount.Value; ++j)
                    {
                        object dbValue = dr.IsDBNull(j) ? null : dr[j];

                        dic.Add(dr.GetName(j), dbValue);
                    }

                    ret.Add(expando);
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return ret;
        }

        #endregion

        public static DataTable QueryDataTable(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            DataTable ret = new DataTable();
            IDataReader dr = null;
            try
            {
                dr = dataAccess.CreateDataReader(query, CommandBehavior.Default);
                ret.Load(dr);//.net standart 2.0 da not suppurted yiyoruz, final da düzelmesi lazım.
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return ret;
        }
        public static async Task<DataTable> QueryDataTableAsync(this IDbAccess dataAccess, SqlQuery query)
        {
            EnsureDbAccess(dataAccess);

            DataTable ret = new DataTable();
            IDataReader dr = null;
            try
            {
                dr = await dataAccess.CreateDataReaderAsync(query, CommandBehavior.Default);
                ret.Load(dr);//.net standart 2.0 da not suppurted yiyoruz, final da düzelmesi lazım.
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return ret;
        }
    }
}
