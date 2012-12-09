using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Raytracing.Pipeline
{
    public delegate Color4 ShadeDelegate(ref TraceResult input,ref RayTrace trace);
}