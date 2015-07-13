using System;
using System.Threading;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Features.Rendering
{
    /// <summary>
    /// Test rendering high performance text by using a managed wrapper for the http://fw1.codeplex.com/ font library.
    /// </summary>
    [TestFixture]
    public class DX11FontWrapperTest
    {
        [Test]
        public void TestTextRendering()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var wrapper = new DX11FontWrapper(game.Device);

            game.GameLoopEvent += delegate
                {
                    wrapper.Draw("Welcome!", 128, 10, 10, new Color4(1, 0, 0));
                    wrapper.Draw("Welcome!", 128, 10, 210, new Color4(0, 1, 0));
                    wrapper.Draw("Welcome!", 128, 10, 410, new Color4(0, 0, 1));

                    //TODO: game.MarkFrameBuffer();
                };

            game.Run();

        }

        [Test]
        public void TestFancyText()
        {
            var game = new DX11Game();
            game.InitDirectX();
            var wrapper = new MHGameWork.TheWizards.DX11FontWrapper(game.Device);


            game.GameLoopEvent += delegate
                {
                    var size = (float)Math.Sin(game.TotalRunTime) + 1;
                    size = size * 20+50;

                    var color = new Color4(
                        ((float) Math.Sin(game.TotalRunTime + 0) + 1)*0.5f,
                        ((float)Math.Sin(game.TotalRunTime + 1) + 1) * 0.5f,
                        ((float)Math.Sin(game.TotalRunTime + 2) + 1) * 0.5f);
                    //color = new Color4(1,1,0);

                    wrapper.Draw("Welcome!", size, 10, 10, color);

                };

            game.Run();

        }


    }
}