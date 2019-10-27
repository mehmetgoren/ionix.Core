namespace Ionix.Migration.SQLiteTests.Models
{
    using Ionix.Data;
    using System.ComponentModel.DataAnnotations.Schema;

    [MigrationVersion(Migration100.VersionNo)]
    [Table("AppSetting")]
    public class AppSetting
    {
        [DbSchema(IsKey = true, MaxLength = 50)]
        public string Name { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 500)]
        public string Value { get; set; }

        [DbSchema(MaxLength = 500)]
        public string DefaultValue { get; set; }

        [DbSchema(MaxLength = 500)]
        public string Description { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50)]
        public string Module { get; set; }

        public bool Enabled { get; set; }
    }
}
