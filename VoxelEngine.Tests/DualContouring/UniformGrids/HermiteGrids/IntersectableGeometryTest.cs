using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using MHGameWork.TheWizards.VoxelEngine.Environments;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine
{
    [TestFixture]
    [EngineTest]
    public class IntersectableGeometryTest : EngineTestFixture
    {
        private float gridWorldSize;
        private int subdivision;
        private float cellSize;
        private LineManager3DLines lines;
        private DualContouringTestEnvironment environment;

        [SetUp]
        public void Setup()
        {
            environment = new DualContouringTestEnvironment();
            environment.AddToEngine(EngineFactory.CreateEngine());

            gridWorldSize = 10f;
            subdivision = 20;
            cellSize = gridWorldSize / subdivision;
            lines = new LineManager3DLines(TW.Graphics.Device);
        }


        [Test]
        public void TestIntersectableGeometryCube()
        {
            environment.Grid = createCubeGrid();
        }

        [Test]
        public void TestIntersectableGeometrySphere()
        {
            environment.Grid = createSphereGrid();
        }

        [Test]
        public void TestIntersectableSpherePrecise()
        {
            var grid = HermiteDataGrid.FromIntersectableGeometry(2, 2, Matrix.Scaling(0.5f, 0.5f, 0.5f) * Matrix.Translation(1, 1, 1), new IntersectableSphere());
            Assert.AreEqual(2, grid.Dimensions.X);
            Assert.AreEqual(true, grid.GetSign(new Point3(1, 1, 1)));
            Assert.AreEqual(false, grid.GetSign(new Point3(0, 1, 1)));
            Assert.AreEqual(false, grid.GetSign(new Point3(1, 0, 1)));

            Assert.AreEqual(0.5, grid.getEdgeData(new Point3(1, 1, 1), new Point3(1, 0, 0)).W, 0.001);

            environment.Grid = grid;
        }

        public HermiteDataGrid createSphereGrid()
        {
            return HermiteDataGrid.FromIntersectableGeometry(gridWorldSize, subdivision, Matrix.Scaling(new Vector3(4)) * Matrix.Translation(5, 5, 5),
                                                             new IntersectableSphere());
        }
        public HermiteDataGrid createCubeGrid()
        {
            return HermiteDataGrid.FromIntersectableGeometry(gridWorldSize, subdivision, Matrix.Scaling(new Vector3(4)) * Matrix.Translation(5, 5, 5),
                                                             new IntersectableCube());
        }
    }
}