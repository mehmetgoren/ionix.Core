namespace Ionix.Data
{
    using Ionix.Utils.Collections;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public partial class Repository<TEntity> : IRepository<TEntity> //Repository and proxy pattern
    {
        private readonly EventHandlerList events;

        public Repository(ICommandAdapter cmd)
        {
            if (null == cmd)
                throw new ArgumentNullException(nameof(cmd));

            this.Cmd = cmd;
            this.events = new EventHandlerList();
        }

        public ICommandAdapter Cmd { get; }

        public IDbAccess DataAccess => this.Cmd.Factory.DataAccess;

        public void Dispose()
        {
            if (null != this.Cmd)
                this.Cmd.Factory.DataAccess.Dispose();
            if (null != this.events)
                this.events.Dispose();
        }

        #region   |   Select   |

        public virtual TEntity SelectById(params object[] keys)
        {
                return this.Cmd.SelectById<TEntity>(keys);
        }
        public virtual Task<TEntity> SelectByIdAsync(params object[] keys)
        {
            return this.Cmd.SelectByIdAsync<TEntity>(keys);
        }

        public virtual TEntity SelectSingle(SqlQuery extendedQuery)
        {
            return this.Cmd.SelectSingle<TEntity>(extendedQuery);
        }
        public virtual Task<TEntity> SelectSingleAsync(SqlQuery extendedQuery)
        {
            return this.Cmd.SelectSingleAsync<TEntity>(extendedQuery);
        }

        public virtual IList<TEntity> Select(SqlQuery extendedQuery)
        {
            return this.Cmd.Select<TEntity>(extendedQuery);
        }
        public virtual Task<IList<TEntity>> SelectAsync(SqlQuery extendedQuery)
        {
            return this.Cmd.SelectAsync<TEntity>(extendedQuery);
        }

        public virtual TEntity QuerySingle(SqlQuery query)
        {
            return this.Cmd.QuerySingle<TEntity>(query);
        }
        public virtual Task<TEntity> QuerySingleAsync(SqlQuery query)
        {
            return this.Cmd.QuerySingleAsync<TEntity>(query);
        }

        public virtual IList<TEntity> Query(SqlQuery query)
        {
            return this.Cmd.Query<TEntity>(query);
        }
        public virtual Task<IList<TEntity>> QueryAsync(SqlQuery query)
        {
            return this.Cmd.QueryAsync<TEntity>(query);
        }

        #endregion


        #region   |   Entity   |

        public virtual int Update(TEntity entity, params string[] updatedFields)
        {
            using (CommandScope scope = new CommandScope(this, entity, EntityCommandType.Update))
            {
                return scope.Execute(() => this.Cmd.Update(entity, updatedFields));
            }
        }
        public virtual Task<int> UpdateAsync(TEntity entity, params string[] updatedFields)
        {
            using (CommandScope scope = new CommandScope(this, entity, EntityCommandType.Update))
            {
                return scope.Execute(() => this.Cmd.UpdateAsync(entity, updatedFields));
            }
        }

        public virtual int Insert(TEntity entity, params string[] insertFields)
        {
            using (CommandScope scope = new CommandScope(this, entity, EntityCommandType.Insert))
            {
                return scope.Execute(() => this.Cmd.Insert(entity, insertFields));
            }
        }
        public virtual Task<int> InsertAsync(TEntity entity, params string[] insertFields)
        {
            using (CommandScope scope = new CommandScope(this, entity, EntityCommandType.Insert))
            {
                return scope.Execute(() => this.Cmd.InsertAsync(entity, insertFields));
            }
        }

        public virtual int Upsert(TEntity entity, string[] updatedFields, string[] insertFields)
        {
            using (CommandScope scope = new CommandScope(this, entity, EntityCommandType.Upsert))
            {
                return scope.Execute(() => this.Cmd.Upsert(entity, updatedFields, insertFields));
            }
        }
        public virtual Task<int> UpsertAsync(TEntity entity, string[] updatedFields, string[] insertFields)
        {
            using (CommandScope scope = new CommandScope(this, entity, EntityCommandType.Upsert))
            {
                return scope.Execute(() => this.Cmd.UpsertAsync(entity, updatedFields, insertFields));
            }
        }

        public virtual int Delete(TEntity entity)
        {
            using (CommandScope scope = new CommandScope(this, entity, EntityCommandType.Delete))
            {
                return scope.Execute(() => this.Cmd.Delete(entity));
            }
        }
        public virtual Task<int> DeleteAsync(TEntity entity)
        {
            using (CommandScope scope = new CommandScope(this, entity, EntityCommandType.Delete))
            {
                return scope.Execute(() => this.Cmd.DeleteAsync(entity));
            }
        }

        #endregion


        #region   |   Batch

        public virtual int BatchUpdate(IEnumerable<TEntity> entityList, params string[] updatedFields)
        {
            using (CommandScope scope = new CommandScope(this, entityList, EntityCommandType.Update))
            {
                return scope.Execute(() => this.Cmd.BatchUpdate(entityList, updatedFields));
            }
        }
        public virtual Task<int> BatchUpdateAsync(IEnumerable<TEntity> entityList, params string[] updatedFields)
        {
            using (CommandScope scope = new CommandScope(this, entityList, EntityCommandType.Update))
            {
                return scope.Execute(() => this.Cmd.BatchUpdateAsync(entityList, updatedFields));
            }
        }


        public virtual int BatchInsert(IEnumerable<TEntity> entityList, params string[] insertFields)
        {
            using (CommandScope scope = new CommandScope(this, entityList, EntityCommandType.Insert))
            {
                return scope.Execute(() => this.Cmd.BatchInsert(entityList, insertFields));
            }
        }
        public virtual Task<int> BatchInsertAsync(IEnumerable<TEntity> entityList, params string[] insertFields)
        {
            using (CommandScope scope = new CommandScope(this, entityList, EntityCommandType.Insert))
            {
                return scope.Execute(() => this.Cmd.BatchInsertAsync(entityList, insertFields));
            }
        }


        public virtual int BatchUpsert(IEnumerable<TEntity> entityList, string[] updatedFields,
            string[] insertFields)
        {
            using (CommandScope scope = new CommandScope(this, entityList, EntityCommandType.Upsert))
            {
                return scope.Execute(() => this.Cmd.BatchUpsert(entityList, updatedFields, insertFields));
            }
        }
        public virtual Task<int> BatchUpsertAsync(IEnumerable<TEntity> entityList, string[] updatedFields,
            string[] insertFields)
        {
            using (CommandScope scope = new CommandScope(this, entityList, EntityCommandType.Upsert))
            {
                return scope.Execute(() => this.Cmd.BatchUpsertAsync(entityList, updatedFields, insertFields));
            }
        }


        public virtual int BatchDelete(IEnumerable<TEntity> entityList)
        {
            using (CommandScope scope = new CommandScope(this, entityList, EntityCommandType.Delete))
            {
                return scope.Execute(() => this.Cmd.BatchDelete(entityList));
            }
        }
        public virtual Task<int> BatchDeleteAsync(IEnumerable<TEntity> entityList)
        {
            using (CommandScope scope = new CommandScope(this, entityList, EntityCommandType.Delete))
            {
                return scope.Execute(() => this.Cmd.BatchDeleteAsync(entityList));
            }
        }

        #endregion
    }
}
