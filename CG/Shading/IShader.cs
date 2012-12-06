using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;

namespace MHGameWork.TheWizards.CG.Shading
{
    public interface IShader
    {
        Color4 Shade(GeometryInput f,RayTrace trace);
    }
}
