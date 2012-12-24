using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.Engine.Tests.PhysX
{
    public class SimpleEntityPhysicsTest : ITestSimulator
    {

        /// <summary>
        /// 1 dyanmic barrel, one static physx object and an entity without physx
        /// 
        /// Press f to drop barrel
        /// </summary>
        public void TestSimpleEntityPhysics()
        {


        }

        public void Simulate()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(SlimDX.DirectInput.Key.F))
            {
                var f = new Engine.WorldRendering.Entity
                {
                    Mesh = MeshFactory.Load("Core\\Barrel01"),
                    Visible = true,
                    Solid = true,
                    Static = false,
                    WorldMatrix = Matrix.Translation(0, 20, 0)
                };
            }
        }

        public void Initialize(TWEngine game)
        {

            game.AddSimulator(new PhysXSimulator());
            game.AddSimulator(new WorldRenderingSimulator());
            game.AddSimulator(new PhysXDebugRendererSimulator());


            var e = new Engine.WorldRendering.Entity
            {
                Mesh = MeshFactory.Load("Core\\TileSet\\ts001sg001"),
                Visible = true,
                Solid = true,
                WorldMatrix = Matrix.Translation(0, 5, 0)
            };

            e = new Engine.WorldRendering.Entity
            {
                Mesh = MeshFactory.Load("Core\\TileSet\\ts001sg001"),
                Visible = true,
                Solid = false,
                WorldMatrix = Matrix.Translation(0, 10, 0)
            };

            e = new Engine.WorldRendering.Entity
            {
                Mesh = MeshFactory.Load("Core\\Barrel01"),
                Visible = true,
                Solid = true,
                Static = false,
                WorldMatrix = Matrix.Translation(0, 20, 0)
            };


        }
    }
}
