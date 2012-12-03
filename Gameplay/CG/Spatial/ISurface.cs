using SlimDX;

namespace MHGameWork.TheWizards.CG.Spatial
{
    public interface ISurface
    {
        float? Intersects(RayTrace trace);
        BoundingBox GetBoundingBox(IBoundingBoxCalculator calc);
    }
}