using System;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.VoxelEngine.EngineServices;
using MHGameWork.TheWizards.VoxelEngine.Environments;
using MHGameWork.TheWizards.VoxelEngine.Worlding;
using NUnit.Framework;

namespace MHGameWork.TheWizards
{
    public class LargeWorldRendererTest : EngineTestFixture
    {
        private LargeWorldRenderer largeWorldRenderer;

        [SetUp]
        public void Setup()
        {
            var vRenderer = new VoxelRenderingService(TW.Graphics);

            largeWorldRenderer = new LargeWorldRenderer(vRenderer.VoxelRenderer);


            engine.AddSimulator(largeWorldRenderer.Update, "LargeWorldRenderer");
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new RecordingSimulator());
        }

        [Test]
        public void TestFlatTwoLevels()
        {
            largeWorldRenderer.LoadWorld(32, v => v.Y - 20);

        }
        [Test]
        public void TestFlatThreeLevels()
        {
            largeWorldRenderer.LoadWorld(64, v => v.Y - 20);

        }

        [Test]
        public void TestFlat()
        {
            largeWorldRenderer.LoadWorld(512, v => v.Y - 100);

        }
        [Test]
        public void TestSin()
        {
            largeWorldRenderer.LoadWorld(512, v => v.Y - 100 + (float)Math.Sin(v.X * 0.3f) + (float)Math.Cos(v.Z * 0.3f));

        }
        [Test]
        public void TestSinBIG()
        {
            largeWorldRenderer.LoadWorld(512 * 16, v => v.Y - 500
                + (float)Math.Sin(v.X * 0.3f) + (float)Math.Cos(v.Z * 0.3f)
                + 10 * (float)Math.Sin(v.X * 0.031f) + 10 * (float)Math.Cos(v.Z * 0.031f)
                + 100 * (float)Math.Sin(v.X * 0.0029f) + 100 * (float)Math.Cos(v.Z * 0.0029f)
                );

        }

        [Test]
        public void TestSpheres()
        {
            var radius = 150;
            var dist = radius * 2 * 3;
            var halfDist = dist * 0.5f;
            largeWorldRenderer.LoadWorld(512 * 16, v =>
                {

                    v.X = TWMath.nfmod(v.X, dist) - halfDist;
                    v.Y = TWMath.nfmod(v.Y, dist) - halfDist;
                    v.Z = TWMath.nfmod(v.Z, dist) - halfDist;
                    return v.Length() - radius;
                });

        }



    }
}