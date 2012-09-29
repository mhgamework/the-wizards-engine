using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Synchronization;
using NUnit.Framework;
using Rhino.Mocks;

namespace MHGameWork.TheWizards.Tests.Gameplay
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

        [SetUp]
        public void SetUp()
        {
            SimpleModelObject.CurrentModelContainer = new Data.ModelContainer();

            object1 = new TestObject
                        {
                            Getal = 32,
                            Mesh = new RAMMesh()
                        };
            object2 = new TestObject() { Getal = 2 };
            object1.Object = object2;



        }

        [Test]
        public void TestSerializeModelObjectInvalidDependency()
        {
            var s = new ModelSerializer(StringSerializer.Create(), mockAssetFactory());

            var strm = new MemoryStream();

            var writer = new StreamWriter(strm);
            s.SerializeObject(object1, writer);

            writer.Flush();

            strm.Position = 0;

            // For testing
            string serialized = getStringFromStream(strm);


            s = new ModelSerializer(StringSerializer.Create(), mockAssetFactory());

            var list = new List<ModelSerializer.ObjectBinding>();

            var obj = s.DeserializeObject(new StreamReader(strm), list);

            Assert.AreEqual(1, list.Count);


        }

        [Test]
        public void TestSerializeModelObject()
        {
            var s = new ModelSerializer(StringSerializer.Create(), mockAssetFactory());

            object1.Object = null; // Remove unresolvable dependency

            var strm = new MemoryStream();

            var writer = new StreamWriter(strm);
            s.SerializeObject(object1, writer);

            writer.Flush();

            strm.Position = 0;

            // For testing
            string serialized = getStringFromStream(strm);


            var objectBindings = new List<ModelSerializer.ObjectBinding>();
            var deserialized = s.DeserializeObject(new StreamReader(strm), objectBindings);

            Assert.AreEqual(0, objectBindings.Count);

            Assert.AreEqual(object1.GetType(), deserialized.GetType());
            Assert.AreEqual(object1.ToString(), deserialized.ToString());



        }

        [Test]
        public void TestSerializeModel()
        {
            IAssetFactory assetFactory = mockAssetFactory();
            var s = new ModelSerializer(StringSerializer.Create(), assetFactory);

            var strm = new MemoryStream();

            var writer = new StreamWriter(strm);
            s.Serialize(model, writer);


            var deserialized = new Data.ModelContainer();

            writer.Flush();

            strm.Position = 0;

            // For testing
            string serialized = getStringFromStream(strm);

            SimpleModelObject.CurrentModelContainer = deserialized;

            s.Deserialize(deserialized, new StreamReader(strm));


            Assert.AreEqual(model.Objects.Count, deserialized.Objects.Count);
            Assert.AreEqual(model.Objects[0].ToString(), deserialized.Objects[0].ToString());
            Assert.AreEqual(model.Objects[1].ToString(), deserialized.Objects[1].ToString());

        }
        [Test]
        public void TestPersistAttributeTypeScope()
        {
            IAssetFactory assetFactory = mockAssetFactory();
            var s = new ModelSerializer(StringSerializer.Create(), assetFactory);

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

            s.Deserialize(deserialized, new StreamReader(strm));


            Assert.AreNotEqual(model.Objects.Count, deserialized.Objects.Count);
            Assert.AreEqual(model.Objects[0].ToString(), deserialized.Objects[0].ToString());
            Assert.AreEqual(model.Objects[1].ToString(), deserialized.Objects[1].ToString());

        }


        private string getStringFromStream(MemoryStream strm)
        {
            var buff = new byte[strm.Length];
            strm.Read(buff, 0, (int)strm.Length);
            strm.Position = 0;
            return Encoding.ASCII.GetString(buff);
        }

        private IAssetFactory mockAssetFactory()
        {
            var assetFactory = MockRepository.GenerateStub<IAssetFactory>();
            assetFactory.Stub(o => o.GetAsset(null, object1.Mesh.Guid)).Return(object1.Mesh);
            return assetFactory;
        }

        [Persist]
        private class TestObject : SimpleModelObject
        {
            public int Getal { get; set; }
            public TestObject Object { get; set; }
            public IMesh Mesh { get; set; }

            public override string ToString()
            {
                return String.Format("Getal: {0}, Object: {1}, Mesh: {2}", Getal, Object, Mesh);
            }
        }

    }
}
