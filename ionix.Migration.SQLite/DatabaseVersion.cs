namespace Ionix.Migration.SQLite
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Data;

    [MigrationVersion(Migration000.VersionNo)]
    [Table("DatabaseVersion")]
    public class DatabaseVersion : DatabaseVersionBase
    {
        private MigrationVersion _version;
        [DbSchema(IsKey = true, MaxLength = 5)]
        public override string Version
        {
            get => this._version;
            set => this._version = value;
        }

        [DbSchema(MaxLength = 50)]
        public override string Description { get; set; }

        public DatabaseVersion()
        {
        }

        public DatabaseVersion(Migration migration)
        {
            this.Version = migration.Version;
            this.StartedOn = DateTime.Now;
            this.Description = migration.Description;
            this.Script = migration.GenerateQuery()?.ToString();
            this.BuiltIn = migration.IsBuiltIn;
        }

        public override string ToString()
        {
            return $"{this.Version} started on {this.StartedOn} completed on {this.CompletedOn}";
        }
    }
}
