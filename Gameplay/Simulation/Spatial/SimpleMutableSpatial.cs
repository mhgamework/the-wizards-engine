using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.Spatial
{
    public class SimpleMutableSpatial : IMutableSpatial
    {
        private readonly BoundingBox localBounding;

        public SimpleMutableSpatial(BoundingBox localBounding)
        {
            this.localBounding = localBounding;
        }

        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public BoundingBox LocalBoundingBox { get { return localBounding; } }
    }
}