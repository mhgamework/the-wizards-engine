using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.Spatial
{
    /// <summary>
    /// The idea is that there is a set of interfaces that generically defines objects having spatial properties.
    /// This interfaces represents something having a position and rotation which is mutable
    /// </summary>
    public interface IMutableOrientation : IMutablePosition
    {
        Quaternion Rotation { get; set; }
    }
}