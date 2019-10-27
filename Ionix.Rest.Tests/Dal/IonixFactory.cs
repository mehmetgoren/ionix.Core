namespace Ionix.RestTests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;
    using Ionix.Data;
    using Ionix.Data.SqlServer;
    using Ionix.Utils.Extensions;

    public static class IonixFactory
    {
        private static DbConnection CreateDbConnection()
        {
            DbConnection conn = new SqlConnection();
            conn.ConnectionString = "Data Source=192.168.70.129;initial Catalog=Auth_BioID;User Id=sa;password=1;";
            conn.Open();

            return conn;
        }

        public static IDbAccess CreatDataAccess()
        {
            var connection = CreateDbConnection();
            DbAccess dataAccess = new DbAccess(connection);

            //dataAccess.ExecuteSqlComplete += (e) =>
            //{
            //    try
            //    {
            //        if (!e.Succeeded)
            //        {
            //            string path = e.Succeeded ? "x:\\sql.txt" : "x:\\sqlHata.txt";

            //            using (Stream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            //            {
            //                using (StreamWriter sw = new StreamWriter(fs))
            //                {
            //                    if (e.Query != null)
            //                    {
            //                        sw.WriteLine(e.Query.ToParameterlessQuery());
            //                        sw.WriteLine();
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    catch { }
            //};

            return dataAccess;
        }

        public static ITransactionalDbAccess CreateTransactionalDataAccess()
        {
            var connection = CreateDbConnection();
            TransactionalDbAccess dataAccess = new TransactionalDbAccess(connection);

            //#if DEBUG
            //dataAccess.ExecuteSqlComplete += (e) =>
            //{
            //    try
            //    {
            //        string path = e.Succeeded ? "x:\\sql.txt" : "x:\\sqlHata.txt";

            //        using (Stream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            //        {
            //            using (StreamWriter sw = new StreamWriter(fs))
            //            {
            //                if (e.Query != null)
            //                {
            //                    sw.WriteLine(e.Query.ToParameterlessQuery());
            //                    sw.WriteLine();
            //                }
            //            }
            //        }
            //    }
            //    catch { }
            //};
            //#endif
            return dataAccess;

        }

        internal static ICommandFactory CreateFactory(IDbAccess dataAccess)
        {
            return new CommandFactory(dataAccess);
        }

        //Orn Custom type ve select işlemleri için.
        internal static ICommandAdapter CreateCommand(IDbAccess dataAccess)
        {
            return new CommandAdapter(CreateFactory(dataAccess), CreateEntityMetaDataProvider);
        }


        public static DbClient CreateDbClient()
        {
            return new DbClient(CreatDataAccess());
        }

        public static TransactionalDbClient CreateTransactionalDbClient()
        {
            return new TransactionalDbClient(CreateTransactionalDataAccess());
        }



        private static readonly IEntityMetaDataProvider DefaultMetaDataProvider = new DbSchemaMetaDataProvider();

        public static IEntityMetaDataProvider CreateEntityMetaDataProvider()
        {
            return DefaultMetaDataProvider;
        }


        public static TEntity CreateEntity<TEntity>()
            where TEntity : new()
        {
            TEntity entity = new TEntity();
            var metaData = DefaultMetaDataProvider.CreateEntityMetaData(typeof(TEntity));
            metaData["OpDate"]?.Property.SetValue(entity, DateTime.Now);
            metaData["OpIp"]?.Property.SetValue(entity, "127.0.0.0");
            metaData["OpUserId"]?.Property.SetValue(entity, 1);

            return entity;
        }

        //
        public static IFluentPaging CreatePaging()
        {
            return new FluentPaging();
        }

        public static void BulkCopy<T>(IEnumerable<T> list, ICommandAdapter cmd)
        {
            if (!list.IsEmptyList())
            {
                BulkCopyCommand bulkCopyCommand = new BulkCopyCommand(cmd.Factory.DataAccess.Cast<DbAccess>().Connection.Cast<SqlConnection>());
                bulkCopyCommand.Execute(list, CreateEntityMetaDataProvider());
            }
        }
    }
}
