using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.Lighting
{
    public interface IShader
    {
        Color4 Shade(TraceResult f,RayTrace trace);
    }
}
