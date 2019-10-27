namespace Ionix.Data
{
    using Utils;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class PropertyMetaData : IEquatable<PropertyMetaData>, IPrototype<PropertyMetaData>
    {
        public PropertyMetaData(SchemaInfo schema, PropertyInfo property)
        {
            if (null == schema)
                throw new ArgumentNullException(nameof(schema));
            if (null == property)
                throw new ArgumentNullException(nameof(property));

            this.Schema = schema;
            this.Property = property;
        }

        public SchemaInfo Schema { get; }

        public PropertyInfo Property { get; }

        //Cache lenen nesnelerde schema info ve parametre isminin değişmesi sıkıntı çıkartıyor(ki command nesneleri bu edğişkikliği yapıyor.)
        public PropertyMetaData Copy()
        {
            PropertyMetaData copy = new PropertyMetaData(this.Schema.Copy(), this.Property);//property zaten readonly bir object. tüm prop lar readonly.
            //ParameterName kopyalanmamalı.
            return copy;
        }

        public bool Equals(PropertyMetaData other)
        {
            if (null != other)
                return this.Schema.Equals(other.Schema);
            return false;
        }
        public override bool Equals(object obj)
        {
            return this.Equals(obj as PropertyMetaData);
        }
        public override int GetHashCode()
        {
            return this.Schema.GetHashCode();
        }

        public override string ToString()
        {
            return this.Schema.ColumnName;
        }
    }

    public class EntityMetaData : IEntityMetaData
    {
        private readonly Dictionary<string, PropertyMetaData> dic;

        public EntityMetaData(Type entityType, string tableName)
        {
            if (null == entityType)
                throw new ArgumentNullException(nameof(entityType));
            if (String.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            this.dic = new Dictionary<string, PropertyMetaData>();
            this.EntityType = entityType;
            this.TableName = tableName;
        }
        public EntityMetaData(Type entityType)
            : this(entityType, AttributeExtension.GetTableName(entityType))
        {

        }

        internal void Add(SchemaInfo schema, PropertyInfo property)
        {
            this.dic.Add(schema.ColumnName, new PropertyMetaData(schema, property));
        }

        public string TableName { get; }

        public Type EntityType { get; }

        public IEnumerable<PropertyMetaData> Properties => this.dic.Values;

        public int Count => this.dic.Count;

        public string GetParameterName(PropertyMetaData pm, int index)
        {
            int factor = this.dic.Count * index;
            return (factor + pm.Schema.Order - 1).ToString();
        }

        public IEntityMetaData Copy()
        {
            EntityMetaData copy = new EntityMetaData(this.EntityType, this.TableName);//Type ReadOnly bir object dir. String de fixed char* kullanılmıyorsa immutable bir nesnedir.
            foreach (KeyValuePair<string, PropertyMetaData> orginal in this.dic)
            {
                copy.dic.Add(orginal.Key, orginal.Value.Copy());
            }

            return copy;
        }

        public override string ToString()
        {
            return this.TableName;
        }

        public PropertyMetaData this[string columnName]
        {
            get
            {
                PropertyMetaData p;
                this.dic.TryGetValue(columnName, out p);
                return p;
            }
        }
    }
}
