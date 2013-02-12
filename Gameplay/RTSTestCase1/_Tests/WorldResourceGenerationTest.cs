using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    [EngineTest]
    public class WorldResourceGenerationTest
    {

        private TWEngine engine = EngineFactory.CreateEngine();

        [SetUp]
        public void Setup()
        {
            TestUtilities.CreateGroundPlane();
        }

        [Test]
        public void TestRockGeneration()
        {
            new Tree { Position = new Vector3(3, 0, 3), Size = 5 };
            engine.AddSimulator(new WorldResourceGenerationSimulator());
            engine.AddSimulator(new RTSRendererSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestTreeGeneration()
        {
            new Tree { Position = new Vector3(3, 0, 3), Size = 5 };
            engine.AddSimulator(new WorldResourceGenerationSimulator());
            engine.AddSimulator(new RTSRendererSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestResourceGeneration()
        {
            new Tree { Position = new Vector3(3, 0, 3), Size = 5 };
            engine.AddSimulator(new WorldResourceGenerationSimulator());
            engine.AddSimulator(new RTSRendererSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestResourceGrowth()
        {
            new Tree { Position = new Vector3(3, 0, 3), Size = 5 };
            engine.AddSimulator(new WorldResourceGenerationSimulator());
            engine.AddSimulator(new RTSRendererSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestOutputPipe()
        {
            engine.AddSimulator(new TestOutputPipeSimulator());

            engine.AddSimulator(new RTSRendererSimulator());
            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            //engine.AddSimulator(new PhysXDebugRendererSimulator());
        }

        public class TestOutputPipeSimulator : ISimulator
        {
            private DroppedThingOutputPipe pipe;

            public TestOutputPipeSimulator()
            {
                var type = new ResourceType() { Texture = TestUtilities.LoadWoodTexture() };

                pipe = new DroppedThingOutputPipe(new Vector3(2, 1, 2), Vector3.UnitX);
                pipe.SpawnItem(new Thing() { Type = type });
            }

            public void Simulate()
            {
                pipe.Update();      
                
            }
        }
    }
}
