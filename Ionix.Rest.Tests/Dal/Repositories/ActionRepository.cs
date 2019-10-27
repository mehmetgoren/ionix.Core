namespace Ionix.RestTests
{
    using Ionix.Data;
    using System.Collections.Generic;


    public class ActionRepository : Repository<Action>
    {
        public ActionRepository(ICommandAdapter cmd) 
            : base(cmd)
        {
        }

        public IList<Action> SelectByControllerId(int controllerId)
        {
            return this.Select(" where ControllerId=@0".ToQuery(controllerId));
        }

        public Action SelectSingleByUniqueKeys(int controllerId, string name)
        {
            return this.SelectSingle(" where ControllerId=@0 and Name=@1".ToQuery(controllerId, name));
        }

        public int DeleteByControllerId(int controllerId)
        {
            SqlQuery q = "DELETE FROM Action".ToQuery();
            q.Sql(" WHERE ControllerId=@0", controllerId);

            return this.DataAccess.ExecuteNonQuery(q);
        }
    }
}
