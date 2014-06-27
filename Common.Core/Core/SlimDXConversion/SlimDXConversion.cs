using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards
{
    public static class SlimDXConversion
    {
        public static Microsoft.Xna.Framework.BoundingBox xna(this SlimDX.BoundingBox v)
        {
            return new Microsoft.Xna.Framework.BoundingBox(v.Minimum.xna(), v.Maximum.xna());
        }
        public static Microsoft.Xna.Framework.BoundingSphere xna(this SlimDX.BoundingSphere v)
        {
            return new Microsoft.Xna.Framework.BoundingSphere(v.Center.xna(), v.Radius);
        }
        public static Microsoft.Xna.Framework.Vector3 xna(this SlimDX.Vector3 v)
        {
            return new Microsoft.Xna.Framework.Vector3(v.X, v.Y, v.Z);
        }
        public static Microsoft.Xna.Framework.Vector4 xna(this SlimDX.Vector4 v)
        {
            return new Microsoft.Xna.Framework.Vector4(v.X, v.Y, v.Z, v.W);
        }
        public static Microsoft.Xna.Framework.Quaternion xna(this SlimDX.Quaternion v)
        {
            return new Microsoft.Xna.Framework.Quaternion(v.X, v.Y, v.Z, v.W);
        }
        public static Microsoft.Xna.Framework.Matrix xna(this SlimDX.Matrix v)
        {
            return new Microsoft.Xna.Framework.Matrix(v.M11, v.M12, v.M13, v.M14,
                                                      v.M21, v.M22, v.M23, v.M24,
                                                      v.M31, v.M32, v.M33, v.M34,
                                                      v.M41, v.M42, v.M43, v.M44);
        }
        public static Color xna(this SlimDX.Color4 v)
        {
            return new Color(v.ToVector4().xna());
        }
        public static Microsoft.Xna.Framework.Plane xna(this SlimDX.Plane v)
        {
            return new Microsoft.Xna.Framework.Plane(v.Normal.xna(), v.D);
        }
        public static Microsoft.Xna.Framework.Ray xna(this SlimDX.Ray v)
        {
            return new Microsoft.Xna.Framework.Ray(v.Position.xna(), v.Direction.xna());
        }


        public static SlimDX.BoundingBox dx(this Microsoft.Xna.Framework.BoundingBox v)
        {
            return new SlimDX.BoundingBox(v.Min.dx(), v.Max.dx());
        }
        public static SlimDX.BoundingSphere dx(this Microsoft.Xna.Framework.BoundingSphere v)
        {
            return new SlimDX.BoundingSphere(v.Center.dx(), v.Radius);
        }
        public static SlimDX.Vector3 dx(this Microsoft.Xna.Framework.Vector3 v)
        {
            return new SlimDX.Vector3(v.X, v.Y, v.Z);
        }
        public static SlimDX.Vector4 dx(this Microsoft.Xna.Framework.Vector4 v)
        {
            return new SlimDX.Vector4(v.X, v.Y, v.Z, v.W);
        }
        public static SlimDX.Quaternion dx(this Microsoft.Xna.Framework.Quaternion v)
        {
            return new SlimDX.Quaternion(v.X, v.Y, v.Z, v.W);
        }
        public static SlimDX.Matrix dx(this Microsoft.Xna.Framework.Matrix v)
        {
            return new SlimDX.Matrix
                       {
                           M11 = v.M11,
                           M12 = v.M12,
                           M13 = v.M13,
                           M14 = v.M14,
                           M21 = v.M21,
                           M22 = v.M22,
                           M23 = v.M23,
                           M24 = v.M24,
                           M31 = v.M31,
                           M32 = v.M32,
                           M33 = v.M33,
                           M34 = v.M34,
                           M41 = v.M41,
                           M42 = v.M42,
                           M43 = v.M43,
                           M44 = v.M44
                       };
        }
        public static SlimDX.Color4 dx(this Color v)
        {
            return new SlimDX.Color4(v.ToVector4().dx());
        }
        public static SlimDX.Plane dx(this Microsoft.Xna.Framework.Plane v)
        {
            return new SlimDX.Plane(v.Normal.dx(), v.D);
        }
        public static SlimDX.Ray dx(this Microsoft.Xna.Framework.Ray v)
        {
            return new SlimDX.Ray(v.Position.dx(), v.Direction.dx());
        }

        public static SlimDX.Color4 dx(this System.Drawing.Color v)
        {
            return new SlimDX.Color4(v);
        }
    }
}
