using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Engine
{
    public static class MathExtensions
    {
        /// <summary>
        /// Creates a bounding box centered on the origin with given size.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static BoundingBox CenteredBoundingbox(this Vector3 size)
        {
            return new BoundingBox(-size * 0.5f, size * 0.5f);
        }

        public static BoundingBox ToBoundingBox(this Vector3 center, Vector3 radius)
        {
            return new BoundingBox(center - radius, center + radius);
        }


        public static Point2 Round(this Vector2 v)
        {
            return new Point2(v);
        }
        public static Point2 Floor(this Vector2 v)
        {
            return Point2.Floor(v);
        }
        public static Point2 Ceiling(this Vector2 v)
        {
            return Point2.Ceiling(v);
        }

        public static int Floor(this double d)
        {
            return (int)Math.Floor(d);
        }
        public static float ToF(this double p)
        {
            return (float)p;
        }

        public static int Floor(this float d)
        {
            return (int)Math.Floor(d);
        }

        public static float F(this double d)
        {
            return (float)d;
        }

    }
}