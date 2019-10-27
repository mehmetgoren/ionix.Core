namespace Ionix.Data.Mongo
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class MigrationVersionAttribute : Attribute
    {
        public string MigrationVersion { get; }

        public MigrationVersionAttribute(string version)
        {
            this.MigrationVersion = version;
        }
    }
}
