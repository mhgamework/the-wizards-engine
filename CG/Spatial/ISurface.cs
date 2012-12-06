using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Spatial
{
    public interface ISurface
    {
        BoundingBox GetBoundingBox(IBoundingBoxCalculator calc);
    }
}