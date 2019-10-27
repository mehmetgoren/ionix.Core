namespace Ionix.RestTests
{
    using Ionix.Data;

    public class V_RoleAppUser : ISqlQueryProvider
    {
        [DbSchema(IsNullable = false, IsKey = true)]
        public int RoleId { get; set; }

        [DbSchema(IsNullable = false, IsKey = true, MaxLength = 50)]
        public string RoleName { get; set; }

        [DbSchema(IsNullable = false, IsKey = true)]
        public bool IsAdmin { get; set; }

        [DbSchema(IsNullable = true)]
        public int? AppUserId { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 50)]
        public string Username { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 150)]
        public string Password { get; set; }

        [DbSchema(IsNullable = true)]
        public long? LoginCount { get; set; }

        public static SqlQuery Query()
        {
            return
                @"select r.Id RoleId, r.Name RoleName, r.IsAdmin, u.Id AppUserId, u.Username, u.Password, u.LoginCount from Role r
                left join DbUser u on r.Id=u.RoleId".ToQuery();
        }

        public SqlQuery ToQuery()
        {
            return Query();
        }
    }
}
