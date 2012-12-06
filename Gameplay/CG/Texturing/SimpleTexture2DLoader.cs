using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    /// <summary>
    /// 
    /// </summary>
    public class SimpleTexture2DLoader : ITexture2DLoader
    {
        private Dictionary<ITexture, Texture2D> cache = new Dictionary<ITexture, Texture2D>();
        public Texture2D Load(ITexture texture)
        {
            lock (this)
            {
                if (cache.ContainsKey(texture)) return cache[texture];

                var bitmap = new BitmapImage(new Uri(texture.GetCoreData().DiskFilePath));
                int offset;
                if (bitmap.Format != PixelFormats.Bgr32)
                    throw new InvalidOperationException("Unsupported texture!");

                int bytesPerPixel = (bitmap.Format.BitsPerPixel / 8);
                int stride = bitmap.PixelWidth * bytesPerPixel;
                byte[] pixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * bytesPerPixel];
                bitmap.CopyPixels(pixels, stride, 0);

                var tex = new Texture2D(bitmap.PixelWidth, bitmap.PixelHeight, pixels);

                cache[texture] = tex;

                return tex;
            }


        }
    }
}
