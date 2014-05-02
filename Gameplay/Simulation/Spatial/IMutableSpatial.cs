using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.Spatial
{
    /// <summary>
    /// The idea is that there is a set of interfaces that generically defines objects having spatial properties.
    /// This interface represents something which has an orientation that can be changed, and has a physical size.
    /// Note: the boundingbox is Local!!
    /// </summary>
    public interface IMutableSpatial : IMutableOrientation
    {
        BoundingBox LocalBoundingBox { get; }
    }
}