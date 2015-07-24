using System;
using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.Building;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Engine.Tests.Facades;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using NUnit.Framework;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;

namespace MHGameWork.TheWizards.VoxelEngine
{
    /// <summary>
    /// Showcase for building with the dual contouring algorithm!
    /// </summary>
    public class DualContouringBuilderTest : EngineTestFixture
    {
        private BuilderUserInterface builderUi;
        private FiniteWorld world;

        private bool drawChunks = false;


        [SetUp]
        public void Setup()
        {
            var surfaceRenderer = VoxelCustomRenderer.CreateDefault(TW.Graphics);
            TW.Graphics.AcquireRenderer().AddCustomGBufferRenderer(surfaceRenderer);

            world = new FiniteWorld();
            world.InitDefaultWorld();
            builderUi = new BuilderUserInterface(world);
            var renderer = new FiniteWorldRenderer( world, surfaceRenderer );


            engine.AddSimulator(builderUi.processUserInput, "UserInput");
            engine.AddSimulator(renderer.UpdateRenderer, "SurfaceUpdating");
            engine.AddSimulator(DrawDebug, "DrawDebug");
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(builderUi.DrawUI, "DrawUI");

            Resolve<IDeveloperConsole>().AddCommand("toggleDrawChunks", 0, _ =>
            {
                drawChunks = !drawChunks;
                return "drawChunks = " + drawChunks;
            });

        }

        public void DrawDebug()
        {
            world.Chunks.ForEach((c, p) =>
            {
                if (drawChunks)
                    TW.Graphics.LineManager3D.AddBox(c.Box, Color.Black);
            });
        }

        [Test]
        public void Run()
        {

        }


    }
}