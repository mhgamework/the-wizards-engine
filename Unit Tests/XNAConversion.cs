using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace DirectX11
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
    }
}
