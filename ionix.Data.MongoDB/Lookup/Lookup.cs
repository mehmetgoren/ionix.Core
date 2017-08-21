namespace ionix.Data.Mongo
{
    using ionix.Data.Mongo.Serializers;
    using ionix.Utils.Extensions;
    using ionix.Utils.Reflection;
    using MongoDB.Driver;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    public partial class Lookup<TLeft> : IMongoDbScriptProvider
    {
        public static Lookup<TLeft> Left(IMongoDatabase db)
        {
            return new Lookup<TLeft>(db);
        }

        private readonly Type _leftType;
        private readonly MongoCollectionAttribute _collectionInfo;
        private readonly IMongoDatabase _db;
        private readonly Filter _match;
        private readonly HashSet<string> fields;

        private Lookup(IMongoDatabase db)
        {
            this._leftType = typeof(TLeft);

            this._batchSize = 1000000;
            this._collectionInfo = MongoExtensions.GetCollectionInfo(this._leftType);
            this._db = db;
            this._match = new Filter(this);
            this.fields = new HashSet<string>();

            this._joins = new Dictionary<Type, IMongoDbScriptProvider>();
        }

        private int _batchSize;
        public Lookup<TLeft> BatchSize(int value)
        {
            this._batchSize = value;
            return this;
        }

        private int _limit;
        public Lookup<TLeft> Limit(int limit)
        {
            this._limit = limit;
            return this;
        }

        private static string GetFieldName<T>(Expression<Func<T, object>> field)
        {
            return DictionarySerializer.GetFieldName(ReflectionExtensions.GetPropertyInfo(field));
        }

        private string _orderField;
        private OrderType? _orderBy;
        public Lookup<TLeft> OrderBy(Expression<Func<TLeft, object>> field)
        {
            if (null != field)
            {
                this._orderField = GetFieldName(field);
                this._orderBy = OrderType.Ascending;
            }
            return this;
        }
        public Lookup<TLeft> OrderByDescending(Expression<Func<TLeft, object>> field)
        {
            if (null != field)
            {
                this._orderField = GetFieldName(field);
                this._orderBy = OrderType.Descending;
            }
            return this;
        }

        public Lookup<TLeft> Project(params Expression<Func<TLeft, object>>[] fields)
        {
            if (null != fields && 0 != fields.Length)
            {
                foreach (var field in fields)
                {
                    this.fields.Add(GetFieldName(field));
                }
            }

            return this;
        }

        public Filter Match()
        {
            return this._match;
        }

        private readonly Dictionary<Type, IMongoDbScriptProvider> _joins;
        public Join<TRight> LookUp<TRight>()
        {
            var join = Join<TRight>.From(this);
            this._joins.Add(typeof(TRight), join);

            return join;
        }


        public IList<T> Execute<T>(Func<IDictionary<string, object>, T> selector)
            where T : class
        {
            if (null == selector)
                return null;

            var dic = MongoAdmin.ExecuteScript(this._db, this.ToString()).ToDictionary();

            var ret = new List<T>();
            var items = ((IDictionary<string, object>)dic.First().Value).First().Value as IEnumerable;
            // if (null != items)
            foreach (var item in items)
            {
                var result = selector(item as IDictionary<string, object>);
                ret.Add(result);
            }

            return ret;
        }

        public async Task<IList<T>> ExecuteAsync<T>(Func<IDictionary<string, object>, T> selector)
            where T : class
        {
            if (null == selector)
                return null;

            var dic = (await MongoAdmin.ExecuteScriptAsync(this._db, this.ToString())).ToDictionary();

            var ret = new List<T>();
            var items = ((IDictionary<string, object>)dic.First().Value).First().Value as IEnumerable;
            // if (null != items)
            foreach (var item in items)
            {
                var result = selector(item as IDictionary<string, object>);
                ret.Add(result);
            }

            return ret;
        }


        public StringBuilder ToScript()
        {
            StringBuilder sb = new StringBuilder("db.")
                .Append(this._collectionInfo.Name)
                .Append(".aggregate([ ");

            if (null != this._match)
            {
                sb.Append(this._match.ToScript())
                    .Append(", ");
            }

            if (this._limit > 0)
            {
                sb.Append("{ $limit: ")
                    .Append(this._limit)
                    .Append("}, ");
            }
            if (this._orderBy != null)
            {
                sb.Append("{ $sort: {")
                    .Append(this._orderField)
                    .Append(":")
                    .Append(this._orderBy.Value == OrderType.Ascending ? "1" : "-1")
                    .Append("} }, ");
            }


            //joins
            foreach (var kvp in this._joins)
            {
                sb.Append(kvp.Value.ToScript()).Append(", ");
            }
            //

            if (this.fields.Count == 0)
            {
                this.fields
                    .AddRange(
                        DictionarySerializer.GetValidProperties(this._leftType)
                            .Select(kvp => kvp.Key).ToList());
            }
            if (this.fields.Count == 0)
                throw new InvalidOperationException($"{this._leftType.FullName} has no valid property.");

            sb.Append("{ $project: { ");
            foreach (var field in this.fields)
            {
                sb.Append(field).Append(" : 1, ");
            }

            //joins projects
            foreach (var kvp in this._joins)
            {
                var joinType = kvp.Key;
                var projectName = MongoExtensions.GetCollectionInfo(joinType).Name;
                sb.Append(projectName)
                    .Append(" : {");
                foreach (var fieldName in DictionarySerializer.GetValidProperties(joinType)
                    .Select(p => DictionarySerializer.GetFieldName(p.Value)))
                {
                    sb.Append(fieldName)
                        .Append(" : 1, ");
                }

                if (sb[sb.Length - 2] == ',')
                    sb.Remove(sb.Length - 2, 2);

                sb.Append(" }, ");

            }
            //

            sb.Remove(sb.Length - 2, 2);

            sb.Append(" } }");


            if (sb[sb.Length - 2] == ',')
                sb.Remove(sb.Length - 2, 2);

            sb.Append("], {cursor: { batchSize: ")
                .Append(this._batchSize)
                .Append(" }} );");

            return sb;
        }

        public override string ToString()
        {
            return this.ToScript().ToString();
        }
    }
}
