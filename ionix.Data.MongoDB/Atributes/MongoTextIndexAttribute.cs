namespace ionix.Data.Mongo
{
    using System;
    using System.Collections.Generic;


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class MongoTextIndexAttribute : Attribute
    {
        public MongoTextIndexAttribute()
        {
        }

        public MongoTextIndexAttribute(params string[] fields)
        {
            this.Fields = fields;
        }

        public IEnumerable<string> Fields { get; set; } //şu an hepsi ascending yani 1.

        public string Name { get; set; }

        public bool Unique { get; set; }

        // public bool Background { get; set; }

        public string DefaultLanguage { get; set; } = "tr";

       // public string LanguageOverride { get; set; } = "language";
    }
}
