namespace Ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public sealed class SqlQueryParameterList : IEnumerable<SqlQueryParameter>
    {
        private readonly SqlQueryParameterDic hash = new SqlQueryParameterDic();

        public void Add(SqlQueryParameter parameter)
        {
            if (null == parameter)
                throw new ArgumentNullException(nameof(parameter));

            this.AddInternal(parameter);
        }

        private void AddInternal(SqlQueryParameter parameter)
        {
            this.hash.Add(parameter);
        }
        public void Add(string parameterName, object value, ParameterDirection direction, bool isNullable)
        {
            SqlQueryParameter parameter = new SqlQueryParameter(parameterName, value, direction, isNullable);
            this.AddInternal(parameter);
        }

        public void Add(string parameterName, object value, ParameterDirection direction)
        {
            this.Add(parameterName, value, direction, true);
        }
        public void Add(string parameterName, object value)
        {
            this.Add(parameterName, value, ParameterDirection.Input, true);
        }

        public void AddRange(IEnumerable<SqlQueryParameter> list)
        {
            if (null != list)
            {
                foreach (SqlQueryParameter item in list)
                    this.Add(item);
            }
        }
        public SqlQueryParameter Find(string parameterName)
        {
            if (!String.IsNullOrEmpty(parameterName))
            {
                return this.hash.Find(parameterName);
            }
            return null;
        }
        public void Clear()
        {
            this.hash.Clear();
        }

        public IEnumerator<SqlQueryParameter> GetEnumerator()
        {
            foreach (KeyValuePair<string, SqlQueryParameter> kvp in this.hash.dic)
                yield return kvp.Value;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private sealed class SqlQueryParameterDic
        {
            internal readonly Dictionary<string, SqlQueryParameter> dic = new Dictionary<string, SqlQueryParameter>();

            public void Add(SqlQueryParameter parameter)
            {
                this.dic[parameter.ParameterName] = parameter;
            }
            public SqlQueryParameter Find(string parameterName)
            {
                SqlQueryParameter ret;
                this.dic.TryGetValue(parameterName, out ret);
                return ret;
            }
            public void Clear()
            {
                this.dic.Clear();
            }
        }
    }
}
