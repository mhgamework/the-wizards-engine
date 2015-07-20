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
    public class DualContouringTest:EngineTestFixture
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