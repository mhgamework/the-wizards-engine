using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Tests.Gameplay.Various;
using MHGameWork.TheWizards.WorldRendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Gameplay.Core
{
    [TestFixture]
    [EngineTest]
    public class WorldRenderingTest
    {
        [Test]
        public void TestWireframebox()
        {
            var game = EngineFactory.CreateEngine();
            game.Initialize();

            game.AddSimulator(new WorldRenderingSimulator());

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
            var game = EngineFactory.CreateEngine();
            game.Initialize();

            game.AddSimulator(new WorldRenderingSimulator());

            var e = new Engine.WorldRendering.Entity()
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
            var game = EngineFactory.CreateEngine();
            game.Initialize();

            var e = new Engine.WorldRendering.Entity()
            {
                Mesh = MeshFactory.Load("Core\\TileSet\\ts001sg001")
            };

            var light = new PointLight
                            {
                                Position = new Vector3(-1, 0, 0)
                            };
            var time = 2f;


            game.AddSimulator(new BasicSimulator(delegate
                                                 {
                                                     time += TW.Graphics.Elapsed;
                                                     light.Position = new Vector3(-1, (float)Math.Sin(time), (float)Math.Cos(time));
                                                     light.Size = 10;
                                                     light.Intensity = 2;
                                                 }));

            
            game.AddSimulator(new WorldRenderingSimulator());




            game.Run();
        }

        [Test]
        public void TestTextarea()
        {
            var game = EngineFactory.CreateEngine();
            game.Initialize();

            var a = new Textarea
                        {
                            Position = new Vector2(100, 100)
                        };

            game.AddSimulator(new BasicSimulator(delegate()
                {
                    a.Text = "hello";
                }));

            game.AddSimulator(new WorldRenderingSimulator());


            game.Run();

        }
    }
}
