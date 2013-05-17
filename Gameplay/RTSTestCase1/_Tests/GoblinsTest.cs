using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Inputting;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    [EngineTest]
    public class GoblinsTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        private CommandFactory f;
        private Data data;

        [SetUp]
        public void Setup()
        {
            f = TW.Data.Get<CommandFactory>();
            data = TW.Data.Get<Data>();
        }
        /// <summary>
        /// This is a full testcase for the goblins functionality. Unit tests should be added later on when the testcase is completed.
        /// </summary>
        [Test]
        public void TestAll()
        {
            createWorld();

            engine.AddSimulator(new Simulator());

            engine.AddSimulator(new GoblinCommandsSimulator());

            engine.AddSimulator(new ItemStorageSimulator());


            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());
        }

        private void createWorld()
        {
            TestUtilities.CreateGroundPlane();

            TestRender();
            TestShowCommands();
            TestHoldOrbs();
            TestFollowOrb();
            TestFollowWithCart();
            TestItemStorage();
            TestMoveItems();

        }

        private void TestMoveItems()
        {
            Matrix offset = Matrix.Translation(0, 0, 28);

            StorageCrate crate1, crate2;
            crate1 = new StorageCrate();
            crate1.Physical.WorldMatrix = Matrix.Translation(2, 0, 0) * offset;

            for (int i = 0; i < 9*3; i++)
            {
                crate1.ItemStorage.Items.Add(new DroppedThing() { Thing = new Thing() { Type = TW.Data.Get<ResourceFactory>().Stone } });
            }

            crate2 = new StorageCrate();
            crate2.Physical.WorldMatrix = Matrix.Translation(10, 0, 0) * offset;



            var orbSource = new GoblinCommandOrb() { Type = TW.Data.Get<CommandFactory>().MoveSource };
            var orbTarget = new GoblinCommandOrb() { Type = TW.Data.Get<CommandFactory>().MoveTarget };
            crate1.CommandHolder.AssignedCommands.Add(orbSource);
            crate2.CommandHolder.AssignedCommands.Add(orbTarget);
            

            Goblin goblin;

            goblin = new Goblin();
            goblin.Physical.WorldMatrix = Matrix.Translation(2, 0, 0)*offset;

            goblin.Commands.Orbs.Add(orbSource);
            goblin.Commands.Orbs.Add(orbTarget);




            Cart cart = new Cart();
            cart.Physical.WorldMatrix = Matrix.Translation(14, 0, 0)*offset;


            orbSource = new GoblinCommandOrb() { Type = TW.Data.Get<CommandFactory>().MoveSource };
            orbTarget = new GoblinCommandOrb() { Type = TW.Data.Get<CommandFactory>().MoveTarget };
            crate2.CommandHolder.AssignedCommands.Add(orbSource);
            cart.CommandHolder.AssignedCommands.Add(orbTarget);

            goblin = new Goblin();
            goblin.Physical.WorldMatrix = Matrix.Translation(15, 0, 0) * offset;

            goblin.Commands.Orbs.Add(orbSource);
            goblin.Commands.Orbs.Add(orbTarget);




        }

        private void TestItemStorage()
        {
            StorageCrate crate;
            crate = new StorageCrate();
            crate.Physical.WorldMatrix = Matrix.Translation(2, 0, 24);

            Cart cart;
            cart = new Cart();
            cart.Physical.WorldMatrix = Matrix.Translation(6, 0, 24);

            for (int i = 0; i < 5; i++)
            {
                crate.ItemStorage.Items.Add(new DroppedThing() { Thing = new Thing() { Type = TW.Data.Get<ResourceFactory>().Stone } });
                cart.ItemStorage.Items.Add(new DroppedThing() { Thing = new Thing() { Type = TW.Data.Get<ResourceFactory>().Stone } });

            }

            crate = new StorageCrate();
            crate.Physical.WorldMatrix = Matrix.Translation(10, 0, 24);

            for (int i = 0; i < 50; i++)
            {
                crate.ItemStorage.Items.Add(new DroppedThing() { Thing = new Thing() { Type = TW.Data.Get<ResourceFactory>().Stone } });
                cart.ItemStorage.Items.Add(new DroppedThing() { Thing = new Thing() { Type = TW.Data.Get<ResourceFactory>().Stone } });

            }




        }

        private void TestFollowOrb()
        {
            Goblin goblin;
            data.FollowOrb = new GoblinCommandOrb() { Type = TW.Data.Get<CommandFactory>().Follow };
            data.FollowOrb.Physical.WorldMatrix = Matrix.Translation(new Vector3(2, 0, 20));
            goblin = new Goblin();
            goblin.Commands.Orbs.Add(data.FollowOrb);
            goblin.Physical.WorldMatrix = Matrix.Translation(2, 0, 16);
        }

        private void TestFollowWithCart()
        {
            Goblin goblin;
            data.FollowOrbCart = new GoblinCommandOrb() { Type = TW.Data.Get<CommandFactory>().Follow };
            data.FollowOrbCart.Physical.WorldMatrix = Matrix.Translation(new Vector3(0, 0, 16));
            goblin = new Goblin();
            goblin.Commands.Orbs.Add(data.FollowOrbCart);
            goblin.Physical.WorldMatrix = Matrix.Translation(2, 0, 16);
            goblin.Cart = new Cart();
        }

        private void TestHoldOrbs()
        {
            data.HolderCart = new Cart();
            data.HolderCart.Physical.WorldMatrix = Matrix.Translation(new Vector3(2, 0, 10));
            data.HolderCart.CommandHolder.AssignedCommands.Add(new GoblinCommandOrb
                {
                    Type = TW.Data.Get<CommandFactory>().Follow
                });
            data.HolderCart.CommandHolder.AssignedCommands.Add(new GoblinCommandOrb
                {
                    Type = TW.Data.Get<CommandFactory>().Defend
                });
        }

        private void TestShowCommands()
        {
            data.CommandsGoblin = new Goblin();
            data.CommandsGoblin.Physical.WorldMatrix = Matrix.Translation(new Vector3(2, 0, 6));
            data.CommandsGoblin.Commands.ShowingCommands = true;
        }

        private void TestRender()
        {
            GoblinCommandOrb g;
            g = new GoblinCommandOrb { Type = f.Follow };
            g.Physical.WorldMatrix = Matrix.Translation(new Vector3(2, 0, 2));

            g = new GoblinCommandOrb { Type = f.Defend };
            g.Physical.WorldMatrix = Matrix.Translation(new Vector3(4, 0, 2));

            Goblin goblin;
            goblin = new Goblin();
            goblin.Physical.WorldMatrix = Matrix.Translation(new Vector3(6, 0, 2));

            Cart cart;
            cart = new Cart();
            cart.Physical.WorldMatrix = Matrix.Translation(new Vector3(8, 0, 2));

            DroppedThing drop;
            drop = new DroppedThing();
            drop.Thing = new Thing() { Type = TW.Data.Get<ResourceFactory>().Stone };
            drop.Physical.WorldMatrix = Matrix.Translation(new Vector3(10, 0, 2));

            StorageCrate crate;
            crate = new StorageCrate();
            crate.Physical.WorldMatrix = Matrix.Translation(12, 0, 2);
        }

        public class Simulator : ISimulator
        {
            private Data data = TW.Data.Get<Data>();
            private ClockedTimer clock = new ClockedTimer(TW.Graphics);
            public void Simulate()
            {
                if (data.CommandsGoblin != null) clock.Tick(data.CommandsGoblin, tickCommandsGoblin());
                if (data.HolderCart != null) clock.Tick(data.HolderCart, tickHolderCart());
                if (data.FollowOrb != null) clock.Tick(data.FollowOrb, tickFollowOrb());
                if (data.FollowOrbCart != null) clock.Tick(data.FollowOrbCart, tickFollowOrbCart());
            }

            private IEnumerable<float> tickHolderCart()
            {
                data.HolderCart.Physical.WorldMatrix = Matrix.Translation(new Vector3(2, 0, 10));
                yield return 2;
                data.HolderCart.Physical.WorldMatrix = Matrix.Translation(new Vector3(4, 0, 10));
                yield return 2;
            }

            private IEnumerable<float> tickCommandsGoblin()
            {
                data.CommandsGoblin.Commands.ShowingCommands = !data.CommandsGoblin.Commands.ShowingCommands;
                yield return 2;
            }

            private IEnumerable<float> tickFollowOrbCart()
            {
                data.FollowOrbCart.Physical.WorldMatrix = Matrix.Translation(2, 0, 16);
                yield return 5;
                data.FollowOrbCart.Physical.WorldMatrix = Matrix.Translation(6, 0, 16);
                yield return 5;
            }
            private IEnumerable<float> tickFollowOrb()
            {
                data.FollowOrb.Physical.WorldMatrix = Matrix.Translation(2, 0, 20);
                yield return 5;
                data.FollowOrb.Physical.WorldMatrix = Matrix.Translation(6, 0, 20);
                yield return 5;
            }
        }

        public class Data : EngineModelObject
        {
            public Goblin CommandsGoblin { get; set; }
            public Cart HolderCart { get; set; }

            public GoblinCommandOrb FollowOrb { get; set; }

            public GoblinCommandOrb FollowOrbCart { get; set; }
        }

        public class ClockedTimer
        {
            private readonly DX11Game game;
            private Dictionary<object, ElementData> tasks = new Dictionary<object, ElementData>();

            public ClockedTimer(DX11Game game)
            {
                this.game = game;
            }

            public void Tick(object obj, IEnumerable<float> cmds)
            {
                if (!tasks.ContainsKey(obj))
                {
                    tasks[obj] = new ElementData
                        {
                            Enumerable = cmds,
                            nextTime = -1
                        };
                }
                var task = tasks[obj];

                if (task.nextTime < game.TotalRunTime)
                {
                    // Create enumerator if not exists
                    if (task.Enumerator == null)
                        task.Enumerator = task.Enumerable.GetEnumerator();
                    else
                    {
                        if (!task.Enumerator.MoveNext())
                            task.Enumerator = task.Enumerable.GetEnumerator();
                    }

                    if (task.Enumerator.Current < 0.0001) //TODO this is bug?
                        return;

                    while (task.nextTime < game.TotalRunTime)
                        task.nextTime = task.nextTime + task.Enumerator.Current;

                    //Console.WriteLine(task.nextTime);
                }
            }

            private class ElementData
            {
                public IEnumerable<float> Enumerable;
                public IEnumerator<float> Enumerator;
                public float nextTime;
            }
        }
    }
}
