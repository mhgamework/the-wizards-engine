using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1._Tests;
using NUnit.Framework;
using Rhino.Mocks;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.SkyMerchant.Building.Shaping
{
    [TestFixture]
    [EngineTest]

    public class ModelBuilderPrototypeTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        private IShapeDetector shapeDetector = new ShapeDetectorBuilder().BuildShapeDetector();
        private IShapeElement shapeElement = new ShapeDetectorTest.SimpleShapeElement();
        private IShapeElement zeroShapeElement = new ShapeDetectorTest.SimpleShapeElement();
        private IShape coolShape;
        private SimpPart constructedEntity;
        private SimpPart zeroPart;
        private OrientedShape finishedShape;
        private IBuildGrid buildGrid;
        private BuildGridRaycaster raycaster;
        private bool buildMode;


        [Test]
        public void TestBuilder()
        {
            var tex = TW.Assets.LoadTexture("RTS/barrel.jpg");
            coolShape = MockRepository.GenerateMock<IShape>();
            zeroPart = new SimpPart() { Physical = new Physical() { Mesh = UtilityMeshes.CreateMeshWithTexture(0.25f, tex), Visible = true },Element = zeroShapeElement};
            buildGrid = new SimpleBuildGrid();
            buildGrid.AddItemAt(new Point3(), zeroPart);
            raycaster = new BuildGridRaycaster(buildGrid, new GridTraverser());

            var shapeElements = new Dictionary<Point3, IShapeElement>();
            shapeElements.Add(new Point3(), zeroPart.Element);
            shapeElements.Add(new Point3() { X = 1 }, shapeElement);
            shapeElements.Add(new Point3() { X = 2 }, shapeElement);
            shapeElements.Add(new Point3() { Y = 1 }, shapeElement);

            shapeDetector.AddBlueprint(coolShape, shapeElements);
            engine.AddSimulator(new BasicSimulator(delegate
            {
                if (buildMode)
                {
                    if (TW.Graphics.Keyboard.IsKeyPressed(Key.J)) addBlock();
                    if (TW.Graphics.Keyboard.IsKeyPressed(Key.K)) removeBlock();
                }
                if (TW.Graphics.Keyboard.IsKeyPressed(Key.L)) switchMode();
            }));
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.Run();
        }

        private void removeBlock()
        {
            var centerScreenRay = TW.Data.Get<CameraInfo>().GetCenterScreenRay();
            var trace = new RayTrace() { Start = 0, End = 30f, Ray = centerScreenRay };
            SimpPart block;
            if (!raycaster.GetFirstFullBlock(trace, out block)) return;
            if (block == null || block.Element.Equals(zeroShapeElement)) return;
            removeItem(block);

        }
        private void addBlock()
        {
            Point3 pos;
            var centerScreenRay = TW.Data.Get<CameraInfo>().GetCenterScreenRay();
            var trace = new RayTrace() { Start = 0, End = 30f, Ray = centerScreenRay };
            if (raycaster.GetLastEmptyBlockPosition(trace, out pos))
            {
                var item = addItem(pos,shapeElement);
            }
        }
        private void removeItem(SimpPart block)
        {
            if (buildGrid.RemoveItem(block))
                TW.Data.RemoveObject(block);
        }
        private SimpPart addItem(Point3 position,IShapeElement sE)
        {
            var simpPart = new SimpPart()
                               {
                                   Element = sE,
                                   Physical = new Physical() { Mesh = UtilityMeshes.CreateMeshWithTexture(0.25f, TW.Assets.LoadTexture("RTS/barrel.jpg")), WorldMatrix = Matrix.Translation(position.ToVector3() * 0.5f) }
                               };
            buildGrid.AddItemAt(position, simpPart);
            return simpPart;
        }


        private void switchMode()
        {
            buildMode = !buildMode;
            if (buildMode)
            {
                deconstruct();
                return;
            }

            var neededLib = buildGrid.GetAllBlocks().ToDictionary(pair => pair.Key, pair => pair.Value.Element);
            var copyLib = buildGrid.GetAllBlocks().ToDictionary(pair => pair.Key, pair => pair.Value);
            var orientedShapes = shapeDetector.GetConstruction(neededLib);
            if (orientedShapes == null)
                return;
            var finishedShapeList = orientedShapes.ToList();
            if(finishedShapeList.Count == 0)
            {
                finishedShape = null;
                return;
            }
            finishedShape = finishedShapeList[0];
            constructedEntity = new SimpPart()
                                    {
                                        Physical = new Physical()
                                                {
                                                    Mesh =
                                                        UtilityMeshes.CreateMeshWithTexture(0.5f,
                                                                                            TW.Assets.LoadTexture("RTS/barrel2.jpg")),
                                                    WorldMatrix = Matrix.Translation(0, 0, 0)
                                                }
                                    };
            foreach (var element in copyLib)
            {
                removeItem(element.Value);
            }
        }

        private void deconstruct()
        {
            if (constructedEntity != null)
            {
                var blocksOfForm = shapeDetector.GetBlocksOfShape(finishedShape);
                foreach (var element in blocksOfForm)
                {
                    addItem(element.Key, element.Value);
                }
                TW.Data.RemoveObject(constructedEntity);
                constructedEntity = null;
                finishedShape = null;
            }
        }
    }

    [ModelObjectChanged]
    public class SimpPart : EngineModelObject, IPhysical
    {
        public IShapeElement Element { get; set; }
        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            //throw new System.NotImplementedException();
        }
    }
}