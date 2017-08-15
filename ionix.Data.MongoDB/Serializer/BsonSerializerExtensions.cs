namespace ionix.Data.Mongo.Serializers
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;

    public static class BsonSerializerExtensions
    {
        public static string BsonSerialize(this object obj)
        {
            return obj?.ToBsonDocument().ToString();
        }

        public static T BsonDeserialize<T>(this string bson)
        {
           return BsonSerializer.Deserialize<T>(bson);
        }
    }
}
