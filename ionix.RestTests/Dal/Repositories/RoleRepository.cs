namespace ionix.RestTests
{
    using ionix.Data;
    using System.Collections.Generic;


    public class RoleRepository : Repository<Role>
    {
        public RoleRepository(ICommandAdapter cmd)
            : base(cmd) { }


        public IEnumerable<V_RoleControllerAction> Select_V_RoleControllerAction()
        {
            return this.Cmd.Query<V_RoleControllerAction>(V_RoleControllerAction.Query());
        }

        //public Role SelectByName(string name)
        //{
        //    return this.SelectSingle(Fluent.Where<Role>().Equals(r => r.Name, name));
        //}

        //public Role SelectById(int id)
        //{
        //    return this.SelectSingle(Fluent.Where<Role>().Equals(r => r.Id, id));
        //}

        //public IEnumerable<SelectItem> GetSelectItems()
        //{
        //    SqlQuery q = "SELECT Id value, Name label FROM Role".ToQuery();

        //    using (var c = ionixFactory.CreateDbClient(DB.Auth))
        //    {
        //        return c.Cmd.Query<SelectItem>(q);
        //    }
        //}

        //public V_RoleAppUser SelectVievByDbUserId(int appUserId)
        //{
        //    return this.Cmd.SelectSingle(Fluent.Where<V_RoleAppUser>().Equals(u => u.AppUserId.Value, appUserId));
        //}
    }
}
