using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.Spatial
{
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