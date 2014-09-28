using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Engine.Persistence;
using NUnit.Framework;
using System;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    [TestFixture]
    public class ObservervableDataInterceptorTest
    {
        [Test]
        public void TestStoreRetrieve()
        {
            var data = createObservable().Target;

            data.Number = 5;
            data.Name = "MH";
            Assert.AreEqual(5, data.Number);
            Assert.AreEqual("MH", data.Name);
        }

        [Test]
        public void TestDatastore()
        {
            var data = createObservable().Target;

            data.Number = 5;
            data.Name = "MH";
            Assert.AreEqual(5, dataStore["Number"]);
            Assert.AreEqual("MH", dataStore["Name"]);
        }

        [Test]
        public void TestStoreArray()
        {
            var data = createObservable().Target;

            data.Numbers = new int[5];
            data.Numbers[4] = 3;
            Assert.AreEqual(5, data.Numbers.Length);
            Assert.AreEqual(data.Numbers[0], 0);
            Assert.AreEqual(data.Numbers[4], 3);
        }

        [Test]
        public void TestStoreStruct()
        {
            var data = createObservable().Target;

            data.Struct = new MyStruct() { Num1 = 1, Num2 = 2 };

            Assert.AreEqual(1, data.Struct.Num1);
            Assert.AreEqual(2, data.Struct.Num2);

            data.Struct = new MyStruct() { Num1 = 1, Num2 = 200 };

            Assert.AreEqual(1, data.Struct.Num1);
            Assert.AreEqual(200, data.Struct.Num2);
        }

        [Test]
        public void TestObserver()
        {
            var observable = createObservable();

            var count = 0;
            observable.Observable.Subscribe(_ => count++);

            observable.Target.Name = "Hello";
            observable.Target.Number = 5;

            Assert.AreEqual(2, count);

        }

        private static ObservervableDataInterceptor<ITestData> createObservable()
        {
            var dataStore = new Dictionary<string, object>();
            var observable = ObservervableDataInterceptor<ITestData>.CreateObservervableData(TODO);
            return observable;
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