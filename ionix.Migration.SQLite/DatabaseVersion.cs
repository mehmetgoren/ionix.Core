namespace ionix.Migration.SQLite
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
            Version = migration.Version;
            StartedOn = DateTime.Now;
            Description = migration.Description;
            Script = migration.GenerateQuery()?.ToString();
        }

        public override string ToString()
        {
            return $"{this.Version} started on {this.StartedOn} completed on {this.CompletedOn}";
        }
    }
}
