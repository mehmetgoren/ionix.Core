namespace ionix.Migration.SQLiteTests.Models
{
    using ionix.Data;
    using System;
    using System.Collections.Generic;
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


    [MigrationVersion(Migration100.VersionNo)]
    [Table("Action")]
    [TableIndex("Name")]
    [TableIndex("ControllerId", "Name", Unique = true)]
    [TableForeignKey("ControllerId", "controller", "ControllerId")]
    public class Action
    {
        [DbSchema(IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int ActionId { get; set; }

        public int? OpUserId { get; set; }

        [DbSchema(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime? OpDate { get; set; }

        [DbSchema( MaxLength = 15)]
        public string OpIp { get; set; }

        public int ControllerId { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50)]
        public string Name { get; set; }
    }


    [MigrationVersion(Migration100.VersionNo)]
    [Table("Role")]
    [TableIndex("Name", Unique = true)]
    public class Role
    {
        [DbSchema(IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int RoleId { get; set; }

        public int? OpUserId { get; set; }

        [DbSchema(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime? OpDate { get; set; }

        [DbSchema(MaxLength = 15)]
        public string OpIp { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50)]
        public string Name { get; set; }

        public bool IsAdmin { get; set; }

        public bool? CanUseWebSockets { get; set; }
    }


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


    [MigrationVersion(Migration100.VersionNo)]
    [Table("AppUser")]
    [TableIndex("Username", Unique = true)]
    [TableForeignKey("RoleId", "Role", "RoleId")]
    public class AppUser
    {
        [DbSchema(IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int AppUserId { get; set; }

        public int? OpUserId { get; set; }

        [DbSchema(ColumnName = "op_date", DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime? OpDate { get; set; }

        [DbSchema(MaxLength = 15)]
        public string OpIp { get; set; }

        [DbSchema(MaxLength = 15)]
        public string IpAddress { get; set; }

        public int RoleId { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50)]
        public string Username { get; set; }

        [DbSchema(MaxLength = 150)]
        public string Password { get; set; }

        public long? LoginCount { get; set; }

        [DbSchema(MaxLength = 50)]
        public string Title { get; set; }

        [Precision(14,2)]
        public decimal LineerScore { get; set; }

        public double LineerBiasScore { get; set; }
    }
}
