namespace Ionix.Data
{
    using System;
    using System.Collections.Generic;
    using Utils;

    public interface IEntityMetaData : IPrototype<IEntityMetaData>
    {
        IEnumerable<PropertyMetaData> Properties { get; }
        string TableName { get; }
        Type EntityType { get; }

        PropertyMetaData this[string columnName] { get; }

        string GetParameterName(PropertyMetaData pm, int index);
    }
    public interface IEntityMetaDataProvider
    {
        IEntityMetaData CreateEntityMetaData(Type entityType);
    }
}
