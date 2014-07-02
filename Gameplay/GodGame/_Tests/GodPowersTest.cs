using System.Drawing;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    [TestFixture]
    [EngineTest]
    public class GodPowersTest
    {
        [Test]
        public void TestShapeLand()
        {
            var engine = EngineFactory.CreateEngine();
            engine.Initialize();

            var world = new World(10, 10);


            var playerInputHandler = new PlayerInputHandler(world);
            var playerInputSimulator = new PlayerInputSimulator(playerInputHandler, world);
            engine.AddSimulator(playerInputSimulator);
            engine.AddSimulator(new SimpleWorldRenderer(world));
            engine.AddSimulator(new WorldRenderingSimulator());

        }
    }
}