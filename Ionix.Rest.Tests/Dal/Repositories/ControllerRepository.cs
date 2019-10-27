namespace Ionix.RestTests
{
    using Ionix.Data;
    using System;

    public class ControllerRepository : Repository<Controller>
    {
        public ControllerRepository(ICommandAdapter cmd) 
            : base(cmd)
        {
        }

        public Controller SelectSingleByName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
               return this.SelectSingle(" where Name=@0".ToQuery(name));
            }
            return null;
        }
    }
}
