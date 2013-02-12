using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
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



            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new RTSRendererSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());
        }

        public class TestOutputPipeSimulator : ISimulator
        {
            private DroppedThingOutputPipe pipe;

            public TestOutputPipeSimulator()
            {
                pipe = new DroppedThingOutputPipe(new Vector3(0, 0, 0), Vector3.UnitX);
                pipe.SpawnItem(new Thing());
            }

            public void Simulate()
            {
                pipe.Update();
            }
        }
    }
}
