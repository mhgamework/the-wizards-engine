using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTS.Commands;
using MHGameWork.TheWizards.Rendering;
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
            engine.AddSimulator(new FirstPersonCameraSimulator());
            engine.AddSimulator(new GoblinRendererSimulator());
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
            var cmd = new GoblinFollowCommand();
            return createGoblinSimulator(cmd);
        }

        private static ISimulator createGoblinSimulator(IGoblinCommand cmd)
        {
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
            var cmd = new GoblinFetchCommand();
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
            var cmd = new GoblinFetchCommand();
            var type = new ResourceType();
            var thing = new Thing() { Type = type };
            var droppedThing = new DroppedThing() { Position = new Vector3(-5, 0.5f, 3), Thing = thing };


            thing = new Thing() { Type = type };
            for (int i = 0; i < 20; i++)
            {
                droppedThing = new DroppedThing() { Position = new Vector3(-20, 0.5f, 3+i), Thing = thing };
                
            }


            new Goblin() {Position = new Vector3(-10, 0.5f, 7)};

            engine.AddSimulator(new BasicSimulator(delegate
                {
                    var i = 0;
                foreach (Goblin g in TW.Data.Objects.Where(o => o is Goblin))
                {
                    cmd.Update(g, new Vector3(-5-i*10, 0.5f, -3), type);
                    i++;

                }
            }));
            engine.AddSimulator(new GoblinMovementSimulatorSimple());

            setupBasic();
        }
    }
}
