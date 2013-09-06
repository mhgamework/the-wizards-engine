using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.SkyMerchant.Building.Shaping;
using NUnit.Framework;
using Rhino.Mocks;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.Building
{
    [TestFixture]
    [EngineTest]
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
            var mrDetect = getShapeDetector(); ;
            var woodShape = CreateShapeElement();
            var stoneShape = CreateShapeElement();
            var stonePickaxe= CreateFirstMockShape();
            var blocks = makePickaxe(stoneShape, woodShape);
            mrDetect.AddBlueprint(stonePickaxe,blocks);
            //Assert.AreEqual(stonePickaxe,mrDetect.FindConstruction(blocks).Shape);
            throw new NotImplementedException();
        }

        [Test]
        public void NormalizeTranslationTest()
        {
            var mrDetect = getShapeDetector();
            var woodShape = CreateShapeElement();
            var stoneShape = CreateShapeElement();
            var stonePickaxe = CreateFirstMockShape();
            var blocks = makePickaxe(stoneShape, woodShape);
            var transBlocks = makeTranslatedPickaxe(stoneShape, woodShape);
            mrDetect.AddBlueprint(stonePickaxe, blocks);
            //Assert.AreEqual(stonePickaxe, mrDetect.FindConstruction(transBlocks).Shape);
            throw new NotImplementedException();
        }

        private static IShapeDetector getShapeDetector()
        {
            return new ShapeDetectorBuilder().BuildShapeDetector();
        }

        [Test]
        public void NormalizeRotationTest()
        {
            var mrDetect = getShapeDetector(); 
            var woodShape = CreateShapeElement();
            var stoneShape = CreateShapeElement();
            var stonePickaxe = CreateFirstMockShape();
            var blocks = makePickaxe(stoneShape, woodShape);
            var transBlocks = makeRotatedPickaxe(stoneShape, woodShape);
            mrDetect.AddBlueprint(stonePickaxe, blocks);
            //Assert.AreEqual(stonePickaxe, mrDetect.FindConstruction(transBlocks).Shape);
            throw new NotImplementedException();
        }
        [Test]
        public void MakeUnmakeFormNoDifference()
        {
           try
            {
                var mrDetect = getShapeDetector(); ;
            var woodShape = CreateShapeElement();
            var stoneShape = CreateShapeElement();
            var woodPickaxe = CreateFirstMockShape();
            var blocks = new Dictionary<Point3, IShapeElement>();
            blocks.Add(new Point3(0,0,0),woodShape);    
            blocks.Add(new Point3(0,0,1),stoneShape);
            mrDetect.AddBlueprint(woodPickaxe, blocks);
            var res =
                mrDetect.GetConstruction(
                    mrDetect.GetBlocksOfShape(new OrientedShape()
                                                 {
                                                     Rotation = Matrix.RotationX((float)Math.PI/2),
                                                     Shape = woodPickaxe,
                                                     Translation = new Vector3(3, 3, 3)
                                                 }));
                var blocksOfForm = mrDetect.GetBlocksOfShape(res.First());
                var res2 =  mrDetect.GetConstruction(blocksOfForm);
                var blocksOfForm2 = mrDetect.GetBlocksOfShape(res2.First());
                var res3 =  mrDetect.GetConstruction(blocksOfForm2);
            Assert.True(res2.First().Translation.Equals(new Vector3(3,3,3)));
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Init prototype");
            }
        }

        [Test]
        public void Test2BlockShapeRotated()
        {
             try
             {



                 var zeroElement = MockRepository.GenerateMock<IShapeElement>();
                 var shapeElement = MockRepository.GenerateMock<IShapeElement>();

                 var coolShape = MockRepository.GenerateMock<IShape>();
                 var coolShape2 = MockRepository.GenerateMock<IShape>();


                 var mrDetect = new ShapeDetectorBuilder().BuildShapeDetector();

                 var shapeElements = new Dictionary<Point3, IShapeElement>();
                 shapeElements.Add(new Point3(), zeroElement);
                 shapeElements.Add(new Point3() { X = 1 }, shapeElement);
                 mrDetect.AddBlueprint(coolShape, shapeElements);




                 var dict = new Dictionary<Point3, IShapeElement>();
                 dict.Add(new Point3(0, 0, 0), zeroElement);
                 dict.Add(new Point3(-1, 0, 0), shapeElement);





                 var result = mrDetect.GetConstruction(dict);
                 var res2 =
                        mrDetect.GetBlocksOfShape(result.First());

                 var res =
                         mrDetect.GetBlocksOfShape(new OrientedShape()
                         {
                             Rotation = Matrix.Identity,//Matrix.RotationY(-(float)Math.PI / 2),
                             Shape = coolShape,
                             Translation = new Vector3(0, 0, 0)
                         });
              
             }
             catch (Exception ex)
             {
                 DI.Get<IErrorLogger>().Log(ex, "Init prototype");
             } 
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