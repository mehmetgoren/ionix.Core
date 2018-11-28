namespace ionix.Migration
{
    using System;
    using System.Collections.Generic;
    using Data;

    public interface IMigrationSqlQueryBuilder
    {
        SqlQuery CreateTable(IEnumerable<Type> types, MigrationCreateTableDbSchemaMetaDataProvider provider, IColumnDbTypeResolver typeResolver);

        SqlQuery AddColumn(IEnumerable<Type> types, MigrationAddColumnDbSchemaMetaDataProvider provider, IColumnDbTypeResolver typeResolver);
    }
}
