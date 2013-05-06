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
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Inputting;
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

        /// <summary>
        /// This is a full testcase for the goblins functionality. Unit tests should be added later on when the testcase is completed.
        /// </summary>
        [Test]
        public void TestAll()
        {
            createWorld();

            engine.AddSimulator(new Simulator());

            engine.AddSimulator(new GoblinCommandsSimulator());

            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());
        }

        private void createWorld()
        {
            var f = TW.Data.Get<CommandFactory>();
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


            var data = TW.Data.Get<Data>();



            data.CommandsGoblin = new Goblin();
            data.CommandsGoblin.Physical.WorldMatrix = Matrix.Translation(new Vector3(2, 0, 6));
            data.CommandsGoblin.Commands.ShowingCommands = true;




            data.HolderCart = new Cart();
            data.HolderCart.Physical.WorldMatrix = Matrix.Translation(new Vector3(2, 0, 10));
            data.HolderCart.CommandHolder.AssignedCommands.Add(new GoblinCommandOrb { Type = f.Follow });
            data.HolderCart.CommandHolder.AssignedCommands.Add(new GoblinCommandOrb { Type = f.Defend });

        }

        public class Simulator : ISimulator
        {
            private Data data = TW.Data.Get<Data>();
            private ClockedTimer clock = new ClockedTimer(TW.Graphics);
            public void Simulate()
            {
                clock.Tick(data.CommandsGoblin, tickCommandsGoblin());
                clock.Tick(data.HolderCart, tickHolderCart());
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
        }

        public class Data : EngineModelObject
        {
            public Goblin CommandsGoblin { get; set; }
            public Cart HolderCart { get; set; }
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

                    task.nextTime = game.TotalRunTime + task.Enumerator.Current;

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
