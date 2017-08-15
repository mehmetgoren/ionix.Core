namespace ionix.RestTests
{
    using ionix.Data;

    public class V_RoleMenu : ISqlQueryProvider
    {
        [DbSchema(IsNullable = false, IsKey = true)]
        public int Id { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 150)]
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

        [DbSchema(IsNullable = false, IsKey = true)]
        public bool Visible { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 20)]
        public string Image { get; set; }

        [DbSchema(IsNullable = true)]
        public int? RoleMenuId { get; set; }

        [DbSchema(IsNullable = true)]
        public int? RoleId { get; set; }

        [DbSchema(IsNullable = true)]
        public bool? HasAccess { get; set; }

        public string ParentName { get; set; }

        public static SqlQuery Query(int roleId)
        {
            return @"select m.Id, m.Name, m.Controller, m.Action, m.Route, m.Description, m.OrderNum, m.ParentId
            , m.Visible, m.Image, rm.Id RoleMenuId, rm.RoleId, rm.HasAccess
            , p.Name ParentName 
            from Menu m
            left join (select * from RoleMenu where RoleId=@0) rm on m.Id=rm.MenuId
            left join Menu p on m.ParentId=p.Id".ToQuery(roleId);
        }

        public SqlQuery ToQuery()
        {
            return Query(-1);
        }
    }
}
