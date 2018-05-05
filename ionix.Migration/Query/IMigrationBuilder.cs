namespace ionix.Migration
{
    using System;
    using System.Collections.Generic;
    using Data;

    public interface IMigrationBuilder
    {
        SqlQuery Build(IEnumerable<Type> types, IColumnDbTypeResolver typeResolver);
    }
}
