namespace Ionix.Data.Mongo
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using Ionix.Utils.Extensions;

    public static partial class MongoAdmin
    {
        private static string ConvertToEvalScript(string script)
        {
            return "{ eval: \"" + script + "\"}";
        }

        public static TEntity ExecuteScript<TEntity>(IMongoDatabase db, string script)
        {
            var command = new JsonCommand<TEntity>(ConvertToEvalScript(script));

            return db.RunCommand(command);
        }
        public static Task<TEntity> ExecuteScriptAsync<TEntity>(IMongoDatabase db, string script)
        {
            var command = new JsonCommand<TEntity>(ConvertToEvalScript(script));

            return db.RunCommandAsync(command);
        }

        public static BsonDocument ExecuteScript(IMongoDatabase db, string script)
        {
            return ExecuteScript<BsonDocument>(db, script);
        }

        public static Task<BsonDocument> ExecuteScriptAsync(IMongoDatabase db, string script)
        {
            return ExecuteScriptAsync<BsonDocument>(db, script);
        }

        private static void EnsureClient(IMongoClient client)
        {
            if (null == client)
                throw new ArgumentNullException(nameof(client));
        }
        private static void EnsureDatabase(IMongoDatabase db)
        {
            if (null == db)
                throw new ArgumentNullException(nameof(db));
        }

        public static IEnumerable<Database> GetDatabaseList(IMongoClient client)
        {
            EnsureClient(client);
            List<Database> ret = new List<Database>();
            using (var cursor = client.ListDatabases())
            {
                foreach (var document in cursor.ToEnumerable())
                {
                    ret.Add(document.ToString().BsonDeserialize<Database>());
                }
            }
            return ret;
        }

        public static void DropDatabase(IMongoDatabase db)
        {
            EnsureDatabase(db);

            db.Client.DropDatabase(db.DatabaseNamespace.DatabaseName);
        }

        public static async void DropDatabaseAsync(IMongoDatabase db)
        {
            EnsureDatabase(db);

            await db.Client.DropDatabaseAsync(db.DatabaseNamespace.DatabaseName);
        }

        public static IMongoDatabase GetDatabase(IMongoClient client, string name)
        {
            EnsureClient(client);
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            return client.GetDatabase(name);
        }

        public static IMongoCollection<TEntity> GetCollection<TEntity>(IMongoDatabase db)
        {
            EnsureDatabase(db);

            var info = MongoExtensions.GetCollectionInfo(typeof(TEntity));
            var table = db.GetCollection<TEntity>(info.Name);
            return table;
        }
        public static IMongoCollection<BsonDocument> GetCollection(IMongoDatabase db, Type modelType)
        {
            EnsureDatabase(db);

            if (null == modelType)
                throw new ArgumentNullException(nameof(modelType));

            var info = MongoExtensions.GetCollectionInfo(modelType);
            return db.GetCollection<BsonDocument>(info.Name);
        }

        private static string CreateIndexTemplate<TEntity>(IMongoDatabase db, CreateIndexOptions<TEntity> options, Expression<Func<TEntity, object>>[] exps
            , Func<FieldDefinition<TEntity>, IndexKeysDefinition<TEntity>> func)
        {
            if (null == exps || 0 == exps.Length)
                throw new ArgumentNullException(nameof(exps));

            var props = new PropertyInfo[exps.Length];
            for (int j = 0; j < exps.Length; ++j)
                props[j] = ReflectionExtensions.GetPropertyInfo(exps[j]);

            var table = GetCollection<TEntity>(db);

            CreateIndexModel<TEntity> cim;
            if (props.Length == 1)
            {
                var prop = props[0];
                FieldDefinition<TEntity> field = prop.Name;
                IndexKeysDefinition<TEntity> keys = func(field);// Builders<TEntity>.IndexKeys.Ascending(prop.Name);

                cim = new CreateIndexModel<TEntity>(keys, options);
            }
            else
            {
                List<IndexKeysDefinition<TEntity>> indexes = new List<IndexKeysDefinition<TEntity>>();
                foreach (var prop in props)
                {
                    FieldDefinition<TEntity> field = prop.Name;
                    IndexKeysDefinition<TEntity> index = func(field);// Builders<TEntity>.IndexKeys.Ascending(field);
                    indexes.Add(index);
                }

                 cim = new CreateIndexModel<TEntity>(Builders<TEntity>.IndexKeys.Combine(indexes.ToArray()), options);
            }

            return table.Indexes.CreateOne(cim);
        }

        //İleride Text Index i de ekle.
        public static string CreateIndex<TEntity>(IMongoDatabase db, CreateIndexOptions<TEntity> options, params Expression<Func<TEntity, object>>[] exps)
        {
            return CreateIndexTemplate(db, options, exps, (field) => Builders<TEntity>.IndexKeys.Ascending(field));
        }

        public static string CreateIndex<TEntity>(IMongoDatabase db, params Expression<Func<TEntity, object>>[] exps)
        {
            return CreateIndex(db, null, exps);
        }

        public static string CreateTextIndex<TEntity>(IMongoDatabase db, CreateIndexOptions<TEntity> options, params Expression<Func<TEntity, object>>[] exps)
        {
            return CreateIndexTemplate(db, options, exps, (field) => Builders<TEntity>.IndexKeys.Text(field));
        }

        public static string CreateTextIndex<TEntity>(IMongoDatabase db, params Expression<Func<TEntity, object>>[] exps)
        {
            return CreateTextIndex(db, null, exps);
        }
    }
}
