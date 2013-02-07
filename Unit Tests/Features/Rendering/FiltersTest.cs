using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Filters;
using MHGameWork.TheWizards.DirectX11.Graphics;
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

        private DX11Game createGame()
        {
            return new DX11Game();
        }
    }
}
