using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTS.Commands;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scripting;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    [TestFixture]
    [EngineTest]
    public class GoblinControlTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        private Goblin goblin;

        [Test]
        public void TestBasicSetup()
        {
            setupBasic();
        }

        private void setupBasic()
        {
            TW.Data.GetSingleton<CameraInfo>().Mode = CameraInfo.CameraMode.FirstPerson;
            engine.AddSimulator(new PlayerPickupSimulator());

            engine.AddSimulator(new FirstPersonCameraSimulator());
            engine.AddSimulator(new GoblinRendererSimulator());
            engine.AddSimulator(new EntityBatcherSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());


            createGroundPlane();

            goblin = new Goblin();
            goblin.Position = new Vector3(-2, 1, 2);
        }

        private void createGroundPlane()
        {
            var builder = new MeshBuilder();
            builder.AddBox(new Vector3(-1000, 0, -1000), new Vector3(1000, -1, 1000));
            var mesh = builder.CreateMesh();

            var ent = new Engine.WorldRendering.Entity { Mesh = mesh };
        }

        [Test]
        public void TestFollowCommand()
        {
            engine.AddSimulator(createFetchSimulator());
            engine.AddSimulator(new GoblinMovementSimulatorSimple());
            setupBasic();
        }

        private ISimulator createFetchSimulator()
        {
            var cmd = new GoblinFollowUpdater();
            return new BasicSimulator(delegate
            {
                foreach (Goblin g in TW.Data.Objects.Where(o => o is Goblin))
                    cmd.Update(g);
            });
        }

        [Test]
        public void TestSimpleCrowdControl()
        {
            engine.AddSimulator(createFetchSimulator());
            engine.AddSimulator(new GoblinSimpleCrowdControlSimulator());
            engine.AddSimulator(new GoblinMovementSimulatorSimple());
            goblin = new Goblin();
            goblin.Position = new Vector3(-2, 1, 2);
            setupBasic();
        }
        [Test]
        public void TestSimpleCrowdControlBig()
        {
            engine.AddSimulator(createFetchSimulator());
            engine.AddSimulator(new GoblinSimpleCrowdControlSimulator());
            engine.AddSimulator(new GoblinMovementSimulatorSimple());

            for (int i = 0; i < 100; i++)
            {
                goblin = new Goblin();
                goblin.Position = new Vector3(-i, 1, 2);
            }
            setupBasic();

        }

        [Test]
        public void TestFetchCommand()
        {
            var cmd = new GoblinFetchUpdater();
            var type = new ResourceType();
            var thing = new Thing() { Type = type };
            var droppedThing = new DroppedThing() { Position = new Vector3(-5, 0.5f, 3), Thing = thing };


            thing = new Thing() { Type = type };
            droppedThing = new DroppedThing() { Position = new Vector3(-20, 0.5f, 3), Thing = thing };

            engine.AddSimulator(new BasicSimulator(delegate
                {
                    foreach (Goblin g in TW.Data.Objects.Where(o => o is Goblin))
                        cmd.Update(g, new Vector3(-5, 1, -3), type);
                }));
            engine.AddSimulator(new GoblinMovementSimulatorSimple());

            setupBasic();
        }

        [Test]
        public void TestStealingGoblins()
        {
            var cmd = new GoblinFetchUpdater();
            var type = new ResourceType();
            var thing = new Thing() { Type = type };
            var droppedThing = new DroppedThing() { Position = new Vector3(-5, 0.5f, 3), Thing = thing };


            thing = new Thing() { Type = type };
            for (int i = 0; i < 20; i++)
            {
                droppedThing = new DroppedThing() { Position = new Vector3(-20, 0.5f, 3 + i), Thing = thing };

            }


            new Goblin() { Position = new Vector3(-10, 0.5f, 7) };

            engine.AddSimulator(new BasicSimulator(delegate
                {
                    var i = 0;
                    foreach (Goblin g in TW.Data.Objects.Where(o => o is Goblin))
                    {
                        cmd.Update(g, new Vector3(-5 - i * 10, 0.5f, -3), type);
                        i++;

                    }
                }));
            engine.AddSimulator(new GoblinMovementSimulatorSimple());

            setupBasic();
        }

        [Test]
        public void TestFactory()
        {
            var input = new ResourceType();
            var output = new ResourceType();

            var factory = new Factory() { Position = new Vector3(), InputType = input, OutputType = output };
            var area = factory.GetInputArea();
            var pos = (area.Minimum + area.Maximum) * 0.5f;

            for (int i = 0; i < 2; i++)
            {
                var thing = new Thing() { Type = input };
                var dropped = new DroppedThing() { Thing = thing, Position = pos };
            }

            engine.AddSimulator(new FactorySimulator());

            setupBasic();
        }

        [Test]
        public void TestPlayerPickup()
        {
            var input = new ResourceType();

            var thing = new Thing() { Type = input };
            var dropped = new DroppedThing() { Thing = thing, Position = new Vector3(2, 0.5f, 2) };

            setupBasic();
        }



        [Test]
        public void TestCommands()
        {

            var goblin2 = new Goblin() { Position = new Vector3(-10, 1, 0) };

            var input = new ResourceType();

            var thing = new Thing() { Type = input };
            var dropped = new DroppedThing() { Thing = thing, Position = new Vector3(2, 0.5f, 2) };

            engine.AddSimulator(new GoblinCommandSimulator());
            engine.AddSimulator(new GoblinMovementSimulatorSimple());

            bool first = true; // hack because of problems with data mode, commands should be data
            engine.AddSimulator(new BasicSimulator(delegate
                {
                    if (!first) return;
                    first = false;
                    goblin.get<GoblinCommandState>().CurrentCommand = new GoblinFollowCommand(new GoblinFollowUpdater());
                    goblin2.get<GoblinCommandState>().CurrentCommand = new GoblinFetchCommand(new GoblinFetchUpdater()) { ResourceType = input, TargetPosition = new Vector3(-5, 1, -2) };

                }));


            setupBasic();


        }

        [Test]
        public void TestCommunicate()
        {
            var input = new ResourceType();

            placeItem(input, new Vector3(2, 0.5f, 2));

            engine.AddSimulator(new GoblinCommunicationSimulator());
            engine.AddSimulator(new GoblinCommandSimulator());
            engine.AddSimulator(new GoblinMovementSimulatorSimple());


            setupBasic();
        }

        private static void placeItem(ResourceType input, Vector3 position)
        {
            var thing = new Thing() { Type = input };
            var dropped = new DroppedThing() { Thing = thing, Position = position };
        }

        [Test]
        public void TestCommunicateBig()
        {
            var wood = new ResourceType();
            var barrel = new ResourceType();
            var plank = new ResourceType();

            for (int i = 0; i < 100; i++)
            {
                placeItem(wood, nextVector(new Vector3(-100, 0.5f, -100), new Vector3(100, 0.5f, 100)));
            }

            for (int i = 0; i < 20; i++)
            {
                placeItem(wood, nextVector(new Vector3(2, 0.5f, 10), new Vector3(2, 0.5f, 10)));
            }


            engine.AddSimulator(new GoblinCommunicationSimulator());
            engine.AddSimulator(new GoblinCommandSimulator());
            engine.AddSimulator(new GoblinMovementSimulatorSimple());
            


            setupBasic();
        }

        private Vector3 nextVector(Vector3 min, Vector3 max)
        {
            return new Vector3(nextFloat(min.X, max.X), nextFloat(min.Y, max.Y), nextFloat(min.Z, max.Z));
        }

        Random r = new Random(789645);

        private float nextFloat(float min, float max)
        {
            return (float)(r.NextDouble() * (max - min) + min);
        }


        
    }
}
