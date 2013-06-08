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
using MHGameWork.TheWizards.RTSTestCase1.Animation;
using MHGameWork.TheWizards.RTSTestCase1.Inputting;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Pickupping;
using MHGameWork.TheWizards.RTSTestCase1.Players;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using NUnit.Framework;
using Rhino.Mocks;
using SlimDX;
using SlimDX.DirectInput;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    [EngineTest]
    public class PlayersTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [SetUp]
        public void Setup()
        {
            TestUtilities.CreateGroundPlane();
        }


        [Test]
        public void TestSimplePlayerInputController()
        {
            // Autoassert example.
            // the autoAssert object is injected as dependency (no static shizzle)
            var p = MockRepository.GenerateMock<UserPlayer>();
            p.Position = Vector3.Zero;

            var c = new SimplePlayerMovementController(p);
            c.MoveForward();
            c.ProcessMovement(1); // moves forward

            // autoAssert.Equal(p.Position)

            c.MoveForward();
            c.MoveLeft();
            c.ProcessMovement(1);
            // autoAssert.Equal(p.Position)

            c.MoveForward();
            c.MoveLeft();
            c.MoveRight();
            c.MoveBackward();
            c.Jump();
            // autoAssert.Equal(p.Position)


        }

        [Test]
        public void TestGroundAttack()
        {
            //TODO: try use windsor here
            // Edit i know i suggested windsor, but now i see the custom service implementation so i dont think
            //    its usefull anymore
            var targeter = MockRepository.GenerateStub<IUserTargeter>(); // TODO: mock here?
            targeter.Stub(o => o.Targeted).Return(new object());
            targeter.Stub(o => o.TargetPoint).Return(new Vector3(2, 0, 2));


            var rocks = new List<IRock>();

            var rockFactory = MockRepository.GenerateStub<IRockCreator>();
            rockFactory.Stub(f => f.CreateRock()).Do(new Func<IRock>(
                delegate
                {
                    var ret = MockRepository.GenerateStub<IRock>();
                    rocks.Add(ret);
                    return ret;
                }));

            rockFactory.Stub(f => f.DestroyRock(null)).IgnoreArguments().Do(new Action<IRock>(delegate(IRock r)
                {
                    rocks.Remove(r);
                }));


            var attack = new PlayerGroundAttacker(targeter, rockFactory, new SimpleAnimationProvider());



            engine.AddSimulator(new BasicSimulator(delegate
                {
                    if (TW.Graphics.Keyboard.IsKeyPressed(Key.F))
                        attack.Attack(1);

                    attack.Update(TW.Graphics.Elapsed);
                    attack.Render(TW.Graphics.LineManager3D);

                    foreach (var r in rocks)
                    {
                        TW.Graphics.LineManager3D.AddCenteredBox(r.Position, 1, new Color4(0.5f, 0.5f, 0));
                    }
                }));



        }

        [Test]
        public void TestTargeting()
        {
            //var drop = new DroppedThing()
            //{
            //    Thing = new Thing() { Type = TW.Data.Get<ResourceFactory>().Wood },

            //};
            //drop.Physical.WorldMatrix = Matrix.Translation(new Vector3(1, 1, 1));
            //var player = new UserPlayer() { Position = new Vector3(3, 3, 3) };

            //engine.AddSimulator(new BasicSimulator(delegate
            //    {
            //        if (player.Targeted == null) return;
            //        player.Targeted.WorldMatrix = player.Targeted.WorldMatrix *
            //                                      Matrix.Translation(TW.Graphics.Elapsed, 0, 0);
            //    }));




            //engine.AddSimulator(new PlayerTargetingSimulator());
            //engine.AddSimulator(new UpdateSimulator());
            //engine.AddSimulator(new PhysicalSimulator());
            //engine.AddSimulator(new RTSEntitySimulator());

            //engine.AddSimulator(new WorldRenderingSimulator());

            throw new NotImplementedException();
        }

        [Test]
        public void TestPickup()
        {
            var drop = new DroppedThing()
                {
                    Thing = new Thing() { Type = TW.Data.Get<ResourceFactory>().Wood },

                };

            drop.Physical.WorldMatrix = Matrix.Translation(new Vector3(1, 1, 1));
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
            //Obsolete! 

            var drop = new DroppedThing()
            {
                Thing = new Thing() { Type = TW.Data.Get<ResourceFactory>().Wood },

            };
            drop.Physical.WorldMatrix = Matrix.Translation(new Vector3(1, 1, 1));
            var player = new UserPlayer() { Position = new Vector3(3, 3, 3) };

            //engine.AddSimulator(new BasicSimulator(delegate
            //{
            //    if (player.Targeted == null) return;
            //    player.Targeted.WorldMatrix = player.Targeted.WorldMatrix *
            //                                  Matrix.Translation(TW.Graphics.Elapsed, 0, 0);
            //}));




            //engine.AddSimulator(new UserPlayerSimulator());
            engine.AddSimulator(new PlayerPickupSimulator());

            engine.AddSimulator(new PickupSimulator());

            engine.AddSimulator(new RTSEntitySimulator());

            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());
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
