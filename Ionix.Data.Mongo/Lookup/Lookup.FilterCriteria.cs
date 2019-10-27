namespace Ionix.Data.Mongo
{
    using Ionix.Utils.Reflection;
    using MongoDB.Bson;
    using System;
    using System.Collections.Generic;
    using System.Text;

    partial class Lookup<TLeft>
    {
        //Bunu Left table kullanacak.
        private sealed class FilterCriteria : IMongoDbScriptProvider //Array lerde Kullanılacağı İçin Struct yapılmadı.
        {
            private readonly string _propertyName;
            private readonly ConditionOperator _op;
            private readonly IList<object> _values;

            public FilterCriteria(string propertyName, ConditionOperator op, params object[] values)
            {
                this.CheckParamaters(propertyName, op, values);

                this._propertyName = propertyName;
                this._op = op;
                this._values = new List<object>(values);
            }

            private void CheckParamaters(string propertyName, ConditionOperator op, object[] values)
            {
                if (String.IsNullOrEmpty(propertyName))
                    throw new ArgumentNullException(nameof(propertyName));
                if (null == values)
                    throw new ArgumentNullException(nameof(values));
                if (values.Length == 0)
                    throw new ArgumentException("'values.Length' must be greater than zero");
                //for (int j = 0; j < values.Length; ++j)
                //{
                //    if (values[j] == null)
                //        throw new NullReferenceException($"'values[{j}]'");
                //}
                if (op == ConditionOperator.Between)
                {
                    if (values.Length < 2)
                        throw new ArgumentException(
                            $"'Between' Operatörü İçin Eksik Sayıda Parametre --> 'values.Legth = '{values.Length}'");
                    if (values.Length > 2)
                        throw new ArgumentException(
                            $"'Between' Operatörü İçin Fazla Sayıda Parametre --> 'values.Legth = '{values.Length}'");
                }
                else if (op != ConditionOperator.In && values.Length > 1)
                {
                    throw new ArgumentException($"'{op}' Operatörü İçin sadece Bir Adet Parametre Girilebilir.");
                }
            }


            public StringBuilder ToScript()
            {
                var m = new StringBuilder()
                    .Append(this._propertyName)
                    .Append(":");


                bool closeScope = true;
                string opString = this.GetOperatorScript();
                if (null != opString)
                {
                    m.Append("{ ")
                        .Append(opString);
                }
                else if (this._op == ConditionOperator.Equals ||
                         this._op == ConditionOperator.StartsWith ||
                         this._op == ConditionOperator.Contains ||
                         this._op == ConditionOperator.EndsWith)
                {
                    closeScope = false;
                    switch (this._op)
                    {
                        case ConditionOperator.Equals:
                            m.Append(GetParameterValue(this._values[0]));
                            break;
                        case ConditionOperator.Contains:
                            m.Append("/").Append(this._values[0]).Append("/i");
                            break;
                        case ConditionOperator.StartsWith:
                            m.Append("/^").Append(this._values[0]).Append("/i");
                            break;
                        case ConditionOperator.EndsWith:
                            m.Append("/").Append(this._values[0]).Append("$/i");
                            break;
                    }
                }
                else
                {
                    m.Append("{ ");
                    switch (this._op)
                    {
                        case ConditionOperator.In:
                            m.Append("$in: [ ");

                            int j = 0;
                            for (; j < this._values.Count - 1; ++j)
                            {
                                m.Append(GetParameterValue(this._values[j])).Append(", ");
                            }
                            m.Append(GetParameterValue(this._values[j]));
                            m.Append(" ]");
                            break;
                        case ConditionOperator.Between:
                            m.Append("$gte: ").Append(GetParameterValue(this._values[0]))
                                .Append(", $lt: ").Append(GetParameterValue(this._values[0]));
                            break;
                        default:
                            throw new NotSupportedException(this._op.ToString());
                    }
                }
                if (closeScope)
                    m.Append(" }");

                return m;
            }

            private string GetOperatorScript()
            {
                switch (this._op)
                {
                    case ConditionOperator.NotEquals:
                        return "$ne: " + GetParameterValue(this._values[0]);
                    case ConditionOperator.GreaterThan:
                        return "$gt: " + GetParameterValue(this._values[0]);
                    case ConditionOperator.LessThan:
                        return "$lt: " + GetParameterValue(this._values[0]);
                    case ConditionOperator.GreaterThanOrEqualsTo:
                        return "$gte: " + GetParameterValue(this._values[0]);
                    case ConditionOperator.LessThanOrEqualsTo:
                        return "$lte: " + GetParameterValue(this._values[0]);
                    default:
                        return null;
                }
            }

            private static readonly HashSet<Type> CachedTypes = new HashSet<Type>()
            {
                typeof(String),
                typeof(Guid),
                typeof(Guid?),
                typeof(DateTime),
                typeof(DateTime?)
            };

            private static string GetParameterValue(object value)
            {
                if (null == value)
                    return "null";

                if (ReflectionExtensions.IsEnumerable(value.GetType()))
                    return null;

                var type = value.GetType();
                if (CachedTypes.Contains(type))
                {
                    if (type == typeof(DateTime) || type == typeof(DateTime?))
                        return "ISODate('" + value + "')";
                    return "'" + value + "'";
                }
                else if (value is ObjectId)
                    return "ObjectId('" + value + "')";
                else if (value is Boolean)
                    return value.ToString().ToLower();

                return value.ToString();
            }
        }


        private enum ConditionOperator : int
        {
            Equals = 0,
            NotEquals = 1,
            GreaterThan = 2,
            LessThan = 3,
            GreaterThanOrEqualsTo = 4,
            LessThanOrEqualsTo = 5,
            In = 6,
            Between = 7,
            Contains = 8,
            StartsWith = 9,
            EndsWith = 10
        }

        private enum OrderType
        {
            Ascending,
            Descending
        }
    }

    //or ve and bir üst seviyede işlenecek.
}
