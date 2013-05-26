using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MHGameWork.TheWizards.Rendering.Text
{
    /// <summary>
    /// This class is responsible for building a Texture2D from provided text
    /// This class uses a GDI bitmap for textrendering and then copies the bitmap to a Texture2D
    /// </summary>
    public class TextTexture : IDisposable
    {
        private DrawingToD3D11Conversion converter = new DrawingToD3D11Conversion();

        private GPUTexture tex;

        public TextTexture(DX11Game game, int width, int height)
        {
            bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            GPUTexture = GPUTexture.CreateCPUWritable(game, bmp.Width, bmp.Height, Format.B8G8R8A8_UNorm);


            SetFont("Verdana", 10);
            
            //Then you create a StringFormat object to specify text alignments etc.
            sf = StringFormat.GenericDefault;
            //Then create a graphics object to be able to write in the bmp image
            g = System.Drawing.Graphics.FromImage(bmp);
            g.PageUnit = GraphicsUnit.Pixel;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            

            Clear();
        }

        public GPUTexture GPUTexture
        {
            get { return tex; }
            set { tex = value; }
        }

        public void SetFont(string family, float emSize)
        {
            font = new Font(family, emSize);
        }

        private StringFormat sf;
        private Font font;


        private Bitmap bmp;
        private System.Drawing.Graphics g;

        public void Clear()
        {
            g.Clear(Color.FromArgb(0,0,0,0));
        }

        public Vector2 MeasureString(string text)
        {
            var s =  g.MeasureString(text, font);
            return new Vector2(s.Width,s.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position">Should be integer</param>
        /// <param name="color"></param>
        public void DrawText(string text, Vector2 position, Color4 color)
        {
            var c = Color.FromArgb(color.ToArgb());
            var textCoordinate = new Point((int)position.X,(int)position.Y);

            Brush brush = new SolidBrush(c);
            g.DrawString(text, font, brush, textCoordinate.X, textCoordinate.Y, sf);

        }

        /// <summary>
        /// Updates the texture with what was drawn (ughj)
        /// Note that this returs the cached texture (read-only!!)
        /// </summary>
        public void UpdateTexture()
        {
            converter.WriteBitmapToTexture(bmp, GPUTexture);

        }


        public void Dispose()
        {
            if (GPUTexture != null)
            GPUTexture.Dispose();
            GPUTexture = null;
        }
    }
}
