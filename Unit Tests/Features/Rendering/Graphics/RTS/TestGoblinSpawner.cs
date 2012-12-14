using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.Simulators;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Graphics.RTS
{
    [TestFixture]
    public class TestGoblinSpawner
    {
        [Test]
        public void TestRenderSpawner()
        {
            var engine = new TWEngine {DontLoadPlugin = true};

            engine.Initialize();    
            new GoblinSpawner() { Position = new Vector3(0, 0, 0) };
            engine.AddSimulator(new GoblinSimulator());
            //engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            //engine.AddSimulator(new PhysXDebugRendererSimulator());
            

            engine.Run();
        }
    }
}
