namespace ionix.Data.Mongo
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using ionix.Utils.Reflection;

    public static class MongoAdmin
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

        public static void DropDatabase(IMongoClient client, string name)
        {
            EnsureClient(client);
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            client.DropDatabase(name);
        }

        public static async void DropDatabaseAsync(IMongoClient client, string name)
        {
            EnsureClient(client);
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            await client.DropDatabaseAsync(name);
        }

        public static IMongoDatabase GetDatabase(IMongoClient client, string name)
        {
            EnsureClient(client);
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            return client.GetDatabase(name);
        }

        public static IMongoCollection<TEntity> GetCollection<TEntity>(IMongoClient client)
        {
            var info = MongoExtensions.GetNames(typeof(TEntity));
            var db = GetDatabase(client, info.Database);
            var table = db.GetCollection<TEntity>(info.Name);
            return table;
        }
        public static IMongoCollection<BsonDocument> GetCollection(IMongoClient client, Type modelType)
        {
            var info = MongoExtensions.GetNames(modelType);
            var db = MongoAdmin.GetDatabase(client, info.Database);
            return db.GetCollection<BsonDocument>(info.Name);
        }

        private static string CreateIndexTemplate<TEntity>(IMongoClient client, CreateIndexOptions<TEntity> options, Expression<Func<TEntity, object>>[] exps
            , Func<FieldDefinition<TEntity>, IndexKeysDefinition<TEntity>> func)
        {
            if (null == exps || 0 == exps.Length)
                throw new ArgumentNullException(nameof(exps));

            var props = new PropertyInfo[exps.Length];
            for (int j = 0; j < exps.Length; ++j)
                props[j] = ReflectionExtensions.GetPropertyInfo(exps[j]);

            var table = GetCollection<TEntity>(client);

            if (props.Length == 1)
            {
                var prop = props[0];
                FieldDefinition<TEntity> field = prop.Name;
                var keys = func(field);// Builders<TEntity>.IndexKeys.Ascending(prop.Name);

                return table.Indexes.CreateOne(keys, options);
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

                return table.Indexes.CreateOne(Builders<TEntity>.IndexKeys.Combine(indexes.ToArray()), options);
            }
        }

        //İleride Text Index i de ekle.
        public static string CreateIndex<TEntity>(IMongoClient client, CreateIndexOptions<TEntity> options, params Expression<Func<TEntity, object>>[] exps)
        {
            return CreateIndexTemplate(client, options, exps, (field) => Builders<TEntity>.IndexKeys.Ascending(field));
        }

        public static string CreateIndex<TEntity>(IMongoClient client, params Expression<Func<TEntity, object>>[] exps)
        {
            return CreateIndex(client, null, exps);
        }

        public static string CreateTextIndex<TEntity>(IMongoClient client, CreateIndexOptions<TEntity> options, params Expression<Func<TEntity, object>>[] exps)
        {
            return CreateIndexTemplate(client, options, exps, (field) => Builders<TEntity>.IndexKeys.Text(field));
        }

        public static string CreateTextIndex<TEntity>(IMongoClient client, params Expression<Func<TEntity, object>>[] exps)
        {
            return CreateTextIndex(client, null, exps);
        }
    }
}
