using System;
using DirectX11;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// Contains helper methods for the math classes in SlimDX
    /// </summary>
    public static class UtilityExtensions2
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

        public static float MaxComponent(this Vector3 v)
        {
            return Math.Max(v.X, Math.Max(v.Y, v.Z));
        }
        public static float MaxComponent(this Vector4 v)
        {
            return Math.Max(v.X, Math.Max(v.Y, Math.Max(v.Z, v.W)));
        }
        public static float MaxComponent(this Vector2 v)
        {
            return Math.Max(v.X, v.Y);
        }

        public static float MinComponent(this Vector3 v)
        {
            return Math.Min(v.X, Math.Min(v.Y, v.Z));
        }
        public static float MinComponent(this Vector4 v)
        {
            return Math.Min(v.X, Math.Min(v.Y, Math.Min(v.Z, v.W)));
        }
        public static float MinComponent(this Vector2 v)
        {
            return Math.Min(v.X, v.Y);
        }



        public static Vector3 TakeXYZ(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Point3 ToPoint3Rounded(this Vector3 v)
        {
            return new Point3(((int)Math.Round(v.X)), ((int)Math.Round(v.Y)), ((int)Math.Round(v.Z)));
        }
        public static Point3 ToFloored(this Vector3 v)
        {
            return new Point3(((int)Math.Floor(v.X)), ((int)Math.Floor(v.Y)), ((int)Math.Floor(v.Z)));
        }



        public static Vector3 GetCenter(this BoundingBox bb)
        {
            return (bb.Maximum + bb.Minimum) * 0.5f;
        }
        public static Vector3 GetSize(this BoundingBox bb)
        {
            return bb.Maximum - bb.Minimum;
        }


        public static bool IsSameAs(this Vector3 a, Vector3 b)
        {
            return Math.Abs(a.X - b.X) < 0.0001f
                   && Math.Abs(a.Y - b.Y) < 0.0001f
                   && Math.Abs(a.Z - b.Z) < 0.0001f;
        }
        public static bool IsSameAs(this Vector2 a, Vector2 b)
        {
            return Math.Abs(a.X - b.X) < 0.0001f
                   && Math.Abs(a.Y - b.Y) < 0.0001f;
        }

        public static Vector3 ToVector3(this float[] arr)
        {
            return new Vector3(arr[0], arr[1], arr[2]);
        }
        public static float[] ToArray(this Vector3 v)
        {
            return new float[] { v.X, v.Y, v.Z };
        }
    }
}
