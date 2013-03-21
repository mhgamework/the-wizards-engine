using System;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Rendering.Deferred.Meshes;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Deferred
{
    /// <summary>
    /// Tests the DeferredMesh.fx shader.
    /// </summary>
    [TestFixture]
    public class DeferredMaterialTest
    {
        [Test]
        public void TestNoMaps() { testGBufferSphere(null, null, null); }
        [Test]
        public void TestDiffuseMap() { testGBufferSphere(RenderingTestsHelper.GetDiffuseMap(), null, null); }
        [Test]
        public void TestNormalMap() { testGBufferSphere(null, RenderingTestsHelper.GetNormalMap(), null); }
        [Test]
        public void TestSpecularMap() { testGBufferSphere(null, null, RenderingTestsHelper.GetSpecularMap()); }



        /// <summary>
        /// Renders a test where the GBuffer is displayed containing a sphere with provided maps.
        /// </summary>
        private void testGBufferSphere(ITexture diffuse, ITexture normal, ITexture specular)
        {
            var game = createGame();

            var pool = createTexturePool(game);
            var txDiffuse = diffuse == null ? null : pool.LoadTexture(diffuse);
            var txNormal = normal == null ? null : pool.LoadTexture(normal);
            var txSpecular = specular == null ? null : pool.LoadTexture(specular);

            var mat = new DeferredMaterial(game, txDiffuse, txNormal, txSpecular);

            var part = RenderingTestsHelper.CreateSphereMeshPart();
            var fact = new MeshRenderDataFactory(game, null, null);

            var partData = fact.CreateMeshPartData(part);

            var buffer = createGBuffer(game);

            var perObject = mat.CreatePerObjectCB();


            var ctx = game.Device.ImmediateContext;

            perObject.UpdatePerObjectBuffer(ctx, Matrix.Identity);


            game.GameLoopEvent += delegate
                {
                    buffer.Clear();
                    buffer.SetTargetsToOutputMerger();

                    mat.SetCamera(game.Camera.View, game.Camera.Projection);

                    mat.SetToContext(ctx);
                    mat.SetPerObjectBuffer(ctx, perObject);

                    partData.Draw(ctx);

                    game.SetBackbuffer();

                    GBufferTest.DrawGBuffer(game, buffer);
                };

            game.Run();
        }

        private static TheWizards.Rendering.Deferred.TexturePool createTexturePool(DX11Game game)
        {
            return new TheWizards.Rendering.Deferred.TexturePool(game);
        }

        private static GBuffer createGBuffer(DX11Game game1)
        {
            return new GBuffer(game1.Device, 300, 300);
        }

        private DX11Game createGame()
        {
            var ret = new DX11Game();
            ret.InitDirectX();


            return ret;
        }

    }
}