using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            goblin.Physical.WorldMatrix = Matrix.Translation(new Vector3(2, 0, 4));

            goblin = new Goblin();
            goblin.Physical.WorldMatrix = Matrix.Translation(new Vector3(2, 0, 6));
            goblin.Commands.ShowingCommands = true;
        }
    }
}
