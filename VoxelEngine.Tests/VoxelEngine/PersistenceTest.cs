using System.IO;
using DirectX11;
using MHGameWork.TheWizards.Engine.Tests;
using NUnit.Framework;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.VoxelEngine
{
    public class PersistenceTest : TWTestFixture
    {

        [Test]
        public void TestSaveLoad()
        {
            var persister = new HermiteDataPersister();
            var hermiteData = createExampleData();
            using (var fs = TestDirectory.CreateFile("SaveLoad").OpenWrite())
                persister.Save(hermiteData, fs);

            IHermiteData loadedGrid;
            using (var fs = TestDirectory.CreateFile("SaveLoad").OpenRead())
                loadedGrid = persister.Load(fs);

        }

        private IHermiteData createExampleData()
        {
            return null;
        }
    }

    public class HermiteDataPersister
    {
        public void Save(IHermiteData hermiteData, FileStream fs)
        {
        }

        public IHermiteData Load(FileStream fs)
        {
            return null;
        }
    }

    /// <summary>
    /// Signs go from 0,0,0 to NumCells,NumCells,NumCells
    /// </summary>
    public interface IHermiteData
    {
        Point3 NumCells { get; }
        /// <summary>
        /// Get intersection point between cell (0) and cell + dir (1), as a lerp factor between 0 and 1
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="dir">X:0,Y:1,Z:2</param>
        /// <returns></returns>
        float GetIntersection(Point3 cell, int dir);
        Vector3 GetNormal(Point3 cell, int dir);

        /// <summary>
        /// Air is null
        /// </summary>
        object GetMaterial(Point3 cell);


    }
}