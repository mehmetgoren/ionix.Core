namespace Ionix.Data
{
    using System;
    using System.Data;
    using System.Text;
    using Utils.Extensions;

    public sealed class SqlQuery
    {
        private readonly StringBuilder text;
        private readonly SqlQueryParameterList parameters;

        public SqlQuery(string sql)
        {
            this.text = new StringBuilder(sql);
            this.parameters = new SqlQueryParameterList();
            this.CmdType = CommandType.Text;
        }
        public SqlQuery()
            : this(null)
        { }

        public StringBuilder Text => this.text;

        public SqlQueryParameterList Parameters => this.parameters;

        public CommandType CmdType { get; set; }

        public bool IsEmpty()
        {
            return this.text.Length == 0;
        }

        public override string ToString()
        {
            return this.text.ToString();
        }


        //Builder
        public SqlQuery Combine(SqlQuery queryInfo)
        {
            if (null != queryInfo)
            {
                this.text.Append(queryInfo.text);
                this.parameters.AddRange(queryInfo.parameters);
            }
            return this;
        }
        public SqlQuery Clear()
        {
            this.text.Length = 0;
            this.parameters.Clear();
            return this;
        }

        public SqlQuery Sql(string sql, params object[] parameters)
        {
            if (!String.IsNullOrEmpty(sql))
            {
                this.text.Append(sql);
                if (!parameters.IsNullOrEmpty())
                {
                    for (int j = 0; j < parameters.Length; ++j)
                    {
                        this.Parameters.Add(j.ToString(), parameters[j]);
                    }
                }
            }

            return this;
        }
        public SqlQuery Sql(string sql)
        {
            if (!String.IsNullOrEmpty(sql))
            {
                this.text.Append(sql);
            }

            return this;
        }
        public SqlQuery Parameter(string parameterName, object value)
        {
            if (!String.IsNullOrEmpty(parameterName))
            {
                this.Parameters.Add(parameterName, value);
            }

            return this;
        }
        //public SqlQuery Parameter(dynamic parameters)
        //{
        //    if (null != parameters)
        //    {
        //        foreach (PropertyInfo prop in parameters.GetType().GetProperties())
        //        {
        //            string parameterName = prop.Name;
        //            object value = prop.GetValue(this.parameters);

        //            this.Parameters.Add(parameterName, value);
        //        }
        //     //   this.Parameters.Add(parameterName, value);
        //    }

        //    return this;
        //}
    }
}
