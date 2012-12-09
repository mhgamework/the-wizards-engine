using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    public interface ISceneObject
    {
        BoundingBox BoundingBox { get; }
        /// <summary>
        /// The given result is an intermediary result passed to the trace, and when a closer hit is found the result should be adjusted
        /// </summary>
        /// <param name="trace"></param>
        /// <param name="result"></param>
        void Intersects(ref RayTrace trace, ref TraceResult result);
    }
}