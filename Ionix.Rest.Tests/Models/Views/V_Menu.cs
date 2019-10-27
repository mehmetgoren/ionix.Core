namespace Ionix.RestTests
{
    using Ionix.Data;

    public sealed class V_Menu : Menu, ISqlQueryProvider
    {
        public string ParentName { get; set; }

        public static SqlQuery Query()
        {
            return @"select m.*, p.Name ParentName from Menu m
            left join Menu p on m.ParentId=p.Id".ToQuery();
        }

        public SqlQuery ToQuery()
        {
            return Query();
        }
    }
}
