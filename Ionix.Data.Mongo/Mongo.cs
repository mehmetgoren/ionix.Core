namespace Ionix.Data.Mongo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using System.Reflection;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using Ionix.Utils.Extensions;
    using Ionix.Utils.Reflection;
    using Ionix.Data.Mongo.Serializers;

    public sealed class Mongo
    {
        private readonly IMongoDatabase db;
        private readonly IMongoClient client;

        private static readonly UpdateOptions ReplaceOrInsertOneOptions = new UpdateOptions { IsUpsert = true };
        public Mongo(IMongoDatabase db)
        {
            if (null == db)
                throw new ArgumentNullException(nameof(db));

            this.db = db;
            this.client = db.Client;
        }


        public IMongoCollection<TEntity> Get<TEntity>()
        {
            return MongoAdmin.GetCollection<TEntity>(this.db);
        }

        #region |   Select   |

        public long Count<TEntity>()
        {
            var table = this.Get<TEntity>();
            return table.CountDocuments(new BsonDocument());
        }

        public Task<long> CountAsync<TEntity>()
        {
            var table = this.Get<TEntity>();
            return table.CountDocumentsAsync(new BsonDocument());
        }

        public IMongoQueryable<TEntity> AsQueryable<TEntity>()
        {
            var table = this.Get<TEntity>();
            return table.AsQueryable();
        }

        private static FilterDefinition<TEntity> GetIdFilterDefination<TEntity>(object id)
        {
            return Builders<TEntity>.Filter.Eq("_id", id);
        }

        public TEntity GetById<TEntity>(object id)
        {
            return this.Get<TEntity>().Find(GetIdFilterDefination<TEntity>(id)).FirstOrDefault();
        }

        public async Task<TEntity> GetByIdAsync<TEntity>(object id)
        {
            var result = await this.Get<TEntity>().FindAsync(GetIdFilterDefination<TEntity>(id));
            return result.FirstOrDefault();
        }

        #endregion

        #region |   Insert     |

        public void InsertOne<TEntity>(TEntity entity, InsertOneOptions options = null)
        {
            if (null != entity)
                this.Get<TEntity>().InsertOne(entity, options);
        }

        public Task InsertOneAsync<TEntity>(TEntity entity, InsertOneOptions options = null)
        {
            if (null != entity)
                return this.Get<TEntity>().InsertOneAsync(entity, options);

            return Task.Delay(0);
        }

        public void InsertMany<TEntity>(IEnumerable<TEntity> entityList, InsertManyOptions options = null)
        {
            if (!entityList.IsNullOrEmpty())
                this.Get<TEntity>().InsertMany(entityList, options);
        }

        public Task InsertManyAsync<TEntity>(IEnumerable<TEntity> entityList, InsertManyOptions options = null)
        {
            if (!entityList.IsNullOrEmpty())
                return this.Get<TEntity>().InsertManyAsync(entityList, options);

            return Task.Delay(0);
        }

        #endregion

        #region |   Replace     |


        private ReplaceOneResult ReplaceOneInternal<TEntity>(FilterDefinition<TEntity> filterDefination
            , TEntity entity, UpdateOptions options)
        {
            //FilterDefinition<TEntity> filter = "";
            if (null != filterDefination && null != entity)
                return this.Get<TEntity>().ReplaceOne(filterDefination, entity, options);

            return null;
        }

        public ReplaceOneResult ReplaceOne<TEntity>(Expression<Func<TEntity, bool>> predicate
            , TEntity entity, UpdateOptions options = null)
        {
            return this.ReplaceOneInternal(predicate, entity, options);
        }
        //1
        public ReplaceOneResult ReplaceOrInsertOne<TEntity>(Expression<Func<TEntity, bool>> predicate
         , TEntity entity)
        {
            return this.ReplaceOne(predicate, entity, ReplaceOrInsertOneOptions);
        }

        //id değeri var ise bunu kullan.
        public ReplaceOneResult ReplaceOne<TEntity>(TEntity entity, UpdateOptions options = null)
        {
            var idPi = MongoExtensions.GetIdProperty<TEntity>(true);
            return this.ReplaceOneInternal(GetIdFilterDefination<TEntity>(idPi.GetValue(entity)), entity, options);
        }

        //2
        public ReplaceOneResult ReplaceOrInsertOne<TEntity>(TEntity entity)
        {
            return this.ReplaceOne(entity, ReplaceOrInsertOneOptions);
        }

        public ReplaceOneResult ReplaceOne<TEntity>(TEntity entity, UpdateOptions options, params Expression<Func<TEntity, object>>[] filterFields)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (null == filterFields || 0 == filterFields.Length)
                throw new ArgumentException($"{nameof(filterFields)} should not be null or empty");

            var properties = filterFields.Select(ReflectionExtensions.GetPropertyInfo).ToList();
            List<WriteModel<TEntity>> requests = new List<WriteModel<TEntity>>();

            var dic = new Dictionary<string, object>();
            foreach (PropertyInfo pi in properties)
            {
                string fieldName = DictionarySerializer.GetFieldName(pi);
                dic[fieldName] = pi.GetValue(entity);
            }
            BsonDocument bd = new BsonDocument(dic);

            FilterDefinition<TEntity> fd = bd;

            return this.ReplaceOneInternal<TEntity>(fd, entity, options);
        }

        //3
        public ReplaceOneResult ReplaceOrInsertOne<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] filterFields)
        {
            return this.ReplaceOne(entity, ReplaceOrInsertOneOptions, filterFields);
        }

        private Task<ReplaceOneResult> ReplaceOneInternalAsync<TEntity>(FilterDefinition<TEntity> filterDefination
            , TEntity entity, UpdateOptions options)
        {
            //FilterDefinition<TEntity> filter = "";
            if (null != filterDefination && null != entity)
                return this.Get<TEntity>().ReplaceOneAsync(filterDefination, entity, options);

            return Task.FromResult<ReplaceOneResult>(null);
        }

        public Task<ReplaceOneResult> ReplaceOneAsync<TEntity>(Expression<Func<TEntity, bool>> predicate
            , TEntity entity, UpdateOptions options = null)
        {
            return this.ReplaceOneInternalAsync(predicate, entity, options);
        }
        //4
        public Task<ReplaceOneResult> ReplaceOrInsertOneAsync<TEntity>(Expression<Func<TEntity, bool>> predicate
            , TEntity entity)
        {
            return this.ReplaceOneAsync(predicate, entity, ReplaceOrInsertOneOptions);
        }

        //id değeri var ise bunu kullan.
        public Task<ReplaceOneResult> ReplaceOneAsync<TEntity>(TEntity entity, UpdateOptions options = null)
        {
            var idPi = MongoExtensions.GetIdProperty<TEntity>(true);
            return this.ReplaceOneInternalAsync(GetIdFilterDefination<TEntity>(idPi.GetValue(entity)), entity, options);
        }

        //5
        public Task<ReplaceOneResult> ReplaceOrInsertOneAsync<TEntity>(TEntity entity)
        {
            return this.ReplaceOneAsync(entity, ReplaceOrInsertOneOptions);
        }

        #endregion

        #region |   Update     |
        //Örneğin şöyle genişlet; Id prop u reflection ile bul ve predicate' e gerek kalmasın.
        public UpdateResult UpdateOne<TEntity>(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            var set = Builders<TEntity>.Update;
            var def = sets(set);

            return this.Get<TEntity>().UpdateOne(filter, def, options);
        }

        public UpdateResult UpdateOne<TEntity>(object id,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            var filter = GetIdFilterDefination<TEntity>(id);
            var set = Builders<TEntity>.Update;
            var def = sets(set);

            return this.Get<TEntity>().UpdateOne(filter, def, options);
        }

        public UpdateResult UpdateOne<TEntity>(TEntity entity, UpdateOptions options
            , params Expression<Func<TEntity, object>>[] fields)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));
            if (null == fields || 0 == fields.Length)
                throw new ArgumentNullException(nameof(fields));

            var idPi = MongoExtensions.GetIdProperty<TEntity>(true);
            object idValue = idPi.GetValue(entity);

            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets = (builder) =>
            {
                Dictionary<string, object> dic = new Dictionary<string, object>(fields.Length);
                foreach (var exp in fields)
                {
                    var pi = ReflectionExtensions.GetPropertyInfo(exp);
                    if (null != pi)
                    {
                        string fieldName = DictionarySerializer.GetFieldName(pi);
                        dic[fieldName] = pi.GetValue(entity);
                    }
                }

                var bd = new BsonDocument("$set", new BsonDocument(dic));
                return bd;
            };

            return this.UpdateOne(idValue, sets, options);
        }



        public Task<UpdateResult> UpdateOneAsync<TEntity>(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            var set = Builders<TEntity>.Update;
            var def = sets(set);

            return this.Get<TEntity>().UpdateOneAsync(filter, def, options);
        }

        public Task<UpdateResult> UpdateOneAsync<TEntity>(object id,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            var filter = GetIdFilterDefination<TEntity>(id);
            var set = Builders<TEntity>.Update;
            var def = sets(set);

            return this.Get<TEntity>().UpdateOneAsync(filter, def, options);
        }


        public UpdateResult UpdateMany<TEntity>(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            var set = Builders<TEntity>.Update;
            var def = sets(set);

            return this.Get<TEntity>().UpdateMany(filter, def, options);
        }

        public Task<UpdateResult> UpdateManyAsync<TEntity>(Expression<Func<TEntity, bool>> filter,
            Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> sets,
            UpdateOptions options = null)
        {
            var set = Builders<TEntity>.Update;
            var def = sets(set);

            return this.Get<TEntity>().UpdateManyAsync(filter, def, options);
        }
        #endregion

        #region |   Delete     |

        public DeleteResult DeleteOne<TEntity>(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            var coll = this.Get<TEntity>();
            return coll.DeleteOne<TEntity>(filter, options);
        }

        public DeleteResult DeleteOne<TEntity>(object id, DeleteOptions options = null)
        {
            var coll = this.Get<TEntity>();
            return coll.DeleteOne(GetIdFilterDefination<TEntity>(id), options);
        }

        public Task<DeleteResult> DeleteOneAsync<TEntity>(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            var coll = this.Get<TEntity>();
            return coll.DeleteOneAsync<TEntity>(filter, options);
        }
        public Task<DeleteResult> DeleteOneAsync<TEntity>(object id, DeleteOptions options = null)
        {
            var coll = this.Get<TEntity>();
            return coll.DeleteOneAsync(GetIdFilterDefination<TEntity>(id), options);
        }


        //Örneğin şöyle genişlet; Id prop u reflection ile bul ve predicate' e gerek kalmasın.
        public DeleteResult DeleteMany<TEntity>(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            var coll = this.Get<TEntity>();
            return coll.DeleteMany<TEntity>(filter, options);
        }

        public Task<DeleteResult> DeleteManyAsync<TEntity>(Expression<Func<TEntity, bool>> filter, DeleteOptions options = null)
        {
            var coll = this.Get<TEntity>();
            return coll.DeleteManyAsync<TEntity>(filter, options);
        }
        #endregion


        #region   |   BulkReplace  |


        private Task<BulkWriteResult<BsonDocument>> BulkReplaceAsync<TEntity>(IEnumerable<TEntity> list, IEnumerable<PropertyInfo> properties, bool isUpsert, BulkWriteOptions options = null)
        {
            List<ReplaceOneModel<BsonDocument>> requests = new List<ReplaceOneModel<BsonDocument>>();
            foreach (var entity in list)
            {
                var dic = new Dictionary<string, object>();
                foreach (PropertyInfo pi in properties)
                {
                    var mongoFieldName = DictionarySerializer.GetFieldName(pi);
                    dic[mongoFieldName] = pi.GetValue(entity);
                }
                BsonDocument filter = new BsonDocument(dic);

                var _mng = entity.ToBsonDocument();
               // _mng.Remove("_id");

                requests.Add(new ReplaceOneModel<BsonDocument>(filter, _mng) { IsUpsert = isUpsert });
            }

            var coll = MongoAdmin.GetCollection(this.db, typeof(TEntity));

            return coll.BulkWriteAsync(requests, options);
        }

        public Task<BulkWriteResult<BsonDocument>> BulkReplaceAsync<TEntity>(IEnumerable<TEntity> list, bool isUpsert, BulkWriteOptions options = null, params Expression<Func<TEntity, object>>[] filterFields)
        {
            if (filterFields.IsNullOrEmpty())
                throw new ArgumentException(nameof(filterFields) + " can not be null or empty.");

            var properties = filterFields.Select(ReflectionExtensions.GetPropertyInfo).ToList();

            return BulkReplaceAsync(list, properties, isUpsert, options);
        }

        public Task<BulkWriteResult<BsonDocument>> BulkReplaceAsync<TEntity>(IEnumerable<TEntity> list, bool isUpsert,
            BulkWriteOptions options = null)
        {
            return BulkReplaceAsync(list, new[] { DictionarySerializer.GetBsonIdProperty<TEntity>() }, isUpsert, options);
        }

        #endregion


        #region   |   BulkUpdate  |

        //Filter id den alacak.
        public Task<BulkWriteResult<TEntity>> BulkUpdateAsync<TEntity>(IEnumerable<TEntity> list,
            BulkWriteOptions options, params Expression<Func<TEntity, object>>[] updateFields)
        {
            var idPi = typeof(TEntity).GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(p => p.GetCustomAttribute<BsonIdAttribute>() != null);
            if (null == idPi)
                throw new InvalidOperationException($"{typeof(TEntity).Name} has no BsonId field");

            var properties = updateFields.Select(ReflectionExtensions.GetPropertyInfo).ToList();

            List<WriteModel<TEntity>> requests = new List<WriteModel<TEntity>>();
            foreach (var entity in list)
            {
                var dic = new Dictionary<string, object>();
                dic["_id"] = idPi.GetValue(entity);

                BsonDocument filterBd = new BsonDocument(dic);

                FilterDefinition<TEntity> fd = filterBd;

                dic.Clear();
                foreach (var pi in properties)
                {
                    string fieldName = DictionarySerializer.GetFieldName(pi);
                    dic[fieldName] = pi.GetValue(entity);//bson Element e göre çalışmıyor....
                }

                var update = new BsonDocument("$set", new BsonDocument(dic));

                requests.Add(new UpdateOneModel<TEntity>(fd, update));
            }
            return this.Get<TEntity>().BulkWriteAsync(requests, options);
        }

        #endregion


        #region   |   BulkDelete   |

        public Task<BulkWriteResult<TEntity>> BulkDeleteAsync<TEntity>(IEnumerable<TEntity> list, BulkWriteOptions options, params Expression<Func<TEntity, object>>[] filterFields)
        {
            if (filterFields.IsNullOrEmpty())
                throw new ArgumentException(nameof(filterFields) + " can not be null or empty.");

            var properties = filterFields.Select(ReflectionExtensions.GetPropertyInfo).ToList();
            List<WriteModel<TEntity>> requests = new List<WriteModel<TEntity>>();
            foreach (var entity in list)
            {
                var dic = new Dictionary<string, object>();
                foreach (PropertyInfo pi in properties)
                {
                    string fieldName = DictionarySerializer.GetFieldName(pi);
                    dic[fieldName] = pi.GetValue(entity);
                }
                BsonDocument bd = new BsonDocument(dic);

                FilterDefinition<TEntity> fd = bd;

                requests.Add(new DeleteOneModel<TEntity>(fd));
            }
            return this.Get<TEntity>().BulkWriteAsync(requests, options);
        }

        public Task<BulkWriteResult<TEntity>> BulkDeleteAsync<TEntity>(IEnumerable<TEntity> list,
            BulkWriteOptions options)
        {
            var idPi = typeof(TEntity).GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(p => p.GetCustomAttribute<BsonIdAttribute>() != null);

            List<WriteModel<TEntity>> requests = new List<WriteModel<TEntity>>();
            foreach (var entity in list)
            {
                var dic = new Dictionary<string, object>();
                dic["_id"] = idPi.GetValue(entity);
                BsonDocument bd = new BsonDocument(dic);

                FilterDefinition<TEntity> fd = bd;

                requests.Add(new DeleteOneModel<TEntity>(fd));
            }
            return this.Get<TEntity>().BulkWriteAsync(requests, options);
        }

        #endregion


        #region   |   Search   |

        public IEnumerable<TEntity> TextSearch<TEntity>(string text)
        {
            return this.Get<TEntity>().Find(Builders<TEntity>.Filter.Text(text)).ToList();
        }

        public async Task<IEnumerable<TEntity>> TextSearchAsync<TEntity>(string text)
        {
            var result = await this.Get<TEntity>().FindAsync(Builders<TEntity>.Filter.Text(text));
            return await result.ToListAsync();
        }

        #endregion
    }
}
