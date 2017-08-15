namespace ionix.Data.Mongo
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using Serializers;
    using ionix.Utils.Reflection;
    using ionix.Utils.Extensions;

    partial class Lookup<TLeft>
    {
        public sealed class Filter : IMongoDbScriptProvider
        {
            private readonly StringBuilder _sb;

            private readonly Lookup<TLeft> _parent;
            internal Filter(Lookup<TLeft> parent)
            {
                this._sb = new StringBuilder();
                this._parent = parent;
            }

            public Filter And()
            {
                this._sb.Append(", ");
                return this;
            }
            public Filter Or()
            {
                throw new NotSupportedException("Not Yet");
            }
            public Filter Not()
            {
                throw new NotSupportedException("Not Yet");
            }



            private void Filtering<TValue>(ConditionOperator op, Expression<Func<TLeft, TValue>> exp, TValue value)
            {
                if (null != exp)
                {
                    PropertyInfo pi = ReflectionExtensions.GetPropertyInfo(exp.Body);
                    if (null != pi)
                    {
                        var fieldName = DictionarySerializer.GetFieldName(pi);
                        object filterValue = value;
                       // if (null == filterValue)
                          //  filterValue = null;
                       // if (null != value)
                       // {
                            FilterCriteria criteria = new FilterCriteria(fieldName, op, filterValue);
                            this._sb.Append(criteria.ToScript());

                       // }
                    }
                }
            }

            public Filter Equals<TValue>(Expression<Func<TLeft, TValue>> exp, TValue value)
            {
                this.Filtering<TValue>(ConditionOperator.Equals, exp, value);
                return this;
            }
            public Filter NotEquals<TValue>(Expression<Func<TLeft, TValue>> exp, TValue value)
            {
                this.Filtering<TValue>(ConditionOperator.NotEquals, exp, value);
                return this;
            }
            public Filter GreaterThan<TValue>(Expression<Func<TLeft, TValue>> exp, TValue value)
            {
                this.Filtering<TValue>(ConditionOperator.GreaterThan, exp, value);
                return this;
            }
            public Filter LessThan<TValue>(Expression<Func<TLeft, TValue>> exp, TValue value)
            {
                this.Filtering<TValue>(ConditionOperator.LessThan, exp, value);
                return this;
            }
            public Filter GreaterThanOrEqualsTo<TValue>(Expression<Func<TLeft, TValue>> exp, TValue value)
            {
                this.Filtering<TValue>(ConditionOperator.GreaterThanOrEqualsTo, exp, value);
                return this;
            }
            public Filter LessThanOrEqualsTo<TValue>(Expression<Func<TLeft, TValue>> exp, TValue value)
            {
                this.Filtering<TValue>(ConditionOperator.LessThanOrEqualsTo, exp, value);
                return this;
            }

            public Filter Contains(Expression<Func<TLeft, string>> exp, string value)
            {
                this.Filtering<string>(ConditionOperator.Contains, exp, value);
                return this;
            }
            public Filter StartsWith(Expression<Func<TLeft, string>> exp, string value)
            {
                this.Filtering<string>(ConditionOperator.StartsWith, exp, value);
                return this;
            }
            public Filter EndsWith(Expression<Func<TLeft, string>> exp, string value)
            {
                this.Filtering<string>(ConditionOperator.EndsWith, exp, value);
                return this;
            }


            public Filter Between<TValue>(Expression<Func<TLeft, TValue>> exp, TValue value1, TValue value2)
            {
                if (null != exp)
                {
                    PropertyInfo pi = ReflectionExtensions.GetPropertyInfo(exp.Body);
                    if (null != pi)
                    {
                        FilterCriteria criteria = new FilterCriteria(pi.Name, ConditionOperator.Between
                            , value1, value2);

                        this._sb.Append(criteria.ToScript());
                    }
                }
                return this;
            }

            public Filter In<TValue>(Expression<Func<TLeft, TValue>> exp, params TValue[] values)
            {
                if (null != exp)
                {
                    PropertyInfo pi = ReflectionExtensions.GetPropertyInfo(exp.Body);
                    if (null != pi && !values.IsEmptyList())
                    {
                        object[] arr = new object[values.Length];
                        Array.Copy(values, arr, values.Length);
                        FilterCriteria criteria = new FilterCriteria(pi.Name, ConditionOperator.In, arr);

                        this._sb.Append(criteria.ToScript());
                    }
                }
                return this;
            }

            public Lookup<TLeft> EndMatch()
            {
                return this._parent;
            }


            public StringBuilder ToScript()
            {
                StringBuilder ret = new StringBuilder();
                ret.Append("{ $match: { ").Append(this._sb)
                    .Append(" } }");
                return ret;
            }

            public override string ToString()
            {
                return this.ToScript().ToString();
            }
        }
    }
}
