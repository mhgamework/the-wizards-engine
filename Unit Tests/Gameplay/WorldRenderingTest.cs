using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
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
        [Test]
        public void TestEntityVisible()
        {
            var game = new LocalGame();

            game.AddSimulator(new Simulators.WorldRenderingSimulator());

            var e = new WorldRendering.Entity()
                        {
                            Mesh = MeshFactory.Load("Core\\TileSet\\ts001sg001"),
                            Visible = false
                        };


            game.AddSimulator(new BasicSimulator(delegate
                                                     {

                                                     }));


            game.Run();
        }

        [Test]
        public void TestPointLight()
        {
            var game = new LocalGame();


            var e = new WorldRendering.Entity()
            {
                Mesh = MeshFactory.Load("Core\\TileSet\\ts001sg001")
            };

            var light = new PointLight
                            {
                                Position = new Vector3(-1, 0, 0)
                            };
            var time = 0f;
            game.AddSimulator(new BasicSimulator(delegate
                                                 {
                                                     time += TW.Graphics.Elapsed;
                                                     light.Position = new Vector3(-1, (float)Math.Sin(time), (float)Math.Cos(time));
                                                     light.Size = 10;
                                                     light.Intensity = 2;
                                                 }));
            game.AddSimulator(new PointLightSimulator());
            game.AddSimulator(new Simulators.WorldRenderingSimulator());




            game.Run();
        }

        [Test]
        public void TestTextarea()
        {
            var game = new LocalGame();

            var a = new Textarea
                        {
                            Position = new Vector2(100, 100)
                        };

            game.AddSimulator(new BasicSimulator(delegate()
                                                     {
                                                         a.Text = TW.Graphics.FPS.ToString();
                                                     }));

            game.AddSimulator(new Simulators.WorldRenderingSimulator());


            game.Run();

        }
    }
}
