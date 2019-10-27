namespace Ionix.Data.Mongo
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Core.Clusters;
    using Utils;

    /// <summary>
    /// The MongoClient instance actually represents a pool of connections to the database; you will only need one instance of class MongoClient even with multiple threads.
    /// Typically you only create one MongoClient instance for a given cluster and use it across your application.Creating multiple MongoClients will,
    /// however, still share the same pool of connections if and only if the connection strings are identical.
    /// </summary>
    public sealed class MongoClientProxy : Singleton, IMongoClient
    {
        private static string ConnectionString = "mongodb://localhost:27017";
        public static void SetConnectionString(string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            ConnectionString = value;

            Concrete = new MongoClient(ConnectionString);
        }

        private static IMongoClient Concrete = new MongoClient(ConnectionString);

        public static readonly MongoClientProxy Instance = new MongoClientProxy();

        private MongoClientProxy()
        {

        }

        public void DropDatabase(string name, CancellationToken cancellationToken = new CancellationToken())
        {
            Concrete.DropDatabase(name, cancellationToken);
        }

        public Task DropDatabaseAsync(string name, CancellationToken cancellationToken = new CancellationToken())
        {
            return Concrete.DropDatabaseAsync(name, cancellationToken);
        }

        public IMongoDatabase GetDatabase(string name, MongoDatabaseSettings settings = null)
        {
            return Concrete.GetDatabase(name, settings);
        }

        public IAsyncCursor<BsonDocument> ListDatabases(CancellationToken cancellationToken = new CancellationToken())
        {
            return Concrete.ListDatabases(cancellationToken);
        }

        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return Concrete.ListDatabasesAsync(cancellationToken);
        }

        public IMongoClient WithReadConcern(ReadConcern readConcern)
        {
            return Concrete.WithReadConcern(readConcern);
        }

        public IMongoClient WithReadPreference(ReadPreference readPreference)
        {
            return Concrete.WithReadPreference(readPreference);
        }

        public IMongoClient WithWriteConcern(WriteConcern writeConcern)
        {
            return Concrete.WithWriteConcern(writeConcern);
        }

        public void DropDatabase(IClientSessionHandle session, string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            Concrete.DropDatabase(session, name, cancellationToken);
        }

        public Task DropDatabaseAsync(IClientSessionHandle session, string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Concrete.DropDatabaseAsync(session, name, cancellationToken);
        }

        public IAsyncCursor<BsonDocument> ListDatabases(IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Concrete.ListDatabases(session, cancellationToken);
        }

        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Concrete.ListDatabasesAsync(session, cancellationToken);
        }

        public IClientSessionHandle StartSession(ClientSessionOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Concrete.StartSession(options, cancellationToken);
        }

        public Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Concrete.StartSessionAsync(options, cancellationToken);
        }

        public ICluster Cluster { get; } = Concrete.Cluster;
        public MongoClientSettings Settings { get; } = Concrete.Settings;



        //MongoDB Client Entended.
        public IAsyncCursor<string> ListDatabaseNames(CancellationToken cancellationToken = default(CancellationToken)) => Concrete.ListDatabaseNames(cancellationToken);

        public IAsyncCursor<string> ListDatabaseNames(IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken)) => Concrete.ListDatabaseNames(session, cancellationToken);

        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(CancellationToken cancellationToken = default(CancellationToken)) => Concrete.ListDatabaseNamesAsync(cancellationToken);

        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken)) => Concrete.ListDatabaseNamesAsync(session,cancellationToken);

        public IAsyncCursor<BsonDocument> ListDatabases(ListDatabasesOptions options, CancellationToken cancellationToken = default(CancellationToken)) => Concrete.ListDatabases(options,cancellationToken);

        public IAsyncCursor<BsonDocument> ListDatabases(IClientSessionHandle session, ListDatabasesOptions options, CancellationToken cancellationToken = default(CancellationToken)) => Concrete.ListDatabases(session, options, cancellationToken);

        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(ListDatabasesOptions options, CancellationToken cancellationToken = default(CancellationToken)) => Concrete.ListDatabasesAsync(options, cancellationToken);

        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(IClientSessionHandle session, ListDatabasesOptions options, CancellationToken cancellationToken = default(CancellationToken)) => Concrete.ListDatabasesAsync(session,options,cancellationToken);

        public IChangeStreamCursor<TResult> Watch<TResult>(PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        => Concrete.Watch(pipeline, options,cancellationToken);

        public IChangeStreamCursor<TResult> Watch<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        => Concrete.Watch(session, pipeline,options,cancellationToken);

        public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        => Concrete.WatchAsync(pipeline,options,cancellationToken);

        public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        => Concrete.WatchAsync(session, pipeline, options, cancellationToken);
    }
}
