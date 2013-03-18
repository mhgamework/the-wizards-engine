using System;
using System.Collections.Generic;
using System.IO;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;

using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.Features.Rendering.DirectX11;
using MHGameWork.TheWizards.Tests.Features.Rendering.XNA;
using NUnit.Framework;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;
using Buffer = SlimDX.Direct3D11.Buffer;
using DataStream = SlimDX.DataStream;
using SpectaterCamera = MHGameWork.TheWizards.DirectX11.Graphics.SpectaterCamera;

namespace MHGameWork.TheWizards.Tests.Features.Rendering
{
    [TestFixture]
    public class DeferredRenderingTest
    {

        public static RAMMesh CreateMerchantsHouseMesh(OBJToRAMMeshConverter c)
        {
            return RenderingTest.CreateMerchantsHouseMesh(c);
        }
        public static RAMMesh CreateGuildHouseMesh(OBJToRAMMeshConverter c)
        {
            return RenderingTest.CreateGuildHouseMesh(c);
        }

        public static RAMMesh CreateMeshFromObj(OBJToRAMMeshConverter c, string obj, string mtl)
        {
            return RenderingTest.CreateMeshFromObj(c, obj, mtl);
        }




        [Test]
        public void TestLoadTexture()
        {
            DX11Game game = new DX11Game();
            var pool = new TheWizards.Rendering.Deferred.TexturePool(game);

            RAMTexture tex = GetTestTexture();

            game.GameLoopEvent += delegate
            {
                //We should do this ACTUALLY in real usage situations, but it proves we cache the data.
                int row = 0;
                int col = 0;
                int width = 10;
                for (int i = 0; i < 100; i++)
                {
                    row = i / width;
                    col = i % width;
                    game.TextureRenderer.Draw(pool.LoadTexture(tex), new Vector2(10 + col * 40, 10 + row * 40), new Vector2(40, 40));
                }


            };

            game.Run();
        }

        public static RAMTexture GetTestTexture()
        {
            var tex = new RAMTexture();

            var data = tex.GetCoreData();
            data.StorageType = TextureCoreData.TextureStorageType.Disk;
            data.DiskFilePath = TestFiles.BrickRoundJPG;
            /*data.StorageType = TextureCoreData.TextureStorageType.Assembly;
            data.Assembly = Assembly.GetExecutingAssembly();
            data.AssemblyResourceName = "MHGameWork.TheWizards.Tests.OBJParser.Files.maps.BrickRound0030_7_S.jpg";*/
            return tex;
        }
        [Test]
        public void TestRenderDefaultModelShader()
        {

            var shaders = new List<BasicShader>();


            DX11Game game = new DX11Game();
            game.InitDirectX();

            var context = game.Device.ImmediateContext;

            var shader = BasicShader.LoadAutoreload(game, DeferredMeshRenderer.DeferredMeshFX);
            shader.SetTechnique("Technique1");

            var gBuffer = new GBuffer(game.Device, 800, 600);


            var layout = new InputLayout(game.Device, shader.GetCurrentPass(0).Description.Signature, DeferredMeshVertex.Elements);
            DeferredMeshVertex[] vertices = new DeferredMeshVertex[4];


            vertices[1] = new DeferredMeshVertex(Vector3.Zero.ToVector4(), Vector2.Zero, MathHelper.Up);
            vertices[0] = new DeferredMeshVertex(MathHelper.Forward.ToVector4(), Vector2.UnitX, MathHelper.Up);
            vertices[2] = new DeferredMeshVertex((MathHelper.Forward + MathHelper.Right).ToVector4(), Vector2.UnitX + Vector2.UnitY, MathHelper.Up);
            vertices[3] = new DeferredMeshVertex(MathHelper.Right.ToVector4(), Vector2.UnitY, MathHelper.Up);

            Buffer vb;
            using (var strm = new DataStream(vertices, true, false))
            {

                vb = new Buffer(game.Device, strm, (int)strm.Length, ResourceUsage.Immutable, BindFlags.VertexBuffer,
                                CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            }



            Texture2D tex;
            using (var strm = File.OpenRead(TestFiles.WoodPlanksBareJPG))
                tex = Texture2D.FromStream(game.Device, strm, (int)strm.Length);

            var texRV = new ShaderResourceView(game.Device, tex);

            BasicShader s;
            //s = shader.Clone();
            //s.DiffuseColor = new Vector4(1, 0, 0, 0);
            //s.Technique = DefaultModelShader.TechniqueType.Colored;
            //shaders.Add(s);

            s = shader.Clone();
            s.Effect.GetVariableByName("txDiffuse").AsResource().SetResource(texRV);
            //s.DiffuseTexture = tex;
            //s.Technique = DefaultModelShader.TechniqueType.Textured;
            shaders.Add(s);



            game.GameLoopEvent += delegate
                                  {
                                      gBuffer.Clear();
                                      gBuffer.SetTargetsToOutputMerger();

                                      context.Rasterizer.State = game.HelperStates.RasterizerShowAll;

                                      context.InputAssembler.InputLayout = layout;
                                      context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;

                                      // Use shared variables
                                      shader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
                                      shader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);

                                      shader.Apply();
                                      for (int i = 0; i < shaders.Count; i++)
                                      {
                                          //shaders[i].ViewProjection = game.Camera.ViewProjection;
                                          shaders[i].Effect.GetVariableByName("World").AsMatrix().SetMatrix(Matrix.Translation(MathHelper.Right * i * 3) * Matrix.Scaling(MathHelper.One * 10));
                                          context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vb, DeferredMeshVertex.SizeInBytes, 0));

                                          shaders[i].Apply();
                                          context.Draw(4, 0);

                                      }

                                      context.ClearState();
                                      game.SetBackbuffer();

                                      DeferredTest.DrawGBuffer(game, gBuffer);
                                  };

            game.Run();
        }

        [Test]
        public void TestMeshRendererSimple()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var context = game.Device.ImmediateContext;

            var mesh = CreateSimpleTestMesh();

            var texturePool = new TheWizards.Rendering.Deferred.TexturePool(game);

            var gBuffer = new GBuffer(game.Device, 800, 600);

            var renderer = new DeferredMeshRenderer(game, gBuffer, texturePool);


            DeferredMeshRenderElement middle = null;

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {

                    var el = renderer.AddMesh(mesh);
                    el.WorldMatrix = Matrix.Translation(MathHelper.Right * i * 2 + Vector3.UnitZ * j * 2);

                    if (i > 20 && i < 30 && j > 20 && j < 30)
                        el.Delete();
                }

            }

            game.GameLoopEvent += delegate
            {
                gBuffer.Clear();
                gBuffer.SetTargetsToOutputMerger();

                context.Rasterizer.State = game.HelperStates.RasterizerShowAll;

                renderer.Draw();

                context.ClearState();
                game.SetBackbuffer();

                DeferredTest.DrawGBuffer(game, gBuffer);
            };


            game.Run();

        }

        [Test]
        public void TestMeshRendererColored()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var context = game.Device.ImmediateContext;

            var mesh = CreateSimpleTestMesh();

            var texturePool = new TheWizards.Rendering.Deferred.TexturePool(game);

            var gBuffer = new GBuffer(game.Device, 800, 600);

            var renderer = new DeferredMeshRenderer(game, gBuffer, texturePool);

            var seeder = new Seeder(0);

            DeferredMeshRenderElement middle = null;

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    var builder = new MeshBuilder();
                    builder.AddBox(new Vector3(), new Vector3(1, 1, 1));
                    var copy = builder.CreateMesh();
                    copy.GetCoreData().Parts[0].MeshMaterial.ColoredMaterial = true;
                    copy.GetCoreData().Parts[0].MeshMaterial.DiffuseColor = seeder.NextColor();
                    var el = renderer.AddMesh(copy);
                    el.WorldMatrix = Matrix.Translation(MathHelper.Right * i * 2 + Vector3.UnitZ * j * 2);

                    if (i > 20 && i < 30 && j > 20 && j < 30)
                        el.Delete();
                }

            }

            game.GameLoopEvent += delegate
            {
                gBuffer.Clear();
                gBuffer.SetTargetsToOutputMerger();

                context.Rasterizer.State = game.HelperStates.RasterizerShowAll;

                renderer.Draw();

                context.ClearState();
                game.SetBackbuffer();

                DeferredTest.DrawGBuffer(game, gBuffer);
            };


            game.Run();

        }


        [Test]
        public void TestMeshRendererAdvanced()
        {
            var texFactory = new RAMTextureFactory();
            var c = new OBJToRAMMeshConverter(texFactory);

            var game = new DX11Game();
            game.InitDirectX();
            var context = game.Device.ImmediateContext;

            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Crate01.mtl", File.OpenRead(TestFiles.CrateMtl));
            importer.ImportObjFile(TestFiles.CrateObj);

            var mesh = c.CreateMesh(importer);

            RAMMesh mesh2 = CreateMerchantsHouseMesh(c);


            RAMMesh mesh3 = CreateGuildHouseMesh(c);

            var gBuffer = new GBuffer(game.Device, 800, 600);

            DeferredMeshRenderer renderer = InitDefaultMeshRenderer(game, gBuffer);


            var el = renderer.AddMesh(mesh);
            el.WorldMatrix = Matrix.Translation(MathHelper.Right * 0 * 2 + Vector3.UnitZ * 0 * 2);

            el = renderer.AddMesh(mesh2);
            el.WorldMatrix = Matrix.Translation(new Vector3(0, 0, 80));

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    el = renderer.AddMesh(mesh3);
                    el.WorldMatrix = Matrix.Translation(new Vector3(j * 30, 0, 70 + i * 30));
                }
            }



            game.GameLoopEvent += delegate
            {
                gBuffer.Clear();
                gBuffer.SetTargetsToOutputMerger();

                context.Rasterizer.State = game.HelperStates.RasterizerShowAll;

                renderer.Draw();

                context.ClearState();
                game.SetBackbuffer();

                DeferredTest.DrawGBuffer(game, gBuffer);
            };
            game.Run();

        }


        [Test]
        public void TestDeferredRendererLineElement()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var renderer = new DeferredRenderer(game);

            var el = renderer.CreateLinesElement();

            el.Lines.AddBox(new BoundingBox(Vector3.Zero, MathHelper.One), new Color4(1, 0, 0));


            game.GameLoopEvent += delegate
            {
                renderer.Draw();

            };

            game.Run();
        }


        [Test]
        public void TestDeferredRendererSimple()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var renderer = new DeferredRenderer(game);

            var otherCam = new SpectaterCamera(game.Keyboard, game.Mouse, 1, 10000);

            var mesh = CreateMerchantsHouseMesh(new OBJToRAMMeshConverter(new RAMTextureFactory()));

            var el = renderer.CreateMeshElement(mesh);
            var directional = renderer.CreateDirectionalLight();
            directional.ShadowsEnabled = true;
            var point = renderer.CreatePointLight();
            point.LightRadius *= 2;
            point.ShadowsEnabled = true;
            var spot = renderer.CreateSpotLight();
            spot.LightRadius *= 2;
            spot.ShadowsEnabled = true;

            int state = 0;

            var camState = false;

            game.GameLoopEvent += delegate
                                      {
                                          if (game.Keyboard.IsKeyPressed(Key.D1))
                                              state = 0;
                                          if (game.Keyboard.IsKeyPressed(Key.D2))
                                              state = 1;
                                          if (game.Keyboard.IsKeyPressed(Key.D3))
                                              state = 2;
                                          if (game.Keyboard.IsKeyPressed(Key.D4))
                                              state = 3;

                                          switch (state)
                                          {
                                              case 0:
                                                  break;
                                              case 1:
                                                  directional.LightDirection = game.SpectaterCamera.CameraDirection;
                                                  break;
                                              case 2:
                                                  point.LightPosition = game.SpectaterCamera.CameraPosition;

                                                  break;
                                              case 3:
                                                  spot.LightPosition = game.SpectaterCamera.CameraPosition;
                                                  spot.SpotDirection = game.SpectaterCamera.CameraDirection;
                                                  break;
                                          }

                                          if (game.Keyboard.IsKeyPressed(Key.C))
                                              camState = !camState;

                                          if (camState)
                                          {
                                              game.Camera = game.SpectaterCamera;
                                              renderer.DEBUG_SeperateCullCamera = null;
                                          }
                                          else
                                          {
                                              game.Camera = otherCam;
                                              renderer.DEBUG_SeperateCullCamera = game.SpectaterCamera;


                                          }
                                          game.SpectaterCamera.EnableUserInput = camState;
                                          otherCam.EnableUserInput = !camState;
                                          otherCam.Update(game.Elapsed);
                                          renderer.Draw();

                                      };

            game.Run();
        }

        [Test]
        public void TestDeferredRendererCastsShadowsField()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var renderer = new DeferredRenderer(game);

            var otherCam = new SpectaterCamera(game.Keyboard, game.Mouse, 1, 10000);

            var mesh = CreateMerchantsHouseMesh(new OBJToRAMMeshConverter(new RAMTextureFactory()));


            var el = renderer.CreateMeshElement(mesh);
            el.CastsShadows = false;
            var directional = renderer.CreateDirectionalLight();
            directional.ShadowsEnabled = true;
            var point = renderer.CreatePointLight();
            point.LightRadius *= 2;
            point.ShadowsEnabled = true;
            var spot = renderer.CreateSpotLight();
            spot.LightRadius *= 2;
            spot.ShadowsEnabled = true;

            int state = 0;

            var camState = false;

            game.GameLoopEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Key.D1))
                    state = 0;
                if (game.Keyboard.IsKeyPressed(Key.D2))
                    state = 1;
                if (game.Keyboard.IsKeyPressed(Key.D3))
                    state = 2;
                if (game.Keyboard.IsKeyPressed(Key.D4))
                    state = 3;

                switch (state)
                {
                    case 0:
                        break;
                    case 1:
                        directional.LightDirection = game.SpectaterCamera.CameraDirection;
                        break;
                    case 2:
                        point.LightPosition = game.SpectaterCamera.CameraPosition;

                        break;
                    case 3:
                        spot.LightPosition = game.SpectaterCamera.CameraPosition;
                        spot.SpotDirection = game.SpectaterCamera.CameraDirection;
                        break;
                }

                if (game.Keyboard.IsKeyPressed(Key.C))
                    camState = !camState;

                if (camState)
                {
                    game.Camera = game.SpectaterCamera;
                    renderer.DEBUG_SeperateCullCamera = null;
                }
                else
                {
                    game.Camera = otherCam;
                    renderer.DEBUG_SeperateCullCamera = game.SpectaterCamera;


                }
                game.SpectaterCamera.EnableUserInput = camState;
                otherCam.EnableUserInput = !camState;
                otherCam.Update(game.Elapsed);
                renderer.Draw();

            };

            game.Run();
        }



        [Test]
        public void TestMeshRendererSimpleCulling()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var context = game.Device.ImmediateContext;

            var mesh = CreateSimpleTestMesh();

            var texturePool = new TheWizards.Rendering.Deferred.TexturePool(game);

            var gBuffer = new GBuffer(game.Device, 800, 600);

            var renderer = new DeferredMeshRenderer(game, gBuffer, texturePool);


            DeferredMeshRenderElement middle = null;









            Vector3 radius = new Vector3(100, 1000, 100);
            FrustumCullerSimple culler = new FrustumCullerSimple(new BoundingBox(-radius, radius), 5);

            //QuadTreeVisualizer visualizer = new QuadTreeVisualizer();


            SpectaterCamera cullCam = new SpectaterCamera(game.Keyboard, game.Mouse, 10f, 80);

            cullCam.Positie = new Vector3(8, 10, 8);
            cullCam.EnableUserInput = false;

            SpectaterCamera renderCam = game.SpectaterCamera;

            culler.CullCamera = cullCam;

            throw new NotImplementedException();
            //renderer.Culler = culler;

            bool rotate = true;
            int selectedNode = -1;



            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {

                    var el = renderer.AddMesh(mesh);
                    el.WorldMatrix = Matrix.Translation(MathHelper.Right * i * 2 + Vector3.UnitZ * j * 2);

                    if (i > 20 && i < 30 && j > 20 && j < 30)
                        el.Delete();
                }

            }
            game.GameLoopEvent += delegate
            {
                culler.UpdateVisibility();

                if (rotate)
                    cullCam.AngleHorizontal += game.Elapsed * MathHelper.Pi * (1 / 8f);
                cullCam.Update(game.Elapsed);
                if (game.Keyboard.IsKeyPressed(Key.NumberPadPlus))
                    selectedNode++;
                if (game.Keyboard.IsKeyPressed(Key.NumberPadMinus))
                    selectedNode--;

                if (game.Keyboard.IsKeyPressed(Key.Return))
                {
                    int count = -1;
                    //visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                    //delegate(FrustumCuller.CullNode node, out Color col)
                    //{
                    //    col = Color.Red;
                    //    count++;
                    //    if (count == selectedNode)
                    //    {
                    //        node.Tag = "SELECTED!";
                    //    }
                    //    return count == selectedNode;
                    //});
                }

                if (game.Keyboard.IsKeyPressed(Key.NumberPad0))
                    rotate = !rotate;


                game.LineManager3D.AddViewFrustum(new BoundingFrustum(cullCam.ViewProjection), new Color4());
                //for (int i = 0; i < cullObjects.Count; i++)
                //{
                //    game.LineManager3D.AddBox(cullObjects[i].BoundingBox, Color.Red);
                //}
                //visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                //    delegate(FrustumCuller.CullNode node, out Color col)
                //    {
                //        if (node.Visible)
                //        {
                //            col = Color.Orange;
                //        }
                //        else
                //        {
                //            col = Color.Green;

                //        }

                //        return true;
                //    });








                gBuffer.Clear();
                gBuffer.SetTargetsToOutputMerger();

                context.Rasterizer.State = game.HelperStates.RasterizerShowAll;

                renderer.Draw();

                context.ClearState();
                game.SetBackbuffer();

                DeferredTest.DrawGBuffer(game, gBuffer);
            };

            game.Run();

        }



        public static DeferredMeshRenderer InitDefaultMeshRenderer(DX11Game game, GBuffer gBuffer)
        {
            var texturePool = new TheWizards.Rendering.Deferred.TexturePool(game);

            var renderer = new DeferredMeshRenderer(game, gBuffer, texturePool);

            return renderer;
        }

        public static IMesh CreateSimpleTestMesh()
        {
            IMesh mesh;

            mesh = new RAMMesh();

            var part = new MeshCoreData.Part();
            part.ObjectMatrix = Microsoft.Xna.Framework.Matrix.Identity;
            part.MeshPart = new RAMMeshPart();
            ((RAMMeshPart)part.MeshPart).SetGeometryData(MeshPartGeometryData.CreateTestSquare());

            var mat = new MeshCoreData.Material();

            mat.DiffuseMap = GetTestTexture();

            part.MeshMaterial = mat;
            mesh.GetCoreData().Parts.Add(part);

            return mesh;
        }
    }
}
