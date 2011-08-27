using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DirectX11;
using DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Rendering.SSAO;
using MHGameWork.TheWizards.Tests.DirectX11;
using NUnit.Framework;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Tests.Rendering
{
    [TestFixture]
    public class SSAOTest
    {
        [Test]
        public void TestHorizonSSAO()
        {
            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());

            var mesh = RenderingTest.CreateMerchantsHouseMesh(c);

            var game = new DX11Game();
            game.InitDirectX();
            var context = game.Device.ImmediateContext;


            var texturePool = new TheWizards.Rendering.Deferred.TexturePool(game);

            var gBuffer = new GBuffer(game.Device, 800, 600);

            var renderer = new DeferredMeshRenderer(game, gBuffer, texturePool);

            var ssao = new HorizonSSAORenderer(game, 800, 600);

            var el = renderer.AddMesh(mesh);
            el.WorldMatrix = SlimDX.Matrix.Translation(MathHelper.Right * 0 * 2 + SlimDX.Vector3.UnitZ * 0 * 2);


            game.GameLoopEvent += delegate
                                  {
                                      gBuffer.Clear();
                                      gBuffer.SetTargetsToOutputMerger();

                                      renderer.Draw();

                                      ssao.OnFrameRender(gBuffer.DepthRV, gBuffer.NormalRV);


                                      context.ClearState();
                                      game.SetBackbuffer();


                                      if (game.Keyboard.IsKeyDown(Key.I))
                                          DeferredTest.DrawGBuffer(game, gBuffer);
                                      else
                                          game.TextureRenderer.Draw(ssao.MSsaoBuffer.pSRV, new SlimDX.Vector2(0, 0),
                                                                    new SlimDX.Vector2(800, 600));


                                  };

            game.Run();

        }

        [Test]
        public void TestBilateralBlur()
        {
            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());


            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Town001.mtl", new FileStream("../../bin/GameData/Core/Town/OBJ03/Town001.mtl", FileMode.Open));
            importer.ImportObjFile("../../bin/GameData/Core/Town/OBJ03/Town001.obj");

            var mesh = c.CreateMesh(importer);

            var game = new DX11Game();
            game.InitDirectX();
            var context = game.Device.ImmediateContext;


            var texturePool = new TheWizards.Rendering.Deferred.TexturePool(game);

            var gBuffer = new GBuffer(game.Device, 800, 600);

            var renderer = new DeferredMeshRenderer(game, gBuffer, texturePool);



            var el = renderer.AddMesh(mesh);
            el.WorldMatrix = SlimDX.Matrix.Translation(MathHelper.Right * 0 * 2 + SlimDX.Vector3.UnitZ * 0 * 2);


            game.GameLoopEvent += delegate
            {
                gBuffer.Clear();
                gBuffer.SetTargetsToOutputMerger();

                renderer.Draw();

                context.ClearState();
                game.SetBackbuffer();

                DeferredTest.DrawGBuffer(game, gBuffer);

            };

            game.Run();

        }

        [Test]
        public void TestCombineFinalSSAO()
        {
            //TODO: add a way to show the specular in the alpha channel

            var game = new DX11Game();
            game.InitDirectX();

            var test = new DeferredTest.TestCombineFinalClass(game);

            var ssao = new HorizonSSAORenderer(game, 800, 600);


            game.GameLoopEvent += delegate
            {
                test.DrawUpdatedDeferredRendering();

                ssao.OnFrameRender(test.FilledGBuffer.GBuffer.DepthRV, test.FilledGBuffer.GBuffer.NormalRV);


                game.Device.ImmediateContext.ClearState();
                game.SetBackbuffer();

                test.DrawCombined(ssao.MSsaoBuffer.pSRV);

                //game.TextureRenderer.Draw(ssao.MSsaoBuffer.pSRV, new SlimDX.Vector2(0, 0),
                //                                                   new SlimDX.Vector2(800, 600));
            };

            game.Run();
        }
    }
}
