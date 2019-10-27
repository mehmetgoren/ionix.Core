namespace Ionix.Migration
{
    using Data;

    public abstract class Migration
    {
        public MigrationVersion Version { get; protected set; }

        public virtual string Description => this.Version.ToString();

        protected Migration(MigrationVersion version)
        {
            Version = version;
        }

        /// <summary>
        /// Defines if a migration script created automatically (i.e. via reflection) or not.
        /// </summary>
        public abstract bool IsBuiltIn { get; }

        public abstract SqlQuery GenerateQuery();

        public virtual void Sync(ICommandAdapter cmd)
        {
            SqlQuery query = this.GenerateQuery();
            if (null == query || query.IsEmpty())
            {
                throw new MigrationException($"{this.GetType()}.{nameof(GenerateQuery)} shouldn't returns null or empty query.");
            }

            cmd.Factory.DataAccess.ExecuteNonQuery(this.GenerateQuery());
        }
    }
}
