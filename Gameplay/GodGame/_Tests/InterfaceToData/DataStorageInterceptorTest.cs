using System.Collections.Generic;
using Castle.DynamicProxy;
using MHGameWork.TheWizards.GodGame._Engine.IntefaceToData;
using NUnit.Framework;
using System;

namespace MHGameWork.TheWizards.GodGame._Tests.InterfaceToData
{
    [TestFixture]
    public class DataStorageInterceptorTest
    {
        private Dictionary<string, object> values = new Dictionary<string, object>();
        private ITestData createData()
        {
            var dataStore = new ObjectStorage(s => values[s], (s, v) => values[s] = v);
            var observable = DataStorageInterceptor<ITestData>.ImplementInterface(dataStore, new ProxyGenerator());
            return observable;
        }
        [Test]
        public void TestStoreRetrieve()
        {
            var data = createData();

            data.Number = 5;
            data.Name = "MH";
            Assert.AreEqual(5, data.Number);
            Assert.AreEqual("MH", data.Name);
        }

        [Test]
        public void TestDatastore()
        {
            var data = createData();

            data.Number = 5;
            data.Name = "MH";
            Assert.AreEqual(5, values["Number"]);
            Assert.AreEqual("MH", values["Name"]);
        }

        [Test]
        public void TestStoreArray()
        {
            var data = createData();

            data.Numbers = new int[5];
            data.Numbers[4] = 3;
            Assert.AreEqual(5, data.Numbers.Length);
            Assert.AreEqual(data.Numbers[0], 0);
            Assert.AreEqual(data.Numbers[4], 3);
        }

        [Test]
        public void TestStoreStruct()
        {
            var data = createData();

            data.Struct = new MyStruct() { Num1 = 1, Num2 = 2 };

            Assert.AreEqual(1, data.Struct.Num1);
            Assert.AreEqual(2, data.Struct.Num2);

            data.Struct = new MyStruct() { Num1 = 1, Num2 = 200 };

            Assert.AreEqual(1, data.Struct.Num1);
            Assert.AreEqual(200, data.Struct.Num2);
        }

        public interface ITestData
        {
            int Number { get; set; }
            string Name { get; set; }
            int[] Numbers { get; set; }
            MyStruct Struct { get; set; }
        }

        public struct MyStruct
        {
            public int Num1;
            public int Num2;
        }
    }
}