namespace ionix.Migration.SQLiteTests.Models
{
    using ionix.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class Migration100 : MigrationCreateTable
    {
        public const string VersionNo = "1.0.0";

        public Migration100() :
            base(VersionNo)
        {
        }

        protected override IEnumerable<Type> GetEntityTypes() => Assembly.GetExecutingAssembly().GetTypes();
    }


    public sealed class Migration101 : Migration
    {
        public const string VersionNo = "1.0.1";

        public Migration101() :
            base(VersionNo)
        {
        }

        public override SqlQuery GenerateQuery()
        {
            return "it's a first data transfer by ionix".ToQuery();
        }

        public override void Sync(ICommandAdapter cmd)
        {
            List<Role> roles = new List<Role>();
            roles.Add(new Role() { Name = "Admin", IsAdmin = true });
            roles.Add(new Role() { Name = "Guest", IsAdmin = false });
            cmd.BatchInsert(roles);

            List<AppUser> appUsers = new List<AppUser>();
            appUsers.Add(new AppUser() { RoleId = 1, Username = "admin", Password = "admin", LoginCount = 0 });
            appUsers.Add(new AppUser() { RoleId = 2, Username = "guest", Password = "guest", LoginCount = 0 });
            cmd.BatchInsert(appUsers);

            List<AppSetting> appSettings = new List<AppSetting>();
            appSettings.Add(new AppSetting() { Name = "FastReportsPath", Value = "c:\\", Module = "Module1", Enabled = true });
            appSettings.Add(new AppSetting() { Name = "WebApiAuthEnabled", Value = "true", Module = "Module1", Enabled = true });
            appSettings.Add(new AppSetting() { Name = "WebApiSessionTimeout", Value = "30", Module = "Module1", Enabled = true });
            cmd.BatchInsert(appSettings);


            Menu parent = new Menu() { Name = "Admin Panel", Route = null, OrderNum = null, ParentId = null, Visible = true, Icon = "apps" };
            cmd.Insert(parent);

            List<Menu> menus = new List<Menu>();
            menus.Add(new Menu() { Name = "Api Management", Route = "RoleActions", OrderNum = 1, ParentId = parent.MenuId, Visible = true, Icon = "fiber_manual_record" });
            menus.Add(new Menu() { Name = "Roles", Route = "Roles", OrderNum = 2, ParentId = parent.MenuId, Visible = true, Icon = "fiber_manual_record" });
            menus.Add(new Menu() { Name = "Menus", Route = "Menus", OrderNum = 3, ParentId = parent.MenuId, Visible = true, Icon = "fiber_manual_record" });
            menus.Add(new Menu() { Name = "Roles&Menus", Route = "RolesMenus", OrderNum = 4, ParentId = parent.MenuId, Visible = true, Icon = "fiber_manual_record" });
            menus.Add(new Menu() { Name = "Application Users", Route = "AppUsers", OrderNum = 5, ParentId = parent.MenuId, Visible = true, Icon = "fiber_manual_record" });
            menus.Add(new Menu() { Name = "System Settings", Route = "AppSettings", OrderNum = 6, ParentId = parent.MenuId, Visible = true, Icon = "fiber_manual_record" });
            menus.Add(new Menu() { Name = "Logs", Route = "Logs", OrderNum = 7, ParentId = parent.MenuId, Visible = true, Icon = "fiber_manual_record" });
            menus.Add(new Menu() { Name = "Server Info", Route = "ServerInfo", OrderNum = 8, ParentId = parent.MenuId, Visible = true, Icon = "fiber_manual_record" });
            menus.Add(new Menu() { Name = "Images&Sockets", Route = "SocketDemo", OrderNum = 9, ParentId = parent.MenuId, Visible = true, Icon = "fiber_manual_record" });
            cmd.BatchInsert(menus);

        }
    }

    public sealed class Migration102 : MigrationCreateTable
    {
        public const string VersionNo = "1.0.2";

        public Migration102() :
            base(VersionNo)
        {
        }

        protected override IEnumerable<Type> GetEntityTypes() => Assembly.GetExecutingAssembly().GetTypes();
    }

    public sealed class Migration103 : MigrationAddColumn
    {
        public const string VersionNo = "1.0.3";

        public Migration103()
            : base(VersionNo) { }

        protected override IEnumerable<Type> GetMigrationTypes() => Assembly.GetExecutingAssembly().GetTypes();
    }

    public sealed class Migration104 : MigrationAddColumn
    {
        public const string VersionNo = "1.0.4";

        public Migration104()
            : base(VersionNo) { }

        protected override IEnumerable<Type> GetMigrationTypes() => Assembly.GetExecutingAssembly().GetTypes();
    }
}
