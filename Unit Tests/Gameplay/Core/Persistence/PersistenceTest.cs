using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Serialization;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Gameplay.Core.Persistence
{
    /// <summary>
    /// These tests smear across layers and should be fixed
    /// </summary>
    [TestFixture]
    public class PersistenceTest
    {
        private Data.ModelContainer model;
        private TestObject object1;
        private TestObject object2;
        private StreamWriter streamWriter;

        [SetUp]
        public void SetUp()
        {
            SimpleModelObject.CurrentModelContainer = new Data.ModelContainer();
            model = SimpleModelObject.CurrentModelContainer;

            object1 = new TestObject
                        {
                            Getal = 32,
                        };
            object2 = new TestObject() { Getal = 2 };
            object1.Object = object2;



        }

        [Test]
        public void TestSerializeModelObject()
        {
            var s = new ModelSerializer(StringSerializer.Create());

            object1.Object = null; // Remove unresolvable dependency

            var strm = new MemoryStream();

            streamWriter = new StreamWriter(strm);
            var writer = new SectionedStreamWriter(streamWriter);

            s.SerializeAttributes(object1, writer);

            streamWriter.Flush();

            strm.Position = 0;

            // For testing
            string serialized = getStringFromStream(strm);


            var deserialized = new TestObject();

            s.DeserializeAttributes(deserialized, new SectionedStreamReader(new StreamReader(strm)));

            Assert.AreEqual(object1.ToString(), deserialized.ToString());



        }

        //[Test]
        //public void TestSerializeModel()
        //{
        //    var s = new ModelSerializer(StringSerializer.Create());

        //    var strm = new MemoryStream();

        //    var writer = new StreamWriter(strm);
        //    s.Serialize(model, writer);


        //    var deserialized = new Data.ModelContainer();

        //    writer.Flush();

        //    strm.Position = 0;

        //    // For testing
        //    string serialized = getStringFromStream(strm);

        //    SimpleModelObject.CurrentModelContainer = deserialized;
        //    s = new ModelSerializer(StringSerializer.Create());
        //    s.Deserialize(new StreamReader(strm));


        //    Assert.AreEqual(model.Objects.Count, deserialized.Objects.Count);
        //    Assert.AreEqual(model.Objects[0].ToString(), deserialized.Objects[0].ToString());
        //    Assert.AreEqual(model.Objects[1].ToString(), deserialized.Objects[1].ToString());

        //}

        [Test]
        public void TestSerializeArray()
        {
            var array = new TestObjectArray();
            array.Objects = new List<TestObject>();
            array.Objects.Add(object1);
            array.Objects.Add(object2);



            var s = new ModelSerializer(StringSerializer.Create());

            var strm = new MemoryStream();

            var writer = new StreamWriter(strm);
            s.Serialize(model, writer);


            var deserialized = new Data.ModelContainer();

            writer.Flush();

            strm.Position = 0;

            // For testing
            string serialized = getStringFromStream(strm);

            SimpleModelObject.CurrentModelContainer = deserialized;
            s = new ModelSerializer(StringSerializer.Create());
            s.Deserialize(new StreamReader(strm));


            Assert.AreEqual(model.Objects.Count, deserialized.Objects.Count);
            Assert.AreEqual(model.Objects[0].ToString(), deserialized.Objects[0].ToString());
            Assert.AreEqual(model.Objects[1].ToString(), deserialized.Objects[1].ToString());
            Assert.AreEqual(model.Objects[2].ToString(), deserialized.Objects[2].ToString());

        }

        [Test]
        public void TestPersistAttributeTypeScope()
        {
            var s = new ModelSerializer(StringSerializer.Create());

            // Add a new entity to the original model, it should not be serialized!
            new WorldRendering.Entity();

            var strm = new MemoryStream();

            var writer = new StreamWriter(strm);
            s.Serialize(model, writer);


            var deserialized = new Data.ModelContainer();

            writer.Flush();

            strm.Position = 0;

            // For testing
            string serialized = getStringFromStream(strm);

            SimpleModelObject.CurrentModelContainer = deserialized;

            s.Deserialize(new StreamReader(strm));


            Assert.AreNotEqual(model.Objects.Count, deserialized.Objects.Count);
            Assert.AreEqual(model.Objects[0].ToString(), deserialized.Objects[0].ToString());
            Assert.AreEqual(model.Objects[1].ToString(), deserialized.Objects[1].ToString());

        }


        [Test]
        public void TestSerializeObjectsTree()
        {
            var model = new ModelContainer();
            SimpleModelObject.CurrentModelContainer = model;

            var array1 = new TestObjectArray();
            var array2 = new TestObjectArray();

            var obj1 = new TestObject {Getal = 1};
            var obj2 = new TestObject { Getal = 2 };
            var obj3 = new TestObject { Getal = 3 };
            var obj4 = new TestObject { Getal = 4 };
            var obj5 = new TestObject { Getal = 5 };

            array1.Objects.Add(obj1);
            array2.Objects.Add(obj2);
            array2.Objects.Add(obj3);
            obj3.Object = obj4;
            obj4.Object = obj4;
            obj5.Object = obj5;


            var strm = new MemoryStream();
            var writer = new StreamWriter(strm);
            var serializer = new ModelSerializer(StringSerializer.Create());

            serializer.QueueForSerialization(array2);

            serializer.Serialize(writer);
            writer.Flush();

            strm.Position = 0;

            var str = getStringFromStream(strm);



        }

        [Test]
        public void TestSerializeCustomStringSerializerAttribute()
        {
            var model = new ModelContainer();
            SimpleModelObject.CurrentModelContainer = model;

            var obj = new TestCustomObject();
            obj.Text = "Hello ";

            var strm = new MemoryStream();
            var writer = new StreamWriter(strm);
            var serializer = new ModelSerializer(StringSerializer.Create());

            serializer.QueueForSerialization(obj);

            serializer.Serialize(writer);
            writer.Flush();

            strm.Position = 0;

            var str = getStringFromStream(strm);


            var objects = serializer.Deserialize(new StreamReader(strm));

            Assert.AreEqual(obj, objects[0]);
        }

        private string getStringFromStream(MemoryStream strm)
        {
            var buff = new byte[strm.Length];
            strm.Read(buff, 0, (int)strm.Length);
            strm.Position = 0;
            return Encoding.ASCII.GetString(buff);
        }


        private class TestCustomObject : SimpleModelObject
        {
            [CustomStringSerializer(typeof(MyCustomSerializer))]
            public string Text { get; set; }

            protected bool Equals(TestCustomObject other)
            {
                return string.Equals(Text, other.Text);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TestCustomObject) obj);
            }

            public override int GetHashCode()
            {
                return (Text != null ? Text.GetHashCode() : 0);
            }

            public class MyCustomSerializer : IConditionalSerializer
            {
                public bool CanOperate(Type type)
                {
                    return true;
                }

                public string Serialize(object obj, Type type, StringSerializer stringSerializer)
                {
                    return (string) obj + "boe";
                }

                public object Deserialize(string value, Type type, StringSerializer stringSerializer)
                {
                    return value.Substring(0, value.Length - 3);
                }
            }
        }

        [Persist]
        private class TestObject : SimpleModelObject
        {
            public int Getal { get; set; }
            public TestObject Object { get; set; }

            public override string ToString()
            {
                return String.Format("Getal: {0}, Object: {1}", Getal, Object);
            }
        }

        [Persist]
        private class TestObjectArray : SimpleModelObject
        {
            public TestObjectArray()
            {
                Objects = new List<TestObject>();
            }
            public List<TestObject> Objects { get; set; }

            public override string ToString()
            {
                return Objects.Select(o => o.ToString()).Aggregate((a, b) => a + b);
            }
        }

    }
}

