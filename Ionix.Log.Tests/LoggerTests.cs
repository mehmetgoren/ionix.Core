using Ionix.Log;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Ionix.Data;
using Xunit;
using FluentAssertions;

namespace SQLogTests
{
    public class LoggerTests
    {
        [Fact]
        public async Task SaveTest()
        {
            bool enable = Logger.Enable;// path = configuration["SQLog:Path"];

            for (int j = 0; j < 10; ++j)
            {
                Logger.Create(new StackTrace()).Info("Save default " + j).Save();
                //Logger.Create(new StackTrace()).Message("Save on other thread " + j).Save();
                await Logger.Create(new StackTrace()).Info("Save on other thread " + j).SaveAsync();
            }

            await Logger.Create(new StackTrace()).Check(() =>
            {
                throw new InvalidOperationException("Oh deme püf de");
            }).SaveAsync();


            true.Should().BeTrue();
        }

        [Fact]
        public void ReadTest()
        {
            var q = "select * from Log t".ToQuery();
            var list = Logger.Logs.Query(q);

            list.Count.Should().NotBe(0);
        }
    }
}
