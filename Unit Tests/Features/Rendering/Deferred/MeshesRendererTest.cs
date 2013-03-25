using System.Collections.Generic;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Rendering.Deferred.Meshes;
using MHGameWork.TheWizards.Tests.Features.Data.OBJParser;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Deferred
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class MeshesRendererTest
    {

        [Test]
        public void TestSphere() { drawMeshTest(RenderingTestsHelper.CreateSphere(RenderingTestsHelper.GetDiffuseMap(), RenderingTestsHelper.GetNormalMap(), RenderingTestsHelper.GetSpecularMap())); }
        [Test]
        public void TestBarrel() { drawMeshTest( OBJParserTest.GetBarrelMesh( new OBJToRAMMeshConverter( new RAMTextureFactory() ) ) ); }


        private void drawMeshTest(IMesh mesh)
        {
            var game = createGame();


            var pool = createTexturePool(game);

            var worldMesh = new WorldMesh { Mesh = mesh, WorldMatrix = Matrix.Identity };

            var meshes = new MeshesRenderer(new RendererResourcePool(new MeshRenderDataFactory(game, null, pool), pool, game), game);


            var ctx = game.Device.ImmediateContext;

            var gBuffer = createGBuffer(game);
            var scene = new RenderingTestsHelper.SimpleLightedScene(game, gBuffer);


            var list = new List<WorldMesh>();
            list.Add(worldMesh);

            game.GameLoopEvent += delegate
            {
                ctx.ClearState();
                gBuffer.Clear();
                gBuffer.SetTargetsToOutputMerger();

                meshes.DrawMeshes(list, game.Camera);

                scene.Render();
            };

            game.Run();
        }

        private DX11Game createGame()
        {
            var ret = new DX11Game();
            ret.InitDirectX();
            return ret;
        }

        private static TheWizards.Rendering.Deferred.TexturePool createTexturePool(DX11Game game)
        {
            return new TheWizards.Rendering.Deferred.TexturePool(game);
        }

        private static GBuffer createGBuffer(DX11Game game1)
        {
            return new GBuffer(game1.Device, 400, 300);
        }

    }
}