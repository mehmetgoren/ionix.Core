namespace ionix.Data.Mongo
{
    using System;
    using System.Linq.Expressions;
    using System.Text;

    partial class Lookup<TLeft>
    {
        public sealed class Join<TRight> : IMongoDbScriptProvider
        {
            public static Join<TRight> From(Lookup<TLeft> parent)
            {
                return new Join<TRight>(parent);
            }


            private readonly Lookup<TLeft> _parent;
            private readonly string _from;

            private Join(Lookup<TLeft> parent)
            {
                this._parent = parent;

                var names = MongoExtensions.GetNames(typeof(TRight));
                this._from = names.Name;
                this._as = names.Name;
            }

            private string _localField;
            public Join<TRight> LocalField(Expression<Func<TLeft, object>> exp)
            {
                if (null == exp)
                    throw new ArgumentNullException(nameof(exp));

                this._localField = GetFieldName(exp);

                return this;
            }


            private string _foreignField;
            public Join<TRight> ForeignField(Expression<Func<TRight, object>> exp)
            {
                if (null == exp)
                    throw new ArgumentNullException(nameof(exp));

                this._foreignField = GetFieldName(exp);

                return this;
            }

            private string _as;
            public Join<TRight> As(string value)
            {
                if (!String.IsNullOrEmpty(value))
                    this._as = value;

                return this;
            }


            public Lookup<TLeft> EndLookup()
            {
                return this._parent;
            }

            public StringBuilder ToScript()
            {
                StringBuilder sb = new StringBuilder()
                    .Append("{  $lookup: { from: '")
                    .Append(this._from)
                    .Append("', localField: '")
                    .Append(this._localField)
                    .Append("', foreignField: '")
                    .Append(this._foreignField)
                    .Append("', as: '")
                    .Append(this._as)
                    .Append("' } }")
                    .Append(", {  $unwind: '$")
                    .Append(this._as).Append("' }");

                return sb;
            }
        }
    }
}
