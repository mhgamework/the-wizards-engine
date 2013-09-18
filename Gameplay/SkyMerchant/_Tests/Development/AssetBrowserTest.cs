using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Tests.Gameplay.Various;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Gameplay
{
    [EngineTest]
    [TestFixture]
    public class AssetBrowserTest
    {
        [Test]
        public void TestAssetBrowser()
        {
            var game = EngineFactory.CreateEngine();

            game.AddSimulator(new Simulators.AssetbrowserSimulator());
            game.AddSimulator(new WorldRenderingSimulator());

            game.Run();
        }
    }
}
