namespace ionix.Migration.SQLiteTests.Models
{
    using ionix.Data;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [MigrationVersion(Migration100.VersionNo)]
    [Table("Controller")]
    [TableIndex("Name", Unique = true)]
    public class Controller
    {
        [DbSchema(IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int ControllerId { get; set; }

        public int? OpUserId { get; set; }

        [DbSchema(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime? OpDate { get; set; }

        [DbSchema(MaxLength = 15)]
        public string OpIp { get; set; }

        [DbSchema(MaxLength = 50)]
        public string Name { get; set; }
    }
}
