namespace Ionix.MongoTests
{
    using System;
    using MongoDB.Driver;
    using Ionix.Data.Mongo;

    public class DbContext
    {
        public const string DatabaseName = "TestDb";

        public IMongoDatabase Database { get; }
        public IMongoClient MongoClient { get;}

        #region constructors


        public DbContext(IMongoDatabase db)
        {
            if (null == db)
                throw new ArgumentNullException(nameof(db));

            this.Database = db;
            this.MongoClient = db.Client;

            this._person = this.GetLazy<Person>();
            this._address = this.GetLazy<Address>();
            this._personAddress = this.GetLazy<PersonAddress>();

        }

        public DbContext(string connectionString)
            : this(MongoAdmin.GetDatabase(new MongoClient(connectionString), DatabaseName))
        {
        }

        public DbContext()
            : this(MongoTests.MongoAddress)
        {
        }
        #endregion

        private Lazy<MongoRepository<TEntity>> GetLazy<TEntity>()
        {
            return new Lazy<MongoRepository<TEntity>>(() => new MongoRepository<TEntity>(this.Database), true);
        }

        private readonly Lazy<MongoRepository<Person>> _person;
        public MongoRepository<Person> Person => _person.Value;


        private readonly Lazy<MongoRepository<Address>> _address;
        public MongoRepository<Address> Adddress => _address.Value;


        private readonly Lazy<MongoRepository<PersonAddress>> _personAddress;
        public MongoRepository<PersonAddress> PersonAddress => _personAddress.Value;

    }
}
