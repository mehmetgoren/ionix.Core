namespace Ionix.Data
{
    using System;

    public interface ICommandFactory
    {
        IDbAccess DataAccess { get; }
        char ParameterPrefix { get; }

        IEntityCommandSelect CreateSelectCommand();

        IEntityCommandExecute CreateEntityCommand(EntityCommandType commandType);
        IBatchCommandExecute CreateBatchCommand(EntityCommandType commandType);
        IBulkCopyCommand CreateBulkCopyCommand();
    }

    public abstract class CommandFactoryBase : ICommandFactory
    {
        protected CommandFactoryBase(IDbAccess dataAccess)
        {
            if (null == dataAccess)
                throw new ArgumentNullException(nameof(dataAccess));

            this.DataAccess = dataAccess;
        }

        public IDbAccess DataAccess { get; }

        public abstract char ParameterPrefix { get; }

        public virtual IEntityCommandSelect CreateSelectCommand()
        {
            return new EntityCommandSelect(this.DataAccess, this.ParameterPrefix);
        }

        public abstract IEntityCommandExecute CreateEntityCommand(EntityCommandType commandType);
        public abstract IBatchCommandExecute CreateBatchCommand(EntityCommandType commandType);
        public abstract IBulkCopyCommand CreateBulkCopyCommand();
    }

    public enum EntityCommandType : int
    {
        Update = 0,
        Insert = 1,
        Upsert = 2,
        Delete = 3
    }
}
