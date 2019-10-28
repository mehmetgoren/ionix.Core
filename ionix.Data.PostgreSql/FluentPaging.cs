namespace Ionix.Data.PostgreSql
{
    using Utils.Extensions;
    using System;

    public class FluentPaging : FluentPagingBase<FluentPaging>
    {
        public override SqlQuery ToQuery()
        {
            if (!this.select.IsNullOrEmpty() && !String.IsNullOrEmpty(this.from))
            {
                SqlQuery query = "SELECT ".ToQuery();//T.*,
                foreach (string column in this.select)
                {
                    query.Sql(column)
                    .Sql(", ");
                }
                query.Text.Remove(query.Text.Length - 2, 2);
                query.Sql(" FROM ").Sql(this.from);

                if (!String.IsNullOrEmpty(this.orderBy))
                    query.Sql(" ORDER BY ").Sql(this.orderBy);

                if (this.take > 0)
                    query.Sql(" LIMIT :0").Parameter("0", this.take);

                query.Sql(" OFFSET :1").Parameter("1", this.GetFromItems() - 1);

                return query;
            }

            return null;
        }
    }
}
