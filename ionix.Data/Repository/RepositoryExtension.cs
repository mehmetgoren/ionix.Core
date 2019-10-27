namespace Ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    //async ler eklenecek.
    public static class RepositoryExtension
    {
        //Select
        public static IList<TEntity> Select<TEntity>(this IRepository<TEntity> repository)
        {
            if (null != repository)
                return repository.Select(null);
            return new List<TEntity>();
        }

        public static Task<IList<TEntity>> SelectAsync<TEntity>(this IRepository<TEntity> repository)
        {
            if (null != repository)
                return repository.SelectAsync(null);

            return Task.FromResult<IList<TEntity>>(null);
        }

        //

        //Entity
        public static int Update<TEntity>(this IRepository<TEntity> repository, TEntity entity, params Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != repository)
            {
                string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);// already checked null value.
                return repository.Update(entity, arr);
            }
            return 0;
        }
        public static Task<int> UpdateAsync<TEntity>(this IRepository<TEntity> repository, TEntity entity, params Expression<Func<TEntity, object>>[] updatedFields)
        {
            if (null != repository)
            {
                string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);// already checked null value.
                return repository.UpdateAsync(entity, arr);
            }
            return Task.FromResult(0);
        }
        public static int Update<TEntity>(this IRepository<TEntity> repository, TEntity entity)
        {
            return RepositoryExtension.Update(repository, entity, null);
        }
        public static Task<int> UpdateAsync<TEntity>(this IRepository<TEntity> repository, TEntity entity)
        {
            return RepositoryExtension.UpdateAsync(repository, entity, null);
        }




        public static int Insert<TEntity>(this IRepository<TEntity> repository, TEntity entity, params Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != repository)
            {
                string[] arr = CommandAdapterExtensions.ToStringArray(insertFields);
                return repository.Insert(entity, arr);
            }
            return 0;
        }
        public static Task<int> InsertAsync<TEntity>(this IRepository<TEntity> repository, TEntity entity, params Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != repository)
            {
                string[] arr = CommandAdapterExtensions.ToStringArray(insertFields);
                return repository.InsertAsync(entity, arr);
            }
            return Task.FromResult(0);
        }
        public static int Insert<TEntity>(this IRepository<TEntity> repository, TEntity entity)
        {
            return RepositoryExtension.Insert(repository, entity, null);
        }
        public static Task<int> InsertAsync<TEntity>(this IRepository<TEntity> repository, TEntity entity)
        {
            return RepositoryExtension.InsertAsync(repository, entity, null);
        }



        public static int Upsert<TEntity>(this IRepository<TEntity> repository, TEntity entity, params string[] updatedFields)
        {
            if (null != repository)
            {
                return repository.Upsert(entity, updatedFields, null);
            }
            return 0;
        }
        public static Task<int> UpsertAsync<TEntity>(this IRepository<TEntity> repository, TEntity entity, params string[] updatedFields)
        {
            if (null != repository)
            {
                return repository.UpsertAsync(entity, updatedFields, null);
            }
            return Task.FromResult(0);
        }
        public static int Upsert<TEntity>(this IRepository<TEntity> repository, TEntity entity)
        {
            return RepositoryExtension.Upsert(repository, entity, (string[])null);
        }
        public static Task<int> UpsertAsync<TEntity>(this IRepository<TEntity> repository, TEntity entity)
        {
            return RepositoryExtension.UpsertAsync(repository, entity, (string[])null);
        }

        public static int Upsert<TEntity>(this IRepository<TEntity> repository, TEntity entity, params Expression<Func<TEntity, object>>[] updatedFields)
        {
            return RepositoryExtension.Upsert(repository, entity, updatedFields, (Expression<Func<TEntity, object>>[])null);
        }
        public static Task<int> UpsertAsync<TEntity>(this IRepository<TEntity> repository, TEntity entity, params Expression<Func<TEntity, object>>[] updatedFields)
        {
            return RepositoryExtension.UpsertAsync(repository, entity, updatedFields, (Expression<Func<TEntity, object>>[])null);
        }
        public static int Upsert<TEntity>(this IRepository<TEntity> repository, TEntity entity, Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != repository)
            {
                string[] updateArr = CommandAdapterExtensions.ToStringArray(updatedFields);
                string[] insertArr = CommandAdapterExtensions.ToStringArray(insertFields);

                return repository.Upsert(entity, updateArr, insertArr);
            }
            return 0;
        }
        public static Task<int> UpsertAsync<TEntity>(this IRepository<TEntity> repository, TEntity entity, Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
        {
            if (null != repository)
            {
                string[] updateArr = CommandAdapterExtensions.ToStringArray(updatedFields);
                string[] insertArr = CommandAdapterExtensions.ToStringArray(insertFields);

                return repository.UpsertAsync(entity, updateArr, insertArr);
            }
            return Task.FromResult(0);
        }
        //


        //Batch
        private static int BatchUpdate<TEntity>(IRepository<TEntity> repository, IEnumerable<TEntity> entityList, string[] updatedFields)
        {
            if (null != repository)
            {
                return repository.BatchUpdate(entityList, updatedFields);
            }
            return 0;
        }
        private static Task<int> BatchUpdateAsync<TEntity>(IRepository<TEntity> repository, IEnumerable<TEntity> entityList, string[] updatedFields)
        {
            if (null != repository)
            {
                return repository.BatchUpdateAsync(entityList, updatedFields);
            }
            return Task.FromResult(0);
        }
        public static int BatchUpdate<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList)
        {
            return RepositoryExtension.BatchUpdate(repository, entityList, (string[])null);
        }
        public static Task<int> BatchUpdateAsync<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList)
        {
            return RepositoryExtension.BatchUpdateAsync(repository, entityList, (string[])null);
        }
        public static int BatchUpdate<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList,
            params Expression<Func<TEntity, object>>[] updatedFields)
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);
            return RepositoryExtension.BatchUpdate(repository, entityList, arr);
        }
        public static Task<int> BatchUpdateAsync<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList,
             params Expression<Func<TEntity, object>>[] updatedFields)
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(updatedFields);
            return RepositoryExtension.BatchUpdateAsync(repository, entityList, arr);
        }


        private static int BatchInsert<TEntity>(IRepository<TEntity> repository, IEnumerable<TEntity> entityList, string[] insertFields)
        {
            if (null != repository)
            {
                return repository.BatchInsert(entityList, insertFields);
            }
            return 0;
        }
        private static Task<int> BatchInsertAsync<TEntity>(IRepository<TEntity> repository, IEnumerable<TEntity> entityList, string[] insertFields)
        {
            if (null != repository)
            {
                return repository.BatchInsertAsync(entityList, insertFields);
            }
            return Task.FromResult(0);
        }
        public static int BatchInsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList)
        {
            return RepositoryExtension.BatchInsert(repository, entityList, (string[])null);
        }
        public static Task<int> BatchInsertAsync<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList)
        {
            return RepositoryExtension.BatchInsertAsync(repository, entityList, (string[])null);
        }
        public static int BatchInsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, params Expression<Func<TEntity, object>>[] insertFields)
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(insertFields);
            return RepositoryExtension.BatchInsert(repository, entityList, arr);
        }
        public static Task<int> BatchInsertAsync<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, params Expression<Func<TEntity, object>>[] insertFields)
        {
            string[] arr = CommandAdapterExtensions.ToStringArray(insertFields);
            return RepositoryExtension.BatchInsertAsync(repository, entityList, arr);
        }





        private static int BatchUpsert<TEntity>(IRepository<TEntity> repository, IEnumerable<TEntity> entityList, string[] updatedFields, string[] insertFields)
        {
            if (null != repository)
            {
                return repository.BatchUpsert(entityList, updatedFields, insertFields);
            }
            return 0;
        }
        private static Task<int> BatchUpsertAsync<TEntity>(IRepository<TEntity> repository, IEnumerable<TEntity> entityList, string[] updatedFields, string[] insertFields)
        {
            if (null != repository)
            {
                return repository.BatchUpsertAsync(entityList, updatedFields, insertFields);
            }
            return Task.FromResult(0);
        }
        public static int BatchUpsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, string[] updatedFields)
        {
            return RepositoryExtension.BatchUpsert(repository, entityList, updatedFields, (string[])null);
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, string[] updatedFields)
        {
            return RepositoryExtension.BatchUpsertAsync(repository, entityList, updatedFields, (string[])null);
        }
        public static int BatchUpsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList)
        {
            return RepositoryExtension.BatchUpsert(repository, entityList, (string[])null, (string[])null);
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList)
        {
            return RepositoryExtension.BatchUpsertAsync(repository, entityList, (string[])null, (string[])null);
        }
        public static int BatchUpsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
        {
            string[] updateArr = CommandAdapterExtensions.ToStringArray(updatedFields);
            string[] insertArr = CommandAdapterExtensions.ToStringArray(insertFields);
            return RepositoryExtension.BatchUpsert(repository, entityList, updateArr, insertArr);
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, Expression<Func<TEntity, object>>[] updatedFields, Expression<Func<TEntity, object>>[] insertFields)
        {
            string[] updateArr = CommandAdapterExtensions.ToStringArray(updatedFields);
            string[] insertArr = CommandAdapterExtensions.ToStringArray(insertFields);
            return RepositoryExtension.BatchUpsertAsync(repository, entityList, updateArr, insertArr);
        }
        public static int BatchUpsert<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, Expression<Func<TEntity, object>>[] updatedFields)
        {
            return RepositoryExtension.BatchUpsert(repository, entityList, updatedFields, (Expression<Func<TEntity, object>>[])null);
        }
        public static Task<int> BatchUpsertAsync<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entityList, Expression<Func<TEntity, object>>[] updatedFields)
        {
            return RepositoryExtension.BatchUpsertAsync(repository, entityList, updatedFields, (Expression<Func<TEntity, object>>[])null);
        }
        //
    }
}
