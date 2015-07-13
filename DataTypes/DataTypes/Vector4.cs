﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SlimDX;

namespace MHGameWork.TheWizards
{
    [DebuggerDisplay("V({X},{Y},{Z},{W})")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4
    {
        public Vector4(float val)
            : this()
        {
            X = val;
            Y = val;
            Z = val;
            W = val;
        }

        public Vector4( Vector3 v, float w ) : this()
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = w;

        }
        public Vector4(float x, float y, float z,float w)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

     /*   public float Length()
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

        public bool Equals(SlimDX.Vector3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
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
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vector3 operator *(Vector3 a, float b)
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


        */public static implicit operator SlimDX.Vector4(Vector4 v)
        {
            return new SlimDX.Vector4(v.X, v.Y, v.Z, v.W);
        }
          public static implicit operator Vector4(SlimDX.Vector4 v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }
          public static implicit operator Microsoft.Xna.Framework.Vector4(Vector4 v)
        {
            return new Microsoft.Xna.Framework.Vector4(v.X, v.Y, v.Z,v.W);
        }
          public static implicit operator Vector4(Microsoft.Xna.Framework.Vector4 v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }/*


        public static void TransformCoordinate(Vector3[] corners, ref Matrix mat, Vector3[] outCorners)
        {
            for (int i = 0; i < corners.Length; i++)
            {
                outCorners[i] = TransformCoordinate(corners[i], mat);
            }
        }
        public static Vector3 TransformCoordinate(Vector3 coord, Matrix mat)
        {
            return SlimDX.Vector3.TransformCoordinate(coord, mat);
        }

        public static float Dot(Vector3 sourceDir, Vector3 targetDir)
        {
            return SlimDX.Vector3.Dot(sourceDir, targetDir);
        }

        public static Vector3 Cross(Vector3 sourceDir, Vector3 targetDir)
        {
            return SlimDX.Vector3.Cross(sourceDir, targetDir);
        }

        public static SlimDX.Vector3 TransformNormal(Vector3 normal, Matrix world)
        {
            throw new NotImplementedException();
        }

        public static float? Distance(SlimDX.Vector3 position, Vector3 point)
        {
            throw new NotImplementedException();
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float f)
        {
            return SlimDX.Vector3.Lerp(a, b, f);
        }*/

    }
}