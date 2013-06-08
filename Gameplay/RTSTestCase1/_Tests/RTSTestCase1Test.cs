using System;
using System.Collections;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.RTSTestCase1.Animation;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Inputting;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Pickupping;
using MHGameWork.TheWizards.RTSTestCase1.Players;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using MHGameWork.TheWizards.RTSTestCase1.Simulators;
using NUnit.Framework;
using SlimDX;
using System.Linq;

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
        public void TestSandboxCatch()
        {
            try
            {
                TestSandbox();
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Test Setup");
            }
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
            c.Register(Component.For<IDamageApplier>().ImplementedBy<SimpleDamageApplier>());

            engine.AddSimulator(c.Resolve<NetworkReceiveSimulator>());
            engine.AddSimulator(c.Resolve<InputSimulator>());
            engine.AddSimulator(c.Resolve<UserPlayerSimulator>());
            engine.AddSimulator(c.Resolve<NPCSimulator>());
            engine.AddSimulator(c.Resolve<RiverSimulator>());
            engine.AddSimulator(c.Resolve<MagicSimulator>());
            engine.AddSimulator(c.Resolve<AnimationSimulator>());
            engine.AddSimulator(c.Resolve<NetworkSendSimulator>());
            engine.AddSimulator(c.Resolve<RendererSimulator>());




            DI.Get<TestSceneBuilder>().Setup = delegate
                {
                    new Groundplane();

                    var g = new Goblin();
                    g.Physical.WorldMatrix = Matrix.Translation(3, 0, 3);

                    var d = new DroppedThing();
                    d.Thing = new Thing() { Type = ResourceFactory.Get.Wood };
                    d.Item.Free = true;
                    d.Physical.WorldMatrix = Matrix.Translation(5, 0, 5);
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



    public class SimpleWorldLocator : IWorldLocator
    {
        public IEnumerable<object> AtPosition(Vector3 point, float radius)
        {
            return
                TW.Data.Objects.OfType<IPhysical>()
                  .Where(p => Vector3.Distance(p.Physical.GetPosition(), point) < radius);
        }
    }

    public class SimpleDamageApplier : IDamageApplier
    {
        public void ApplyDamage(object o)
        {
            if (o is Goblin)
            {
                ((IPhysical)o).Physical.Visible = false;
                TW.Data.RemoveObject((IModelObject)o);
            }
        }
    }
}