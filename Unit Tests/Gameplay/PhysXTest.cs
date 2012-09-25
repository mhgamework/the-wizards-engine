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
    public class PhysXTest
    {
        [Test]
        public void TestSimpleEntityPhysics()
        {
            var game = new LocalGame();

            game.AddSimulator(new PhysXSimulator());
            game.AddSimulator(new WorldRenderingSimulator());

            var e = new WorldRendering.Entity()
            {
                Mesh = MeshFactory.Load("Core\\TileSet\\ts001sg001"),
                Visible = true
            };

            game.Run();
        }
    }
}
