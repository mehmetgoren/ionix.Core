namespace Ionix.Migration.SQLiteTests.Models
{
    using Ionix.Data;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [MigrationVersion(Migration100.VersionNo)]
    [Table("RoleAction")]
    [TableIndex("RoleId", "ActionId", Unique = true)]
    [TableForeignKey("RoleId", "Role", "RoleId")]
    [TableForeignKey("ActionId", "Action", "ActionId")]
    public class RoleAction
    {
        [DbSchema(IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int RoleActionId { get; set; }

        public int? OpUserId { get; set; }

        [DbSchema(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime? OpDate { get; set; }

        [DbSchema(MaxLength = 15)]
        public string OpIp { get; set; }

        public int RoleId { get; set; }

        public int ActionId { get; set; }

        public bool? Enabled { get; set; }
    }
}
