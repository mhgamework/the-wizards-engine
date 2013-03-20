using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Rendering.Text;
using NUnit.Framework;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DirectWrite;
using SlimDX.DXGI;
using SpriteTextRenderer;
using Font = System.Drawing.Font;
using FontStyle = SlimDX.DirectWrite.FontStyle;

namespace MHGameWork.TheWizards.Tests.Features.Rendering
{
    [TestFixture]
    public class TextRenderingFunctionalityTest
    {

        /// <summary>
        /// WORKING TEST!! renders text!
        /// First render text on GDI bitmap, then lock the gdi, copy bytes using marshalling to a .NET array, next write the array to a texture2D using mapsubresource
        /// </summary>
        [Test]
        public void TestGDITextToTexture2D()
        {
            var bmp = new Bitmap(100, 100, PixelFormat.Format32bppArgb);



            var canvasSize = new Point(bmp.Width, bmp.Height);
            var c = Color.Red;
            var textCoordinate = new Point(0, 0);



            //Then create a graphics object to be able to write in the bmp image
            var g = System.Drawing.Graphics.FromImage(bmp);
            //g.MeasureString(text, font, canvasSize, sf);
            g.PageUnit = GraphicsUnit.Pixel;
            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            //then create a brush (with a color) and write the text in the bmp file specifying text coordinates and alignment
            Brush brush = new SolidBrush(c);
            g.DrawString("Hello Wizard", new Font("Verdana", 10), brush, textCoordinate.X, textCoordinate.Y, StringFormat.GenericDefault);

            g.Flush();



            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);


            var game = new DX11Game();
            game.InitDirectX();

            var tex = new Texture2D(game.Device, new Texture2DDescription
            {
                ArraySize = 1,
                MipLevels = 1,
                Format = Format.B8G8R8A8_UNorm,
                Usage = ResourceUsage.Dynamic,
                CpuAccessFlags = CpuAccessFlags.Write,
                Width = bmp.Width,
                Height = bmp.Height,
                SampleDescription = new SampleDescription(1, 0),
                BindFlags = BindFlags.ShaderResource
            });

            var box = game.Device.ImmediateContext.MapSubresource(tex, 0, 0, MapMode.WriteDiscard,
                                                        SlimDX.Direct3D11.MapFlags.None);

            for (int iRow = 0; iRow < bmp.Height; iRow++)
            {
                box.Data.Seek(iRow * box.RowPitch, SeekOrigin.Begin);
                box.Data.Write(rgbValues, iRow * 4 * bmp.Width, bmp.Width * 4);
            }

            game.Device.ImmediateContext.UnmapSubresource(tex, 0);

            var view = new ShaderResourceView(game.Device, tex);

            game.GameLoopEvent += delegate
            {
                game.TextureRenderer.Draw(view, Vector2.Zero, new Vector2(bmp.Width, bmp.Height));
            };
            game.Run();

        }

       

        [Test]
        public void TestRenderText()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var Sprite = new SpriteRenderer(game.Device);
            Sprite.ScreenSize = new SlimDX.Vector2(800, 600);


            var myTextBlockRenderer = new TextBlockRenderer(Sprite, "Arial", FontWeight.Bold, FontStyle.Normal, FontStretch.Normal, 12);



            game.GameLoopEvent += delegate
                                  {
                                      myTextBlockRenderer.DrawString("Hello Wizard", new SlimDX.Vector2(20, 20),
                                                                     new SlimDX.Color4(1, 1, 1));
                                      Sprite.Flush();
                                  };
        }

        /// <summary>
        /// Unfinished!
        /// </summary>
        [Test]
        public void TestRenderUsingGDI()
        {
            var game = new DX11Game();
            game.InitDirectX();

            // Create the DirectX11 texture2D.  This texture will be shared with the DirectX10
            // device.  The DirectX10 device will be used to render text onto this texture.  DirectX11
            // will then draw this texture (blended) onto the screen.
            // The KeyedMutex flag is required in order to share this resource.
            SlimDX.Direct3D11.Texture2D textureD3D11 = new Texture2D(game.Device, new Texture2DDescription
            {
                Width = 100,
                Height = 100,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.KeyedMutex
            });

            var surface = textureD3D11.AsSurface();
            var surface1 = Surface1.FromPointer(surface.ComPointer);





            game.GameLoopEvent += delegate
            {
            };
        }

        /// <summary>
        /// Not working!
        /// </summary>
        [Test]
        public void TestRendererSharedResource()
        {
            TextRendererSharedResource.Main();
        }

        [Test]
        public void TestMemoryBitmap()
        {

            var bmp = createBitmap();

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);





        }

        [Test]
        public void TestCreateTexture2D()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var tex = new Texture2D(game.Device, new Texture2DDescription
                                                     {
                                                         ArraySize = 1,
                                                         MipLevels = 1,
                                                         Format = Format.B8G8R8A8_UNorm,
                                                         Usage = ResourceUsage.Dynamic,
                                                         CpuAccessFlags = CpuAccessFlags.Write,
                                                         Width = 2,
                                                         Height = 1,
                                                         SampleDescription = new SampleDescription(1, 0),
                                                         BindFlags = BindFlags.ShaderResource
                                                     });

            var box = game.Device.ImmediateContext.MapSubresource(tex, 0, 0, MapMode.WriteDiscard,
                                                        SlimDX.Direct3D11.MapFlags.None);

            box.Data.Write(new byte[] { 0, 0, 255, 255 }, 0, 4);

            game.Device.ImmediateContext.UnmapSubresource(tex, 0);

            var view = new ShaderResourceView(game.Device, tex);

            game.GameLoopEvent += delegate
                                  {
                                      game.TextureRenderer.Draw(view, Vector2.Zero, new Vector2(100, 100));
                                  };
            game.Run();
        }

        [Test]
        public void TestCreateTexture2DFromBitmap()
        {
            Bitmap bitmap = createBitmap();

            var bmp = bitmap;

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);


            var game = new DX11Game();
            game.InitDirectX();

            var tex = new Texture2D(game.Device, new Texture2DDescription
            {
                ArraySize = 1,
                MipLevels = 1,
                Format = Format.R8G8B8A8_UNorm,
                Usage = ResourceUsage.Dynamic,
                CpuAccessFlags = CpuAccessFlags.Write,
                Width = bmp.Width,
                Height = bmp.Height,
                SampleDescription = new SampleDescription(1, 0),
                BindFlags = BindFlags.ShaderResource
            });

            var box = game.Device.ImmediateContext.MapSubresource(tex, 0, 0, MapMode.WriteDiscard,
                                                        SlimDX.Direct3D11.MapFlags.None);
            for (int iRow = 0; iRow < bmp.Height; iRow++)
            {
                box.Data.Seek(iRow*box.RowPitch, SeekOrigin.Begin);
                box.Data.Write(rgbValues, iRow*4*bmp.Width, bmp.Width*4);
            }

            game.Device.ImmediateContext.UnmapSubresource(tex, 0);

            var view = new ShaderResourceView(game.Device, tex);

            game.GameLoopEvent += delegate
            {
                game.TextureRenderer.Draw(view, Vector2.Zero, new Vector2(100, 100));
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
