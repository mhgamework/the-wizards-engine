using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Engine
{
    public static class MathExtensions
    {
        public static BoundingBox CenteredBoundingbox(this Vector3 v)
        {
            return new BoundingBox(-v * 0.5f, v * 0.5f);
        }

        public static Point2 Floor(this Vector2 v)
        {
            return Point2.Floor(v);
        }
        public static Point2 Ceiling(this Vector2 v)
        {
            return Point2.Ceiling(v);
        }

        public static int Floor(this float f)
        {
            return (int) Math.Floor(f);
        }

    }
}