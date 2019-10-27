namespace Ionix.MongoTests
{
    using System;

    public abstract class MigrationBaseAutoGen : MigrationBase
    {
        protected MigrationBaseAutoGen(string versionNo)
            : base(versionNo)
        {
        }

        public sealed override string GenerateMigrationScript()
        {
            this.AlsoCheckMigrationVersion = true;
            return GetDefaultGenerateMigrationScript(this);
        }
    }
}
