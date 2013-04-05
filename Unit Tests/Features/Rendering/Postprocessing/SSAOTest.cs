using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Rendering.Deferred.Meshes;
using MHGameWork.TheWizards.Rendering.SSAO;
using MHGameWork.TheWizards.Tests.Features.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.Features.Rendering.DirectX11;
using MHGameWork.TheWizards.Tests.Features.Rendering.XNA;
using NUnit.Framework;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Postprocessing
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

            var renderer = new DeferredMeshesRenderer(game, gBuffer, texturePool);

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
                                          GBufferTest.DrawGBuffer(game, gBuffer);
                                      else
                                          game.TextureRenderer.Draw(ssao.MSsaoBuffer.pSRV, new SlimDX.Vector2(0, 0),
                                                                    new SlimDX.Vector2(800, 600));


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
