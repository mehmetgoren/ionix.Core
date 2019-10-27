namespace Ionix.Migration.PostgreSql
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Data;

    [MigrationVersion(Migration000.VersionNo)]
    [Table("database_version")]
    public class DatabaseVersion : DatabaseVersionBase
    {
        private MigrationVersion _version;
        [DbSchema(ColumnName ="version", IsKey=true, MaxLength = 5)]
        public override string Version
        {
            get => this._version;
            set => this._version = value;
        }

        [DbSchema(ColumnName = "description", MaxLength = 50)]
        public override string Description { get; set; }

        [DbSchema(ColumnName = "started_on")]
        public override DateTime? StartedOn { get; set; }

        [DbSchema(ColumnName = "completed_on")]
        public override DateTime? CompletedOn { get; set; }

        [DbSchema(ColumnName = "script")]
        public override string Script { get; set; }

        [DbSchema(ColumnName = "warning")]//amaç tablo da pk yok gibi uyarıların verilmesi
        public override string Warning { get; set; }

        [DbSchema(ColumnName = "exception")]
        public override string Exception { get; set; }

        [DbSchema(ColumnName = "built_in")]
        public override bool? BuiltIn { get; set; }

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
            return $"{this.Version} started at {this.StartedOn} completed at {this.CompletedOn}";
        }
    }
}
