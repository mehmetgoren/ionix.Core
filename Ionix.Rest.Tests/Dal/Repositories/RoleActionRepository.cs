namespace Ionix.RestTests
{
    using Ionix.Utils.Extensions;
    using Ionix.Data;
    using System.Collections.Generic;

    public class RoleActionRepository : Repository<RoleAction>
    {
        public RoleActionRepository(ICommandAdapter cmd) 
            : base(cmd)
        {
        }

        public int DeleteByControllerActionIds(IEnumerable<int> controllerActionIds)
        {
            if (!controllerActionIds.IsEmptyList())
            {
                SqlQuery q = "DELETE FROM RoleAction ActionId=@ActionIds".ToQuery2( new { ActionIds = controllerActionIds });
               // q.Combine( Fluent.Where<RoleAction>().In(a => a.ActionId, controllerActionIds.ToArray()).ToQuery());

                return this.DataAccess.ExecuteNonQuery(q);
            }

            return 0;
        }
    }
}
