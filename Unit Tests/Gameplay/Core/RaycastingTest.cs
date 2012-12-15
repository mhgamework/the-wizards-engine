using System.Drawing;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.Tests.Gameplay.Various;
using MHGameWork.TheWizards.WorldRendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Gameplay.Core
{
    [TestFixture]
    public class RaycastingTest
    {
        /// <summary>
        /// Highlights the raycasted triangle
        /// </summary>
        [Test]
        public void TestRaycastEntity()
        {
            var game = new TWEngine();
            game.DontLoadPlugin = true;
            game.Initialize();

            var world = TW.Data.GetSingleton<WorldRendering.World>();

            game.AddSimulator(new BasicSimulator(delegate
                                                     {

                                                         var rayPos = TW.Graphics.Camera.ViewInverse.xna().Translation.dx();
                                                         var rayDir = TW.Graphics.Camera.ViewInverse.xna().Forward.dx();
                                                         var ray = new Ray(rayPos, rayDir);
                                                         var result = world.Raycast(ray);
                                                         if (result.IsHit)
                                                         {
                                                             TW.Graphics.LineManager3D.AddTriangle(result.V1, result.V2,
                                                                                               result.V3,
                                                                                               new Color4(Color.Yellow));
                                                         }
                                                     }));

            game.AddSimulator(new WorldRenderingSimulator());

            new WorldRendering.Entity
               {
                   Mesh = MeshFactory.Load("Core\\Barrel01"),
                   Visible = true,
                   Solid = true,
                   Static = false,
                   WorldMatrix = Matrix.Translation(0, 0.5f, 0)
               };
            new WorldRendering.Entity
                {
                    Mesh = MeshFactory.Load("Core\\Barrel01"),
                    Visible = true,
                    Solid = true,
                    Static = false,
                    WorldMatrix = Matrix.Translation(0, 0.5f, 1)
                };

            game.Run();
        }
    }
}
