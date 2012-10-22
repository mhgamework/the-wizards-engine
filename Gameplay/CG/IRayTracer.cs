using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerGraphics.Math;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    public interface IRayTracer
    {
        Color4 GetPixel(Point2 pos);
    }
}
