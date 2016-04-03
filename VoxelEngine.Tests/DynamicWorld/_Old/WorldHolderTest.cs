using DirectX11;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld.Tests
{
    public class WorldHolderTest
    {
        private WorldHolder worldHolder;
        private Point3 size;

        [SetUp]
        public void Setup()
        {
            size = new global::DirectX11.Point3(5, 3, 5);
            worldHolder = new WorldHolder(size);
        }

        [Test]
        public void TestHasChunks()
        {
            Assert.NotNull(worldHolder.GetChunk(new global::DirectX11.Point3(0, 0, 0)));
            Assert.NotNull(worldHolder.GetChunk(new global::DirectX11.Point3(4, 2, 4)));
            Assert.Null(worldHolder.GetChunk(new global::DirectX11.Point3(2, 4, 4)));

        }

        [Test]
        public void TestChunksHaveCorrectCoords()
        {
            Point3.ForEach( size, p =>
            {
                Assert.AreEqual( p, worldHolder.GetChunk( p ).Coord );
            } );
        }
    }
}