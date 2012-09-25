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
    public class WorldRenderingTest
    {
        [Test]
        public void TestWireframebox()
        {
            var game = new LocalGame();

            game.AddSimulator(new Simulators.WorldRenderingSimulator());

            var b = new WireframeBox()
                        {
                            WorldMatrix = Matrix.Translation(new Vector3(1, 0, 1)),
                            Color = new Color4(1, 0, 0)
                        };

            game.Run();
        }
    }
}
