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

        /// <summary>
        /// 1 dyanmic barrel, one static physx object and an entity without physx
        /// 
        /// Press f to drop barrel
        /// </summary>
        [Test]
        public void TestSimpleEntityPhysics()
        {
            var game = new LocalGame();

            game.AddSimulator(new BasicSimulator(delegate()
                                                     {
                                                         if (TW.Game.Keyboard.IsKeyPressed( SlimDX.DirectInput.Key.F))
                                                         {
                                                             var f = new WorldRendering.Entity
                                                             {
                                                                 Mesh = MeshFactory.Load("Core\\Barrel01"),
                                                                 Visible = true,
                                                                 Solid = true,
                                                                 Static = false,
                                                                 WorldMatrix = Matrix.Translation(0, 20, 0)
                                                             };
                                                         }
                                                        
                                                     }));

            game.AddSimulator(new PhysXSimulator());
            game.AddSimulator(new WorldRenderingSimulator());
            game.AddSimulator(new PhysXDebugRendererSimulator());


            var e = new WorldRendering.Entity
                        {
                            Mesh = MeshFactory.Load("Core\\TileSet\\ts001sg001"),
                            Visible = true,
                            Solid = true,
                            WorldMatrix = Matrix.Translation(0, 5, 0)
                        };

            e = new WorldRendering.Entity
                    {
                        Mesh = MeshFactory.Load("Core\\TileSet\\ts001sg001"),
                        Visible = true,
                        Solid = false,
                        WorldMatrix = Matrix.Translation(0, 10, 0)
                    };

            e = new WorldRendering.Entity
            {
                Mesh = MeshFactory.Load("Core\\Barrel01"),
                Visible = true,
                Solid = true,
                Static = false,
                WorldMatrix = Matrix.Translation(0, 20, 0)
            };


            game.Run();
        }
    }
}
