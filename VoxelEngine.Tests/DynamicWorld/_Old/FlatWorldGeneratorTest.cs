using System.Linq;
using MHGameWork.TheWizards.Engine.Tests;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld.Tests
{
    public class FlatWorldGeneratorTest 
    {
        [Test]
        public void TestGenerate()
        {
            var world = new WorldHolder( new global::DirectX11.Point3( 5, 3, 5 ) );
            var gen = new FlatWorldGenerator( 0 ,128);

            var c = world.GetChunk(new global::DirectX11.Point3(0, 0, 0));
            gen.GenerateChunk(c);
            Assert.AreEqual( null, c.SignedOctree.Children );
            Assert.AreEqual( true, c.SignedOctree.Signs.All( s => s == true ) );

             c = world.GetChunk(new global::DirectX11.Point3(0, 1, 0));
            gen.GenerateChunk(c);
            Assert.AreEqual(null, c.SignedOctree.Children);
            Assert.AreEqual(true, c.SignedOctree.Signs.All(s => s == false));

             c = world.GetChunk(new global::DirectX11.Point3(1, 0, 0));
            gen.GenerateChunk(c);
            Assert.AreEqual(null, c.SignedOctree.Children);
            Assert.AreEqual(true, c.SignedOctree.Signs.All(s => s == true));

        }

    }
}