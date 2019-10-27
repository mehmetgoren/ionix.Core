namespace Ionix.Migration
{
    using System;
    using System.Reflection;
    using Data;

    public abstract class MigrationInitializerBase
    {
        public bool CheckTransactionalDbAccess { get; set; }

        private readonly Action backUp;
        protected MigrationInitializerBase(Action backUp)
        {
            this.backUp = backUp;
            this.CheckTransactionalDbAccess = true;
        }
        protected MigrationInitializerBase()
            : this(null) { }

        private static bool _isRegistered;
        protected abstract void RegisterMigrationServices();

        public virtual bool Execute(Assembly asm, ICommandAdapter cmd, bool throwIfNotLatestVersion)
        {
            if (!_isRegistered)
            {
                this.RegisterMigrationServices();
                _isRegistered = true;
            }

            if (null != asm && null != cmd)
            {
                if (this.CheckTransactionalDbAccess)
                {
                    ITransactionalDbAccess dbAccess = cmd.Factory.DataAccess as ITransactionalDbAccess;
                    if (null == dbAccess)
                        throw new InvalidOperationException("please use transactional IDbaccess object.");
                }

                if (null != this.backUp)
                    this.backUp();

                var runner = new MigrationRunner(cmd);

                runner.MigrationReflection.LookForMigrationsInAssembly(asm);

                runner.DatabaseStatus.ValidateMigrationsVersions();

                if (throwIfNotLatestVersion)
                    runner.DatabaseStatus.ThrowIfNotLatestVersion();//?

                runner.UpdateToLatest();
                return true;
            }

            return false;
        }
    }
}
