using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Simulation.Spatial;
using NUnit.Framework;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Rendering.Lod
{
    [TestFixture]
    [EngineTest]
    public class LodTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

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

        /// <summary>
        /// Test the Lod renderer with a dummy octree implementation
        /// </summary>
        [Test]
        public void TestPhysicalLodRendererDummy()
        {
            float size = 400f;
            setupObjects(size, () => new RenderablePhysical());

            var lod = new LinebasedLodRenderer(new SimpleWorldOctree<RenderablePhysical>(new Vector3(size, 200, size),TW.Data.Objects.OfType<RenderablePhysical>()));

            TW.Data.Objects.OfType<Physical>().ForEach(delegate(Physical p) { p.Visible = false; });

            engine.AddSimulator(new BasicSimulator(lod.UpdateRendererState));

            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.AddSimulator(new BasicSimulator(lod.RenderLines));

        }

        /// <summary>
        /// Test the lod render with a 
        /// </summary>
        [Test]
        public void TestPhysicalLodRendererOptimized()
        {
            float size = 1000f;
            setupObjects(size, () => new RenderablePhysical());

            var tree = new OptimizedWorldOctree<RenderablePhysical>(new Vector3(size, 200, size), 5);
            var lod = new LinebasedLodRenderer(tree);

            TW.Data.Objects.OfType<RenderablePhysical>().ForEach(delegate(RenderablePhysical p) { tree.AddWorldObject(p); p.Visible = false; });

            engine.AddSimulator(new BasicSimulator(lod.UpdateRendererState));



            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.AddSimulator(new BasicSimulator(lod.RenderLines));

        }

        public class RenderablePhysical : Physical, IRenderable
        {
            public BoundingBox BoundingBox { get { return GetBoundingBox(); } }
        }


    }
}