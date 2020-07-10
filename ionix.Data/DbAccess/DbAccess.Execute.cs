namespace Ionix.Data
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;

    partial class DbAccess
    {
        private DbCommand CreateCommand(SqlQuery query, out OutParameters outParameters)
        {
            if (null == query)
                throw new ArgumentNullException(nameof(query));

            string commandText = query.Text.ToString();
            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("query, SqlQuery.Text is null");

            outParameters = null;

            this.OnPreExecuteSql(query);
            DbCommand cmd = null;
            try

            {
                cmd = this.Connection.CreateCommand();
                if (this.commandTimeout.HasValue)
                    cmd.CommandTimeout = this.commandTimeout.Value;

                foreach (SqlQueryParameter parameter in query.Parameters)
                {
                    DbParameter dbParameter = cmd.CreateParameter();
                    dbParameter.ParameterName = parameter.ParameterName;

                    dbParameter.IsNullable = parameter.IsNullable;

                    if (parameter.dbType.HasValue)
                        dbParameter.DbType = parameter.dbType.Value;

                    object parameterValue = parameter.Value;
                    dbParameter.Value = (null == parameterValue) ? DBNull.Value : parameterValue;

                    ParameterDirection direction = parameter.Direction;
                    dbParameter.Direction = direction;

                    cmd.Parameters.Add(dbParameter);

                    if (direction == ParameterDirection.InputOutput || direction == ParameterDirection.Output)
                    {
                        if (null == outParameters)
                            outParameters = new OutParameters();
                        outParameters.Add(parameter, dbParameter);
                    }
                }

                cmd.CommandText = commandText;
                cmd.CommandType = query.CmdType;
            }
            catch (Exception)
            {
                if (null != cmd) cmd.Dispose();
                throw;
            }

            this.OnCommandCreated(cmd);

            return cmd;
        }

        //Added for transaction
        protected virtual void OnCommandCreated(DbCommand cmd)
        {

        }

        private T Execute<T>(SqlQuery query, Func<DbCommand, T> func, bool disposeCommand)
        {
            DbCommand cmd = null;
            OutParameters outParameters;
            T ret = default(T);
            DateTime executionStart = DateTime.Now;
            try
            {
                cmd = this.CreateCommand(query, out outParameters);

                ret = func(cmd);
            }
            catch (Exception ex)
            {
                this.OnExecuteSqlComplete(query, executionStart, ex);

                throw;
            }
            finally
            {
                if (disposeCommand && null != cmd) cmd.Dispose();
            }

            if (null != outParameters)
            {
                outParameters.SetOutParametersValues();
            }

            this.OnExecuteSqlComplete(query, executionStart, null);

            return ret;
        }

        //Neden ayırdık çünkü identity gibi yeni değerlerin eklenmesi durumunda func task olarak döndüğünden execute edilmiyor ve output parametreler set edilmiyordu...
        //Aynı şekilde transaction de commit den sonra çalışarak da sıkıntı çıkarabilirdi.
        private async Task<T> ExecuteAsync<T>(SqlQuery query, Func<DbCommand, Task<T>> funcAsync, bool disposeCommand)
        {
            DbCommand cmd = null;
            OutParameters outParameters;
            T ret = default(T);
            DateTime executionStart = DateTime.Now;
            try
            {
                cmd = this.CreateCommand(query, out outParameters);

                ret = await funcAsync(cmd);
            }
            catch (Exception ex)
            {
                this.OnExecuteSqlComplete(query, executionStart, ex);

                throw;
            }
            finally
            {
                if (disposeCommand && null != cmd) cmd.Dispose();
            }

            if (null != outParameters)
            {
                outParameters.SetOutParametersValues();
            }

            this.OnExecuteSqlComplete(query, executionStart, null);

            return ret;
        }

        public int ExecuteNonQuery(SqlQuery query)
        {
            return this.Execute(query, (cmd) => cmd.ExecuteNonQuery(), true);
        }
        public Task<int> ExecuteNonQueryAsync(SqlQuery query)
        {
            return this.ExecuteAsync(query, (cmd) => cmd.ExecuteNonQueryAsync(), true);
        }


        public object ExecuteScalar(SqlQuery query)
        {
            return this.Execute(query, (cmd) => cmd.ExecuteScalar(), true);
        }
        public Task<object> ExecuteScalarAsync(SqlQuery query)
        {
            return this.ExecuteAsync(query, (cmd) => cmd.ExecuteScalarAsync(), true);
        }

        public virtual AutoCloseCommandDataReader CreateDataReader(SqlQuery query, CommandBehavior behavior)
        {
            return this.Execute(query, (cmd) => AutoCloseCommandDataReader.Create(cmd, cmd.ExecuteReader(behavior)), false);
        }
        public virtual Task<AutoCloseCommandDataReader> CreateDataReaderAsync(SqlQuery query, CommandBehavior behavior)
        {
            return this.ExecuteAsync(query, async (cmd) => AutoCloseCommandDataReader.Create(cmd, await cmd.ExecuteReaderAsync(behavior)), false);
        }
    }
}
