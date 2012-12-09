using System.Collections.Generic;
using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;
using MHGameWork.TheWizards.CG.Texturing;

namespace MHGameWork.TheWizards.CG.Shading
{
    public class NormalsShader : IShader
    {
        public Color4 Shade(TraceResult f, RayTrace trace)
        {
            return new Color4(f.Normal * 0.5f + Vector3.One * 0.5f);
        }
    }
}
