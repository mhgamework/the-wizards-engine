using SlimDX;

namespace MHGameWork.TheWizards.Simulation.Spatial
{
    /// <summary>
    /// The idea is that there is a set of interfaces that generically defines objects having spatial properties.
    /// This interfaces represents something having a physical space occupation (size), defined by a bounding box
    /// Note that this is not a spatial object, so it should be considered a local bounding box
    /// </summary>
    public interface IWorldObject
    {
        BoundingBox BoundingBox { get; }
    }
}