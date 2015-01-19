using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame._Tests;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Persistence.POSystem
{
    [TestFixture]
    [EngineTest]
    public class POSystemTest
    {

        [Test]
        public void TestSerialize()
        {
            var serializer = createSerializer();
            var s = serializer.Serialize(SimplePO.Default());
        }

        private static POSerializer createSerializer()
        {
            return new POSerializer(new CustomizablePOFactory());
        }

        [Test]
        public void TestRoundtripAll()
        {
            testRountrip(SimplePO.Default());
        }

        [Test]
        public void TestRoundtripNull()
        {
            testRountrip(new SimplePO());
        }

        [Test]
        public void TestRoundtripCustom()
        {
            testRountrip(CustomPO.Default);
        }
        private static void testRountrip(object simplePo)
        {
            var serializer = createSerializer();
            var s = serializer.Serialize(simplePo);

            serializer = createSerializer();

            var deserialized = serializer.Deserialize(s);

            Console.WriteLine(deserialized[0].ToString());
            Assert.AreEqual(simplePo.ToString(), deserialized[0].ToString());
            Assert.AreEqual(simplePo, deserialized[0]);
        }

        [Test]
        public void TestFactoryOverride()
        {
            var fact = new CustomizablePOFactory();
            fact.AddTypeOverride(typeof(NoDefaultConstructorPO), () => new NoDefaultConstructorPO(GetType().GetMethod("TestFactoryOverride")));


            var target = new NoDefaultConstructorPO(GetType().GetMethod("TestFactoryOverride")) {Value = 99};

            var serializer = new POSerializer(fact);
            var s = serializer.Serialize(target);

            serializer = new POSerializer(fact);

            var deserialized = serializer.Deserialize(s);

            Assert.AreEqual(target, deserialized[0]);
        }

        [Test]
        public void TestSerializeToXml()
        {
            var serializer = createSerializer();
            var s = serializer.Serialize(SimplePO.Default());
            var xSer = new XmlSerializer(typeof(SerializationResult));
            using (var memStrm = new MemoryStream())
            {
                xSer.Serialize(memStrm, s);
                Console.WriteLine(Encoding.Default.GetString(memStrm.ToArray()));
            }

        }

        public void TestNulls()
        {
            new SimplePO();

        }

        public IEnumerable<TestCaseData> GetTargets()
        {
            yield return new TestCaseData(SimplePO.Default());
        }

        [PersistedObject]
        public class NoDefaultConstructorPO
        {
            [DoNotPersist]
            public MethodInfo Method;

            public int Value;

            public NoDefaultConstructorPO(MethodInfo method)
            {
                Method = method;
            }

            protected bool Equals(NoDefaultConstructorPO other)
            {
                return Equals(Method, other.Method) && Value == other.Value;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((NoDefaultConstructorPO) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Method != null ? Method.GetHashCode() : 0)*397) ^ Value;
                }
            }
        }

        [PersistedObject]
        public class SimplePO
        {
            public int Number;
            public string Text;
            public SimplePO Nested;
            public List<int> List;
            public int[] Array;
            public Dictionary<int, string> Dictionary;
            [DoNotPersist]
            public int IgnoreInt;

            private int PrivateInt;

            public IInterface Polymorf1;
            public object Polymorf2;
            public CustomPO Custom;
            public MyStruct Struct;

            public static SimplePO Default()
            {
                return new SimplePO()
                {
                    Number = 7,
                    Text = "Hello",
                    Nested = new SimplePO() { Text = "Im nested and have nulls!!" },
                    List = new List<int>(new[] { 3, 4, 5 }),
                    Array = new[] { 6, 7, 8 },
                    Dictionary = new[] { "The", "Text" }.ToDictionary(v => v.Length),
                    IgnoreInt = 10,
                    PrivateInt = 999,
                    Polymorf1 = PolymorphPOA.Default,
                    Polymorf2 = PolymorphPOB.Default,
                    Custom = CustomPO.Default,
                    Struct = new MyStruct() { Number = 38, PO = new SimplePO(), Nested = Vector3.UnitY }
                };
            }

            public override string ToString()
            {
                return string.Format("Number: {0}, Text: {1}, Nested: {2}, List: {3}, Array: {4}, Dictionary: {5}, PrivateInt: {6}, Polymorf1: {7}, Polymorf2: {8}, Custom: {9}, Struct: {10}",
                    Number,
                    Text,
                    Nested,
                    List == null ? "NULL" : string.Join(", ", List),
                    Array == null ? "NULL" : string.Join(", ", Array),
                    Dictionary,
                    PrivateInt,
                    Polymorf1,
                    Polymorf2,
                    Custom,
                    Struct);
            }

            protected bool Equals(SimplePO other)
            {
                return Number == other.Number
                    && string.Equals(Text, other.Text)
                    && Equals(Nested, other.Nested)
                    && List == null ? other.List == null : List.SequenceEqual(other.List)
                    && Array == null ? other.Array == null : Array.SequenceEqual(other.Array)
                    && Dictionary == null ? other.Dictionary == null : Dictionary.All(e => other.Dictionary.ContainsKey(e.Key) && Equals(e.Value, other.Dictionary[e.Key]))
                    && PrivateInt == other.PrivateInt
                    && Equals(Polymorf1, other.Polymorf1)
                    && Equals(Polymorf2, other.Polymorf2)
                    && Equals(Custom, other.Custom)
                    && Struct.Equals(other.Struct);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((SimplePO)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = Number;
                    hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Nested != null ? Nested.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (List != null ? List.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Array != null ? Array.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Dictionary != null ? Dictionary.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ IgnoreInt;
                    hashCode = (hashCode * 397) ^ PrivateInt;
                    hashCode = (hashCode * 397) ^ (Polymorf1 != null ? Polymorf1.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Polymorf2 != null ? Polymorf2.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Custom != null ? Custom.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ Struct.GetHashCode();
                    return hashCode;
                }
            }
        }

        public struct MyStruct
        {
            public int Number;
            public SimplePO PO;
            public Vector3 Nested;

            public override string ToString()
            {
                return string.Format("Number: {0}, Po: {1}, Nested: {2}", Number, PO, Nested);
            }

            public bool Equals(MyStruct other)
            {
                return Number == other.Number && Equals(PO, other.PO) && Nested.Equals(other.Nested);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is MyStruct && Equals((MyStruct)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = Number;
                    hashCode = (hashCode * 397) ^ (PO != null ? PO.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ Nested.GetHashCode();
                    return hashCode;
                }
            }
        }

        public interface IInterface
        {

        }

        [PersistedObject]
        public class PolymorphPOA : IInterface
        {
            public int Number;

            public static PolymorphPOA Default
            {
                get { return new PolymorphPOA() { Number = 666 }; }
            }

            public override string ToString()
            {
                return string.Format("Number: {0}", Number);
            }

            protected bool Equals(PolymorphPOA other)
            {
                return Number == other.Number;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((PolymorphPOA)obj);
            }

            public override int GetHashCode()
            {
                return Number;
            }
        }

        [PersistedObject]

        public class PolymorphPOB : IInterface
        {
            public string Text;
            public static PolymorphPOB Default
            {
                get { return new PolymorphPOB() { Text = "Devil" }; }
            }

            public override string ToString()
            {
                return string.Format("Text: {0}", Text);
            }

            protected bool Equals(PolymorphPOB other)
            {
                return string.Equals(Text, other.Text);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((PolymorphPOB)obj);
            }

            public override int GetHashCode()
            {
                return (Text != null ? Text.GetHashCode() : 0);
            }
        }

        [PersistedObject]
        public class CustomPO : IPOEventsReceiver
        {
            [DoNotPersist]
            public MethodInfo Method;
            public string SerializedMethod;

            public void OnBeforeSerialize()
            {
                SerializedMethod = Method.Name;
            }

            public void OnAfterDeserialize()
            {
                Method = GetType().GetMethod(SerializedMethod);
            }

            public static CustomPO Default
            {
                get { return new CustomPO() { Method = typeof(CustomPO).GetMethod("OnBeforeSerialize") }; }
            }

            protected bool Equals(CustomPO other)
            {
                return Equals(Method, other.Method);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((CustomPO)obj);
            }

            public override int GetHashCode()
            {
                return (Method != null ? Method.GetHashCode() : 0);
            }

            public override string ToString()
            {
                return string.Format("Method: {0}", Method);
            }
        }
    }
}