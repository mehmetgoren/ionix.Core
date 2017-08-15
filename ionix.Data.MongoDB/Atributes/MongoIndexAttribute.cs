namespace ionix.Data.Mongo
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// refer to https://docs.mongodb.com/manual/reference/method/db.collection.createIndex/ for more information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class MongoIndexAttribute : Attribute
    {
        public MongoIndexAttribute()
        {

        }

        public MongoIndexAttribute(params string[] fields)
        {
            this.Fields = fields;
        }

        public IEnumerable<string> Fields { get; set; } //şu an hepsi ascending yani 1.

        public string Name { get; set; }

        public bool Unique { get; set; }

        // public bool Background { get; set; }
    }
}
