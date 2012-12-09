// Type: SlimDX.Vector4
// Assembly: SlimDX, Version=4.0.13.43, Culture=neutral, PublicKeyToken=b1b0c32fd1ffe4f9
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_32\SlimDX\v4.0_4.0.13.43__b1b0c32fd1ffe4f9\SlimDX.dll

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MHGameWork.TheWizards.CG.Math
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector4 : IEquatable<Vector4>
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public static int SizeInBytes
        {
            get { return Marshal.SizeOf(typeof(Vector4)); }
        }

        public static Vector4 UnitW
        {
            get { return new Vector4(0.0f, 0.0f, 0.0f, 1f); }
        }

        public static Vector4 UnitZ
        {
            get { return new Vector4(0.0f, 0.0f, 1f, 0.0f); }
        }

        public static Vector4 UnitY
        {
            get { return new Vector4(0.0f, 1f, 0.0f, 0.0f); }
        }

        public static Vector4 UnitX
        {
            get { return new Vector4(1f, 0.0f, 0.0f, 0.0f); }
        }

        public static Vector4 Zero
        {
            get { return new Vector4(0.0f, 0.0f, 0.0f, 0.0f); }
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
                    case 3:
                        return W;
                    default:
                        throw new ArgumentOutOfRangeException("index", "Indices for Vector4 run from 0 to 3, inclusive.");
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
                    case 3:
                        W = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("index", "Indices for Vector4 run from 0 to 3, inclusive.");
                }
            }
        }

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4(Vector3 value, float w)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = w;
        }

        public Vector4(Vector2 value, float z, float w)
        {
            X = value.X;
            Y = value.Y;
            Z = z;
            W = w;
        }

        public Vector4(float value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }

        public static Vector4 operator +(Vector4 left, Vector4 right)
        {
            Vector4 vector4;
            vector4.X = left.X + right.X;
            vector4.Y = left.Y + right.Y;
            vector4.Z = left.Z + right.Z;
            vector4.W = left.W + right.W;
            return vector4;
        }

        public static Vector4 operator -(Vector4 value)
        {
            float num1 = -value.X;
            float num2 = -value.Y;
            float num3 = -value.Z;
            float num4 = -value.W;
            Vector4 vector4;
            vector4.X = num1;
            vector4.Y = num2;
            vector4.Z = num3;
            vector4.W = num4;
            return vector4;
        }

        public static Vector4 operator -(Vector4 left, Vector4 right)
        {
            Vector4 vector4;
            vector4.X = left.X - right.X;
            vector4.Y = left.Y - right.Y;
            vector4.Z = left.Z - right.Z;
            vector4.W = left.W - right.W;
            return vector4;
        }

        public static Vector4 operator *(float scale, Vector4 vector)
        {
            return vector * scale;
        }

        public static Vector4 operator *(Vector4 vector, float scale)
        {
            Vector4 vector4;
            vector4.X = vector.X * scale;
            vector4.Y = vector.Y * scale;
            vector4.Z = vector.Z * scale;
            vector4.W = vector.W * scale;
            return vector4;
        }

        public static Vector4 operator /(Vector4 vector, float scale)
        {
            Vector4 vector4;
            vector4.X = vector.X / scale;
            vector4.Y = vector.Y / scale;
            vector4.Z = vector.Z / scale;
            vector4.W = vector.W / scale;
            return vector4;
        }

        public static bool operator ==(Vector4 left, Vector4 right)
        {
            return Equals(ref left, ref right);
        }

        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return !Equals(ref left, ref right);
        }

        public float Length()
        {
            return (float)System.Math.Sqrt(LengthSquared());
        }

        public float LengthSquared()
        {
            return (float)(X * (double)X + Y * (double)Y + Z * (double)Z + W * (double)W);
        }

        public static void Normalize(ref Vector4 vector, out Vector4 result)
        {
            Vector4 vector4 = vector;
            result = vector4;
            result.Normalize();
        }

        public static Vector4 Normalize(Vector4 vector)
        {
            vector.Normalize();
            return vector;
        }

        public void Normalize()
        {
            float num1 = Length();
            if (num1 == 0.0)
                return;
            float num2 = 1f / num1;
            X *= num2;
            Y *= num2;
            Z *= num2;
            W *= num2;
        }

        public static void Add(ref Vector4 left, ref Vector4 right, out Vector4 result)
        {
            Vector4 vector4;
            vector4.X = left.X + right.X;
            vector4.Y = left.Y + right.Y;
            vector4.Z = left.Z + right.Z;
            vector4.W = left.W + right.W;
            result = vector4;
        }

        public static Vector4 Add(Vector4 left, Vector4 right)
        {
            Vector4 vector4;
            vector4.X = left.X + right.X;
            vector4.Y = left.Y + right.Y;
            vector4.Z = left.Z + right.Z;
            vector4.W = left.W + right.W;
            return vector4;
        }

        public static void Subtract(ref Vector4 left, ref Vector4 right, out Vector4 result)
        {
            Vector4 vector4;
            vector4.X = left.X - right.X;
            vector4.Y = left.Y - right.Y;
            vector4.Z = left.Z - right.Z;
            vector4.W = left.W - right.W;
            result = vector4;
        }

        public static Vector4 Subtract(Vector4 left, Vector4 right)
        {
            Vector4 vector4;
            vector4.X = left.X - right.X;
            vector4.Y = left.Y - right.Y;
            vector4.Z = left.Z - right.Z;
            vector4.W = left.W - right.W;
            return vector4;
        }

        public static void Multiply(ref Vector4 vector, float scale, out Vector4 result)
        {
            Vector4 vector4;
            vector4.X = vector.X * scale;
            vector4.Y = vector.Y * scale;
            vector4.Z = vector.Z * scale;
            vector4.W = vector.W * scale;
            result = vector4;
        }

        public static Vector4 Multiply(Vector4 value, float scale)
        {
            Vector4 vector4;
            vector4.X = value.X * scale;
            vector4.Y = value.Y * scale;
            vector4.Z = value.Z * scale;
            vector4.W = value.W * scale;
            return vector4;
        }

        public static void Modulate(ref Vector4 left, ref Vector4 right, out Vector4 result)
        {
            Vector4 vector4;
            vector4.X = left.X * right.X;
            vector4.Y = left.Y * right.Y;
            vector4.Z = left.Z * right.Z;
            vector4.W = left.W * right.W;
            result = vector4;
        }

        public static Vector4 Modulate(Vector4 left, Vector4 right)
        {
            Vector4 vector4;
            vector4.X = left.X * right.X;
            vector4.Y = left.Y * right.Y;
            vector4.Z = left.Z * right.Z;
            vector4.W = left.W * right.W;
            return vector4;
        }


        public static float Dot(Vector4 left, Vector4 right)
        {
            return
                (float)
                (left.Y * (double)right.Y + left.X * (double)right.X + left.Z * (double)right.Z + left.W * (double)right.W);
        }


        public static Vector4[] Transform(Vector4[] vectors, ref Matrix transformation)
        {
            if (vectors == null)
                throw new ArgumentNullException("vectors");
            int length = vectors.Length;
            var vector4Array = new Vector4[length];
            int index = 0;
            if (0 < length)
            {
                do
                {
                    vector4Array[index] = new Vector4
                                              {
                                                  X =
                                                      (float)
                                                      (vectors[index].Y * (double)transformation.M21 +
                                                       vectors[index].X * (double)transformation.M11 +
                                                       vectors[index].Z * (double)transformation.M31 +
                                                       vectors[index].W * (double)transformation.M41),
                                                  Y =
                                                      (float)
                                                      (vectors[index].Y * (double)transformation.M22 +
                                                       vectors[index].X * (double)transformation.M12 +
                                                       vectors[index].Z * (double)transformation.M32 +
                                                       vectors[index].W * (double)transformation.M42),
                                                  Z =
                                                      (float)
                                                      (vectors[index].Y * (double)transformation.M23 +
                                                       vectors[index].X * (double)transformation.M13 +
                                                       vectors[index].Z * (double)transformation.M33 +
                                                       vectors[index].W * (double)transformation.M43),
                                                  W =
                                                      (float)
                                                      (vectors[index].Y * (double)transformation.M24 +
                                                       vectors[index].X * (double)transformation.M14 +
                                                       vectors[index].Z * (double)transformation.M34 +
                                                       vectors[index].W * (double)transformation.M44)
                                              };
                    ++index;
                } while (index < length);
            }
            return vector4Array;
        }

        public static void Transform(ref Vector4 vector, ref Matrix transformation, out Vector4 result)
        {
            result = new Vector4
                         {
                             X =
                                 (float)
                                 (vector.Y * (double)transformation.M21 + vector.X * (double)transformation.M11 +
                                  vector.Z * (double)transformation.M31 + vector.W * (double)transformation.M41),
                             Y =
                                 (float)
                                 (vector.Y * (double)transformation.M22 + vector.X * (double)transformation.M12 +
                                  vector.Z * (double)transformation.M32 + vector.W * (double)transformation.M42),
                             Z =
                                 (float)
                                 (vector.Y * (double)transformation.M23 + vector.X * (double)transformation.M13 +
                                  vector.Z * (double)transformation.M33 + vector.W * (double)transformation.M43),
                             W =
                                 (float)
                                 (vector.Y * (double)transformation.M24 + vector.X * (double)transformation.M14 +
                                  vector.Z * (double)transformation.M34 + vector.W * (double)transformation.M44)
                         };
        }

        public static Vector4 Transform(Vector4 vector, Matrix transformation)
        {
            return new Vector4
                       {
                           X =
                               (float)
                               (transformation.M21 * (double)vector.Y + transformation.M11 * (double)vector.X +
                                transformation.M31 * (double)vector.Z + transformation.M41 * (double)vector.W),
                           Y =
                               (float)
                               (transformation.M22 * (double)vector.Y + transformation.M12 * (double)vector.X +
                                transformation.M32 * (double)vector.Z + transformation.M42 * (double)vector.W),
                           Z =
                               (float)
                               (transformation.M23 * (double)vector.Y + transformation.M13 * (double)vector.X +
                                transformation.M33 * (double)vector.Z + transformation.M43 * (double)vector.W),
                           W =
                               (float)
                               (transformation.M24 * (double)vector.Y + transformation.M14 * (double)vector.X +
                                transformation.M34 * (double)vector.Z + transformation.M44 * (double)vector.W)
                       };
        }

        public static void Minimize(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            float num1 = value1.X >= (double)value2.X ? value2.X : value1.X;
            result.X = num1;
            float num2 = value1.Y >= (double)value2.Y ? value2.Y : value1.Y;
            result.Y = num2;
            float num3 = value1.Z >= (double)value2.Z ? value2.Z : value1.Z;
            result.Z = num3;
            float num4 = value1.W >= (double)value2.W ? value2.W : value1.W;
            result.W = num4;
        }

        public static Vector4 Minimize(Vector4 value1, Vector4 value2)
        {
            var vector4 = new Vector4();
            float num1 = value1.X >= (double)value2.X ? value2.X : value1.X;
            vector4.X = num1;
            float num2 = value1.Y >= (double)value2.Y ? value2.Y : value1.Y;
            vector4.Y = num2;
            float num3 = value1.Z >= (double)value2.Z ? value2.Z : value1.Z;
            vector4.Z = num3;
            float num4 = value1.W >= (double)value2.W ? value2.W : value1.W;
            vector4.W = num4;
            return vector4;
        }

        public static void Maximize(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            float num1 = value1.X <= (double)value2.X ? value2.X : value1.X;
            result.X = num1;
            float num2 = value1.Y <= (double)value2.Y ? value2.Y : value1.Y;
            result.Y = num2;
            float num3 = value1.Z <= (double)value2.Z ? value2.Z : value1.Z;
            result.Z = num3;
            float num4 = value1.W <= (double)value2.W ? value2.W : value1.W;
            result.W = num4;
        }

        public static Vector4 Maximize(Vector4 value1, Vector4 value2)
        {
            var vector4 = new Vector4();
            float num1 = value1.X <= (double)value2.X ? value2.X : value1.X;
            vector4.X = num1;
            float num2 = value1.Y <= (double)value2.Y ? value2.Y : value1.Y;
            vector4.Y = num2;
            float num3 = value1.Z <= (double)value2.Z ? value2.Z : value1.Z;
            vector4.Z = num3;
            float num4 = value1.W <= (double)value2.W ? value2.W : value1.W;
            vector4.W = num4;
            return vector4;
        }

        public override string ToString()
        {
            var objArray = new object[4];
            float num1 = X;
            objArray[0] = num1.ToString(CultureInfo.CurrentCulture);
            float num2 = Y;
            objArray[1] = num2.ToString(CultureInfo.CurrentCulture);
            float num3 = Z;
            objArray[2] = num3.ToString(CultureInfo.CurrentCulture);
            float num4 = W;
            objArray[3] = num4.ToString(CultureInfo.CurrentCulture);
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", objArray);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + (Z.GetHashCode() + W.GetHashCode() + Y.GetHashCode());
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool Equals(ref Vector4 value1, ref Vector4 value2)
        {
            return value1.X == (double)value2.X && value1.Y == (double)value2.Y &&
                   (value1.Z == (double)value2.Z && value1.W == (double)value2.W);
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool Equals(Vector4 other)
        {
            return X == (double)other.X && Y == (double)other.Y && (Z == (double)other.Z && W == (double)other.W);
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            else
                return Equals((Vector4)obj);
        }
    }
}