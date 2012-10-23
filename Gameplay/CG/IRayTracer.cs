using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    public interface IRayTracer
    {
        Color4 GetPixel(Vector2 pos);
    }
}
