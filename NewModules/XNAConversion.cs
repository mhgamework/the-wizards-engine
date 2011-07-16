using SlimDX;

namespace MHGameWork.TheWizards
{
    public static class XnaConversion
    {
        public static Vector4 ToSlimDX(this Microsoft.Xna.Framework.Vector4 v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }
        public static Vector3 ToSlimDX(this Microsoft.Xna.Framework.Vector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        public static Vector2 ToSlimDX(this Microsoft.Xna.Framework.Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static Matrix ToSlimDX(this Microsoft.Xna.Framework.Matrix v)
        {
            Matrix m;
            m.M11 = v.M11;
            m.M12 = v.M12;
            m.M13 = v.M13;
            m.M14 = v.M14;

            m.M21 = v.M21;
            m.M22 = v.M22;
            m.M23 = v.M23;
            m.M24 = v.M24;

            m.M31 = v.M31;
            m.M32 = v.M32;
            m.M33 = v.M33;
            m.M34 = v.M34;

            m.M41 = v.M41;
            m.M42 = v.M42;
            m.M43 = v.M43;
            m.M44 = v.M44;

            return m;
        }


        // This is not XNA related
        /// <summary>
        /// Creates a vector4 with as w coordinate 1
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector4 ToVector4(this Vector3 v)
        {
            return new Vector4(v, 1);
        }
    }
}
