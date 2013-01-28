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
    }
}
