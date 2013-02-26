using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.RTSTestCase1.Inputting;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Pickupping;
using MHGameWork.TheWizards.RTSTestCase1.Players;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using NUnit.Framework;
using SlimDX;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    [EngineTest]
    public class UserPlayerTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [SetUp]
        public void Setup()
        {
            TestUtilities.CreateGroundPlane();
        }

        [Test]
        public void TestTargeting()
        {
            var drop = new DroppedThing()
            {
                Thing = new Thing() { Type = TW.Data.Get<ResourceFactory>().Wood },
                InitialPosition = new Vector3(1, 1, 1)
            };

            var player =  new UserPlayer() { Position = new Vector3(3, 3, 3) };

            engine.AddSimulator(new BasicSimulator(delegate
                {
                    if (player.Targeted == null) return;
                    player.Targeted.WorldMatrix = player.Targeted.WorldMatrix*
                                                  Matrix.Translation(TW.Graphics.Elapsed, 0, 0);
                }));
            

            

            engine.AddSimulator(new PlayerTargetingSimulator());
            engine.AddSimulator(new UpdateSimulator());

            engine.AddSimulator(new RTSEntitySimulator());

            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestPickup()
        {
            var drop = new DroppedThing()
                {
                    Thing = new Thing() { Type = TW.Data.Get<ResourceFactory>().Wood },
                    InitialPosition = new Vector3(1, 1, 1)
                };


            engine.AddSimulator(new UpdateSimulator());

            new PickupTest() { Drop = drop };

            engine.AddSimulator(new PlayerPickupSimulator());

            engine.AddSimulator(new PickupSimulator());

            engine.AddSimulator(new RTSEntitySimulator());

            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());
        }

        [Test]
        public void TestAll()
        {
            var drop = new DroppedThing()
            {
                Thing = new Thing() { Type = TW.Data.Get<ResourceFactory>().Wood },
                InitialPosition = new Vector3(1, 1, 1)
            };

            var player = new UserPlayer() { Position = new Vector3(3, 3, 3) };

            //engine.AddSimulator(new BasicSimulator(delegate
            //{
            //    if (player.Targeted == null) return;
            //    player.Targeted.WorldMatrix = player.Targeted.WorldMatrix *
            //                                  Matrix.Translation(TW.Graphics.Elapsed, 0, 0);
            //}));




            engine.AddSimulator(new PlayerTargetingSimulator());
            engine.AddSimulator(new PlayerPickupSimulator());

            engine.AddSimulator(new PickupSimulator());

            engine.AddSimulator(new RTSEntitySimulator());

            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());
        }


        [Test]
        public void TestMovement()
        {
            var player = new UserPlayer() { Position = new Vector3(3, 3, 3) };

            engine.AddSimulator(new InputSimulator());  

            engine.AddSimulator(new PlayerMovementSimulator());
            engine.AddSimulator(new PlayerCameraSimulator());

            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [ModelObjectChanged]
        public class PickupTest : EngineModelObject, IUpdatableObject
        {
            public UserPlayer Player { get; private set; }

            public DroppedThing Drop { get; set; }

            public PickupTest()
            {
                Player = new UserPlayer() { Position = new Vector3(3, 3, 3) };

            }

            public void Update()
            {
                Player.Targeted = Drop.get<Entity>();
                Player.TargetDistance = 2;
            }
        }
        public interface IUpdatableObject : IModelObject
        {
            void Update();
        }
        public class UpdateSimulator : ISimulator
        {
            public void Simulate()
            {
                foreach (var g in TW.Data.Objects.Where(o => o is IUpdatableObject).Cast<IUpdatableObject>().ToArray())
                    g.Update();
            }
        }
    }

}
