namespace ionix.Data.Mongo.Migration
{
	public abstract class MigrationFilter
	{
		public abstract bool Exclude(Migration migration);
	}
}