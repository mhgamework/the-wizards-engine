using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.Simulators;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.RTS
{
    [TestFixture]
    public class TestGoblinSpawner
    {
        [Test]
        public void TestRenderSpawner()
        {
            var engine = new TWEngine();
            engine.DontLoadPlugin = true;
            engine.Initialize();

            var spawner = new GoblinSpawner() { Position = new Vector3(0, 0, 0) };

            //engine.AddSimulator(new BasicSimulator(delegate
            //{
            //    spawner.Position += new Vector3(0.01f, 0, 0);
            //}));

            engine.AddSimulator(new GoblinSimulator());
            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());

            

            engine.Run();
        }
    }
}
