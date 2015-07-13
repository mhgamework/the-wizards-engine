using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.Meshes;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using NUnit.Framework;
using SlimDX;
using TexturePool = MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.TexturePool;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Deferred
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class MeshesRendererTest
    {
        public static RAMMesh GetBarrelMesh(OBJToRAMMeshConverter c)
        {
            var fsMat = new FileStream(TestFiles.BarrelMtl, FileMode.Open);

            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Barrel01.mtl", fsMat);

            importer.ImportObjFile(TestFiles.BarrelObj);

            var meshes = c.CreateMeshesFromObjects(importer);

            fsMat.Close();

            return meshes[0];
        }

        [Test]
        public void TestSphere() { drawMeshTest(RenderingTestsHelper.CreateSphere(RenderingTestsHelper.GetDiffuseMap(), RenderingTestsHelper.GetNormalMap(), RenderingTestsHelper.GetSpecularMap()), Matrix.Identity); }
        [Test]
        public void TestBarrel() { drawMeshTest(GetBarrelMesh(new OBJToRAMMeshConverter(new RAMTextureFactory())), Matrix.Identity); }
        [Test]



        public void drawMeshTest(IMesh mesh, Matrix worldMatrix)
        {
            var game = createGame();


            var pool = createTexturePool(game);

            var worldMesh = new WorldMesh { Mesh = mesh, WorldMatrix = worldMatrix };

            var meshes = new MeshesRenderer(new RendererResourcePool(new MeshRenderDataFactory(game, null, pool), pool, game), game);


            var ctx = game.Device.ImmediateContext;

            var gBuffer = createGBuffer(game);
            var scene = new RenderingTestsHelper.SimpleLightedScene(game, gBuffer);


            var list = new List<WorldMesh>();
            list.Add(worldMesh);

            game.GameLoopEvent += delegate
            {
                ctx.ClearState();
                ctx.Rasterizer.State = game.HelperStates.RasterizerShowAll;
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

        private static TexturePool createTexturePool(DX11Game game)
        {
            return new TexturePool(game);
        }

        private static GBuffer createGBuffer(DX11Game game1)
        {
            return new GBuffer(game1.Device, 400, 300);
        }

    }
}