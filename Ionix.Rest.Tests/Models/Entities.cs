

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Configuration file:     "0BioID.Models\App.config"
//     Connection String Name: "SqlConnAuth"
//     Connection String:      "Data Source=192.168.253.137;Initial Catalog=Auth_BioID;User Id=sa;password=**zapped**;"

// ReSharper disable RedundantUsingDirective
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable RedundantNameQualifier

using System;
using Ionix.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ionix.RestTests
{

    // ************************************************************************
    // POCO classes

	[Table("Action")]
    public partial class Action
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
		public int Id { get; set; }

        [DbSchema(IsNullable = true)]
		public int? OpUserId { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "getdate()")]
		public DateTime? OpDate { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
		public string OpIp { get; set; }

        [DbSchema(IsNullable = false)]
		public int ControllerId { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50)]
		public string Name { get; set; }

    }


	[Table("AppSetting")]
    public partial class AppSetting
    {
        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 50)]
		public string Name { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 500)]
		public string Value { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 500)]
		public string DefaultValue { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 500)]
		public string Description { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50)]
		public string Module { get; set; }

        [DbSchema(IsNullable = false)]
		public bool Enabled { get; set; }

    }


	[Table("AppUser")]
    public partial class AppUser
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
		public int Id { get; set; }

        [DbSchema(IsNullable = true)]
		public int? OpUserId { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "getdate()")]
		public DateTime? OpDate { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
		public string OpIp { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
		public string IpAddress { get; set; }

        [DbSchema(IsNullable = false)]
		public int RoleId { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50)]
		public string Username { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 150)]
		public string Password { get; set; }

        [DbSchema(IsNullable = true)]
		public long? LoginCount { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 50)]
		public string Title { get; set; }

        [DbSchema(IsNullable = true)]
		public int? KisiId { get; set; }

    }


	[Table("Controller")]
    public partial class Controller
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
		public int Id { get; set; }

        [DbSchema(IsNullable = true)]
		public int? OpUserId { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "getdate()")]
		public DateTime? OpDate { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
		public string OpIp { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50)]
		public string Name { get; set; }

        [DbSchema(IsNullable = false)]
		public byte Type { get; set; }

    }


	[Table("Menu")]
    public partial class Menu
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
		public int Id { get; set; }

        [DbSchema(IsNullable = true)]
		public int? OpUserId { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "getdate()")]
		public DateTime? OpDate { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
		public string OpIp { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 150)]
		public string Name { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 50)]
		public string Controller { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 100)]
		public string Action { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 50)]
		public string Route { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 250)]
		public string Description { get; set; }

        [DbSchema(IsNullable = true)]
		public short? OrderNum { get; set; }

        [DbSchema(IsNullable = true)]
		public int? ParentId { get; set; }

        [DbSchema(IsNullable = false)]
		public bool Visible { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 20)]
		public string Image { get; set; }

    }


	[Table("Role")]
    public partial class Role
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
		public int Id { get; set; }

        [DbSchema(IsNullable = true)]
		public int? OpUserId { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "getdate()")]
		public DateTime? OpDate { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
		public string OpIp { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50)]
		public string Name { get; set; }

        [DbSchema(IsNullable = false)]
		public bool IsAdmin { get; set; }

    }


	[Table("RoleAction")]
    public partial class RoleAction
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
		public int Id { get; set; }

        [DbSchema(IsNullable = true)]
		public int? OpUserId { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "getdate()")]
		public DateTime? OpDate { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
		public string OpIp { get; set; }

        [DbSchema(IsNullable = false)]
		public int RoleId { get; set; }

        [DbSchema(IsNullable = false)]
		public int ActionId { get; set; }

        [DbSchema(IsNullable = true)]
		public bool? Enabled { get; set; }

    }


	[Table("RoleMenu")]
    public partial class RoleMenu
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = Ionix.Data.StoreGeneratedPattern.Identity)]
		public int Id { get; set; }

        [DbSchema(IsNullable = true)]
		public int? OpUserId { get; set; }

        [DbSchema(IsNullable = true)]
		public DateTime? OpDate { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15)]
		public string OpIp { get; set; }

        [DbSchema(IsNullable = false)]
		public int RoleId { get; set; }

        [DbSchema(IsNullable = false)]
		public int MenuId { get; set; }

        [DbSchema(IsNullable = false)]
		public bool HasAccess { get; set; }

    }


}

