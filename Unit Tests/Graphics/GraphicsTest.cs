using System.Collections.Generic;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Graphics
{
    [TestFixture]
    public class GraphicsTest
    {
        /// <summary>
        /// Tests the QuadtreeVisualizer by creating a quadtree from scratch.
        /// </summary>
        [NUnit.Framework.Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestRunXNAGame()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            game.Run();


        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestSphereMesh()
        {
            XNAGame game = new XNAGame();

            List<SphereMesh> meshes = new List<SphereMesh>();


            TangentVertex[] vertices;
            short[] indices;
            SphereMesh.CreateUnitSphereVerticesAndIndices(4, out vertices, out indices);

            bool wireframe = false;

            game.InitializeEvent +=
                delegate
                {
                    SphereMesh mesh;
                    mesh = new SphereMesh(1, 4, Color.Green);
                    mesh.Initialize(game);
                    meshes.Add(mesh);

                    mesh = new SphereMesh(1, 5, Color.PowderBlue);
                    mesh.WorldMatrix = Matrix.CreateTranslation(3, 0, 0);
                    mesh.Initialize(game);
                    meshes.Add(mesh);

                    mesh = new SphereMesh(1, 12, Color.Yellow);
                    mesh.WorldMatrix = Matrix.CreateTranslation(6, 0, 0);
                    mesh.Initialize(game);
                    meshes.Add(mesh);

                    mesh = new SphereMesh(1, 30, Color.Turquoise);
                    mesh.WorldMatrix = Matrix.CreateTranslation(9, 0, 0);
                    mesh.Initialize(game);
                    meshes.Add(mesh);

                    mesh = new SphereMesh(40, 100, Color.Red);
                    mesh.WorldMatrix = Matrix.CreateTranslation(0, 0, -60);
                    mesh.Initialize(game);
                    meshes.Add(mesh);


                    mesh = new SphereMesh(40, 13, Color.Red);
                    mesh.WorldMatrix = Matrix.CreateTranslation(100, 0, -60);
                    mesh.Initialize(game);
                    meshes.Add(mesh);

                };
            game.UpdateEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.W))
                    {
                        wireframe = !wireframe;
                    }
                };

            game.DrawEvent +=
                delegate
                {
                    game.LineManager3D.AddAABB(meshes[1].BoundingBox, Matrix.Identity, Color.Red);

                    int i;
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.R))
                    {
                        for (i = 0; i < meshes.Count; i++)
                        {

                            meshes[i].ReloadShader(game);
                        }
                    }
                    game.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
                    game.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

                    if (!wireframe)
                        for (i = 1; i < meshes.Count; i++)
                        {
                            meshes[i].Render(game);
                        }


                    i = 0;
                    meshes[i].Color = Color.LightGreen;
                    meshes[i].Render(game);

                    game.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
                    meshes[i].Color = Color.Red;
                    meshes[i].Render(game);

                    game.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
                    game.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

                    meshes[i].Color = Color.Black;
                    meshes[i].Render(game);

                    if (wireframe)
                        for (i = 1; i < meshes.Count; i++)
                        {
                            meshes[i].Render(game);
                        }
                    for (i = 0; i < vertices.Length; i++)
                    {
                        game.LineManager3D.AddCenteredBox(vertices[i].pos, 0.05f, Color.Red);
                    }




                };

            game.Run();

        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestPlaneMesh()
        {
            XNAGame game = new XNAGame();

            List<PlaneMesh> meshes = new List<PlaneMesh>();

            bool wireframe = false;

            game.InitializeEvent +=
                delegate
                {
                    PlaneMesh mesh;
                    mesh = new PlaneMesh(10, 10, Color.White);
                    mesh.Initialize(game);
                    meshes.Add(mesh);

                    mesh = new PlaneMesh(100, 100, Color.White);
                    mesh.WorldMatrix = Matrix.CreateTranslation(150, 0, 150);
                    mesh.Initialize(game);
                    meshes.Add(mesh);


                };
            game.UpdateEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.W))
                    {
                        wireframe = !wireframe;
                    }
                };

            game.DrawEvent +=
                delegate
                {
                    game.LineManager3D.AddAABB(meshes[1].BoundingBox, Matrix.Identity, Color.Red);

                    int i;
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.R))
                    {
                        for (i = 0; i < meshes.Count; i++)
                        {

                            meshes[i].ReloadShader(game);
                        }
                    }
                    game.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
                    game.GraphicsDevice.RenderState.CullMode = CullMode.None;

                    if (!wireframe)
                        for (i = 1; i < meshes.Count; i++)
                        {
                            meshes[i].Render(game);
                        }


                    i = 0;
                    meshes[i].Color = Color.LightGreen;
                    meshes[i].Render(game);

                    game.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
                    meshes[i].Color = Color.Red;
                    meshes[i].Render(game);

                    game.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
                    game.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

                    meshes[i].Color = Color.Black;
                    meshes[i].Render(game);

                    if (wireframe)
                        for (i = 1; i < meshes.Count; i++)
                        {
                            meshes[i].Render(game);
                        }




                };

            game.Run();

        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestRenderBoxMesh()
        {
            XNAGame game;
            BoxMesh mesh = null;

            game = new XNAGame();

            game.InitializeEvent +=
                delegate
                {
                    mesh = new BoxMesh();

                    mesh.Initialize(game);

                };

            game.DrawEvent +=
                delegate
                {
                    game.LineManager3D.AddAABB(mesh.BoundingBox, Matrix.Identity, Color.Red);
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.R))
                    {
                        mesh.ReloadShader(game);
                    }
                    mesh.Render(game);
                };


            game.Run();


        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestRenderLines()
        {
            TestXNAGame.Start("TestRenderLines",
                               delegate
                               {

                               },
                               delegate // 3d render code
                               {
                                   for (int num = 0; num < 200; num++)
                                   {
                                       TestXNAGame.Instance.LineManager3D.AddLine(
                                           new Vector3(-12.0f + num / 4.0f, 13.0f, 0),
                                           new Vector3(-17.0f + num / 4.0f, -13.0f, 0),
                                           new Color((byte)(255 - num), 14, (byte)num));
                                   } // for

                                   TestXNAGame.Instance.LineManager3D.DrawGroundShadows = true;
                                   TestXNAGame.Instance.LineManager3D.AddCenteredBox(new Vector3(4, 4, 4), 2, Color.Red);

                                   TestXNAGame.Instance.LineManager3D.WorldMatrix =
                                       Matrix.CreateTranslation(Vector3.Up*30);

                                   for (int num = 0; num < 200; num++)
                                   {
                                       TestXNAGame.Instance.LineManager3D.AddLine(
                                           new Vector3(-12.0f + num / 4.0f, 13.0f, 0),
                                           new Vector3(-17.0f + num / 4.0f, -13.0f, 0),
                                           new Color((byte)(255 - num), 14, (byte)num));
                                   } // for

                                   /*TextureFont.WriteText( 2, 30,
                    "cam pos=" + BaseGame.CameraPos );*/
                               });
        } // TestRenderLines()



        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestBasicShaderAutoReload()
        {
            BasicShader shader = null;
            FullScreenQuad quad = null;
            XNAGame game = new XNAGame();
            game.InitializeEvent += delegate
                                        {
                                            shader = BasicShader.LoadFromEmbeddedFile(game,
                                                typeof(GraphicsTest).Assembly,"MHGameWork.TheWizards.Tests.Graphics.Files.TestShader.fx",
                                                "..\\..\\Unit Tests\\Graphics\\Files\\TestShader.fx",
                                                new EffectPool());
                                            shader.SetTechnique("Technique1");
                                            quad = new FullScreenQuad(game.GraphicsDevice);
                                        };

            game.DrawEvent += delegate
                                  {
                                      shader.RenderMultipass(delegate
                                                                 {
                                                                     quad.DrawOld(game.GraphicsDevice);
                                                                 });
                                  };
            game.Run();
        }
    }
}