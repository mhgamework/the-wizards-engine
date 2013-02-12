using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards
{
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



        public static Vector3 TakeXYZ(this Vector4 v)
        {
            return new Vector3(v.X,v.Y,v.Z);
        }

        public static Point3 ToPoint3Rounded(this Vector3 v)
        {
            return new Point3(((int)Math.Round(v.X)), ((int)Math.Round(v.Y)), ((int)Math.Round(v.Z)));
        }
    }
}
