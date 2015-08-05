using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards
{
    [DebuggerDisplay("V({X},{Y},{Z})")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public Vector3(float val)
            : this()
        {
            X = val;
            Y = val;
            Z = val;
        }
        public Vector3(float x, float y, float z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float X;

        public float Y;

        public float Z;

        public static Vector3 UnitX { get { return new Vector3(1, 0, 0); } }
        public static Vector3 UnitY { get { return new Vector3(0, 1, 0); } }
        public static Vector3 UnitZ { get { return new Vector3(0, 0, 1); } }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public float LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        public void Normalize()
        {
            var len = Length();
            X /= len;
            Y /= len;
            Z /= len;
        }

        public static Vector3 Normalize(Vector3 v)
        {
            var len = v.Length();
            return new Vector3(v.X /= len, v.Y /= len, v.Z /= len);
        }

        public bool Equals(Vector3 other)
        {
            return X == other.X && X == other.X && X == other.X;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    default: throw new InvalidOperationException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default: throw new InvalidOperationException();
                }
            }
        }


        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }
        public static Vector3 operator *(float b, Vector3 a)
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }
        public static Vector3 operator /(Vector3 a, float b)
        {
            b = 1f / b;
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }
        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.X, -a.Y, -a.Z);
        }


        public static implicit operator SlimDX.Vector3(Vector3 v)
        {
            return new SlimDX.Vector3(v.X, v.Y, v.Z);
        }
        public static implicit operator Vector3(SlimDX.Vector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        /*public static implicit operator Microsoft.xna.Framework.Vector3(Vector3 v)
        {
            return new Microsoft.xna.Framework.Vector3(v.x, v.y, v.z);
        }*/
        public static implicit operator Vector3(Microsoft.Xna.Framework.Vector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        public static implicit operator Vector3(Point3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }



        public static void TransformCoordinate(Vector3[] corners, ref Matrix mat, Vector3[] outCorners)
        {
            for (int i = 0; i < corners.Length; i++)
            {
                outCorners[i] = TransformCoordinate(corners[i], mat);
            }
        }
        public static Vector3 TransformCoordinate(Vector3 coord, Matrix mat)
        {
            return SlimDX.Vector3.TransformCoordinate(coord.dx(), mat);
        }

        public static float Dot(Vector3 sourceDir, Vector3 targetDir)
        {
            return SlimDX.Vector3.Dot(sourceDir.dx(), targetDir.dx());
        }

        public static Vector3 Cross(Vector3 sourceDir, Vector3 targetDir)
        {
            return SlimDX.Vector3.Cross(sourceDir.dx(), targetDir.dx());
        }

        public static SlimDX.Vector3 TransformNormal(Vector3 normal, Matrix world)
        {
            return SlimDX.Vector3.TransformNormal(normal, world);
        }

        public static float Distance(Vector3 position, Vector3 point)
        {
            return SlimDX.Vector3.Distance(position, point);
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float f)
        {
            return SlimDX.Vector3.Lerp(a.dx(), b.dx(), f);
        }

        public Point3 ToFloored()
        {
            return this.dx().ToFloored();
        }


        public Microsoft.Xna.Framework.Vector3 xna() { return new Microsoft.Xna.Framework.Vector3(X, Y, Z); }
        public SlimDX.Vector3 dx() { return new SlimDX.Vector3(X, Y, Z); }
    }
}