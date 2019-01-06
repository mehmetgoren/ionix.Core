namespace ionix.Migration.SQLiteTests.Models
{
    using ionix.Data;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [MigrationVersion(Migration100.VersionNo)]
    [Table("RoleMenu")]
    [TableIndex("RoleId", "MenuId", Unique = true)]
    [TableForeignKey("RoleId", "Role", "RoleId")]
    [TableForeignKey("MenuId", "Menu", "MenuId")]
    public class RoleMenu
    {
        [DbSchema(IsKey = true, DatabaseGeneratedOption = ionix.Data.StoreGeneratedPattern.Identity)]
        public int RoleMenuId { get; set; }

        public int? OpUserId { get; set; }

        [DbSchema(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime? OpDate { get; set; }

        [DbSchema(MaxLength = 15)]
        public string OpIp { get; set; }

        public int RoleId { get; set; }

        public int MenuId { get; set; }

        public bool HasAccess { get; set; }
    }
}
