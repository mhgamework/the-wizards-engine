using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.Rendering.Lod;
using MHGameWork.TheWizards.Scattered.Bindings;
using MHGameWork.TheWizards.Scattered.GameLogic;
using MHGameWork.TheWizards.Scattered.GameLogic.Objects;
using MHGameWork.TheWizards.Scattered.GameLogic.Services;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered.Simulation;
using MHGameWork.TheWizards.Simulators;
using NUnit.Framework;
using SlimDX;
using MHGameWork.TheWizards.Scattered._Engine;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    [TestFixture]
    [EngineTest]
    public class LodTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestBigMap()
        {
            engine.Initialize();
            var builder = new ContainerBuilder();
            builder.RegisterModule<ScatteredModule>();
            builder.RegisterType<BigMapTest>();
            var cont = builder.Build();

            cont.Resolve<BigMapTest>().Run();

           

        }

        public class BigMapTest
        {
            private readonly LinebasedLodRenderer lodRenderer;
            private readonly WorldRenderingSimulator worldRenderingSimulator;
            private readonly Level level;

            public BigMapTest(LinebasedLodRenderer lodRenderer,WorldRenderingSimulator worldRenderingSimulator, Level level)
            {
                this.lodRenderer = lodRenderer;
                this.worldRenderingSimulator = worldRenderingSimulator;
                this.level = level;
            }

            public void Run()
            {
                var engine = EngineFactory.CreateEngine();
                engine.AddSimulator(new BasicSimulator(() =>
                    {
                        lodRenderer.UpdateRendererState();

                        lodRenderer.RenderLines();
                    }));
                engine.AddSimulator(new WorldRenderingSimulator());
                for (int x = 0; x < 200; x++)
                {
                    for (int y = 0; y < 200; y++)
                    {
                        var i = level.CreateNewIsland(new Vector3(x*100, 0, y*100));
                        i.Mesh = TW.Assets.LoadMesh("Scattered\\TestIsland");
                    }
                }
            }
        }



    }
}
