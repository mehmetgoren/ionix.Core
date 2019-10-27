namespace Ionix.Data
{
    using Utils.Collections;
    using System.Linq;
    using System;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    //it s a proxy.
    public partial class CachedRepository<TEntity> : Repository<TEntity>
    {
        private readonly bool throwExceptionOnNonCachedOperation;
        private readonly Expression<Func<TEntity, object>>[] keys;

        public CachedRepository(ICommandAdapter cmd, bool throwExceptionOnNonCachedOperation, params Expression<Func<TEntity, object>>[] keys)
            : base(cmd)
        {
            if (!keys.Any())
                throw new ArgumentNullException(nameof(keys));

            this.keys = keys;
            this.throwExceptionOnNonCachedOperation = throwExceptionOnNonCachedOperation;
        }


        private readonly object syncRoot = new object();
        private volatile IndexedEntityList<TEntity> list;
        private IndexedEntityList<TEntity> List
        {
            get
            {
                if (null == this.list)
                {
                    lock (this.syncRoot)
                    {
                        if (null == this.list)
                        {
                            var allRecords = this.Cmd.Select<TEntity>();
                            this.list = IndexedEntityList<TEntity>.CreateConcurrent(this.keys);
                            this.list.AddRange(allRecords);
                        }
                    }
                }
                return this.list;
            }
        }

        public void Refresh()
        {
            this.list = null;
        }

        public override TEntity SelectById(params object[] idValues)
        {
            return this.List.Find(idValues);
        }
        public override Task<TEntity> SelectByIdAsync(params object[] keys)
        {
            return Task.FromResult(this.List.Find(keys));
        }

        public override TEntity SelectSingle(SqlQuery extendedQuery)
        {
            if (this.throwExceptionOnNonCachedOperation)
                throw new NotSupportedException();

            return base.SelectSingle(extendedQuery);
        }
        public override Task<TEntity> SelectSingleAsync(SqlQuery extendedQuery)
        {
            if (this.throwExceptionOnNonCachedOperation)
                throw new NotSupportedException();

            return base.SelectSingleAsync(extendedQuery);
        }


        public override IList<TEntity> Select(SqlQuery extendedQuery)
        {
            if (null != extendedQuery && this.throwExceptionOnNonCachedOperation)
                throw new NotSupportedException();

            return this.List.ToList();
        }
        public override Task<IList<TEntity>> SelectAsync(SqlQuery extendedQuery)
        {
            if (null != extendedQuery && this.throwExceptionOnNonCachedOperation)
                throw new NotSupportedException();

            return Task.FromResult<IList<TEntity>>(this.List.ToList());
        }

        public override TEntity QuerySingle(SqlQuery query)
        {
            if (this.throwExceptionOnNonCachedOperation)
                throw new NotSupportedException();

            return base.QuerySingle(query);
        }
        public override Task<TEntity> QuerySingleAsync(SqlQuery query)
        {
            if (this.throwExceptionOnNonCachedOperation)
                throw new NotSupportedException();

            return base.QuerySingleAsync(query);
        }

        public override IList<TEntity> Query(SqlQuery query)
        {
            if (this.throwExceptionOnNonCachedOperation)
                throw new NotSupportedException();

            return base.Query(query);
        }
        public override Task<IList<TEntity>> QueryAsync(SqlQuery query)
        {
            if (this.throwExceptionOnNonCachedOperation)
                throw new NotSupportedException();

            return base.QueryAsync(query);
        }

        public override int Update(TEntity entity, params string[] updatedFields)
        {
            int ret = base.Update(entity, updatedFields);
            if (ret > 0)
            {
                this.List.Replace(entity);
            }
            return ret;
        }

        public override async Task<int> UpdateAsync(TEntity entity, params string[] updatedFields)
        {
            int ret = await base.UpdateAsync(entity, updatedFields);
            if (ret > 0)
            {
                this.List.Replace(entity);
            }
            return ret;
        }

        public override int Insert(TEntity entity, params string[] insertFields)
        {
            int ret = base.Insert(entity, insertFields);
            if (ret > 0)
            {
                this.List.Add(entity);
            }
            return ret;
        }
        public override async Task<int> InsertAsync(TEntity entity, params string[] insertFields)
        {
            int ret = await base.InsertAsync(entity, insertFields);
            if (ret > 0)
            {
                this.List.Add(entity);
            }
            return ret;
        }

        public override int Upsert(TEntity entity, string[] updatedFields, string[] insertFields)
        {
            int ret = base.Upsert(entity, updatedFields, insertFields);
            if (ret > 0)
            {
                this.List.Add(entity); //Add zaten Dictionary Indexer' ı Kullanıyor.
            }
            return ret;
        }
        public override async Task<int> UpsertAsync(TEntity entity, string[] updatedFields, string[] insertFields)
        {
            int ret = await base.UpsertAsync(entity, updatedFields, insertFields);
            if (ret > 0)
            {
                this.List.Add(entity); //Add zaten Dictionary Indexer' ı Kullanıyor.
            }
            return ret;
        }

        public override int Delete(TEntity entity)
        {
            int ret = base.Delete(entity);
            if (ret > 0)
            {
                this.List.Remove(entity);
            }
            return ret;
        }
        public override async Task<int> DeleteAsync(TEntity entity)
        {
            int ret = await base.DeleteAsync(entity);
            if (ret > 0)
            {
                this.List.Remove(entity);
            }
            return ret;
        }

        public override int BatchUpdate(IEnumerable<TEntity> entityList, params string[] updatedFields)
        {
            int ret = base.BatchUpdate(entityList, updatedFields);
            if (ret > 0)
            {
                foreach (var entity in entityList)
                {
                    this.List.Replace(entity);
                }
            }
            return ret;
        }
        public override async Task<int> BatchUpdateAsync(IEnumerable<TEntity> entityList, params string[] updatedFields)
        {
            int ret = await base.BatchUpdateAsync(entityList, updatedFields);
            if (ret > 0)
            {
                foreach (var entity in entityList)
                {
                    this.List.Replace(entity);
                }
            }
            return ret;
        }

        public override int BatchInsert(IEnumerable<TEntity> entityList, params string[] insertFields)
        {
            int ret = base.BatchInsert(entityList, insertFields);
            if (ret > 0)
            {
                foreach (var entity in entityList)
                {
                    this.List.Add(entity);
                }
            }
            return ret;
        }
        public override async Task<int> BatchInsertAsync(IEnumerable<TEntity> entityList, params string[] insertFields)
        {
            int ret = await base.BatchInsertAsync(entityList, insertFields);
            if (ret > 0)
            {
                foreach (var entity in entityList)
                {
                    this.List.Add(entity);
                }
            }
            return ret;
        }

        public override int BatchUpsert(IEnumerable<TEntity> entityList, string[] updatedFields, string[] insertFieldss)
        {
            int ret = base.BatchUpsert(entityList, updatedFields, insertFieldss);
            if (ret > 0)
            {
                foreach (var entity in entityList)
                {
                    this.List.Add(entity);
                }
            }
            return ret;
        }
        public override async Task<int> BatchUpsertAsync(IEnumerable<TEntity> entityList, string[] updatedFields, string[] insertFieldss)
        {
            int ret = await base.BatchUpsertAsync(entityList, updatedFields, insertFieldss);
            if (ret > 0)
            {
                foreach (var entity in entityList)
                {
                    this.List.Add(entity);
                }
            }
            return ret;
        }

        public override int BatchDelete(IEnumerable<TEntity> entityList)
        {
            int ret = base.BatchDelete(entityList);
            if (ret == entityList.Count())
            {
                foreach (var entity in entityList)
                {
                    this.List.Remove(entity);
                }
            }
            else
            {
                this.Refresh();
            }
            return ret;
        }

        public override async Task<int> BatchDeleteAsync(IEnumerable<TEntity> entityList)
        {
            int ret = await base.BatchDeleteAsync(entityList);
            if (ret == entityList.Count())
            {
                foreach (var entity in entityList)
                {
                    this.List.Remove(entity);
                }
            }
            else
            {
                this.Refresh();
            }
            return ret;
        }
    }
}
