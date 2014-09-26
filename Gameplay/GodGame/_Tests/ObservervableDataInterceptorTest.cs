using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.Persistence;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    [TestFixture]
    public class ObservervableDataInterceptorTest
    {
        [Test]
        public void TestStoreRetrieve()
        {
            var dataStore = new Dictionary<string, object>();
            var data = ObservervableDataInterceptor.CreateObservervableData<ITestData>(dataStore);

            data.Number = 5;
            data.Name = "MH";
            Assert.AreEqual(5, data.Number);
            Assert.AreEqual("MH", data.Name);
        }


        public interface ITestData
        {
            int Number { get; set; }
            string Name { get; set; }
        }
    }
}