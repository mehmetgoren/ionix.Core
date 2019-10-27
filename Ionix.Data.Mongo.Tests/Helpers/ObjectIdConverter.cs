namespace Ionix.MongoTests
{
    using MongoDB.Bson;
    using System;
    using Newtonsoft.Json;

    public class ObjectIdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string strValue = value == null ? String.Empty : value.ToString();
            serializer.Serialize(writer, strValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,JsonSerializer serializer)
        {
            object value = reader.Value;
            if (null != value)
            {
                ObjectId id;
                if (ObjectId.TryParse(value.ToString(), out id))
                {
                    return id;
                }
            }

            if (objectType == TypeObjectIdNullable)
                return null;  
           return ObjectId.Empty;
        }

        private static readonly Type TypeObjectId = typeof(ObjectId);
        private static readonly Type TypeObjectIdNullable = typeof(ObjectId?);

        public override bool CanConvert(Type objectType)
        {
            return (objectType == TypeObjectId || objectType == TypeObjectIdNullable);
        }
    }
}
