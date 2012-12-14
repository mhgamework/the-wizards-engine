using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.PhysX;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.Tests.Gameplay.Various;
using MHGameWork.TheWizards.WorldRendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Gameplay.Core
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
                                                         if (TW.Graphics.Keyboard.IsKeyPressed( SlimDX.DirectInput.Key.F))
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

        /// <summary>
        /// 1 dyanmic barrel, one static physx object and an entity without physx
        /// 
        /// Press f to drop barrel
        /// </summary>
        [Test]
        public void TestBarrelShooterSimulator()
        {
            var game = new TWEngine();
            game.DontLoadPlugin = true;
            game.Initialize();


            game.AddSimulator(new BarrelShooterSimulator());
            game.AddSimulator(new PhysXSimulator());
            game.AddSimulator(new WorldRenderingSimulator());
            game.AddSimulator(new PhysXDebugRendererSimulator());

            game.Run();
        }
    }
}
