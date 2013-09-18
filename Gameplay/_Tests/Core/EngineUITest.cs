using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Gameplay.Core
{
    [TestFixture]
    public class EngineUITest
    {
        [Test]
        public void TestEngineUI()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            engine.AddSimulator(new EngineUISimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();
        }
    }
}
