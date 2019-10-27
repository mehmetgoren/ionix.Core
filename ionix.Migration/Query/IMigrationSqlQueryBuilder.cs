namespace Ionix.Migration
{
    using System;
    using System.Collections.Generic;
    using Data;

    public interface IMigrationSqlQueryBuilder
    {
        SqlQuery CreateTable(IEnumerable<Type> types, DbSchemaMetaDataProvider provider, IColumnDbTypeResolver typeResolver);
    }
}
