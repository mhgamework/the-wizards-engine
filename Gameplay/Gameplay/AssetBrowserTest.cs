using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.Tests.Gameplay.Various;
using MHGameWork.TheWizards.WorldRendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    [TestFixture]
    public class AssetBrowserTest
    {
        [Test]
        public void TestAssetBrowser()
        {
            var game = new LocalGame();

            game.AddSimulator(new Simulators.AssetbrowserSimulator());
            game.AddSimulator(new WorldRenderingSimulator());

            game.Run();
        }
    }
}
