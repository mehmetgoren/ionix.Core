using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SQLogTests
{
    [TestClass]
    public class LoggerTests
    {
        [TestMethod]
        public async Task SaveTest()
        {

            for (int j = 0; j < 10; ++j)
            {
                Logger.Create(new StackTrace()).Message("Save default " + j).Save();
                //Logger.Create(new StackTrace()).Message("Save on other thread " + j).Save();
                await Logger.Create(new StackTrace()).Message("Save on other thread " + j).SaveAsync();
            }


            await Logger.Create(new StackTrace()).Check(() =>
            {
                throw new InvalidOperationException("Oh deme püf de");
            }).SaveAsync();


            Assert.IsTrue(true);
        }
    }
}
