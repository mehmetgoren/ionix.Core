namespace Ionix.RestTests
{
    using Ionix.Data;

    public class V_RoleControllerAction : ISqlQueryProvider
    {
        [DbSchema(IsNullable = false, IsKey = true)]
        public int RoleId { get; set; }

        [DbSchema(IsNullable = true)]
        public int? RoleActionId { get; set; }

        [DbSchema(IsNullable = true)]
        public int? ActionId { get; set; }

        [DbSchema(IsNullable = true)]
        public int? ControllerId { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 50)]
        public string RoleName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 50)]
        public string ControllerName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 50)]
        public string ActionName { get; set; }

        [DbSchema(IsNullable = true)]
        public bool? Enabled { get; set; }


        [DbSchema(IsNullable = true)]
        public bool? IsAdmin { get; set; }

        public static SqlQuery Query()
        {
            return
                @"select  r.Id RoleId, ra.Id RoleActionId, a.Id ActionId, c.Id ControllerId, r.Name RoleName, c.Name ControllerName, a.Name ActionName, ra.Enabled
                , r.IsAdmin
                from Role r
                left join RoleAction ra on r.Id = ra.RoleId
                left join Action a on a.Id = ra.ActionId
                left join Controller c on a.ControllerId = c.Id".ToQuery();
        }

        public SqlQuery ToQuery()
        {
            return Query();
        }
    }
}
