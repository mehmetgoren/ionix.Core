namespace Ionix.Data
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    partial class CommandAdapterExtensions
    {
        #region   |   Select   |

        public static TEntity SelectSingle<TEntity>(this ICommandAdapter adapter)
        {
            if (null != adapter)
            {
                return adapter.SelectSingle<TEntity>(null);
            }
            return default(TEntity);
        }
        public static Task<TEntity> SelectSingleAsync<TEntity>(this ICommandAdapter adapter)
        {
            if (null != adapter)
            {
                return adapter.SelectSingleAsync<TEntity>(null);
            }
            return Task.FromResult(default(TEntity));
        }

        public static IList<TEntity> Select<TEntity>(this ICommandAdapter adapter)
        {
            if (null != adapter)
            {
                return adapter.Select<TEntity>(null);
            }
            return new List<TEntity>();
        }
        public static Task<IList<TEntity>> SelectAsync<TEntity>(this ICommandAdapter adapter)
        {
            if (null != adapter)
            {
                return adapter.SelectAsync<TEntity>(null);
            }
            return Task.FromResult(default(IList<TEntity>));
        }

        #endregion


        #region   |   Entity   |

        internal static string[] ToStringArray<TEntity>(Expression<Func<TEntity, object>>[] updatedFields)
        {
            string[] arr = null;
            if (!updatedFields.IsNullOrEmpty())
            {
                arr = new string[updatedFields.Length];
                for (int j = 0; j < updatedFields.Length; ++j)
                {
                    Expression<Func<TEntity, object>> exp = updatedFields[j];
                    PropertyInfo pi = ReflectionExtensions.GetPropertyInfo(exp.Body);
                    string columnName = pi.Name;

                    var attr = pi.GetCustomAttribute<DbSchemaAttribute>();
                    if (null != attr && !String.IsNullOrEmpty(attr.ColumnName))
                        columnName = attr.ColumnName;
                    else
                    {
                        var attr2 = pi.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>();
                        if (null != attr2 && !String.IsNullOrEmpty(attr2.Name))
                            columnName = attr2.Name;
                    }

                    arr[j] = columnName;
                }
            }
            return arr;
        }

        public static int Update<TEntity>(this ICommandAdapter adapter, TEntity entity)
        {
            if (null != adapter)
                return adapter.Update(entity, null);
            return 0;
        }
        public static Task<int> UpdateAsync<TEntity>(this ICommandAdapter adapter, TEntity entity)
        {
            if (null != adapter)
                return adapter.UpdateAsync(entity, null);
            return Task.FromResult(0);
        }

        public static int Update<TEntity>(this ICommandAdapter adapter, TEntity entity,
            params Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] arr = ToStringArray(updatedFields);
                return adapter.Update(entity, arr);
            }
            return 0;
        }
        public static Task<int> UpdateAsync<TEntity>(this ICommandAdapter adapter, TEntity entity,
            params Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] arr = ToStringArray(updatedFields);
                return adapter.UpdateAsync(entity, arr);
            }
            return Task.FromResult(0);
        }



        public static int Insert<TEntity>(this ICommandAdapter adapter, TEntity entity)
        {
            if (null != adapter)
            {
                return adapter.Insert(entity, null);
            }
            return 0;
        }
        public static Task<int> InsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity)
        {
            if (null != adapter)
            {
                return adapter.InsertAsync(entity, null);
            }
            return Task.FromResult(0);
        }

        public static int Insert<TEntity>(this ICommandAdapter adapter, TEntity entity,
            params Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] arr = ToStringArray(insertFields);
                return adapter.Insert(entity, arr);
            }

            return 0;
        }
        public static Task<int> InsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity,
            params Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] arr = ToStringArray(insertFields);
                return adapter.InsertAsync(entity, arr);
            }
            return Task.FromResult(0);
        }



        public static int Upsert<TEntity>(this ICommandAdapter adapter, TEntity entity)
        {
            if (null != adapter)
            {
                return adapter.Upsert(entity, null, null);
            }
            return 0;
        }
        public static Task<int> UpsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity)
        {
            if (null != adapter)
            {
                return adapter.UpsertAsync(entity, null, null);
            }
            return Task.FromResult(0);
        }

        public static int Upsert<TEntity>(this ICommandAdapter adapter, TEntity entity, string[] updatedFields)
        {
            if (null != adapter)
            {
                return adapter.Upsert(entity, updatedFields, null);
            }
            return 0;
        }
        public static Task<int> UpsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity, string[] updatedFields)
        {
            if (null != adapter)
            {
                return adapter.UpsertAsync(entity, updatedFields, null);
            }
            return Task.FromResult(0);
        }

        public static int Upsert<TEntity>(this ICommandAdapter adapter, TEntity entity, Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] arr = ToStringArray(updatedFields);
                return adapter.Upsert(entity, arr, null);
            }
            return 0;
        }
        public static Task<int> UpsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity, Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] arr = ToStringArray(updatedFields);
                return adapter.UpsertAsync(entity, arr, null);
            }
            return Task.FromResult(0);
        }

        public static int Upsert<TEntity>(this ICommandAdapter adapter, TEntity entity,
            Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                string[] insertArr = ToStringArray(insertFields);
                return adapter.Upsert(entity, updateArr, insertArr);
            }
            return 0;
        }
        public static Task<int> UpsertAsync<TEntity>(this ICommandAdapter adapter, TEntity entity,
             Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                string[] insertArr = ToStringArray(insertFields);
                return adapter.UpsertAsync(entity, updateArr, insertArr);
            }
            return Task.FromResult(0);
        }

        #endregion


        #region   |   Batch   |

        public static int BatchUpdate<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList)
        {
            if (null != adapter)
            {
                return adapter.BatchUpdate(entityList, null);
            }
            return 0;
        }
        public static Task<int> BatchUpdateAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList)
        {
            if (null != adapter)
            {
                return adapter.BatchUpdateAsync(entityList, null);
            }
            return Task.FromResult(0);
        }

        public static int BatchUpdate<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            params Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                return adapter.BatchUpdate(entityList, updateArr);
            }
            return 0;
        }
        public static Task<int> BatchUpdateAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            params Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                return adapter.BatchUpdateAsync(entityList, updateArr);
            }
            return Task.FromResult(0);
        }



        public static int BatchInsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList)
        {
            if (null != adapter)
            {
                return adapter.BatchInsert(entityList, null);
            }
            return 0;
        }
        public static Task<int> BatchInsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList)
        {
            if (null != adapter)
            {
                return adapter.BatchInsertAsync(entityList, null);
            }
            return Task.FromResult(0);
        }

        public static int BatchInsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            params Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] insertArr = ToStringArray(insertFields);
                return adapter.BatchInsert(entityList, insertArr);
            }
            return 0;
        }
        public static Task<int> BatchInsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            params Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] insertArr = ToStringArray(insertFields);
                return adapter.BatchInsertAsync(entityList, insertArr);
            }
            return Task.FromResult(0);
        }



        public static int BatchUpsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList)
        {
            if (null != adapter)
            {
                return adapter.BatchUpsert(entityList, null, null);
            }
            return 0;
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList)
        {
            if (null != adapter)
            {
                return adapter.BatchUpsertAsync(entityList, null, null);
            }
            return Task.FromResult(0);
        }

        public static int BatchUpsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            string[] updatedFields)
        {
            if (null != adapter)
            {
                return adapter.BatchUpsert(entityList, updatedFields, null);
            }
            return 0;
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            string[] updatedFields)
        {
            if (null != adapter)
            {
                return adapter.BatchUpsertAsync(entityList, updatedFields, null);
            }
            return Task.FromResult(0);
        }

        public static int BatchUpsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
            Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                return adapter.BatchUpsert(entityList, updateArr, null);
            }
            return 0;
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
             Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                return adapter.BatchUpsertAsync(entityList, updateArr, null);
            }
            return Task.FromResult(0);
        }

        public static int BatchUpsert<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
             Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                string[] insertArr = ToStringArray(insertFields);
                return adapter.BatchUpsert(entityList, updateArr, insertArr);
            }
            return 0;
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this ICommandAdapter adapter, IEnumerable<TEntity> entityList,
              Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != adapter)
            {
                string[] updateArr = ToStringArray(updatedFields);
                string[] insertArr = ToStringArray(insertFields);
                return adapter.BatchUpsertAsync(entityList, updateArr, insertArr);
            }
            return Task.FromResult(0);
        }
        #endregion

    }
}
