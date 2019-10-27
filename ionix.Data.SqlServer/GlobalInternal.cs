namespace Ionix.Data.SqlServer
{
    using System.Text;

    internal static class GlobalInternal
    {
        internal const char Prefix = '@';

        private static readonly object lockSqlServerBeginStatement = new object();
        private static string sqlServerBeginStatement;
        internal static string SqlServerBeginStatement
        {
            get
            {
                if (sqlServerBeginStatement == null)
                {
                    lock (lockSqlServerBeginStatement)
                    {
                        if (sqlServerBeginStatement == null)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("BEGIN TRY");
                            sb.AppendLine();
                            sb.Append("BEGIN TRANSACTION");
                            sb.AppendLine();
                            sqlServerBeginStatement = sb.ToString();
                        }
                    }
                }

                return sqlServerBeginStatement;
            }
        }



        private static readonly object lockSqlServerEndStatement = new object();
        private static string sqlServerEndStatement;
        internal static string SqlServerEndStatement
        {
            get
            {
                if (sqlServerEndStatement == null)
                {
                    lock (lockSqlServerEndStatement)
                    {
                        if (sqlServerEndStatement == null)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine();
                            sb.Append("COMMIT TRAN");
                            sb.AppendLine();
                            sb.Append("END TRY");
                            sb.AppendLine();
                            sb.Append("BEGIN CATCH");
                            sb.AppendLine();
                            sb.Append("ROLLBACK TRAN");
                            sb.AppendLine();
                            sb.Append("DECLARE @ERROR_MESSAGE NVARCHAR(1000)");
                            sb.AppendLine();
                            sb.Append("SET @ERROR_MESSAGE = ERROR_MESSAGE()");
                            sb.AppendLine();
                            sb.Append("RAISERROR(@ERROR_MESSAGE, 16,1)");
                            sb.AppendLine();
                            sb.Append("END CATCH");
                            sb.AppendLine();
                            sqlServerEndStatement = sb.ToString();
                        }
                    }
                }

                return sqlServerEndStatement;
            }
        }
    }

    internal sealed class ValueSetter : DbValueSetter
    {
        internal static readonly ValueSetter Instance = new ValueSetter();

        private ValueSetter()
        {

        }

        public override char Prefix => GlobalInternal.Prefix;
    }
}
