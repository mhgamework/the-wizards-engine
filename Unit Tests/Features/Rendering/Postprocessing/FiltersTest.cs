using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Filters;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.Features.Rendering.DirectX11;
using NUnit.Framework;
using SlimDX;
using SlimDX.DXGI;

namespace MHGameWork.TheWizards.Tests.Features.Rendering
{
    [TestFixture, RequiresSTA]
    public class FiltersTest
    {
        private DX11Game game;

        [SetUp]
        public void Setup()
        {
            game = createGame();
            game.InitDirectX();
        }

        [Test]
        public void TestGaussionBlur()
        {

            var blur = new GaussianBlurFilter(game);

            var input = GPUTexture.FromFile(game, TWDir.GameData.FullName + "\\Core\\Checker.png");
            var buffer = GPUTexture.CreateUAV(game, input.Resource.Description.Width, input.Resource.Description.Height, Format.R8G8B8A8_UNorm);
            var output = GPUTexture.CreateUAV(game, input.Resource.Description.Width, input.Resource.Description.Height, Format.R8G8B8A8_UNorm);
            game.GameLoopEvent += delegate
                {
                    blur.Blur(input.View,buffer, output.UnorderedAccessView);

                    game.Device.ImmediateContext.ClearState();
                    game.SetBackbuffer();
                    var size = 256;
                    game.TextureRenderer.Draw(input.View, new Vector2(0, 0), new Vector2(size, size));
                    game.TextureRenderer.Draw(output.View, new Vector2(size , 0), new Vector2(size, size));
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

        private DX11Game createGame()
        {
            return new DX11Game();
        }
    }
}
