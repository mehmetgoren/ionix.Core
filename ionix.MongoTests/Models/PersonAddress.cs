namespace ionix.MongoTests
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using ionix.Data.Mongo;

    [MongoCollection(Database = DbContext.DatabaseName, Name = "PersonAddress")]
    [MigrationVersion(Migration100.VersionNo)]
    [MongoIndex("PersonId", "AddressId")]
    [MongoIndex("AddressId")]
    [MongoIndex("AddressId")]
    public class PersonAddress
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public ObjectId PersonId { get; set; }

        public ObjectId AddressId { get; set; }

        public bool Active { get; set; } = true;
    }
}
