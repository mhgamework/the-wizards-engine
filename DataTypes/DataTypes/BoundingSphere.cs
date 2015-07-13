using SlimDX.DirectSound;

namespace MHGameWork.TheWizards
{
    public struct BoundingSphere
    {
        private SlimDX.BoundingSphere bdx;
        public BoundingSphere(Vector3 center, float radius)
        {
            bdx = new SlimDX.BoundingSphere(center.dx(), radius);
        }

        public SlimDX.BoundingSphere dx() { return bdx; }
    }
}