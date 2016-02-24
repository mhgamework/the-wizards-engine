using System.IO;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld.Tests
{
    [TestFixture]
    public class SignedOctreeSerializerTest
    {
        [Test]
        public void TestSerializeDeserialize()
        {
            var tree = new SignedOctreeBuilderTest().Alter( b => b.Setup() ).CreateOctreeSmallCube();

            var algo = new SignedOctreeSerializer();
            var memStrm = new MemoryStream();
            algo.Serialize( tree, new BinaryWriter( memStrm ));

            memStrm.Seek( 0, SeekOrigin.Begin );

            var des = algo.Deserialize( new BinaryReader( memStrm ) );

            checkEqual( tree, des );
        }

        private void checkEqual( SignedOctreeNode a, SignedOctreeNode b )
        {
            if (a == null && b == null) return;
            Assert.NotNull( a );
            Assert.NotNull( b );
            Assert.AreEqual( a.Size, b.Size );
            Assert.AreEqual( a.LowerLeft, b.LowerLeft );
            
            if (a.Children == null && b.Children == null) return;
            Assert.NotNull( a.Children );
            Assert.NotNull( b.Children );
            for ( int i = 0; i < 8; i++ )
            {
                checkEqual(a.Children[i], b.Children[i]);
            }
        }
    }
}