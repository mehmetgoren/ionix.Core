namespace Ionix.Data.SQLite
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
                query.Text.Remove(query.Text.Length - 1, 1);
                query.Sql(" FROM ").Sql(this.from);

                if (!String.IsNullOrEmpty(this.orderBy))
                    query.Sql(" ORDER BY ").Sql(this.orderBy);

                query.Sql(" LIMIT @0, @1");
                query.Parameter("0", this.GetFromItems() - 1).Parameter("1", this.GetToItems());

                return query;
            }

            return null;
        }
    }
}
