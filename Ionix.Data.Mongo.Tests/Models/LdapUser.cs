namespace Ionix.MongoTests
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Ionix.Data.Mongo;

    [MongoCollection(Name = "LdapUser")]
    [MigrationVersion(Migration100.VersionNo)]
    [MongoIndex("UserName", Unique = true)]
    [MongoTextIndex("DisplayName", "physicalDeliveryOfficeName")]
    public class LdapUser
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string EMail { get; set; }

        public string Description { get; set; }

        public bool IsMapped { get; set; }

        public string FirstName { get; set; }

        public string Lastname { get; set; }

        public string DepartmenName { get; set; }

        public string DepartmentCode { get; set; }

        public string Title { get; set; }

        public string SamaAcountName { get; set; }

        public string UserGroup { get; set; }

        public string physicalDeliveryOfficeName { get; set; }

        public object Enabled { get; set; }
    }
}
