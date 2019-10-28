namespace Ionix.Data
{
    using Utils;
    using Utils.Extensions;
    using System.Text;
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Collections.Generic;

    public abstract class DbValueSetter
    {
        private static readonly CultureInfo NumericCulture = new CultureInfo("en-US");

        private static readonly HashSet<Type> WithQuotes = new HashSet<Type>() { CachedTypes.String, CachedTypes.DateTime, CachedTypes.Guid };

        public abstract char Prefix { get; }

        public virtual void SetColumnValue(IEntityMetaData metaData, int index, SqlQuery query, PropertyMetaData pm, object entity)//Parametewnin eklenip eklenmeyeceği bilinmdeğinden prefix ve entity verilmek zorunda.
        {
            SchemaInfo schema = pm.Schema;
            PropertyInfo pi = pm.Property;
            object parValue = pm.Property.GetValue(entity, null);

            StringBuilder text = query.Text;
            if (schema.DefaultValue.Length != 0)
            {
                object defaultValue = ReflectionExtensions.GetDefault(pi.PropertyType);
                if (Object.Equals(defaultValue, parValue))//Eğer Property Değeri Default Haldeyse yazdır Bunu
                {
                    text.Append(schema.DefaultValue);
                    return;
                }
            }

            switch (schema.SqlValueType)
            {
                case SqlValueType.Parameterized:
                    text.Append(this.Prefix);

                    string parameterName = metaData.GetParameterName(pm, index);
                    text.Append(parameterName);

                    SqlQueryParameter par = SqlQueryParameter.Create(parameterName, pm, parValue);

                    query.Parameters.Add(par);
                    break;
                case SqlValueType.Text:
                    string textValue = null;
                    if (null != parValue)
                    {
                        Type dataType = pm.Schema.DataType;//Neden Schema.DataType çünkü pi.PropertyType nullable olabalir.
                        if (WithQuotes.Contains(dataType))
                        {
                            textValue = "'" + parValue + "'";
                        }
                        else
                        {
                            IFormattable f = parValue as IFormattable;
                            textValue = null != f ? f.ToString(null, NumericCulture) : parValue.ToString();
                        }
                    }
                    text.Append(textValue);
                    break;
                default:
                    throw new NotSupportedException(schema.SqlValueType.ToString());
            }
        }

    }
}
