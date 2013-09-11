using System;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using NUnit.Framework;
using SlimDX;
using System.Linq;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;
using Castle.Core.Internal;

namespace MHGameWork.TheWizards.SkyMerchant.Lod
{
    [TestFixture]
    [EngineTest]
    public class LodTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestLineLod()
        {
            setupObjects(
                2000f, () => new LineLodPhysical());

            engine.AddSimulator(new BasicSimulator(delegate
                {
                    foreach (var i in TW.Data.Objects.OfType<LineLodPhysical>())
                    {
                        i.UpdateMeshVisibility();
                    }

                }));



            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        private static void setupObjects(float size, Func<Physical> createPhysical)
        {
            var mesh = TW.Assets.LoadMesh("SkyMerchant/Tree/Tree_WithLeaves");
            var seeder = new Seeder(0);

            var height = 200f;
            var density1D = 40f;

            TW.Graphics.SpectaterCamera.FarClip = 10000;

            var min = new Vector3(-size * 0.5f, -height * 0.5f, -size * 0.5f);
            var max = new Vector3(size * 0.5f, height * 0.5f, size * 0.5f);

            var numObjects = size * size * height / density1D / density1D / density1D;
            for (int i = 0; i < numObjects; i++)
            {
                var p = createPhysical();
                p.Mesh = mesh;
                p.WorldMatrix = Matrix.Translation(seeder.NextVector3(min.xna(), max.xna()).dx());
            }

            var text = new Textarea();
            text.Size = new Vector2(200, 100);
            text.Text = "Total objects: " + numObjects;

            new WireframeBox() { Color = new Color4(1, 0, 0), WorldMatrix = Matrix.Scaling(size, height, size) };
        }

        [Test]
        public void TestPhysicalLodRendererDummy()
        {
            float size = 400f;
            setupObjects(size, () => new Physical());

            var lod = new PhysicalLodRenderer(new SimpleWorldOctree(new Vector3(size, 200, size)));

            engine.AddSimulator(new BasicSimulator(delegate
            {
                lod.UpdateRendererState();
            }));



            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.AddSimulator(new BasicSimulator(delegate
            {
                lod.RenderLines();
            }));

        }

        [Test]
        public void TestWorldOctree()
        {
            var radius = new Vector3(200, 200, 200);
            var tree = new SimpleWorldOctree(radius);
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

                    var inChunk = tree.FindChunks(2, delegate(ChunkCoordinate c)
                        {
                            BoundingBox chunkBoundingBox = tree.GetChunkBoundingBox(c);
                            ContainmentType containmentType = chunkBoundingBox.xna().Contains(campos.xna());
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
    }
}