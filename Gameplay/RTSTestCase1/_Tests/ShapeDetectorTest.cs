using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.RTSTestCase1.Shaping;
using NUnit.Framework;
using Rhino.Mocks;
namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    public class ShapeDetectorTest
    {
        [Test]
        public void PointValueHashTest()
        {
            Assert.AreEqual(new Point3(1, 2, 3), new Point3(1, 2, 3));
            Assert.AreNotEqual(new Point3(1, 2, 3), new Point3(1, 2, 4));
        }
        [Test]
        public void NormalizeTranslationTest()
        {
            var mrDetect = new ShapeDetector();
            var woodShape = CreateShapeElement();
            var stoneShape = CreateShapeElement();
            var stonePickaxe= CreateFirstMockShape();
            var blocks = makePickaxe(stoneShape, woodShape);
            mrDetect.AddConstruction(stonePickaxe,blocks);
            Assert.AreEqual(stonePickaxe,mrDetect.FindConstruction(blocks).Shape);
        }

        private static Dictionary<Point3, IShapeElement> makePickaxe(IShapeElement stoneShape, IShapeElement woodShape)
        {
            var blocks = new Dictionary<Point3, IShapeElement>();
            blocks.Add(new Point3(0, 0, 0), stoneShape);
            blocks.Add(new Point3(1, 0, 0), stoneShape);
            blocks.Add(new Point3(2, 0, 0), stoneShape);
            blocks.Add(new Point3(1, 1, 0), woodShape);
            blocks.Add(new Point3(1, 2, 0), woodShape);
            return blocks;
        }

        public class SimpleShapeElement : IShapeElement
        {

        }
        public IShapeElement CreateShapeElement()
        {
            return Rhino.Mocks.MockRepository.GenerateMock<IShapeElement>();
        }
        public IShape CreateFirstMockShape()
        {
            var mock = Rhino.Mocks.MockRepository.GenerateMock<IShape>();
            return mock;
        }
    }
}