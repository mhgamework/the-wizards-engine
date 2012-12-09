namespace MHGameWork.TheWizards.CG.Raytracing
{
    /// <summary>
    /// Responsible for tracing the world and returning fragment input
    /// </summary>
    public interface ITraceableScene
    {
        void Intersect(RayTrace rayTrace, out TraceResult result);
    }
}
