using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// Contains helper methods for the math classes in SlimDX
    /// </summary>
    public static class UtilityExtensions
    {
        public static Vector3 ToXZ(this Vector2 v)
        {
            return v.ToXZ(0);
        }
        public static Vector3 ToXZ(this Vector2 v, float y)
        {
            return new Vector3(v.X, y, v.Y);
        }
        public static Vector2 TakeXZ(this Vector3 v)
        {
            return new Vector2(v.X, v.Z);
        }
        public static Vector2 TakeXY(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vector3 ChangeX(this Vector3 v, float x)
        {
            return new Vector3(x, v.Y, v.Z);
        }
        public static Vector3 ChangeY(this Vector3 v, float y)
        {
            return new Vector3(v.X, y, v.Z);
        }
        public static Vector3 ChangeZ(this Vector3 v, float z)
        {
            return new Vector3(v.X, v.Y, z);
        }



        public static Vector3 TakeXYZ(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Point3 ToPoint3Rounded(this Vector3 v)
        {
            return new Point3(((int)Math.Round(v.X)), ((int)Math.Round(v.Y)), ((int)Math.Round(v.Z)));
        }



        public static Vector3 GetCenter(this BoundingBox bb)
        {
            return (bb.Maximum + bb.Minimum) * 0.5f;
        }
        public static Vector3 GetSize(this BoundingBox bb)
        {
            return bb.Maximum - bb.Minimum;
        }

        public static Vector3 GetPoint(this Ray ray, float dist)
        {
            return ray.Position + ray.Direction * dist;
        }
    }
}
