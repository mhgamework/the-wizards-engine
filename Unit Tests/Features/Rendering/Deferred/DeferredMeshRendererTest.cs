using System;
using System.IO;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.Features.Rendering.DirectX11;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Deferred
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class DeferredMeshRendererTest
    {
        [Test]
        public void TestMeshRendererSimpleCulling()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var context = game.Device.ImmediateContext;

            var mesh = RenderingTestsHelper.CreateSimpleTestMesh();

            var texturePool = new TheWizards.Rendering.Deferred.TexturePool(game);

            var gBuffer = new GBuffer(game.Device, 800, 600);

            var renderer = new DeferredMeshesRenderer(game, gBuffer, texturePool);


            DeferredRendererMeshes middle = null;









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

                GBufferTest.DrawGBuffer(game, gBuffer);
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

            RAMMesh mesh2 = RenderingTestsHelper.CreateMerchantsHouseMesh(c);


            RAMMesh mesh3 = RenderingTestsHelper.CreateGuildHouseMesh(c);

            var gBuffer = new GBuffer(game.Device, 800, 600);

            DeferredMeshesRenderer renderer = InitDefaultMeshRenderer(game, gBuffer);


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

                GBufferTest.DrawGBuffer(game, gBuffer);
            };
            game.Run();

        }





        [Test]
        public void TestMeshRendererSimple()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var context = game.Device.ImmediateContext;

            var mesh = RenderingTestsHelper.CreateSimpleTestMesh();

            var texturePool = new TheWizards.Rendering.Deferred.TexturePool(game);

            var gBuffer = new GBuffer(game.Device, 800, 600);

            var renderer = new DeferredMeshesRenderer(game, gBuffer, texturePool);


            DeferredRendererMeshes middle = null;

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

                GBufferTest.DrawGBuffer(game, gBuffer);
            };


            game.Run();

        }








        public static DeferredMeshesRenderer InitDefaultMeshRenderer(DX11Game game, GBuffer gBuffer)
        {
            var texturePool = new TheWizards.Rendering.Deferred.TexturePool(game);

            var renderer = new DeferredMeshesRenderer(game, gBuffer, texturePool);

            return renderer;
        }

    }
}