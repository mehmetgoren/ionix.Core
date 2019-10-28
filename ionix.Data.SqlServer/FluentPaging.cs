namespace Ionix.Data.SqlServer
{
    using Utils.Extensions;
    using System;

    public class FluentPaging : FluentPagingBase<FluentPaging>
    {
        public override SqlQuery ToQuery()
        {
            if (!this.select.IsNullOrEmpty() && !String.IsNullOrEmpty(this.from) && !String.IsNullOrEmpty(this.orderBy))
            {
                SqlQuery query = "WITH Paged AS (SELECT TOP 100 PERCENT ".ToQuery();
                foreach (string column in this.select)
                {
                    query.Sql(column);
                    query.Sql(", ");
                }
                query.Sql(" ROW_NUMBER() OVER (ORDER BY ")
                .Sql(this.orderBy).Sql(") AS RowNumber")
                .Sql(" FROM ").Sql(this.from).Sql(" ) SELECT * FROM Paged WHERE RowNumber BETWEEN @0 AND @1")
                .Parameter("0", this.GetFromItems()).Parameter("1", this.GetToItems());

                return query;
            }

            return null;
        }
    }
}
