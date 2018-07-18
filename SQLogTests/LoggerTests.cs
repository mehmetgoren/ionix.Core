using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ionix.Data;

namespace SQLogTests
{
    [TestClass]
    public class LoggerTests
    {
        [TestMethod]
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


            Assert.IsTrue(true);
        }

        [TestMethod]
        public void ReadTest()
        {
            var q = "select * from Log t".ToQuery();
            var list = SQLog.Logger.Logs.Query(q);

            Assert.AreNotEqual(list.Count, 0);
        }
    }
}
