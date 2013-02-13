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
            engine.AddSimulator(new PhysXDebugRendererSimulator());
        }
        [Test]
        public void TestOutputPipe2()
        {
            engine.AddSimulator(new TestOutputPipeSimulator());
            engine.AddSimulator(new TestOutputPipeSimulator());

            engine.AddSimulator(new RTSRendererSimulator());
            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());
        }

        public class TestOutputPipeSimulator : ISimulator
        {
            private DroppedThingOutputPipe pipe;

            private static int Num = 0;
            private ResourceType type;

            public TestOutputPipeSimulator()
            {
                Num++;
                type = new ResourceType() { Texture = TestUtilities.LoadWoodTexture() };


                switch (Num)
                {
                    case 1:
                        pipe = new DroppedThingOutputPipe(new Vector3(0, 1, 2), Vector3.Normalize(new Vector3(1, 0, 0)));
                        break;
                    case 2:
                        pipe = new DroppedThingOutputPipe(new Vector3(2, 1, 0), Vector3.Normalize(new Vector3(0, 0, 1)));
                        break;
                }


            }

            public void Simulate()
            {
                if (pipe.IsFree)
                    pipe.SpawnItem(new Thing() { Type = type });

                pipe.Update();

            }
        }
    }
}
