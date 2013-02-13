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
        public void SimpleWorkingTest()
        {
            var mrDetect = new ShapeDetector();
            var woodShape = CreateShapeElement();
            var stoneShape = CreateShapeElement();
            var stonePickaxe= CreateFirstMockShape();
            var blocks = makePickaxe(stoneShape, woodShape);
            mrDetect.AddConstruction(stonePickaxe,blocks);
            Assert.AreEqual(stonePickaxe,mrDetect.FindConstruction(blocks).Shape);
        }

        [Test]
        public void NormalizeTranslationTest()
        {
            var mrDetect = new ShapeDetector();
            var woodShape = CreateShapeElement();
            var stoneShape = CreateShapeElement();
            var stonePickaxe = CreateFirstMockShape();
            var blocks = makePickaxe(stoneShape, woodShape);
            var transBlocks = makeTranslatedPickaxe(stoneShape, woodShape);
            mrDetect.AddConstruction(stonePickaxe, blocks);
            Assert.AreEqual(stonePickaxe, mrDetect.FindConstruction(transBlocks).Shape);
        }

        [Test]
        public void NormalizeRotationTest()
        {
            var mrDetect = new ShapeDetector();
            var woodShape = CreateShapeElement();
            var stoneShape = CreateShapeElement();
            var stonePickaxe = CreateFirstMockShape();
            var blocks = makePickaxe(stoneShape, woodShape);
            var transBlocks = makeRotatedPickaxe(stoneShape, woodShape);
            mrDetect.AddConstruction(stonePickaxe, blocks);
            Assert.AreEqual(stonePickaxe, mrDetect.FindConstruction(transBlocks).Shape);
        }

        private Dictionary<Point3, IShapeElement> makeRotatedPickaxe(IShapeElement stoneShape, IShapeElement woodShape)
        {
            var blocks = new Dictionary<Point3, IShapeElement>
                             {
                                 {new Point3(1, 0, 1), stoneShape},
                                 {new Point3(1, 0, 0), stoneShape},
                                 {new Point3(1, 0, -1), stoneShape},
                                 {new Point3(1, 1, 0), woodShape},
                                 {new Point3(1, 2, 0), woodShape}
                             };
            return blocks;

        }


        private static Dictionary<Point3, IShapeElement> makePickaxe(IShapeElement stoneShape, IShapeElement woodShape)
        {
            var blocks = new Dictionary<Point3, IShapeElement>
                             {
                                 {new Point3(0, 0, 0), stoneShape},
                                 {new Point3(1, 0, 0), stoneShape},
                                 {new Point3(2, 0, 0), stoneShape},
                                 {new Point3(1, 1, 0), woodShape},
                                 {new Point3(1, 2, 0), woodShape}
                             };
            return blocks;
        }

        private static Dictionary<Point3, IShapeElement> makeTranslatedPickaxe(IShapeElement stoneShape, IShapeElement woodShape)
        {
            var blocks = new Dictionary<Point3, IShapeElement>
                             {
                                 {new Point3(1, 0, 4), stoneShape},
                                 {new Point3(2, 0, 4), stoneShape},
                                 {new Point3(3, 0, 4), stoneShape},
                                 {new Point3(2, 1, 4), woodShape},
                                 {new Point3(2, 2, 4), woodShape}
                             };
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