using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame._Tests;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame.Persistence.POSystem
{
    [TestFixture]
    [EngineTest]
    public class POSystemTest
    {

        [Test, TestCaseSource("GetTargets")]
        public void TestSerialize(Object obj)
        {

        }

        [Test]
        public void TestSerializeWorld()
        {
            var world = TestWorldBuilder.createTestWorld(10, 3);

            serializeAndOutput(world);
        }

        private static void serializeAndOutput(Object world)
        {
            var output = new MemoryStream();
            var poSerialize = new POSerializer();

            poSerialize.Serialize(world, output);

            Console.WriteLine(Encoding.Default.GetString(output.ToArray()));
        }

        public void TestNulls()
        {
            new SimplePO();

        }

        public IEnumerable<TestCaseData> GetTargets()
        {
            yield return new TestCaseData(new SimplePO()).SetName("NullFields");
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
                    Polymorf1 =  PolymorphPOA.Default,
                    Polymorf2 = PolymorphPOB.Default
                };
            }
        }

        public interface IInterface
        {

        }

        [PersistedObject]
        public class PolymorphPOA : IInterface
        {
            public int Number;
        }

        [PersistedObject]

        public class PolymorphPOB : IInterface
        {
            public string Text;
        }

        [PersistedObject]
        public class CustomPO : IPOEventsReceiver
        {
            public void OnBeforeSerialize()
            {

            }

            public void OnAfterDeserialize()
            {
            }
        }
    }

    public class POSerializer
    {
        public POSerializer()
        {
        }

        public void Serialize(object obj, Stream strm)
        {

        }
    }
}