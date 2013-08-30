using System;
using System.Collections;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.RTSTestCase1.Animation;
using MHGameWork.TheWizards.RTSTestCase1.Building;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Spawning;
using MHGameWork.TheWizards.RTSTestCase1.Inputting;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Magic;
using MHGameWork.TheWizards.RTSTestCase1.Pickupping;
using MHGameWork.TheWizards.RTSTestCase1.Players;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using MHGameWork.TheWizards.RTSTestCase1.Simulators;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;
using MHGameWork.TheWizards.RTSTestCase1._Common;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    /// <summary>
    /// The in the wiki explained 'bigtest'. This is a sandbox version of the game test!
    /// </summary>
    [EngineTest]
    [TestFixture]
    public class RTSTestCase1Test
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        /// <summary>
        /// This allows displaying errors when testing, due to bug in the NUnitTestRunner not using the iErrorLogger
        /// </summary>
        [Test]
        [Timeout(100000)]
        public void TestSandboxCatch()
        {
            engine.Initialize();
            try
            {
                TestSandbox();
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Test Setup");
            }
            engine.Run();
        }

        [Test]
        public void TestSandbox()
        {
            var c = new WindsorContainer();
            // Register all simulators first: these should be used as primary dependencies!?
            c.Register(
                Classes.FromThisAssembly()
                // doesnt work for some reason? .InNamespace("MHGamework.TheWizards.RTSTestCase1")
                        .Where(o => typeof(ISimulator).IsAssignableFrom(o))
                        .WithServiceAllInterfaces().WithServiceSelf()
                        );
            //c.Register(
            //    Classes.FromThisAssembly().Pick().WithServiceSelf().WithServiceAllInterfaces()
            //    //.Where(o => typeof(ISimulator).IsAssignableFrom(o)).WithServiceSelf()
            //            );
            c.Register(Component.For<IPlayerMovementController>().ImplementedBy<SimplePlayerMovementController>());
            c.Register(Component.For<UserPlayer>().Instance(TW.Data.Get<LocalGameData>().LocalPlayer));
            c.Register(Component.For<PlayerGroundAttacker>());
            c.Register(Component.For<IAnimationProvider>().ImplementedBy<SimpleAnimationProvider>());

            c.Register(Component.For<IWorldLocator>().ImplementedBy<SimpleWorldLocator>());
            c.Register(Component.For<IWorldDestroyer>().ImplementedBy<SimpleWorldDestroyer>());
            c.Register(Component.For<IDamageApplier>().ImplementedBy<SimpleDamageApplier>());

            c.Register(Component.For<SimpleItemPhysicsUpdater>());
            c.Register(Component.For<SimpleBuilder>());
            


            c.Register(Component.For<GoblinSpawner>());
            c.Register(Component.For<IGoblinCreator>().ImplementedBy<GoblinCreator>());
            
            
            


            engine.AddSimulator(c.Resolve<NetworkReceiveSimulator>());
            engine.AddSimulator(c.Resolve<InputSimulator>());
            engine.AddSimulator(c.Resolve<UserPlayerSimulator>());
            engine.AddSimulator(c.Resolve<NPCSimulator>());
            engine.AddSimulator(c.Resolve<WorldSimulator>());
            engine.AddSimulator(c.Resolve<MagicSimulator>());
            engine.AddSimulator(c.Resolve<AnimationSimulator>());
            engine.AddSimulator(c.Resolve<NetworkSendSimulator>());
            engine.AddSimulator(c.Resolve<RendererSimulator>());




            DI.Get<TestSceneBuilder>().Setup = delegate
                {
                    new Groundplane();

                    //var g = new Goblin();
                    //g.Physical.WorldMatrix = Matrix.Translation(3, 0, 3);
                    //var crystal = new SimpleCrystal(){Capacity = 1000,Energy = 500};
                    
                    var spawner = new GoblinSpawnPoint();
                    spawner.Position = new Vector3(24, 0, 16);
                    spawner = new GoblinSpawnPoint();
                    spawner.Position = new Vector3(18, 0, 24);
                    spawner = new GoblinSpawnPoint();
                    spawner.Position = new Vector3(20, 0, 19);

                    for (int i = 0; i < 10; i++)
                    {
                        var d = new DroppedThing();
                        d.Thing = new Thing() { Type = ResourceFactory.Get.Wood };
                        d.Item.Free = true;
                        d.Physical.WorldMatrix = Matrix.Translation(5 + i, 0, 5);


                        d = new DroppedThing();
                        d.Thing = new Thing() { Type = ResourceFactory.Get.Stone };
                        d.Item.Free = true;
                        d.Physical.WorldMatrix = Matrix.Translation(5 + i, 0, 6);

                        d = new DroppedThing();
                        d.Thing = new Thing() { Type = ResourceFactory.Get.Cannonball };
                        d.Item.Free = true;
                        d.Physical.WorldMatrix = Matrix.Translation(5 + i, 0, 7);
                    }


                    var cr = new Cart();
                    cr.Physical.WorldMatrix = Matrix.Translation(5, 0, 0);


                    var t = new Tree();
                    t.Position = new Vector3(12,0,3);

                    t = new Tree();
                    t.Position = new Vector3(15, 0, 3);

                    t = new Tree();
                    t.Position = new Vector3(17, 0, 3);


                    var r = new Rock();
                    r.Position = new Vector3(-12, 0, 8);

                    r = new Rock();
                    r.Position = new Vector3(-15, 0, 20);

                    r = new Rock();
                    r.Position = new Vector3(-17, 0, 14);
                };



        }

        [ModelObjectChanged]
        public class Groundplane : EngineModelObject, IPhysical
        {
            public Groundplane()
            {
                Physical = new Physical();
            }
            public Physical Physical { get; set; }
            public void UpdatePhysical()
            {
                Physical.Mesh = TW.Assets.LoadMesh("Core\\Building\\Plane");
                Physical.ObjectMatrix = Matrix.Scaling(1000, 1000, 1000);
                Physical.WorldMatrix = Matrix.Identity;
            }
        }
    }
}