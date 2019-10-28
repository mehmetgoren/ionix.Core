namespace Ionix.Data
{
    using Utils;
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class CommandAdapterBase : ICommandAdapter
    {
        private readonly ICommandFactory factory;

        protected CommandAdapterBase(ICommandFactory factory)
        {
            if (null == factory)
                throw new ArgumentNullException(nameof(factory));

            this.factory = factory;
        }

        public ICommandFactory Factory => this.factory;

        public TypeConversionMode ConversionMode { get; set; } = TypeConversionMode.ConvertSafely;

        protected abstract IEntityMetaDataProvider CreateProvider();

        private IEntityMetaDataProvider entityMetaDataProvider;
        public IEntityMetaDataProvider EntityMetaDataProvider
        {
            get
            {
                if (null == this.entityMetaDataProvider)
                    this.entityMetaDataProvider = this.CreateProvider();
                return this.entityMetaDataProvider;
            }
        }

        private IEntityCommandSelect CreateSelectCommand()
        {
            IEntityCommandSelect cmd = this.factory.CreateSelectCommand();
            cmd.ConversionMode = this.ConversionMode;
            return cmd;
        }

        #region   |      Select     |

        public virtual TEntity SelectById<TEntity>(params object[] idValues)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.SelectById<TEntity>(this.EntityMetaDataProvider, idValues);
        }
        public virtual Task<TEntity> SelectByIdAsync<TEntity>(params object[] idValues)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.SelectByIdAsync<TEntity>(this.EntityMetaDataProvider, idValues);
        }


        public virtual TEntity SelectSingle<TEntity>(SqlQuery extendedQuery)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.SelectSingle<TEntity>(this.EntityMetaDataProvider, extendedQuery);
        }
        public virtual Task<TEntity> SelectSingleAsync<TEntity>(SqlQuery extendedQuery)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.SelectSingleAsync<TEntity>(this.EntityMetaDataProvider, extendedQuery);
        }


        public virtual IList<TEntity> Select<TEntity>(SqlQuery extendedQuery)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.Select<TEntity>(this.EntityMetaDataProvider, extendedQuery);
        }
        public virtual Task<IList<TEntity>> SelectAsync<TEntity>(SqlQuery extendedQuery)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.SelectAsync<TEntity>(this.EntityMetaDataProvider, extendedQuery);
        }



        #endregion


        #region   |      Query     |

        public virtual TEntity QuerySingle<TEntity>(SqlQuery query)
        {
            Type elementType = typeof(TEntity);
            if (ReflectionExtensions.IsPrimitiveType(elementType))
            {
                if (elementType == CachedTypes.ObjectType || elementType == CachedTypes.ExpandoObjectType)
                {
                    object ret = this.factory.DataAccess.QuerySingle(query);
                    return (TEntity)ret;
                }
                return this.factory.DataAccess.ExecuteScalar<TEntity>(query);
            }
            else
            {
                var cmd = this.CreateSelectCommand();
                return cmd.QuerySingle<TEntity>(this.EntityMetaDataProvider, query);
            }
        }
        public virtual async Task<TEntity> QuerySingleAsync<TEntity>(SqlQuery query)
        {
            Type elementType = typeof(TEntity);
            if (ReflectionExtensions.IsPrimitiveType(elementType))
            {
                if (elementType == CachedTypes.ObjectType || elementType == CachedTypes.ExpandoObjectType)
                {
                    object ret = await this.factory.DataAccess.QuerySingleAsync(query);
                    return (TEntity)ret;
                }
                return await this.factory.DataAccess.ExecuteScalarAsync<TEntity>(query);
            }
            else
            {
                var cmd = this.CreateSelectCommand();
                return await cmd.QuerySingleAsync<TEntity>(this.EntityMetaDataProvider, query);
            }
        }
        public (TEntity1, TEntity2) QuerySingle<TEntity1, TEntity2>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QuerySingle<TEntity1, TEntity2>(this.EntityMetaDataProvider, query);
        }
        public Task<(TEntity1, TEntity2)> QuerySingleAsync<TEntity1, TEntity2>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QuerySingleAsync<TEntity1, TEntity2>(this.EntityMetaDataProvider, query);
        }

        public (TEntity1, TEntity2, TEntity3) QuerySingle<TEntity1, TEntity2, TEntity3>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QuerySingle<TEntity1, TEntity2, TEntity3>(this.EntityMetaDataProvider, query);
        }
        public Task<(TEntity1, TEntity2, TEntity3)> QuerySingleAsync<TEntity1, TEntity2, TEntity3>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QuerySingleAsync<TEntity1, TEntity2, TEntity3>(this.EntityMetaDataProvider, query);
        }

        public (TEntity1, TEntity2, TEntity3, TEntity4) QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4>(this.EntityMetaDataProvider, query);
        }
        public Task<(TEntity1, TEntity2, TEntity3, TEntity4)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4>(this.EntityMetaDataProvider, query);
        }

        public (TEntity1, TEntity2, TEntity3, TEntity4, TEntity5) QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(this.EntityMetaDataProvider, query);
        }
        public Task<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(this.EntityMetaDataProvider, query);
        }

        public (TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6) QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(this.EntityMetaDataProvider, query);
        }
        public Task<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(this.EntityMetaDataProvider, query);
        }

        public (TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7) QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(this.EntityMetaDataProvider, query);
        }
        public Task<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(this.EntityMetaDataProvider, query);
        }


        public virtual IList<TEntity> Query<TEntity>(SqlQuery query)
        {
            Type elementType = typeof(TEntity);
            if (ReflectionExtensions.IsPrimitiveType(elementType))
            {
                if (elementType == CachedTypes.ObjectType || elementType == CachedTypes.ExpandoObjectType)
                {
                    var list = this.factory.DataAccess.Query(query);
                    List<TEntity> ret = new List<TEntity>(list.Count);
                    foreach (object item in list)
                    {
                        ret.Add((TEntity)item);//TEntity is object here.
                    }
                    return ret;
                }

                return this.factory.DataAccess.ExecuteScalarList<TEntity>(query);
            }
            else
            {
                var cmd = this.CreateSelectCommand();
                return cmd.Query<TEntity>(this.EntityMetaDataProvider, query);
            }
        }
        public virtual async Task<IList<TEntity>> QueryAsync<TEntity>(SqlQuery query)
        {
            Type elementType = typeof(TEntity);
            if (ReflectionExtensions.IsPrimitiveType(elementType))
            {
                if (elementType == CachedTypes.ObjectType || elementType == CachedTypes.ExpandoObjectType)
                {
                    var list = await this.factory.DataAccess.QueryAsync(query);
                    List<TEntity> ret = new List<TEntity>(list.Count);
                    foreach (object item in list)
                    {
                        ret.Add((TEntity)item);//TEntity is object here.
                    }
                    return ret;
                }

                return await this.factory.DataAccess.ExecuteScalarListAsync<TEntity>(query);
            }
            else
            {
                var cmd = this.CreateSelectCommand();
                return await cmd.QueryAsync<TEntity>(this.EntityMetaDataProvider, query);
            }
        }

        public IList<(TEntity1, TEntity2)> Query<TEntity1, TEntity2>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.Query<TEntity1, TEntity2>(this.EntityMetaDataProvider, query);
        }
        public Task<IList<(TEntity1, TEntity2)>> QueryAsync<TEntity1, TEntity2>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QueryAsync<TEntity1, TEntity2>(this.EntityMetaDataProvider, query);
        }

        public IList<(TEntity1, TEntity2, TEntity3)> Query<TEntity1, TEntity2, TEntity3>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.Query<TEntity1, TEntity2, TEntity3>(this.EntityMetaDataProvider, query);
        }
        public Task<IList<(TEntity1, TEntity2, TEntity3)>> QueryAsync<TEntity1, TEntity2, TEntity3>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QueryAsync<TEntity1, TEntity2, TEntity3>(this.EntityMetaDataProvider, query);
        }

        public IList<(TEntity1, TEntity2, TEntity3, TEntity4)> Query<TEntity1, TEntity2, TEntity3, TEntity4>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.Query<TEntity1, TEntity2, TEntity3, TEntity4>(this.EntityMetaDataProvider, query);
        }
        public Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4>(this.EntityMetaDataProvider, query);
        }

        public IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5)> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(this.EntityMetaDataProvider, query);
        }
        public Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(this.EntityMetaDataProvider, query);
        }

        public IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6)> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(this.EntityMetaDataProvider, query);
        }
        public Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(this.EntityMetaDataProvider, query);
        }

        public IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7)> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(this.EntityMetaDataProvider, query);
        }
        public Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(SqlQuery query)
        {
            var cmd = this.CreateSelectCommand();
            return cmd.QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(this.EntityMetaDataProvider, query);
        }

        #endregion

        #region   |      Entity     |

        private IEntityCommandUpdate CreateEntityCommandUpdate(string[] updatedFields)
        {
            IEntityCommandUpdate cmd = (IEntityCommandUpdate)this.factory.CreateEntityCommand(EntityCommandType.Update);
            if (!updatedFields.IsNullOrEmpty())
                cmd.UpdatedFields = new HashSet<string>(updatedFields);

            return cmd;
        }
        public virtual int Update<TEntity>(TEntity entity, params string[] updatedFields)
        {
            IEntityCommandUpdate cmd = this.CreateEntityCommandUpdate(updatedFields);

            return cmd.Update(entity, this.EntityMetaDataProvider);
        }
        public virtual Task<int> UpdateAsync<TEntity>(TEntity entity, params string[] updatedFields)
        {
            IEntityCommandUpdate cmd = this.CreateEntityCommandUpdate(updatedFields);

            return cmd.UpdateAsync(entity, this.EntityMetaDataProvider);
        }


        private IEntityCommandInsert CreateEntityCommandInsert(string[] insertFields)
        {
            IEntityCommandInsert cmd = (IEntityCommandInsert)this.factory.CreateEntityCommand(EntityCommandType.Insert);
            if (!insertFields.IsNullOrEmpty())
                cmd.InsertFields = new HashSet<string>(insertFields);

            return cmd;
        }
        public virtual int Insert<TEntity>(TEntity entity, params string[] insertFields)
        {
            IEntityCommandInsert cmd = this.CreateEntityCommandInsert(insertFields);

            return cmd.Insert(entity, this.EntityMetaDataProvider);
        }
        public virtual Task<int> InsertAsync<TEntity>(TEntity entity, params string[] insertFields)
        {
            IEntityCommandInsert cmd = this.CreateEntityCommandInsert(insertFields);

            return cmd.InsertAsync(entity, this.EntityMetaDataProvider);
        }

        private IEntityCommandUpsert CreateEntityCommandUpsert(string[] updatedFields, string[] insertFields)
        {
            IEntityCommandUpsert cmd = (IEntityCommandUpsert)this.factory.CreateEntityCommand(EntityCommandType.Upsert);
            if (!updatedFields.IsNullOrEmpty())
                cmd.UpdatedFields = new HashSet<string>(updatedFields);
            if (!insertFields.IsNullOrEmpty())
                cmd.InsertFields = new HashSet<string>(insertFields);

            return cmd;
        }
        public virtual int Upsert<TEntity>(TEntity entity, string[] updatedFields, string[] insertFields)
        {
            IEntityCommandUpsert cmd = this.CreateEntityCommandUpsert(updatedFields, insertFields);

            return cmd.Upsert(entity, this.EntityMetaDataProvider);
        }
        public virtual Task<int> UpsertAsync<TEntity>(TEntity entity, string[] updatedFields, string[] insertFields)
        {
            IEntityCommandUpsert cmd = this.CreateEntityCommandUpsert(updatedFields, insertFields);

            return cmd.UpsertAsync(entity, this.EntityMetaDataProvider);
        }



        public virtual int Delete<TEntity>(TEntity entity)
        {
            IEntityCommandDelete cmd = (IEntityCommandDelete)this.factory.CreateEntityCommand(EntityCommandType.Delete);
            return cmd.Delete(entity, this.EntityMetaDataProvider);
        }
        public virtual Task<int> DeleteAsync<TEntity>(TEntity entity)
        {
            IEntityCommandDelete cmd = (IEntityCommandDelete)this.factory.CreateEntityCommand(EntityCommandType.Delete);
            return cmd.DeleteAsync(entity, this.EntityMetaDataProvider);
        }

        #endregion


        #region   |     Batch     |

        private IBatchCommandUpdate CreateBatchCommandUpdate(string[] updatedFields)
        {
            IBatchCommandUpdate cmd = (IBatchCommandUpdate)this.factory.CreateBatchCommand(EntityCommandType.Update);
            if (!updatedFields.IsNullOrEmpty())
                cmd.UpdatedFields = new HashSet<string>(updatedFields);

            return cmd;
        }
        public virtual int BatchUpdate<TEntity>(IEnumerable<TEntity> entityList, params string[] updatedFields)
        {
            IBatchCommandUpdate cmd = this.CreateBatchCommandUpdate(updatedFields);
            return cmd.Update(entityList, this.EntityMetaDataProvider);
        }
        public virtual Task<int> BatchUpdateAsync<TEntity>(IEnumerable<TEntity> entityList, params string[] updatedFields)
        {
            IBatchCommandUpdate cmd = this.CreateBatchCommandUpdate(updatedFields);
            return cmd.UpdateAsync(entityList, this.EntityMetaDataProvider);
        }



        private IBatchCommandInsert CreateBatchCommandInsert(string[] insertFields)
        {
            IBatchCommandInsert cmd = (IBatchCommandInsert)this.factory.CreateBatchCommand(EntityCommandType.Insert);
            if (!insertFields.IsNullOrEmpty())
                cmd.InsertFields = new HashSet<string>(insertFields);

            return cmd;
        }
        public virtual int BatchInsert<TEntity>(IEnumerable<TEntity> entityList, params string[] insertFields)
        {
            IBatchCommandInsert cmd = this.CreateBatchCommandInsert(insertFields);
            return cmd.Insert(entityList, this.EntityMetaDataProvider);
        }
        public virtual Task<int> BatchInsertAsync<TEntity>(IEnumerable<TEntity> entityList, params string[] insertFields)
        {
            IBatchCommandInsert cmd = this.CreateBatchCommandInsert(insertFields);
            return cmd.InsertAsync(entityList, this.EntityMetaDataProvider);
        }



        private IBatchCommandUpsert CreateBatchCommandUpsert(string[] updatedFields, string[] insertFields)
        {
            IBatchCommandUpsert cmd = (IBatchCommandUpsert)this.factory.CreateBatchCommand(EntityCommandType.Upsert);
            if (!updatedFields.IsNullOrEmpty())
                cmd.UpdatedFields = new HashSet<string>(updatedFields);
            if (!insertFields.IsNullOrEmpty())
                cmd.InsertFields = new HashSet<string>(insertFields);

            return cmd;
        }
        public virtual int BatchUpsert<TEntity>(IEnumerable<TEntity> entityList, string[] updatedFields, string[] insertFields)
        {
            IBatchCommandUpsert cmd = this.CreateBatchCommandUpsert(updatedFields, insertFields);
            return cmd.Upsert(entityList, this.EntityMetaDataProvider);
        }
        public virtual Task<int> BatchUpsertAsync<TEntity>(IEnumerable<TEntity> entityList, string[] updatedFields, string[] insertFields)
        {
            IBatchCommandUpsert cmd = this.CreateBatchCommandUpsert(updatedFields, insertFields);
            return cmd.UpsertAsync(entityList, this.EntityMetaDataProvider);
        }




        public virtual int BatchDelete<TEntity>(IEnumerable<TEntity> entityList)
        {
            IBatchCommandDelete cmd = (IBatchCommandDelete)this.factory.CreateBatchCommand(EntityCommandType.Delete);
            return cmd.Delete(entityList, this.EntityMetaDataProvider);
        }
        public virtual Task<int> BatchDeleteAsync<TEntity>(IEnumerable<TEntity> entityList)
        {
            IBatchCommandDelete cmd = (IBatchCommandDelete)this.factory.CreateBatchCommand(EntityCommandType.Delete);
            return cmd.DeleteAsync(entityList, this.EntityMetaDataProvider);
        }
        #endregion


        #region   |   Bulk Copy   |

        public virtual void BulkCopy<TEntity>(IEnumerable<TEntity> entityList)
        {
            IBulkCopyCommand cmd = this.factory.CreateBulkCopyCommand();
            cmd.Execute(entityList, this.EntityMetaDataProvider);
        }
        public virtual Task BulkCopyAsync<TEntity>(IEnumerable<TEntity> entityList)
        {
            IBulkCopyCommand cmd = this.factory.CreateBulkCopyCommand();
            return cmd.ExecuteAsync(entityList, this.EntityMetaDataProvider);
        }

        #endregion
    }
}
