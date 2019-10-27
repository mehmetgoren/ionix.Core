namespace Ionix.MongoTests
{
    using Ionix.Data.Mongo;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Bson;

    [MongoCollection(Name = "Address")]
    [MigrationVersion(Migration100.VersionNo)]
    [MongoIndex("Name")]
    public class Address
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Street { get; set; }

        public string HouseNumber { get; set; }
    }
}
