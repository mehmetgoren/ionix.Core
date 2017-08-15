namespace ionix.MongoTests
{
    using System;
    using MongoDB.Driver;
    using ionix.Data.Mongo;

    public class DbContext
    {
        internal const string MongoAddress = "mongodb://localhost:27017";//"mongodb://172.19.3.171:46000"

        public const string DatabaseName = "TestDb";


        private static bool _initialedGlobally;

        public static void InitialGlobal()
        {
            MongoClientProxy.SetConnectionString(MongoAddress);
            MongoHelper.InitializeMongo(new Migration100().GetMigrationsAssembly(),
                MongoAddress, DbContext.DatabaseName);


            _initialedGlobally = MongoHelper.InitializeMongo(new Migration100().GetMigrationsAssembly(),
                MongoAddress, DatabaseName);

        }


        public IMongoClient MongoClient { get;}

        #region constructors


        public DbContext(IMongoClient client)
        {
            if (null == client)
                throw new ArgumentNullException(nameof(client));

            this.MongoClient = client;

            if (!_initialedGlobally)
            {
                throw new ArgumentException($"{nameof(DbContext)} must be initialized globally.");
            }

            this._person = this.GetLazy<Person>();
            this._address = this.GetLazy<Address>();
            this._personAddress = this.GetLazy<PersonAddress>();

        }

        public DbContext(string connectionString)
            : this(new MongoClient(connectionString))
        {
        }

        public DbContext()
            : this(MongoClientProxy.Instance)
        {
        }
        #endregion

        private Lazy<MongoRepository<TEntity>> GetLazy<TEntity>()
        {
            return new Lazy<MongoRepository<TEntity>>(() => new MongoRepository<TEntity>(this.MongoClient), true);
        }

        private readonly Lazy<MongoRepository<Person>> _person;
        public MongoRepository<Person> Person => _person.Value;


        private readonly Lazy<MongoRepository<Address>> _address;
        public MongoRepository<Address> Adddress => _address.Value;


        private readonly Lazy<MongoRepository<PersonAddress>> _personAddress;
        public MongoRepository<PersonAddress> PersonAddress => _personAddress.Value;

    }
}
