namespace ionix.Migration.SQLiteTests.Models
{
    using ionix.Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [MigrationVersion(Migration100.VersionNo)]
    [Table("Menu")]
    [TableIndex("Name", Unique = true)]
    [TableForeignKey("ParentId", "Menu", "MenuId")]
    public class Menu
    {
        [DbSchema(IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int MenuId { get; set; }

        public int? OpUserId { get; set; }

        [DbSchema(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime? OpDate { get; set; }

        [DbSchema(MaxLength = 15)]
        public string OpIp { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 150)]
        public string Name { get; set; }

        [DbSchema(MaxLength = 50)]
        public string Route { get; set; }

        [DbSchema(MaxLength = 250)]
        public string Description { get; set; }

        public short? OrderNum { get; set; }

        public int? ParentId { get; set; }

        public bool Visible { get; set; }

        [DbSchema(MaxLength = 20)]
        public string Icon { get; set; }


        [NotMapped]
        public List<Menu> Childs { get; set; } = new List<Menu>();
    }
}
