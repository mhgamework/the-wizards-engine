using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Raytracing.Pipeline
{
    public delegate Color4 ShadeDelegate(ref GeometryInput input,ref RayTrace trace);
}