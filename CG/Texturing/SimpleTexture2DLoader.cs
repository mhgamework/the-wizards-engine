using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MHGameWork.TheWizards.CG.OBJParser;

namespace MHGameWork.TheWizards.CG.Texturing
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

                var bitmap = new BitmapImage(new Uri(texture.DiskFilePath));
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
