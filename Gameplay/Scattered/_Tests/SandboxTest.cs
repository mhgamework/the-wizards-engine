using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Rendering;
using MHGameWork.TheWizards.Scattered.Simulation;
using MHGameWork.TheWizards.Scattered.Simulation.Sandbox;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    [TestFixture]
    [EngineTest]
    public class SandboxTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestSandbox()
        {
            var level = new Level();
            level.CreateNewIsland(new Vector3(20, 0, 30));
            var config = new EditorConfiguration();

            engine.AddSimulator(new SandboxControllerSimulator(level, config));
            engine.AddSimulator(new WorldInputtingSimulator(config));

            engine.AddSimulator(new LevelRenderer(level));
            engine.AddSimulator(new WorldRenderingSimulator());
        }
    }
}