namespace ionix.Migration.SQLiteTests.Models
{
    using ionix.Data;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [MigrationVersion(Migration100.VersionNo)]
    [Table("Action")]
    [TableIndex("Name")]
    [TableIndex("ControllerId", "Name", Unique = true)]
    [TableForeignKey("ControllerId", "Controller", "ControllerId")]
    public class Action
    {
        [DbSchema(IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int ActionId { get; set; }

        public int? OpUserId { get; set; }

        [DbSchema(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime? OpDate { get; set; }

        [DbSchema(MaxLength = 15)]
        public string OpIp { get; set; }

        public int ControllerId { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50)]
        public string Name { get; set; }
    }
}
