namespace Ionix.Data.Mongo.Migration
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System;

	public abstract class Migration
	{
		public MigrationVersion Version { get; protected set; }
		public string Description { get; set; }

        //public string Script { get; set; }

        protected Migration(MigrationVersion version)
		{
			Version = version;
		}

		public IMongoDatabase Database { get; set; }

        public virtual string Script { get; }

        public abstract void Update();
	}
}