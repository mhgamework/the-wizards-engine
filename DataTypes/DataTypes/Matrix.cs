using System;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards
{
    public struct Matrix
    {
        private SlimDX.Matrix m;

        public static Matrix operator *(Matrix a, Matrix b)
        {
            return a.m * b.m;
        }

        public static implicit operator Microsoft.Xna.Framework.Matrix(Matrix m)
        {
            var v = m.m;
            return new Microsoft.Xna.Framework.Matrix(v.M11, v.M12, v.M13, v.M14,
                                                    v.M21, v.M22, v.M23, v.M24,
                                                    v.M31, v.M32, v.M33, v.M34,
                                                    v.M41, v.M42, v.M43, v.M44);
        }
        public static implicit operator Matrix(Microsoft.Xna.Framework.Matrix v)
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
        public static implicit operator SlimDX.Matrix(Matrix m)
        {
            return m.m;
        }
        public static implicit operator Matrix(SlimDX.Matrix m)
        {
            return new Matrix() { m = m };
        }

        public static Matrix Identity { get { return SlimDX.Matrix.Identity; }}

        public static Matrix Invert(Matrix world)
        {
            return SlimDX.Matrix.Invert(world.m);
        }

        public static Matrix Scaling( float x, float y, float z )
        {
            return SlimDX.Matrix.Scaling( x, y, z );
        }
        public static Matrix Scaling(Vector3 v)
        {
            return SlimDX.Matrix.Scaling(v.dx());
        }

        public static Matrix Translation(Vector3 v)
        {
            return SlimDX.Matrix.Translation(v.dx());
        }

        public static Matrix Translation(float x, float y, float z)
        {
            return SlimDX.Matrix.Translation(x, y, z);
        }
    }
}