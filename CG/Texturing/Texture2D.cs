using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    /// <summary>
    /// Memory structure for a 2D texture
    /// </summary>
    public class Texture2D
    {
        private readonly byte[] pixels;

        /// <summary>
        /// Default in BGRA32 format
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pixels"></param>
        public Texture2D(int width, int height, byte[] pixels)
        {
            this.Width = width;
            this.Height = height;
            this.pixels = pixels;
        }

        public int Height { get; private set; }

        public int Width { get; private set; }

        public Color4 GetPixel(int x, int y)
        {
            var b = (y * Width + x) * 4;
            return new Color4(pixels[b + 3] + 255f, pixels[b + 2] / 255f, pixels[b + 1] / 255f, pixels[b + 0] / 255f);
        }
        public void SetPixel(int x, int y, Color4 color)
        {
            var b = x * Width + y;
            pixels[b + 0] = (byte)(color.Blue * 255);
            pixels[b + 1] = (byte)(color.Green * 255);
            pixels[b + 2] = (byte)(color.Red * 255);
            pixels[b + 3] = (byte)(color.Alpha * 255);
        }
    }
}
