using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Graphics.Xna.XML;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Features.Rendering;
using MHGameWork.TheWizards.Tests.Features.Rendering.XNA;
using MHGameWork.TheWizards.Xml;
using MHGameWork.TheWizards.XML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Data.Xml
{
    [TestFixture]
    public class XmlTest
    {
        [Test]
        public void TestSerializeDeserialize()
        {
            var t = new TestSerializeClass();
            t.Number = 3.14159265358979323846264f;
            t.Text = "Hellow";
            t.Color = Color.Yellow;
            t.FloatArray = new float[] { 1, 1, 2, 3, 5, 8, 13 };
            t.FloatList = new List<float>();
            t.FloatList.AddRange(t.FloatArray);
            t.Enum = FileAccess.ReadWrite;
            t.Null = null;


            t.NumberProperty = 42;

            t.Position = new Vector3(1, 2, 3);
            t.NestedClass = new TestSerializeNestedClass { Getal = 113 };





            var serializer = new TWXmlSerializer<TestSerializeClass>();

            string path = TWDir.Test.CreateSubdirectory("XmlTest") + "\\Test.xml";

            using (var fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Delete))
            {
                serializer.Serialize(t, fs);
            }

            var tRead = new TestSerializeClass();
            using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                serializer.Deserialize(tRead, fs);
            }

            Assert.AreEqual(t, tRead);
        }

      [Test]
        public void TestCustomSerializer()
        {
            var container = new SimpleContainerClass();
            container.Simple = new SimpleClass();

            var serializer = new TWXmlSerializer<SimpleContainerClass>();
            serializer.AddCustomSerializer(new SimpleCustomSerializer());

            var mem = new MemoryStream();
            serializer.Serialize(container, mem);
            mem.Seek(0, SeekOrigin.Begin);
            var cont2 = new SimpleContainerClass();
            serializer.Deserialize(cont2, mem);

            Assert.NotNull(cont2.Simple);
        }

        /// <summary>
        /// Move to rendering tests?
        /// </summary>
        [Test]
        public void TestSerializeDeserializeAdvanced()
        {
            var converter = new OBJToRAMMeshConverter(new RAMTextureFactory());

            var mesh = DefaultMeshes.CreateMerchantsHouseMesh(converter);

            var data = mesh.GetCoreData();

            //serializeCoreData(data);
            serializeGeometryData(data);
        }

        private void serializeCoreData(MeshCoreData data)
        {
            var serializer = new TWXmlSerializer<MeshCoreData>();
            serializer.AddCustomSerializer(AssetSerializer.CreateSerializer());

            string path = TWDir.Test.CreateSubdirectory("XmlTest") + "\\CoreData.xml";

            using (var fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Delete))
            {
                serializer.Serialize(data, fs);
            }

            /*serializer = new TWXmlSerializer<MeshCoreData>();
            serializer.AddCustomSerializer(AssetSerializer.CreateDeserializer(new ClientRenderingAssetFactory()));

            var tRead = new MeshCoreData();
            using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                serializer.Deserialize(tRead, fs);
            }*/
        }
        private void serializeGeometryData(MeshCoreData data)
        {
            var serializer = new TWXmlSerializer<MeshPartGeometryData>();
            serializer.AddCustomSerializer(AssetSerializer.CreateSerializer());

            string path = TWDir.Test.CreateSubdirectory("XmlTest") + "\\GeometryData.xml";

            using (var fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Delete))
            {
                serializer.Serialize(data.Parts[0].MeshPart.GetGeometryData(), fs);
            }

            /*var tRead = new MeshCoreData();
            using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                serializer.Deserialize(tRead, fs);
            }*/
        }


        public class TestSerializeClass
        {
            public float Number;
            public string Text;
            public Color Color;
            public float[] FloatArray;
            public List<float> FloatList;
            public FileAccess Enum;
            public object Null;


            public float NumberProperty { get; set; }
            public float NumberPropertyReadonly { get; private set; }

            private float privateField;

            /// <summary>
            /// Saved as nested type
            /// </summary>
            public Vector3 Position;
            public TestSerializeNestedClass NestedClass;

            public bool Equals(TestSerializeClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Math.Abs(other.Number - Number) < 0.0001
                    && Equals(other.Text, Text)
                    && other.Color.Equals(Color)
                    && other.FloatArray.SequenceEqual(FloatArray)
                    && other.FloatList.SequenceEqual(FloatList)
                    && Equals(other.Enum, Enum)
                    && Equals(other.Null, Null)
                    && other.Position.Equals(Position)
                    && Equals(other.NestedClass, NestedClass)
                    && other.NumberProperty.Equals(NumberProperty)
                    && other.NumberPropertyReadonly.Equals(NumberPropertyReadonly)
                    && other.privateField.Equals(privateField);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(TestSerializeClass)) return false;
                return Equals((TestSerializeClass)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = Number.GetHashCode();
                    result = (result * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                    result = (result * 397) ^ Color.GetHashCode();
                    result = (result * 397) ^ (FloatArray != null ? FloatArray.GetHashCode() : 0);
                    result = (result * 397) ^ (FloatList != null ? FloatList.GetHashCode() : 0);
                    result = (result * 397) ^ Enum.GetHashCode();
                    result = (result * 397) ^ (Null != null ? Null.GetHashCode() : 0);
                    result = (result * 397) ^ Position.GetHashCode();
                    result = (result * 397) ^ (NestedClass != null ? NestedClass.GetHashCode() : 0);
                    result = (result * 397) ^ NumberProperty.GetHashCode();
                    result = (result * 397) ^ NumberPropertyReadonly.GetHashCode();
                    result = (result * 397) ^ privateField.GetHashCode();
                    return result;
                }
            }
        }

        public class TestSerializeNestedClass
        {
            public int Getal;

            public bool Equals(TestSerializeNestedClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.Getal == Getal;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(TestSerializeNestedClass)) return false;
                return Equals((TestSerializeNestedClass)obj);
            }

            public override int GetHashCode()
            {
                return Getal;
            }
        }


        public class SimpleCustomSerializer : ICustomSerializer
        {
            public bool SerializeElement(TWXmlNode node, Type type, object value, IInternalSerializer s)
            {
                if (type != typeof(SimpleClass)) return false;

                node.Value = "SimpleClass!!";

                return true;
            }

            public bool DeserializeElement(TWXmlNode node, Type type, out object value, IInternalSerializer s)
            {
                value = null;
                if (type != typeof(SimpleClass)) return false;
                if (node.Value != "SimpleClass!!") throw new InvalidOperationException("This xml is invalid!");

                value = new SimpleClass();

                return true;

            }
        }
        public class SimpleClass
        {

        }
        public class SimpleContainerClass
        {
            public SimpleClass Simple;
        }
    }
}
