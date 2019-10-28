namespace Ionix.Data
{
    using Utils.Extensions;
    using System;
    using System.Collections;
    using System.Reflection;

    //RESTful Servisler için Eklendi.
    public static partial class CommandAdapterExtensions
    {
        private static readonly Type ICommandAdapterType = typeof(ICommandAdapter);

        private static MethodInfo _selectByIdMethod;
        private static readonly object sync_SelectByIdMethod = new object();
        private static MethodInfo SelectByIdMethod(Type genericType)
        {
            if (null == _selectByIdMethod)
            {
                lock (sync_SelectByIdMethod)
                {
                    if (null == _selectByIdMethod)
                        _selectByIdMethod = ReflectionExtensions.GetMethod(ICommandAdapterType, "SelectById", 1);
                }

            }
            return _selectByIdMethod.MakeGenericMethod(genericType);
        }
        public static object SelectByIdNonGeneric(this ICommandAdapter adapter, Type entityType, params object[] idValues)
        {
            if ((null != adapter && null != entityType) && !idValues.IsNullOrEmpty())
            {
                return SelectByIdMethod(entityType).Invoke(adapter, new object[] { idValues });
            }
            return null;
        }





        private static MethodInfo _selectSingleMethod;
        private static readonly object sync_SelectSingleMethod = new object();
        private static MethodInfo SelectSingleMethod(Type genericType)
        {
            if (null == _selectSingleMethod)
            {
                lock (sync_SelectSingleMethod)
                {
                    if (null == _selectSingleMethod)
                        _selectSingleMethod = ReflectionExtensions.GetMethod(ICommandAdapterType, "SelectSingle", 1);
                }

            }
            return _selectSingleMethod.MakeGenericMethod(genericType);
        }
        public static object SelectSingleNonGeneric(this ICommandAdapter adapter, Type entityType, SqlQuery extendedQuery)//extendedQuery null olabilir
        {
            if (null != adapter && null != entityType)
            {
                return SelectSingleMethod(entityType).Invoke(adapter, new object[] { extendedQuery });
            }
            return null;
        }




        private static MethodInfo _selectMethod;
        private static readonly object sync_SelectMethod = new object();
        private static MethodInfo SelectMethod(Type genericType)
        {
            if (null == _selectMethod)
            {
                lock (sync_SelectMethod)
                {
                    if (null == _selectMethod)
                        _selectMethod = ReflectionExtensions.GetMethod(ICommandAdapterType, "Select", 1);
                }

            }
            return _selectMethod.MakeGenericMethod(genericType);
        }
        public static IEnumerable SelectNonGeneric(this ICommandAdapter adapter, Type entityType, SqlQuery extendedQuery)
        {
            if (null != adapter && null != entityType)//extendedQuery null olabilir.
            {
                return SelectMethod(entityType).Invoke(adapter, new object[] { extendedQuery }) as IEnumerable;
            }
            return null;
        }





        private static MethodInfo _querySingleMethod;
        private static readonly object sync_QuerySingleMethod = new object();
        private static MethodInfo QuerySingleMethod(Type genericType)
        {
            if (null == _querySingleMethod)
            {
                lock (sync_QuerySingleMethod)
                {
                    if (null == _querySingleMethod)
                        _querySingleMethod = ReflectionExtensions.GetMethod(ICommandAdapterType, "QuerySingle", 1);
                }

            }
            return _querySingleMethod.MakeGenericMethod(genericType);
        }
        public static object QuerySingleNonGeneric(this ICommandAdapter adapter, Type entityType, SqlQuery query)
        {
            if ((null != adapter && null != entityType) && null != query)
            {
                return QuerySingleMethod(entityType).Invoke(adapter, new object[] { query });
            }
            return null;
        }




        private static MethodInfo _queryMethod;
        private static readonly object sync_QueryMethod = new object();
        private static MethodInfo QueryMethod(Type genericType)
        {
            if (null == _queryMethod)
            {
                lock (sync_QueryMethod)
                {
                    if (null == _queryMethod)
                        _queryMethod = ReflectionExtensions.GetMethod(ICommandAdapterType, "Query", 1);
                }

            }
            return _queryMethod.MakeGenericMethod(genericType);
        }
        public static IEnumerable QueryNonGeneric(this ICommandAdapter adapter, Type entityType, SqlQuery query)
        {
            if ((null != adapter && null != entityType) && null != query)
            {
                return QueryMethod(entityType).Invoke(adapter, new object[] { query }) as IEnumerable;
            }
            return null;
        }





        private static MethodInfo _updateMethod;
        private static readonly object sync_UpdateMethod = new object();
        private static MethodInfo UpdateMethod(Type genericType)
        {
            if (null == _updateMethod)
            {
                lock (sync_UpdateMethod)
                {
                    if (null == _updateMethod)
                        _updateMethod = ReflectionExtensions.GetMethod(ICommandAdapterType, "Update", 2);
                }

            }
            return _updateMethod.MakeGenericMethod(genericType);
        }
        public static int UpdateNonGeneric(this ICommandAdapter adapter, object entity, params string[] updatedFields)
        {
            if (null != adapter && null != entity)
            {
                Type entityType = entity.GetType();
                return (Int32)UpdateMethod(entityType).Invoke(adapter, new object[] { entity, updatedFields });
            }
            return 0;
        }





        private static MethodInfo _insertMethod;
        private static readonly object sync_InsertMethod = new object();
        private static MethodInfo InsertMethod(Type genericType)
        {
            if (null == _insertMethod)
            {
                lock (sync_InsertMethod)
                {
                    if (null == _insertMethod)
                        _insertMethod = ReflectionExtensions.GetMethod(ICommandAdapterType, "Insert", 2);
                }

            }
            return _insertMethod.MakeGenericMethod(genericType);
        }
        public static int InsertNonGeneric(this ICommandAdapter adapter, object entity, params string[] insertFields)
        {
            if (null != adapter && null != entity)
            {
                Type entityType = entity.GetType();
                return (Int32)InsertMethod(entityType).Invoke(adapter, new object[] { entity, insertFields });
            }
            return 0;
        }





        private static MethodInfo _deleteMethod;
        private static readonly object sync_DeleteMethod = new object();
        private static MethodInfo DeleteMethod(Type genericType)
        {
            if (null == _deleteMethod)
            {
                lock (sync_DeleteMethod)
                {
                    if (null == _deleteMethod)
                        _deleteMethod = ReflectionExtensions.GetMethod(ICommandAdapterType, "Delete", 1);
                }

            }
            return _deleteMethod.MakeGenericMethod(genericType);
        }
        public static int DeleteNonGeneric(this ICommandAdapter adapter, object entity)
        {
            if (null != adapter && null != entity)
            {
                Type entityType = entity.GetType();
                return (Int32)DeleteMethod(entityType).Invoke(adapter, new object[] { entity });
            }
            return 0;
        }





        private static MethodInfo _upsertMethod;
        private static readonly object sync_UpsertMethod = new object();
        private static MethodInfo UpsertMethod(Type genericType)
        {
            if (null == _upsertMethod)
            {
                lock (sync_UpsertMethod)
                {
                    if (null == _upsertMethod)
                        _upsertMethod = ReflectionExtensions.GetMethod(ICommandAdapterType, "Upsert", 3);
                }

            }
            return _upsertMethod.MakeGenericMethod(genericType);
        }
        public static int UpsertNonGeneric(this ICommandAdapter adapter, object entity, string[] updatedFields, string[] insertFields)
        {
            if (null != adapter && null != entity)
            {
                Type entityType = entity.GetType();
                return (Int32)UpsertMethod(entityType).Invoke(adapter, new object[] { entity, updatedFields, insertFields });
            }
            return 0;
        }






        private static object IsBatchValid(ICommandAdapter adapter, IEnumerable entityList)
        {
            object first = null;
            if (null != adapter && null != entityList)
            {
                IEnumerator enumerator = entityList.GetEnumerator();
                if (enumerator.MoveNext())
                    first = enumerator.Current;
            }
            return first;
        }





        private static MethodInfo _batchUpdateMethod;
        private static readonly object sync_BatchUpdateMethod = new object();
        private static MethodInfo BatchUpdateMethod(Type genericType)
        {
            if (null == _batchUpdateMethod)
            {
                lock (sync_BatchUpdateMethod)
                {
                    if (null == _batchUpdateMethod)
                        _batchUpdateMethod = ReflectionExtensions.GetMethod(ICommandAdapterType, "BatchUpdate", 2);
                }

            }
            return _batchUpdateMethod.MakeGenericMethod(genericType);
        }
        public static int BatchUpdateNonGeneric(this ICommandAdapter adapter, IEnumerable entityList, params string[] updatedFields)
        {
            object first = IsBatchValid(adapter, entityList);
            if (null != first)
            {
                Type entityType = first.GetType();
                return (Int32)BatchUpdateMethod(entityType).Invoke(adapter, new object[] { entityList, updatedFields });
            }
            return 0;
        }





        private static MethodInfo _batchInsertMethod;
        private static readonly object sync_BatchInsertMethod = new object();
        private static MethodInfo BatchInsertMethod(Type genericType)
        {
            if (null == _batchInsertMethod)
            {
                lock (sync_BatchInsertMethod)
                {
                    if (null == _batchInsertMethod)
                        _batchInsertMethod = ReflectionExtensions.GetMethod(ICommandAdapterType, "BatchInsert", 2);
                }

            }
            return _batchInsertMethod.MakeGenericMethod(genericType);
        }
        public static int BatchInsertNonGeneric(this ICommandAdapter adapter, IEnumerable entityList, params string[] insertFields)
        {
            object first = IsBatchValid(adapter, entityList);
            if (null != first)
            {
                Type entityType = first.GetType();
                return (Int32)BatchInsertMethod(entityType).Invoke(adapter, new object[] { entityList, insertFields });
            }
            return 0;
        }





        private static MethodInfo _batchUpsertMethod;
        private static readonly object sync_BatchUpsertMethod = new object();
        private static MethodInfo BatchUpsertMethod(Type genericType)
        {
            if (null == _batchUpsertMethod)
            {
                lock (sync_BatchUpsertMethod)
                {
                    if (null == _batchUpsertMethod)
                        _batchUpsertMethod = ReflectionExtensions.GetMethod(ICommandAdapterType, "BatchUpsert", 3);
                }

            }
            return _batchUpsertMethod.MakeGenericMethod(genericType);
        }
        public static int BatchUpsertNonGeneric(this ICommandAdapter adapter, IEnumerable entityList, string[] updatedFields, string[] insertFields)
        {
            object first = IsBatchValid(adapter, entityList);
            if (null != first)
            {
                Type entityType = first.GetType();
                return (Int32)BatchUpsertMethod(entityType).Invoke(adapter, new object[] { entityList, updatedFields, insertFields });
            }
            return 0;
        }





        private static MethodInfo _batchDeleteMethod;
        private static readonly object sync_BatchDeleteMethod = new object();
        private static MethodInfo BatchDeleteMethod(Type genericType)
        {
            if (null == _batchDeleteMethod)
            {
                lock (sync_BatchDeleteMethod)
                {
                    if (null == _batchDeleteMethod)
                        _batchDeleteMethod = ReflectionExtensions.GetMethod(ICommandAdapterType, "BatchDelete", 1);
                }

            }
            return _batchDeleteMethod.MakeGenericMethod(genericType);
        }
        public static int BatchDeleteNonGeneric(this ICommandAdapter adapter, IEnumerable entityList)
        {
            object first = IsBatchValid(adapter, entityList);
            if (null != first)
            {
                Type entityType = first.GetType();
                return (Int32)BatchDeleteMethod(entityType).Invoke(adapter, new object[] { entityList });
            }
            return 0;
        }
    }
}
