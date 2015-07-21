using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.VoxelEngine.Persistence;
using NUnit.Framework;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.VoxelEngine
{
    public class PersistenceTest : TWTestFixture
    {

        [Test]
        public void TestSaveLoadSimple()
        {
            var persister = new HermiteDataPersister();
            var hermiteData = createExampleData();
            using (var fs = TestDirectory.CreateFile("out.txt").Alter(f => f.Delete()).OpenWrite())
                persister.Save(hermiteData, fs);

            IHermiteData loadedGrid;
            using (var fs = TestDirectory.CreateFile("out.txt").OpenRead())
                loadedGrid = persister.Load(fs, p => HermiteDataGrid.Empty(p));

            assertHermiteEqual(hermiteData, loadedGrid);
        }
        [Test]
        public void TestSaveLoadSmallCube()
        {
            var persister = new HermiteDataPersister();
            var hermiteData = new BasicShapeBuilder().CreateCube(1);
            using (var fs = TestDirectory.CreateFile("out.txt").Alter(f => f.Delete()).OpenWrite())
                persister.Save(hermiteData, fs);

            IHermiteData loadedGrid;
            using (var fs = TestDirectory.CreateFile("out.txt").OpenRead())
                loadedGrid = persister.Load(fs, p => HermiteDataGrid.Empty(p));

            assertHermiteEqual(hermiteData, loadedGrid);
        }

        [Test]
        public void TestSaveLoadCube()
        {
            var persister = new HermiteDataPersister();
            var hermiteData = createCube();
            using (var fs = TestDirectory.CreateFile("out.txt").Alter(f => f.Delete()).OpenWrite())
                persister.Save(hermiteData, fs);

            IHermiteData loadedGrid;
            using (var fs = TestDirectory.CreateFile("out.txt").OpenRead())
                loadedGrid = persister.Load(fs, p => HermiteDataGrid.Empty(p));

            assertHermiteEqual(hermiteData, loadedGrid);
        }

        private IHermiteData createExampleData()
        {
            var size = 32;
            return HermiteDataGrid.Empty(new Point3(size, size, size));
        }
        private IHermiteData createCube()
        {
            var size = 32;
            return new BasicShapeBuilder().CreateCube(size);
        }

        private void assertHermiteEqual(IHermiteData a, IHermiteData b)
        {
            Assert.AreEqual(a.NumCells, b.NumCells);
            Point3.ForEach(a.NumCells + new Point3(1, 1, 1), p =>
                {
                    Assert.AreEqual(a.GetMaterial(p), b.GetMaterial(p));
                    for (int i = 0; i < 3; i++)
                    {
                        Assert.AreEqual(a.GetIntersection(p, i), b.GetIntersection(p, i));
                        Assert.AreEqual(a.GetNormal(p, i), b.GetNormal(p, i));
                    }

                });
        }
    }
}