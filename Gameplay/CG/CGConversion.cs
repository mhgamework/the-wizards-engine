
using System.Drawing;

namespace MHGameWork.TheWizards.CG
{
    public static class CGConversion
    {
        public static MHGameWork.TheWizards.CG.Math.BoundingBox cg(this SlimDX.BoundingBox v)
        {
            return new MHGameWork.TheWizards.CG.Math.BoundingBox(v.Minimum.cg(), v.Maximum.cg());
        }
        public static MHGameWork.TheWizards.CG.Math.Vector2 cg(this SlimDX.Vector2 v)
        {
            return new MHGameWork.TheWizards.CG.Math.Vector2(v.X, v.Y);
        }
        public static MHGameWork.TheWizards.CG.Math.Vector3 cg(this SlimDX.Vector3 v)
        {
            return new MHGameWork.TheWizards.CG.Math.Vector3(v.X, v.Y, v.Z);
        }
        public static MHGameWork.TheWizards.CG.Math.Vector4 cg(this SlimDX.Vector4 v)
        {
            return new MHGameWork.TheWizards.CG.Math.Vector4(v.X, v.Y, v.Z, v.W);
        }
        public static MHGameWork.TheWizards.CG.Math.Matrix cg(this SlimDX.Matrix v)
        {
            return new Math.Matrix
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
        public static Math.Color4 cg(this SlimDX.Color4 v)
        {
            return new Math.Color4(v.ToVector4().cg());
        }
        public static MHGameWork.TheWizards.CG.Math.Plane cg(this SlimDX.Plane v)
        {
            return new MHGameWork.TheWizards.CG.Math.Plane(v.Normal.cg(), v.D);
        }
        public static MHGameWork.TheWizards.CG.Math.Ray cg(this SlimDX.Ray v)
        {
            return new MHGameWork.TheWizards.CG.Math.Ray(v.Position.cg(), v.Direction.cg());
        }


        public static SlimDX.BoundingBox dx(this MHGameWork.TheWizards.CG.Math.BoundingBox v)
        {
            return new SlimDX.BoundingBox(v.Minimum.dx(), v.Maximum.dx());
        }
        public static SlimDX.Vector2 dx(this MHGameWork.TheWizards.CG.Math.Vector2 v)
        {
            return new SlimDX.Vector2(v.X, v.Y);
        }
        public static SlimDX.Vector3 dx(this MHGameWork.TheWizards.CG.Math.Vector3 v)
        {
            return new SlimDX.Vector3(v.X, v.Y, v.Z);
        }
        public static SlimDX.Vector4 dx(this MHGameWork.TheWizards.CG.Math.Vector4 v)
        {
            return new SlimDX.Vector4(v.X, v.Y, v.Z, v.W);
        }
        public static SlimDX.Matrix dx(this MHGameWork.TheWizards.CG.Math.Matrix v)
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
        public static SlimDX.Color4 dx(this Math.Color4 v)
        {
            return new SlimDX.Color4(v.ToVector4().dx());
        }
        public static SlimDX.Plane dx(this MHGameWork.TheWizards.CG.Math.Plane v)
        {
            return new SlimDX.Plane(v.Normal.dx(), v.D);
        }
        public static SlimDX.Ray dx(this MHGameWork.TheWizards.CG.Math.Ray v)
        {
            return new SlimDX.Ray(v.Position.dx(), v.Direction.dx());
        }

    }
}
