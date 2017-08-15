namespace ionix.Data.Mongo.Migration
{
	using System;
	using MongoDB.Bson.Serialization.Attributes;

	public class AppliedMigration
	{
		public AppliedMigration()
		{
		}

		public AppliedMigration(Migration migration)
		{
			Version = migration.Version;
			StartedOn = DateTime.Now;
			Description = migration.Description;
            Script = migration.Script;
		}

		[BsonId]
		public MigrationVersion Version { get; set; }
		public string Description { get; set; }
		public DateTime StartedOn { get; set; }
		public DateTime? CompletedOn { get; set; }

        public string Script { get; set; }
        public string Exception { get; set; }

        public override string ToString()
		{
			return Version.ToString() + " started on " + StartedOn + " completed on " + CompletedOn;
		}
	}
}