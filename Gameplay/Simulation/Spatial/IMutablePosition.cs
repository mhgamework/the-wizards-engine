using SlimDX;


namespace MHGameWork.TheWizards.SkyMerchant._Engine.Spatial
{
    /// <summary>
    /// The idea is that there is a set of interfaces that generically defines objects having spatial properties.
    /// This interfaces represents something having a position which is mutable
    /// </summary>
    public interface IMutablePosition
    {
        Vector3 Position { get; set; }
    }
}