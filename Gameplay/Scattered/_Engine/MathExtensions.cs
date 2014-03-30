using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Engine
{
    public static class MathExtensions
    {
        public static BoundingBox CenteredBoundingbox(this Vector3 v)
        {
            return new BoundingBox(-v * 0.5f, v * 0.5f);
        }
    }
}