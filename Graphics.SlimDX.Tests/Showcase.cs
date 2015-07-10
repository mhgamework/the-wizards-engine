using System.IO;
using System.Threading;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.Meshes;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Features.Rendering.Deferred;
using NUnit.Framework;
using MathHelper = DirectX11.MathHelper;

namespace Graphics.SlimDX.Tests
{
    [TestFixture]
    public class Showcase
    {


        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestDeferredMeshRendererRenderCity()
        {
            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());


            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Town001.mtl", new FileStream("../../bin/GameData/Core/Town/OBJ03/Town001.mtl", FileMode.Open));
            importer.ImportObjFile("../../bin/GameData/Core/Town/OBJ03/Town001.obj");

            var mesh = c.CreateMesh(importer);

            var game = new DX11Game();
            game.InitDirectX();
            var context = game.Device.ImmediateContext;


            var texturePool = new MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.TexturePool(game);

            var gBuffer = new GBuffer(game.Device, 800, 600);

            var renderer = new DeferredMeshesRenderer(game, gBuffer, texturePool);



            var el = renderer.AddMesh(mesh);
            el.WorldMatrix = global::SlimDX.Matrix.Translation(MathHelper.Right * 0 * 2 + global::SlimDX.Vector3.UnitZ * 0 * 2);


            game.GameLoopEvent += delegate
                                  {





                                      gBuffer.Clear();
                                      gBuffer.SetTargetsToOutputMerger();

                                      renderer.Draw();

                                      context.ClearState();
                                      game.SetBackbuffer();

                                      GBufferTest.DrawGBuffer(game, gBuffer);

                                  };
            global::SlimDX.Configuration.EnableObjectTracking = false;

            game.Run();

        }

    }
}
