using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Simulators;
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
            game.AddSimulator(new Simulators.RenderingSimulator());

            game.Run();
        }
    }
}
