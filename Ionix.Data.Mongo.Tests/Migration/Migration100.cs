namespace Ionix.MongoTests
{
    using System.Linq;
    using System.Text;

    public class Migration100 : MigrationBase
    {
        public const string VersionNo = "1.0.0";

        public Migration100()
            : base(VersionNo)
        {
        }

        public override string GenerateMigrationScript()
        {
            var libAssembly = GetMigrationsAssembly();

            StringBuilder sb = new StringBuilder();
            var types = libAssembly.GetTypes()
              //  .Where(t => !t.Name.StartsWith("MngVul"))
                .ToList();
            types.ForEach(type => sb.Append(GetModelCreateScript(type)));
            return sb.ToString();
        }
    }
}
