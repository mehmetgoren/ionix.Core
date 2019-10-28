using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using Ionix.Data.SqlServer;

namespace Ionix.Data.Tests.SqlServer
{
    public static class IonixFactory
    {
        private static readonly object syncRoot = new object();

        private static Action<ExecuteSqlCompleteEventArgs> _OnExecuteSqlComplete;
        public static Action<ExecuteSqlCompleteEventArgs> OnExecuteSqlComplete
        {
            get
            {
                if (null == _OnExecuteSqlComplete)
                {
                    lock (syncRoot)
                    {
                        if (null == _OnExecuteSqlComplete)
                        {

                            _OnExecuteSqlComplete = (e) =>
                            {
                                try
                                {
                                    using (Stream fs = new FileStream((e.ExecutionException == null ? "X:\\logs\\sql.txt" : "X:\\logs\\sqlHata.txt")
                                        , FileMode.Append, FileAccess.Write))
                                    {
                                        using (StreamWriter sw = new StreamWriter(fs))
                                        {
                                            if (e.Query != null)
                                            {
                                                sw.WriteLine(e.Query);
                                                sw.WriteLine(e.Query.ToParameterlessQuery());
                                                sw.WriteLine();
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            };
                        }
                    }
                }
                return _OnExecuteSqlComplete;
            }
            set { _OnExecuteSqlComplete = value; }
        }

        public static DbConnection CreateEmptyConnection()
        {
            return new SqlConnection();
        }

        public static DbConnection CreateDbConnection()
        {
            DbConnection conn = CreateEmptyConnection();

            conn.ConnectionString = @"Data Source=.;Initial Catalog=Northwind;User Id=sa;Password=1242;";
            conn.Open();

            return conn;
        }


        public static IDbAccess CreatDataAccess()
        {
            DbAccess dataAccess = new DbAccess(CreateDbConnection());
            dataAccess.ExecuteSqlComplete += (e) =>
            {
                OnExecuteSqlComplete(e);
            };
            return dataAccess;
        }

        public static ITransactionalDbAccess CreateTransactionalDataAccess()
        {
            TransactionalDbAccess dataAccess = new TransactionalDbAccess(CreateDbConnection());
            dataAccess.ExecuteSqlComplete += (e) =>
            {
                OnExecuteSqlComplete(e);
            };
            return dataAccess;

        }

        public static ICommandFactory CreateFactory(IDbAccess dataAccess)
        {
            return new CommandFactory(dataAccess);
        }


        public static ICommandAdapter CreateCommandAdapter(IDbAccess dataAccess)
        {
            return new CommandAdapter(CreateFactory(dataAccess), CreateEntityMetaDataProvider);
        }

        public static IEntityMetaDataProvider CreateEntityMetaDataProvider()
        {
            return new DbSchemaMetaDataProvider();
        }




        //public static TRepository CreateRepository<TRepository>()
        //    where TRepository : IDisposable
        //{
        //    return (TRepository)Activator.CreateInstance(typeof(TRepository), CreateCommandAdapter(CreatDataAccess()));
        //}

        public static DbClient CreateDbClient()
        {
            return new DbClient();
        }

        public static TransactionalDbClient CreateTransactionalDbClient()
        {
            return new TransactionalDbClient();
        }

    }
}
