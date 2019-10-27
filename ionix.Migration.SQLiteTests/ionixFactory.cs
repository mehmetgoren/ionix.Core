namespace Ionix.Migration.SQLiteTests
{
    using System;
    using System.Data.Common;
    using System.IO;
    using System.Reflection;
    using Ionix.Data;
    using Ionix.Data.SQLite;
    using Ionix.Migration.SQLite;
    using Microsoft.Data.Sqlite;

    internal static class IonixFactory
    {
        private static readonly string _directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static string GetConnectionString()
        {
            string dbFileName = $"migration_tests.db";
            string dbFilePath = _directoryPath + "\\" + dbFileName;

            return "Data Source=" + dbFilePath;
        }

       // private static readonly object _lockObject = new object();
        private static void LogSqlScript(ExecuteSqlCompleteEventArgs e)
        {
           // lock (_lockObject)
           // {
                try
                {
                    string path = _directoryPath + "\\" + (e.Succeeded ? "sql.txt" : "sqlError.txt");
                    using (Stream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            if (e.Query != null)
                            {
                                sw.WriteLine(e.Query.ToParameterlessQuery());
                                sw.WriteLine();
                            }
                        }
                    }
                }
                catch { }
           // }
        }

        private static DbConnection CreateDbConnection()
        {
            try
            {
                DbConnection conn = new SqliteConnection();
                conn.ConnectionString = GetConnectionString();
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
            dataAccess.ExecuteSqlComplete += LogSqlScript;
            return dataAccess;
        }
        public static ITransactionalDbAccess CreateTransactionalDataAccess()
        {
            var connection = CreateDbConnection();
            TransactionalDbAccess dataAccess = new TransactionalDbAccess(connection);
            dataAccess.ExecuteSqlComplete += LogSqlScript;
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

        public static void InitMigration(bool deleteFiles)
        {
            if (deleteFiles)
            {
                File.Delete(_directoryPath + "\\migration_tests.db");
                File.Delete(_directoryPath + "\\sql.txt");
                File.Delete(_directoryPath + "\\sqlError.txt");
            }

            using (var client = CreateDbClient())
            {
                new MigrationInitializer(null).Execute(
                    Assembly.GetExecutingAssembly()
                    , client.Cmd, false);
            }
        }
    }
}