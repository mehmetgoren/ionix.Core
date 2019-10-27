namespace ionix.Migration
{
    public abstract class MigrationManuel : Migration
    {
        protected MigrationManuel(MigrationVersion version)
            : base(version) { }

        public sealed override bool IsBuiltIn => false;
    }
}
