using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Ideas
{
    /// <summary>
    /// This is an idea to attach data to any object, as to easy write algorithms that need to store additional data about objects.
    /// It uses the weakobjecttable to store the data.
    /// 
    /// </summary>
    [TestFixture]
    public class DynamicObjectsTest
    {
        /// <summary>
        /// Attach data to objects using a global weaktable!
        /// </summary>
        [Test]
        public void TestFirstMethod()
        {
            var obj = new object();

            obj.Set(new MyData());

            var data = obj.Get<MyData>();
        }

        private class MyData
        {
            public int getal = 78;
        }




        /// <summary>
        /// Another idea is to additionaly scope the data that can be stored within objects, by using an storage object in the using class.
        /// </summary>
        [Test]
        public void TestLocalWeaktable()
        {
            var table = new AddonStore();
            var obj = new object();

            obj.Set(table, new MyData());

            var data = obj.Get<MyData>(table);
        }


        /// <summary>
        /// The third idea is to use postsharp, to automatically use the correct addon store in each class!
        /// </summary>
        [Test]
        public void TestPostsharpAddonStore()
        {
            throw new NotImplementedException();
            var table = new AddonStore();
            // TODO: postsharp shizzles?
            var obj = new object();

            obj.Set(new MyData());

            var data = obj.Get<MyData>();
        }




    }
    public class AddonStore
    {
        private ConditionalWeakTable<object, Dictionary<Type, object>> table = new ConditionalWeakTable<object, Dictionary<Type, object>>();

        public ConditionalWeakTable<object, Dictionary<Type, object>> Table
        {
            get { return table; }
        }
    }
    public static class AddonExtensions
    {
        private static ConditionalWeakTable<object, Dictionary<Type, object>> table = new ConditionalWeakTable<object, Dictionary<Type, object>>();
        public static T Get<T>(this object obj) where T : class
        {
            var dict = table.GetOrCreateValue(obj);
            return dict[typeof(T)] as T;
        }
        public static void Set<T>(this object obj, T value)
        {
            var dict = table.GetOrCreateValue(obj);
            dict[typeof(T)] = value;
        }




        public static T Get<T>(this object obj,AddonStore store) where T : class
        {
            var dict = store.Table.GetOrCreateValue(obj);
            return dict[typeof(T)] as T;
        }
        public static void Set<T>(this object obj, AddonStore store, T value)
        {
            var dict = store.Table.GetOrCreateValue(obj);
            dict[typeof(T)] = value;
        }
    }

}