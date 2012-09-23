﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MHGameWork.TheWizards.Rendering.Text
{
    /// <summary>
    /// This class is responsible for building a Texture2D from provided text
    /// </summary>
    public class TextTexture
    {
        private DrawingToD3D11Conversion converter = new DrawingToD3D11Conversion();

        public TextTexture(DX11Game game, int width, int height)
        {
            bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            tex = new Texture2D(game.Device, new Texture2DDescription
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


            SetFont("Verdana", 10);
            
            //Then you create a StringFormat object to specify text alignments etc.
            sf = StringFormat.GenericDefault;
            //Then create a graphics object to be able to write in the bmp image
            g = System.Drawing.Graphics.FromImage(bmp);
            g.PageUnit = GraphicsUnit.Pixel;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            Clear();
        }

        public void SetFont(string family, float emSize)
        {
            font = new Font(family, emSize);
        }

        private StringFormat sf;
        private Font font;


        private Bitmap bmp;
        private Texture2D tex;
        private System.Drawing.Graphics g;

        public void Clear()
        {
            g.Clear(Color.FromArgb(0,0,0,0));
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
        public Texture2D UpdateTexture()
        {
            converter.WriteBitmapToTexture(bmp, tex);

            return tex;
        }



    }
}
