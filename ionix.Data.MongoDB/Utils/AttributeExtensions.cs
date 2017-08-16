namespace ionix.Data.Mongo
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;

    public static class AttributeExtensions
    {
        public static string GetCollectionName(Type type)
        {
            if (null != type)
            {
                var attr = type.GetTypeInfo().GetCustomAttribute<MongoCollectionAttribute>();
                return null != attr && !String.IsNullOrEmpty(attr.Name) ? attr.Name : type.Name;
            }

            return String.Empty;
        }

        public static string Script(this MongoCollectionAttribute attr, Type owner)
        {
            if (null != attr)
            {
                if (String.IsNullOrEmpty(attr.Name))
                {
                    attr.Name = owner.Name;
                }

                StringBuilder sb = new StringBuilder().Append($"db.createCollection('{attr.Name}', {{ ")
                    .Append(" autoIndexId: ").Append(attr.AutoIndexId.ToString().ToLower());

                if (attr.Size > 0)
                {
                    sb.Append(", size: ").Append(attr.Size);
                }
                if (attr.Max > 0)
                {
                    sb.Append(", max: ").Append(attr.Max);
                }

                sb.Append("}")
                    .Append(" )");

                return sb.ToString();
            }

            return String.Empty;
        }


        public static IEnumerable<string> Scripts(this IEnumerable<MongoIndexAttribute> attrList, Type owner)
        {
            List<string> scripts = new List<string>();
            if (null != attrList && attrList.Any() && null != owner)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var attr in attrList)
                {
                    if (null != attr.Fields && attr.Fields.Any())
                    {
                        string name = attr.Name;
                        if (String.IsNullOrEmpty(name))
                        {
                            foreach (var field in attr.Fields)
                            {
                                name += field + '_';
                            }
                            name = name.Remove(name.Length - 1, 1);
                        }

                        sb.Append("db.")
                        .Append(GetCollectionName(owner))
                        .Append(".createIndex( { ");
                        foreach (var field in attr.Fields)
                        {
                            sb.Append(field)
                                .Append(": 1, ");
                        }
                        sb.Remove(sb.Length - 2, 2)
                        .Append(" }, { ")//options
                        .Append("name: '")
                        .Append(name)
                        .Append("', ")
                        .Append("unique: ")
                        .Append(attr.Unique.ToString().ToLower())
                        .Append(" }")
                        .Append(" )");

                        scripts.Add(sb.ToString());
                    }
                    sb.Length = 0;
                }
            }

            return scripts;
        }

        public static string Script(this MongoTextIndexAttribute attr, Type owner)
        {
            string script = String.Empty;
            if (null != attr && null != owner)
            {
                StringBuilder sb = new StringBuilder();
                if (null != attr.Fields && attr.Fields.Any())
                {
                    string name = attr.Name;
                    if (String.IsNullOrEmpty(name))
                    {
                        name = "txtIndex_";
                        foreach (var field in attr.Fields)
                        {
                            name += field + '_';
                        }
                        name = name.Remove(name.Length - 1, 1);
                    }

                    sb.Append("db.")
                        .Append(GetCollectionName(owner))
                        .Append(".createIndex( { ");
                    foreach (var fieldOrginal in attr.Fields)
                    {
                        var field = fieldOrginal;
                        if (field == "*")
                        {
                            field = "'$**'";
                            name = "txtIndex_All";
                        }

                        sb.Append(field)
                            .Append(": 'text', ");
                    }
                    sb.Remove(sb.Length - 2, 2)
                        .Append(" }, { ")//options
                        .Append("name: '")
                        .Append(name)
                        .Append("', ")
                        .Append("unique: ")
                        .Append(attr.Unique.ToString().ToLower())
                        .Append(", ")
                        .Append("default_language: '")
                        .Append(attr.DefaultLanguage)
                        .Append("'")
                        .Append(" }")
                        .Append(" )");

                    script = sb.ToString();
                }
            }

            return script;
        }
    }
}
