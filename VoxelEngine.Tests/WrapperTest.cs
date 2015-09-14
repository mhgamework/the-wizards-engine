using NUnit.Framework;
using VoxelEngineNative;

namespace MHGameWork.TheWizards
{
    [TestFixture]
    public class WrapperTest
    {
        [Test]
        public void TestSumWrapper()
        {
            var w = new Wrapper();

            Assert.AreEqual(2, w.Sum(1, 1));
        }

        [Test]
        public void TestDualContouring()
        {
            var w = new Wrapper();

            w.DCTest();
        }
    }
}