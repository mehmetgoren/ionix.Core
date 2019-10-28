namespace Ionix.Data.Oracle
{
    using Utils.Extensions;
    using System;

    public class FluentPaging : FluentPagingBase<FluentPaging>
    {
        public override SqlQuery ToQuery()
        {
            if (!this.select.IsNullOrEmpty() && !String.IsNullOrEmpty(this.from) && !String.IsNullOrEmpty(this.orderBy))
            {
                SqlQuery query = "SELECT * FROM (SELECT ".ToQuery();//T.*,
                foreach (string column in this.select)
                {
                    query.Sql("T.")
                    .Sql(column)
                    .Sql(", ");
                }

                query.Sql(" ROW_NUMBER() OVER (ORDER BY ").Sql(this.orderBy).Sql(") ROWNUMBER FROM ").Sql(this.from).Sql(" T)");
                query.Sql("WHERE ROWNUMBER BETWEEN :0 AND :1 ORDER BY ROWNUMBER");
                query.Parameter("0", this.GetFromItems()).Parameter("1", this.GetToItems());

                return query;
            }

            return null;
        }
    }
}
