using System.Drawing;
using System.Drawing.Imaging;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Rendering.Text;
using NUnit.Framework;
using SlimDX;
using SlimDX.DXGI;

namespace MHGameWork.TheWizards.Tests.Features.Rendering
{
    [TestFixture]
    public class TextRenderingTest
    {

        /// <summary>
        /// Converts a bitmap to a Texture2D and renders the texture
        /// </summary>
        [Test]
        public void TestDrawingToD3D11Conversion()
        {
            var bmp = createBitmap();

            var game = new DX11Game();
            game.InitDirectX();

            var tex = GPUTexture.CreateCPUWritable(game, bmp.Width, bmp.Height, Format.B8G8R8A8_UNorm);


            var convert = new DrawingToD3D11Conversion();
            convert.WriteBitmapToTexture(bmp, tex);




            game.GameLoopEvent += delegate
            {
                game.TextureRenderer.Draw(tex.View, Vector2.Zero, new Vector2(100, 100));
            };
            game.Run();

        }

        /// <summary>
        /// Test of unfinished code
        /// </summary>
        [Test]
        public void TestGDIPerformance()
        {
            /*var renderer = new TextRendererDrawing();
            renderer.Init();

            var watch = new Stopwatch();

            watch.Start();
            var count = 10000;
            for (int i = 0; i < count; i++)
            {
                using (var fs = new MemoryStream())
                    renderer.RenderToStream(fs, "Hello Wizard", new SlimDX.Vector2(100, 100), new SlimDX.Color4(1, 0, 0));
            }
            watch.Stop();

            Console.WriteLine("FPS: {0}", count / watch.Elapsed.TotalSeconds);*/


        }

        [Test]
        public void TestRenderText()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var txt = new TextTexture(game, 100, 100);

            txt.DrawText("The Wizards", new Vector2(0, 0), new Color4(0.3f, 0.3f, 0.3f));

            txt.UpdateTexture();

            game.GameLoopEvent += delegate
                                  {
                                      txt.Clear();
                                      txt.DrawText("The Wizards", new Vector2(0, 0), new Color4(0.3f, 0.3f, 0.3f));

                                      txt.UpdateTexture();

                                      game.Device.ImmediateContext.OutputMerger.BlendState =
                                          game.HelperStates.AlphaBlend;

                                      game.TextureRenderer.Draw(txt.GPUTexture.View, new Vector2(0, 0),
                                                                new Vector2(100, 100));

                                  };

            game.Run();
        }


        private Bitmap createBitmap()
        {
            var bitmap = new Bitmap(2, 2, PixelFormat.Format32bppArgb);
            bitmap.SetPixel(0, 0, Color.Red);
            bitmap.SetPixel(1, 0, Color.Blue);
            bitmap.SetPixel(1, 1, Color.Green);
            return bitmap;
        }
    }
}
