namespace Ionix.Log
{
    using System;
    using System.Data.Common;
    using Ionix.Data;
    using Ionix.Data.SQLite;
    using Microsoft.Data.Sqlite;

    internal static class IonixFactory
    {
        private static DbConnection CreateDbConnection()
        {
            try
            {
                DbConnection conn = new SqliteConnection();
                var str = InjectorConnectionString.ConnectionStringProvider.GetConnectionString();
                conn.ConnectionString = str;
                conn.Open();

                return conn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static IDbAccess CreatDataAccess()
        {
            var connection = CreateDbConnection();
            DbAccess dataAccess = new DbAccess(connection);
            return dataAccess;
        }
        public static ITransactionalDbAccess CreateTransactionalDataAccess()
        {
            var connection = CreateDbConnection();
            TransactionalDbAccess dataAccess = new TransactionalDbAccess(connection);
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

    }
}