namespace ionix.Migration
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

        public abstract SqlQuery GenerateQuery();

        public abstract void Sync(ICommandAdapter cmd);
    }
}
