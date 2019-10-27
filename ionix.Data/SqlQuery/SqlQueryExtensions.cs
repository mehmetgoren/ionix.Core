namespace Ionix.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using Utils;

    public static class SqlQueryExtensions
    {
        //i.e. "update Perseon set Name=@0, No=@1 where Id=@2".ToQuery("Memo", 12, 1);
        public static SqlQuery ToQuery(this string sql, params object[] parameters)
        {
            SqlQuery query = new SqlQuery();
            query.Sql(sql, parameters);
            return query;
        }
        public static SqlQuery ToQuery(this string sql)
        {
            return new SqlQuery(sql);
            // return ConversionExtensions.ToQuery(sql, null);
        }

        //Arama Yapısından Sık Kullanılan İç Sorgular İçin Eklendi
        public static SqlQuery ToInnerQuery(this SqlQuery query, string alias)
        {
            if (null != query)
            {
                if (String.IsNullOrEmpty(alias))
                    alias = "T";

                SqlQuery inner = new SqlQuery();
                inner.Text.Append("SELECT * FROM (");
                inner.Combine(query);
                inner.Text.Append(") ");
                inner.Text.Append(alias);
                return inner;
            }
            return null;
        }
        public static SqlQuery ToInnerQuery(this SqlQuery query)
        {
            return ToInnerQuery(query, "T");
        }


        private static readonly HashSet<Type> collTtpes = new HashSet<Type>
        {
            CachedTypes.String,
            CachedTypes.ByteArray
        };
        public static SqlQuery ToQuery2(this string sql, object parameters)// burada anonim tip oluştur
        {
            SqlQuery q = new SqlQuery();
            if (null != parameters)
            {
                Type type = parameters.GetType();
                foreach (PropertyInfo pi in type.GetTypeInfo().GetProperties())
                {
                    bool flag = false;
                    object value = pi.GetValue(parameters);

                    if (null != value)
                    {
                        Type valueType = value.GetType();
                        if (!collTtpes.Contains(valueType))
                        {
                            IEnumerable list = value as IEnumerable;
                            if (null != list)
                            {
                                char prefix = sql.Contains("@") ? '@' : ':';
                                StringBuilder sb = new StringBuilder("(");
                                int index = 0;
                                foreach (var item in list)
                                {
                                    sb.Append(prefix)
                                        .Append(pi.Name + index)
                                        .Append(',');
                                    q.Parameters.Add(pi.Name + index, item);
                                    ++index;
                                }
                                sb.Remove(sb.Length - 1, 1);
                                sb.Append(')');

                                sql = sql.Replace(prefix + pi.Name, sb.ToString());
                                flag = true;
                            }
                        }
                    }
                    if (!flag)
                        q.Parameter(pi.Name, value);
                }
            }
            q.Sql(sql);

            return q;
        }

        //Useage;
        //SqlQuery q = "select t.* from Person t where t.No:@No and t.Adi=@Adi".ToQuery2(new { No = 1, Adi = "Mehmet" });

        //q ="select * from (select 1 as Id union all select 2 union all select 3) as X where Id in @Ids".ToQuery2(new{Ids = new [] {1, 2, 3}});

        //    q =
        //        "select * from Person p where p.Id in @Ids and p.No in @Nos and p.Adi like '@Adi%'".ToQuery2(
        //            new {Ids = new[] {1, 2, 3}, Nos = new[] {"12", "42"}, Adi = "Mehmet"});

    }
}
