namespace ionix.MongoTests
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using ionix.Data.Mongo;


    [MongoCollection(Database = DbContext.DatabaseName, Name = "Person")]
    [MigrationVersion(Migration100.VersionNo)]
    [MongoIndex("Name", Unique = true)]
    [MongoTextIndex("*")]
    //[BsonIgnoreExtraElements]
    public class Person
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; } = true;

        public string Description { get; set; }
    }
}
