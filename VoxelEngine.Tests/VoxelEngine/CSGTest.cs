using DirectX11;
using MHGameWork.TheWizards.Gameplay;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring._Test
{
    [TestFixture]
    public class CSGTest
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
        public void TestUnitCube()
        {
            env.Grid = builder.CreateCube(1);
        }
        [Test]
        public void Test4Cube()
        {
            env.Grid = builder.CreateCube(4);
        }

        [Test]
        public void TestUnitSphere()
        {
            env.Grid = builder.CreateSphere(1);

        }
        [Test]
        public void Test4Sphere()
        {
            env.Grid = builder.CreateSphere(4);

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