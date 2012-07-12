using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.WorldRendering;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    [TestFixture]
    public class WorldRenderingTest
    {
        public void TestWireframebox()
        {
            var game = new LocalGame();

            game.AddSimulator(new RenderingSimulator());
        }
    }
}
