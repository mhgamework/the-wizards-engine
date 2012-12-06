using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Texturing
{
    /// <summary>
    /// Responsible for sampling a Texture2D
    /// Currently billinear tiling
    /// </summary>
    public class Texture2DSampler
    {
        public Color4 SampleBilinear(Texture2D tex, Vector2 pos)
        {
            pos.X = (pos.X * tex.Width) - 0.5f;
            pos.Y = (pos.Y * tex.Height) - 0.5f;

            //return SampleTiled(tex, new Point2((int)pos.X, (int)pos.Y));
            var fracX = pos.X - (int)pos.X;
            var fracY = pos.Y - (int)pos.Y;

            var a1 = SampleTiled(tex, new Point2((int)pos.X, (int)pos.Y));
            var a2 = SampleTiled(tex, new Point2((int)pos.X + 1, (int)pos.Y));
            var b1 = SampleTiled(tex, new Point2((int)pos.X, (int)pos.Y + 1));
            var b2 = SampleTiled(tex, new Point2((int)pos.X + 1, (int)pos.Y + 1));

            return Color4.Lerp(Color4.Lerp(a1, a2, fracX), Color4.Lerp(b1, b2, fracX), fracY);
        }

        public Color4 SampleTiled(Texture2D tex, Point2 pos)
        {
            //return tex.GetPixel(pos.X, pos.Y);
            return tex.GetPixel(MathHelper.Modulo(pos.X, tex.Width), MathHelper.Modulo(pos.Y, tex.Height));
        }
    }
}
