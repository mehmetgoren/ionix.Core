namespace ionix.RestTests
{
    using System.Collections.Generic;
    using ionix.Data;

    public class AppUserRepository : Repository<AppUser>
    {
        public AppUserRepository(ICommandAdapter cmd) 
            : base(cmd)
        {
        }

        public IEnumerable<V_AppUser> QueryView()
        {
            return this.Cmd.Query<V_AppUser>(V_AppUser.Query());
        }
    }
}
