using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Simulators;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    [TestFixture]
    public class DebugTest
    {
        [Test]
        public void TestProfiler()
        {
            var game = new TWEngine();
            game.DontLoadPlugin = true;
            game.Initialize();

            game.AddSimulator(new ProfilerSimulator());
            game.AddSimulator(new WorldRenderingSimulator());

            game.Run();
        }
    }
}
