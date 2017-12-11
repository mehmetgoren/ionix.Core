namespace SQLog
{
    using Microsoft.Data.Sqlite;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.IO;
    using System.Reflection;

    public interface IConnectionStringProvider
    {
        string GetConnectionString();
    }

    public abstract class BaseConnectionStringProvider : IConnectionStringProvider
    {
        public IConfigurationRoot Configuration { get; set; } = Logger.AppSettings;

        public abstract string GetConnectionString();

        public virtual string GetPath()
        {
            return this.Configuration?["SQLog:Path"];
        }
    }

    public class DailyConnectionStringProvider : BaseConnectionStringProvider
    {
        public override string GetConnectionString()
        {
            var path = base.GetPath();
            if (String.IsNullOrEmpty(path))
                path = Assembly.GetExecutingAssembly().Location;// Directory.GetCurrentDirectory();

            var directoryPath = Path.GetDirectoryName(path);
            DateTime today = DateTime.Today;

            string dbFileName = $"logs_{today.Day}-{today.Month}-{today.Year}.db";
            string dbFilePath = directoryPath + "\\" + dbFileName;

            string connStr = "Data Source=" + dbFilePath;
            //+ ";Version=3;"   burası system.data.SQlite ile çalışan versiyon.

            if (!File.Exists(dbFilePath))
            {
                using (SqliteConnection tempConn = new SqliteConnection(connStr))
                //SqlConnection kendi file inı oluşturuyor.
                {
                    tempConn.Open();
                    var cmd = tempConn.CreateCommand();
                    cmd.CommandText = LogEntity.CreateSql();
                    cmd.ExecuteNonQuery();
                }
            }

            return connStr;
        }
    }

    public static class InjectorConnectionString
    {
        #region Connection String Dependecy Injection

        private static Type ConnectionStringProviderType;


        private static IConnectionStringProvider connectionStringProvider;
        internal static IConnectionStringProvider ConnectionStringProvider
        {
            get
            {
                if (null == connectionStringProvider)
                {
                    if (null == ConnectionStringProviderType)
                        ConnectionStringProviderType = typeof(DailyConnectionStringProvider);

                    connectionStringProvider =
                        (IConnectionStringProvider)Activator.CreateInstance(ConnectionStringProviderType);
                }
                return connectionStringProvider;
            }
        }
        #endregion

        public static void Resolve<TConnectionStringProvider>()
            where TConnectionStringProvider : IConnectionStringProvider
        {
            ConnectionStringProviderType = typeof(TConnectionStringProvider);
            connectionStringProvider = null;
        }
    }
}
