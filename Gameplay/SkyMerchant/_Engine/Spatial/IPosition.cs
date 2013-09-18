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
    /// <summary>
    /// The idea is that there is a set of interfaces that generically defines objects having spatial properties.
    /// This interfaces represents something having a position and rotation which is mutable
    /// </summary>
    public interface IMutableOrientation : IMutablePosition
    {
        Quaternion Rotation { get; set; }
    }
    /// <summary>
    /// The idea is that there is a set of interfaces that generically defines objects having spatial properties.
    /// This interfaces represents something having a physical space occupation (size), defined by a bounding box
    /// Note that this is not a spatial object, so it should be considered a local bounding box
    /// </summary>
    public interface IBoundingBox
    {
        BoundingBox LocalBoundingBox { get; }
    }

    /// <summary>
    /// The idea is that there is a set of interfaces that generically defines objects having spatial properties.
    /// This interface represents something which has an orientation that can be changed, and has a physical size.
    /// Note: the boundingbox is Local!!
    /// </summary>
    public interface IMutableSpatial : IMutableOrientation, IBoundingBox
    {

    }
    public static class SpatialExtensions
    {
        public static Matrix CalculateWorld(this IMutableOrientation orientation)
        {
            return Matrix.RotationQuaternion(orientation.Rotation)*Matrix.Translation(orientation.Position);
        }

        public static BoundingBox CalculateWorldBoundingBox(this IMutableSpatial spatial)
        {
            return spatial.LocalBoundingBox.Transform(spatial.CalculateWorld());
        }
    }
}