namespace Ionix.Data.Tests
{
    using FluentAssertions;
    using Ionix.Data;
    using Ionix.Data.Tests.SqlServer;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Linq;
    using Xunit;

    public class ExtensionsTests
    {
        [Fact]
        public void CopyTest()
        {
            IEntityMetaDataProvider provider = new DbSchemaMetaDataProvider();

            EntityMetaData metaData = (EntityMetaData)provider.CreateEntityMetaData(typeof(Invoices));

            var copy = metaData.Copy();

            int len = metaData.Properties.Count();

            Stopwatch bench = Stopwatch.StartNew();
            for (int j = 0; j < 100; ++j)
                metaData.Copy();
            bench.Stop();

            Debug.WriteLine("Schema Xml Copy: " + bench.ElapsedMilliseconds);

            metaData.Should().NotBeNull();
        }

        [Fact]
        public void ValidationTest()
        {
            Employees e = new Employees();
            e.LastName = "LastName";
            e.FirstName = "FirstName";
            e.BirthDate = DateTime.Now;
            e.HireDate = DateTime.Now;
            e.Title = Guid.NewGuid().ToString().Substring(0, 10); //+ Guid.NewGuid().ToString() + Guid.NewGuid().ToString() +
                      //Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

            bool isValid = e.IsModelValid();// EntityMetadaExtensions.IsModelValid(e);


            var m = new ValidationTestModel();
            m.Port = 1003;// 1000000000;
            m.Email = "gecelerinyargici_19@mynet.com";// "gecelerinyargici_19#mynet.com";
            m.Description = "a test object";//null;
            m.Code = "123";// "123456";

            isValid = m.IsModelValid() && isValid;

            isValid.Should().BeTrue();
        }
    }

    public sealed class ValidationTestModel
    {
        [Range(0, short.MaxValue)]
        public int Port { get; set; }

        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")]
        public string Email { get; set; }

        [Required]
        public string Description { get; set; }

        [StringLength(5)]
        public string Code { get; set; }
    }
}
