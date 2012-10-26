using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
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
            if (cache.ContainsKey(texture)) return cache[texture];

            var bitmap = new BitmapImage(new Uri(texture.GetCoreData().DiskFilePath));
            int offset;
            
            int bytesPerPixel = (bitmap.Format.BitsPerPixel / 8);
            int stride = bitmap.PixelWidth * bytesPerPixel;
            Color4[] pixels = new Color4[bitmap.PixelWidth * bitmap.PixelHeight * bytesPerPixel];
            bitmap.CopyPixels(pixels, stride, 0);


        }
    }
}
