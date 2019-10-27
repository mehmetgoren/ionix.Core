namespace Ionix.RestTests
{
    using Ionix.Data;

    public class V_AppUser : AppUser, ISqlQueryProvider
    {
        public string RoleName { get; set; }

        public bool IsAdmin { get; set; }


        public static SqlQuery Query()
        {
            return $@"select a.*, r.Name RoleName, r.IsAdmin
            from AppUser a 
            inner join Role r on a.RoleId=r.Id".ToQuery();
        }

        public SqlQuery ToQuery()
        {
            return Query();
        }
    }
}
