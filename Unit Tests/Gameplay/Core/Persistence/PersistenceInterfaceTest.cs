using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Persistence;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Simulators;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Gameplay.Core.Persistence
{
    [TestFixture]
    public class PersistenceInterfaceTest
    {
        [Test]
        public void TestSimulator()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            engine.AddSimulator(new PersistenceInterfaceSimulator());
            engine.AddSimulator(new BarrelShooterSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();
        }
    }
}
