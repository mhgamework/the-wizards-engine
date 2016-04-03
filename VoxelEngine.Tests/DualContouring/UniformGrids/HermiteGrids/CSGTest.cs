using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.VoxelEngine.Environments;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine
{
    public class CSGTest :EngineTestFixture
    {
        private DualContouringTestEnvironment env;
        private BasicShapeBuilder builder = new BasicShapeBuilder();
        [SetUp]
        public void Setup()
        {
            env = new DualContouringTestEnvironment();
            env.AddToEngine(EngineFactory.CreateEngine());
        }

        [Test]
        public void TestUnionEqualSize()
        {
            env.Grid = HermiteDataGrid.CopyGrid(new UnionGrid(HermiteDataGrid.Empty(new Point3(2, 2, 2)), builder.CreateCube(1)));
        }
        [Test]
        public void TestUnionDifferentSize()
        {
            env.Grid = HermiteDataGrid.CopyGrid(new UnionGrid(HermiteDataGrid.Empty(new Point3(3, 3, 3)), builder.CreateCube(1)));
        }
        [Test]
        public void TestUnionDifferentSizeOffset()
        {
            env.Grid = HermiteDataGrid.CopyGrid(new UnionGrid(HermiteDataGrid.Empty(new Point3(3, 3, 3)), builder.CreateCube(1), new Point3(1, 1, 1)));
        }
    }
}