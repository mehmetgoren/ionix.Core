namespace Ionix.Data.Mongo.Migration
{
	using System.Linq;
    using System.Reflection;

    public class ExcludeExperimentalMigrations : MigrationFilter
	{
		public override bool Exclude(Migration migration)
		{
			if (migration == null)
			{
				return false;
			}
			return migration.GetType()
                .GetTypeInfo()
				.GetCustomAttributes(true)
				.OfType<ExperimentalAttribute>()
				.Any();
		}
	}
}