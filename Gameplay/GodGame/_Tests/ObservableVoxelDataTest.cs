using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class ObservableVoxelDataTest
    {
        [Test]
        public void TestDataValue()
        {
            var count = 0;
            var data = new ObservableVoxelData(() => { count++; });

            Assert.AreEqual(0, count);

            data.DataValue = 5;
            Assert.AreEqual(1, count);

            data.DataValue = 6;
            Assert.AreEqual(2, count);

            data.DataValue = 7;
            data.DataValue = 7;
            Assert.AreEqual(3, count);
        }

        [Test]
        public void TestExtensionChangesSimple()
        {
            var count = 0;
            var data = new ObservableVoxelData(() => { count++; });

            Assert.AreEqual(0, count);

            data.Get<MyExtension>().Number = 5;
            Assert.AreEqual(1, count);
            
            data.Get<MyExtension>().Number = 6;
            Assert.AreEqual(2, count);

            data.Get<MyExtension>().Number = 7;
            data.Get<MyExtension>().Number = 7;
            Assert.AreEqual(3, count);
        }

        public interface MyExtension : IVoxelDataExtension
        {
            int Number { get; set; }
        }

    }
}