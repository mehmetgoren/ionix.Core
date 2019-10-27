namespace Ionix.Data
{
    using Utils.Extensions;
    using System.Collections.Generic;

    public interface IFluentPaging : ISqlQueryProvider
    {
        IFluentPaging Select(params string[] columns);
        IFluentPaging From(string table);
        IFluentPaging OrderBy(string column);
        IFluentPaging Page(int page);
        IFluentPaging Take(int take);
    }


    public abstract class FluentPagingBase : IFluentPaging
    {
        private int page;// pagingCurrentPage
        private int _take;// pagingItemsPerPage
        protected int take { get { return this._take; } }

        private string _orderBy;
        protected string orderBy { get { return this._orderBy; } }

        private string _from;
        protected string from { get { return this._from; } }

        private readonly HashSet<string> _select;
        protected HashSet<string> select { get { return this._select; } }

        internal FluentPagingBase()
        {
            this._select = new HashSet<string>();
            this.page = 1;
        }

        public IFluentPaging Select(params string[] columns)
        {
            this._select.AddRange(columns);
            return this;
        }

        public IFluentPaging From(string table)
        {
            this._from = table;
            return this;
            ;
        }

        public IFluentPaging OrderBy(string column)
        {
            this._orderBy = column;
            return this;
        }
        public IFluentPaging Page(int page)
        {
            this.page = page;
            return this;
        }
        public IFluentPaging Take(int take)
        {
            this._take = take;
            return this;
        }


        protected int GetFromItems()
        {
            return (this.GetToItems() - this._take + 1);
        }

        protected int GetToItems()
        {
            return (this.page * this._take);
        }



        public abstract SqlQuery ToQuery();

        public override string ToString()
        {
            return this.ToQuery().ToString();
        }
    }

    public abstract class FluentPagingBase<TDerived> : FluentPagingBase
        where TDerived : FluentPagingBase
    {
        public new TDerived Select(params string[] columns)
        {
            return (TDerived)base.Select(columns);
        }

        public new TDerived From(string table)
        {
            return (TDerived)base.From(table);
        }

        public new TDerived OrderBy(string column)
        {
            return (TDerived)base.OrderBy(column);
        }

        public new TDerived Page(int page)
        {
            return (TDerived)base.Page(page);
        }

        public new TDerived Take(int take)
        {
            return (TDerived)base.Take(take);
        }
    }
}
