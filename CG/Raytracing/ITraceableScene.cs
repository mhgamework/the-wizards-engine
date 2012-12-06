namespace MHGameWork.TheWizards.CG.Raytracing
{
    /// <summary>
    /// Responsible for tracing the world and returning fragment input
    /// </summary>
    public interface ITraceableScene
    {
        bool Intersect(RayTrace rayTrace, out IShadeCommand command, bool generateShadeCommand);
    }
}
