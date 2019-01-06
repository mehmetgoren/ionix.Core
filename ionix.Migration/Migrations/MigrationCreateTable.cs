namespace ionix.Migration
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using Data;
    using System.Linq;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;

    //Migration000 ve MigrationCreateTable' ın ikisinin de buna ihtiyacı var.
    //MigrationVersionAttribute attributu' u nu Migration000' ıun da
    public abstract class MigrationCreateTable : Migration
    {
        protected MigrationCreateTable(MigrationVersion version)
            : base(version) { }


        protected abstract IEnumerable<Type> GetEntityTypes();//örneğin assembly versin.

        private readonly Lazy<IMigrationService> migrationService = new Lazy<IMigrationService>(Injector.GetInstance<IMigrationService>);
        protected IMigrationService MigrationService => migrationService.Value;



        private IEnumerable<Type> GetEntityTypesWithCheckTableAttributes() => this.GetEntityTypes()?.Where(p => p.GetCustomAttribute<TableAttribute>() != null);

        public override SqlQuery GenerateQuery()
        {
            IEnumerable<Type> migrationTypes = this.GetEntityTypesWithCheckTableAttributes();
            if (!migrationTypes.IsEmptyList())
            {
                List<Type> filteredEntityTypes = new List<Type>();
                foreach (Type migrationType in migrationTypes)
                {
                    bool flag = false;
                    var migrationVersionAttr = migrationType.GetCustomAttribute<MigrationVersionAttribute>();
                    if (null != migrationVersionAttr && !String.IsNullOrEmpty(migrationVersionAttr.MigrationVersion))
                    {
                        string migrationVersion = migrationVersionAttr.MigrationVersion;
                        flag = !String.IsNullOrEmpty(migrationVersion) && new MigrationVersion(migrationVersion) == this.Version;
                    }
                    if (flag)
                        filteredEntityTypes.Add(migrationType);
                }

                if (!filteredEntityTypes.IsEmptyList())
                {
                    filteredEntityTypes = SortTypesHierarchical(filteredEntityTypes);
                    return this.MigrationService.MigrationSqlQueryBuilder.CreateTable(filteredEntityTypes, MigrationCreateTableDbSchemaMetaDataProvider.Create(this.Version), this.MigrationService.ColumnDbTypeResolver);
                }
            }

            return new SqlQuery();
        }

        private static List<Type> SortTypesHierarchical(List<Type> entiyTypes)
        {
            bool Sort(ref List<Type> list)
            {
                bool sorted = false;

                LinkedList<Type> linkedList = new LinkedList<Type>(list);
                for (int j = 0; j < list.Count; ++j)
                {
                    Type entiyType = list[j];

                    IEnumerable<TableForeignKeyAttribute> tfkas = entiyType.GetCustomAttributes<TableForeignKeyAttribute>();
                    if (null != tfkas)
                    {
                        foreach (TableForeignKeyAttribute tfka in tfkas)
                        {
                            Type parentTableType = list.FirstOrDefault(type => AttributeExtension.GetTableName(type) == tfka.ReferenceTable);
                            if (null == parentTableType)
                                throw new InvalidOperationException("parent table was not found.");

                            if (parentTableType != entiyType)//if the table is not self referenced
                            {
                                int parentIndex = list.IndexOf(parentTableType);
                                if (-1 == parentIndex)
                                    throw new InvalidOperationException("parent table was not found.");

                                if (parentIndex >= j)//yani daha sonda ise.
                                {
                                    linkedList.Remove(parentTableType);
                                    linkedList.AddBefore(linkedList.Find(entiyType), parentTableType);
                                    sorted = true;
                                    goto SetList;
                                }
                            }
                        }
                    }
                    
                }

                SetList:
                list = linkedList.ToList();

                return sorted;
            }

            while (Sort(ref entiyTypes)) ;
            return entiyTypes;

            //var tablesAndOrders = new Dictionary<Type, int>();
            //entiyTypes.ForEach(t => tablesAndOrders.Add(t, 0));

            //foreach (Type entiyType in entiyTypes)
            //{
            //    IEnumerable<TableForeignKeyAttribute> tfkas = entiyType.GetCustomAttributes<TableForeignKeyAttribute>();
            //    if (null != tfkas)
            //    {
            //        foreach (TableForeignKeyAttribute tfka in tfkas)
            //        {
            //            Type parentTableType = entiyTypes.FirstOrDefault(type => AttributeExtension.GetTableName(type) == tfka.ReferenceTable);
            //            if (null == parentTableType)
            //                throw new InvalidOperationException("parent table was not found.");

            //            ++tablesAndOrders[parentTableType];
            //        }
            //    }
            //}

            //return tablesAndOrders.Select(p => (p.Key, p.Value)).OrderByDescending(p => p.Value).Select(p => p.Key);// .Values.OrderBy(p => p);
        }

        public override void Sync(ICommandAdapter cmd)
        {
            SqlQuery query = this.GenerateQuery();
            if (null == query || query.IsEmpty())
            {
                throw new MigrationException($"{this.GetType()}.{nameof(GenerateQuery)} shouldn't returns null or empty query.");
            }

            cmd.Factory.DataAccess.ExecuteNonQuery(this.GenerateQuery());
        }
    }
}
