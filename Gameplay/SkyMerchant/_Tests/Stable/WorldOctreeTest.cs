using System;
using System.Linq;
using Castle.Core.Internal;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.SkyMerchant.Lod;
using NUnit.Framework;
using SlimDX;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Stable
{
    /// <summary>
    /// Tests the ChunkCoordinate and WorldOctreeExtensions (findup and finddown)
    /// </summary>
    [EngineTest]
    [TestFixture]
    public class WorldOctreeTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        /// <summary>
        /// Test the FindChunksDown and GetChunksInRange extension on WorldOctreeExtensions
        /// </summary>
        [Test]
        public void TestGetChunksInRange_FindChunksDown()
        {
            var radius = new Vector3(200, 200, 200);
            var tree = new SimpleWorldOctree(radius); // Using dummy octree
            TW.Graphics.SpectaterCamera.FarClip = 1000;
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.AddSimulator(new BasicSimulator(delegate
            {
                Vector3 campos = TW.Graphics.Camera.ViewInverse.xna().Translation.dx();
                var chunks = tree.GetChunksInRange(campos, 100, 200, 2);
                foreach (var c in chunks)
                {
                    TW.Graphics.LineManager3D.AddCenteredBox(tree.GetChunkCenter(c), tree.GetChunkRadius(c).MaxComponent() * 2, new Color4(1, 1, 0));
                }

                var inChunk = tree.FindChunksDown(2, delegate(ChunkCoordinate c)
                {
                    BoundingBox chunkBoundingBox = tree.GetChunkBoundingBox(c);
                    Microsoft.Xna.Framework.ContainmentType containmentType = chunkBoundingBox.xna().Contains(campos.xna());
                    return containmentType == ContainmentType.Contains;
                });
                if (inChunk.Any())
                {
                    var bb = tree.GetChunkBoundingBox(inChunk.First());
                    bb.Minimum += MathHelper.One * 0.01f;
                    bb.Maximum -= MathHelper.One * 0.01f;
                    TW.Graphics.LineManager3D.AddBox(bb, new Color4(1, 0.5f, 0));
                }
                TW.Graphics.LineManager3D.AddCenteredBox(new Vector3(), radius.MaxComponent() * 2, new Color4(0, 0, 0));
            }));
        }

        [Test]
        public void TestChunkCoordinateChildren()
        {
            var coord = ChunkCoordinate.Root;
            printChildren(coord, 2);
        }

        private void printChildren(ChunkCoordinate c, int depth)
        {
            for (int i = 0; i < c.Depth; i++) Console.Write("---");
            Console.WriteLine(c);
            if (c.Depth == depth)
            {
                return;
            }

            c.GetChildren().ForEach(v => printChildren(v, depth));
        }

        [Test]
        public void TestUnrolledIndex()
        {
            var coord = ChunkCoordinate.Root;
            Assert.AreEqual(0, coord.GetUnrolledIndex());


            var level1 = coord.GetChildren().Select(p => p.GetUnrolledIndex());

            Assert.AreEqual(8, level1.Distinct().Count());
            Assert.False(level1.Any(i => !(i >= 1 && i < 1 + 8)));


            var level2 = coord.GetChildren().SelectMany(c => c.GetChildren()).Select(p => p.GetUnrolledIndex());

            Assert.AreEqual(8 * 8, level2.Distinct().Count());
            Assert.False(level2.Any(i => !(i >= 1 + 8 && i < 1 + 8 + 8 * 8)));

        }

        [Test]
        public void TestGetParent()
        {
            Assert.AreEqual(ChunkCoordinate.Empty, ChunkCoordinate.Root.GetParent());

            var children = ChunkCoordinate.Root.GetChildren().ToArray();

            foreach (var rootChild in children)
            {
                Assert.AreEqual(ChunkCoordinate.Root, rootChild.GetParent());

                var subChildren = rootChild.GetChildren().ToArray();
                foreach (var subChild in subChildren)
                {
                    Assert.AreEqual(rootChild, subChild.GetParent());

                }
            }
        }
    }
}