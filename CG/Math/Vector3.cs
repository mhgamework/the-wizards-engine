// Type: SlimDX.Vector3
// Assembly: SlimDX, Version=4.0.13.43, Culture=neutral, PublicKeyToken=b1b0c32fd1ffe4f9
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_32\SlimDX\v4.0_4.0.13.43__b1b0c32fd1ffe4f9\SlimDX.dll

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MHGameWork.TheWizards.CG.Math
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector3 : IEquatable<Vector3>
    {
        public float X;
        public float Y;
        public float Z;

        public static Vector3 UnitZ
        {
            get { return new Vector3(0.0f, 0.0f, 1f); }
        }

        public static Vector3 UnitY
        {
            get { return new Vector3(0.0f, 1f, 0.0f); }
        }

        public static Vector3 UnitX
        {
            get { return new Vector3(1f, 0.0f, 0.0f); }
        }

        public static Vector3 Zero
        {
            get { return new Vector3(0.0f, 0.0f, 0.0f); }
        }

        public float this[int index]
        {
            get
            {
                if (index == 0)
                    return X;
                if (index == 1)
                    return Y;
                if (index != 2)
                    throw new ArgumentOutOfRangeException();
                else
                    return Z;
            }
            set
            {
                if (index != 0)
                {
                    if (index != 1)
                    {
                        if (index != 2)
                            throw new ArgumentOutOfRangeException();
                        Z = value;
                    }
                    else
                        Y = value;
                }
                else
                    X = value;
            }
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(Vector2 value, float z)
        {
            X = value.X;
            Y = value.Y;
            Z = z;
        }

        public Vector3(float value)
        {
            X = value;
            Y = value;
            Z = value;
        }

        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            Vector3 vector3;
            vector3.X = left.X + right.X;
            vector3.Y = left.Y + right.Y;
            vector3.Z = left.Z + right.Z;
            return vector3;
        }

        public static Vector3 operator -(Vector3 value)
        {
            Vector3 vector3;
            vector3.X = -value.X;
            vector3.Y = -value.Y;
            vector3.Z = -value.Z;
            return vector3;
        }

        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            Vector3 vector3;
            vector3.X = left.X - right.X;
            vector3.Y = left.Y - right.Y;
            vector3.Z = left.Z - right.Z;
            return vector3;
        }

        public static Vector3 operator *(float scale, Vector3 vector)
        {
            return vector * scale;
        }

        public static Vector3 operator *(Vector3 vector, float scale)
        {
            Vector3 vector3;
            vector3.X = vector.X * scale;
            vector3.Y = vector.Y * scale;
            vector3.Z = vector.Z * scale;
            return vector3;
        }

        public static Vector3 operator /(Vector3 vector, float scale)
        {
            return vector * (1 / scale);
        }

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return Equals(ref left, ref right);
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !Equals(ref left, ref right);
        }

        public float Length()
        {
            return (float)System.Math.Sqrt(LengthSquared());
        }

        public float LengthSquared()
        {
            return (float)(X * (double)X + Y * (double)Y + Z * (double)Z);
        }

        public static void Normalize(ref Vector3 vector, out Vector3 result)
        {
            Vector3 vector3 = vector;
            result = vector3;
            result.Normalize();
        }

        public static Vector3 Normalize(Vector3 vector)
        {
            vector.Normalize();
            return vector;
        }

        public void Normalize()
        {
            float len = Length();
            if (len == 0.0)
                return;
            float oneOverLen = 1f / len;
            X *= oneOverLen;
            Y *= oneOverLen;
            Z *= oneOverLen;
        }

        public static void Subtract(ref Vector3 left, ref Vector3 right, out Vector3 result)
        {
            Vector3 vector3;
            vector3.X = left.X - right.X;
            vector3.Y = left.Y - right.Y;
            vector3.Z = left.Z - right.Z;
            result = vector3;
        }

        public static void Modulate(ref Vector3 left, ref Vector3 right, out Vector3 result)
        {
            Vector3 vector3;
            vector3.X = left.X * right.X;
            vector3.Y = left.Y * right.Y;
            vector3.Z = left.Z * right.Z;
            result = vector3;
        }

        public static Vector3 Modulate(Vector3 left, Vector3 right)
        {
            Vector3 vector3;
            vector3.X = left.X * right.X;
            vector3.Y = left.Y * right.Y;
            vector3.Z = left.Z * right.Z;
            return vector3;
        }


        public static void Clamp(ref Vector3 value, ref Vector3 min, ref Vector3 max, out Vector3 result)
        {
            float num1 = value.X;
            float num2 = num1 <= (double)max.X ? num1 : max.X;
            float num3 = num2 >= (double)min.X ? num2 : min.X;
            float num4 = value.Y;
            float num5 = num4 <= (double)max.Y ? num4 : max.Y;
            float num6 = num5 >= (double)min.Y ? num5 : min.Y;
            float num7 = value.Z;
            float num8 = num7 <= (double)max.Z ? num7 : max.Z;
            float num9 = num8 >= (double)min.Z ? num8 : min.Z;
            Vector3 vector3;
            vector3.X = num3;
            vector3.Y = num6;
            vector3.Z = num9;
            result = vector3;
        }

        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            Vector3 vector3;
            vector3.X = (value.X <= (double)max.X ? value.X : max.X) >= (double)min.X ? (value.X <= (double)max.X ? value.X : max.X) : min.X;
            vector3.Y = (value.Y <= (double)max.Y ? value.Y : max.Y) >= (double)min.Y ? (value.Y <= (double)max.Y ? value.Y : max.Y) : min.Y;
            vector3.Z = (value.Z <= (double)max.Z ? value.Z : max.Z) >= (double)min.Z ? (value.Z <= (double)max.Z ? value.Z : max.Z) : min.Z;
            return vector3;
        }

        public static void Lerp(ref Vector3 start, ref Vector3 end, float amount, out Vector3 result)
        {
            result.X = (end.X - start.X) * amount + start.X;
            result.Y = (end.Y - start.Y) * amount + start.Y;
            result.Z = (end.Z - start.Z) * amount + start.Z;
        }

        public static Vector3 Lerp(Vector3 start, Vector3 end, float amount)
        {
            return new Vector3
                       {
                           X = (end.X - start.X) * amount + start.X,
                           Y = (end.Y - start.Y) * amount + start.Y,
                           Z = (end.Z - start.Z) * amount + start.Z
                       };
        }

        public static float Dot(Vector3 left, Vector3 right)
        {
            float ret;
            Dot(ref left, ref right, out ret);
            return ret;
        }

        public static void Dot(ref Vector3 left, ref Vector3 right, out float determinant)
        {
            determinant = (float)(left.Y * (double)right.Y + left.X * (double)right.X + left.Z * (double)right.Z);
        }

        public static void Cross(ref Vector3 left, ref Vector3 right, out Vector3 result)
        {
            result = new Vector3
                         {
                             X = (float)(left.Y * (double)right.Z - left.Z * (double)right.Y),
                             Y = (float)(left.Z * (double)right.X - left.X * (double)right.Z),
                             Z = (float)(left.X * (double)right.Y - left.Y * (double)right.X)
                         };
        }

        public static Vector3 Cross(Vector3 left, Vector3 right)
        {
            return new Vector3
                       {
                           X = (float)(right.Z * (double)left.Y - left.Z * (double)right.Y),
                           Y = (float)(left.Z * (double)right.X - right.Z * (double)left.X),
                           Z = (float)(right.Y * (double)left.X - left.Y * (double)right.X)
                       };
        }

        public static void Reflect(ref Vector3 vector, ref Vector3 normal, out Vector3 result)
        {
            double num = (vector.Y * (double)normal.Y + vector.X * (double)normal.X + vector.Z * (double)normal.Z) * 2.0;
            result.X = vector.X - normal.X * (float)num;
            result.Y = vector.Y - normal.Y * (float)num;
            result.Z = vector.Z - normal.Z * (float)num;
        }

        public static Vector3 Reflect(Vector3 vector, Vector3 normal)
        {
            var vector3 = new Vector3();
            double num = (vector.Y * (double)normal.Y + vector.X * (double)normal.X + vector.Z * (double)normal.Z) * 2.0;
            vector3.X = vector.X - normal.X * (float)num;
            vector3.Y = vector.Y - normal.Y * (float)num;
            vector3.Z = vector.Z - normal.Z * (float)num;
            return vector3;
        }


        public static void Transform(ref Vector3 vector, ref Matrix transformation, out Vector4 result)
        {
            var vector4 = new Vector4();
            result = vector4;
            result.X =
                (float)
                (vector.Y * (double)transformation.M21 + vector.X * (double)transformation.M11 +
                 vector.Z * (double)transformation.M31) + transformation.M41;
            result.Y =
                (float)
                (vector.Y * (double)transformation.M22 + vector.X * (double)transformation.M12 +
                 vector.Z * (double)transformation.M32) + transformation.M42;
            result.Z =
                (float)
                (vector.Y * (double)transformation.M23 + vector.X * (double)transformation.M13 +
                 vector.Z * (double)transformation.M33) + transformation.M43;
            result.W =
                (float)
                (transformation.M24 * (double)vector.Y + transformation.M14 * (double)vector.X +
                 vector.Z * (double)transformation.M34) + transformation.M44;
        }

        public static Vector4 Transform(Vector3 vector, Matrix transformation)
        {
            return new Vector4
                       {
                           X =
                               (float)
                               (transformation.M21 * (double)vector.Y + transformation.M11 * (double)vector.X +
                                transformation.M31 * (double)vector.Z) + transformation.M41,
                           Y =
                               (float)
                               (transformation.M22 * (double)vector.Y + transformation.M12 * (double)vector.X +
                                transformation.M32 * (double)vector.Z) + transformation.M42,
                           Z =
                               (float)
                               (transformation.M23 * (double)vector.Y + transformation.M13 * (double)vector.X +
                                transformation.M33 * (double)vector.Z) + transformation.M43,
                           W =
                               (float)
                               (transformation.M24 * (double)vector.Y + transformation.M14 * (double)vector.X +
                                transformation.M34 * (double)vector.Z) + transformation.M44
                       };
        }


        public static void TransformCoordinate(ref Vector3 coordinate, ref Matrix transformation, out Vector3 result)
        {
            var vector4 = new Vector4();
            vector4.X =
                (float)
                (coordinate.Y * (double)transformation.M21 + coordinate.X * (double)transformation.M11 +
                 coordinate.Z * (double)transformation.M31) + transformation.M41;
            vector4.Y =
                (float)
                (coordinate.Y * (double)transformation.M22 + coordinate.X * (double)transformation.M12 +
                 coordinate.Z * (double)transformation.M32) + transformation.M42;
            vector4.Z =
                (float)
                (coordinate.Y * (double)transformation.M23 + coordinate.X * (double)transformation.M13 +
                 coordinate.Z * (double)transformation.M33) + transformation.M43;
            var num =
                (float)
                (1.0 /
                 (transformation.M24 * (double)coordinate.Y + transformation.M14 * (double)coordinate.X +
                  coordinate.Z * (double)transformation.M34 + transformation.M44));
            vector4.W = num;
            Vector3 vector3;
            vector3.X = vector4.X * num;
            vector3.Y = vector4.Y * num;
            vector3.Z = vector4.Z * num;
            result = vector3;
        }

        public static Vector3 TransformCoordinate(Vector3 coordinate, Matrix transformation)
        {
            var vector4 = new Vector4();
            vector4.X =
                (float)
                (transformation.M21 * (double)coordinate.Y + transformation.M11 * (double)coordinate.X +
                 transformation.M31 * (double)coordinate.Z) + transformation.M41;
            vector4.Y =
                (float)
                (transformation.M22 * (double)coordinate.Y + transformation.M12 * (double)coordinate.X +
                 transformation.M32 * (double)coordinate.Z) + transformation.M42;
            vector4.Z =
                (float)
                (transformation.M23 * (double)coordinate.Y + transformation.M13 * (double)coordinate.X +
                 transformation.M33 * (double)coordinate.Z) + transformation.M43;
            var num =
                (float)
                (1.0 /
                 (transformation.M24 * (double)coordinate.Y + transformation.M14 * (double)coordinate.X +
                  transformation.M34 * (double)coordinate.Z + transformation.M44));
            vector4.W = num;
            Vector3 vector3;
            vector3.X = vector4.X * num;
            vector3.Y = vector4.Y * num;
            vector3.Z = vector4.Z * num;
            return vector3;
        }


        public static void TransformNormal(ref Vector3 normal, ref Matrix transformation, out Vector3 result)
        {
            result.X =
                (float)
                (normal.Y * (double)transformation.M21 + normal.X * (double)transformation.M11 +
                 normal.Z * (double)transformation.M31);
            result.Y =
                (float)
                (normal.Y * (double)transformation.M22 + normal.X * (double)transformation.M12 +
                 normal.Z * (double)transformation.M32);
            result.Z =
                (float)
                (normal.Y * (double)transformation.M23 + normal.X * (double)transformation.M13 +
                 normal.Z * (double)transformation.M33);
        }

        public static Vector3 TransformNormal(Vector3 normal, Matrix transformation)
        {
            return new Vector3
                       {
                           X =
                               (float)
                               (transformation.M21 * (double)normal.Y + transformation.M11 * (double)normal.X +
                                transformation.M31 * (double)normal.Z),
                           Y =
                               (float)
                               (transformation.M22 * (double)normal.Y + transformation.M12 * (double)normal.X +
                                transformation.M32 * (double)normal.Z),
                           Z =
                               (float)
                               (transformation.M23 * (double)normal.Y + transformation.M13 * (double)normal.X +
                                transformation.M33 * (double)normal.Z)
                       };
        }

        public static void Minimize(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            float num1 = value1.X >= (double)value2.X ? value2.X : value1.X;
            result.X = num1;
            float num2 = value1.Y >= (double)value2.Y ? value2.Y : value1.Y;
            result.Y = num2;
            float num3 = value1.Z >= (double)value2.Z ? value2.Z : value1.Z;
            result.Z = num3;
        }

        public static Vector3 Minimize(Vector3 value1, Vector3 value2)
        {
            var vector3 = new Vector3();
            float num1 = value1.X >= (double)value2.X ? value2.X : value1.X;
            vector3.X = num1;
            float num2 = value1.Y >= (double)value2.Y ? value2.Y : value1.Y;
            vector3.Y = num2;
            float num3 = value1.Z >= (double)value2.Z ? value2.Z : value1.Z;
            vector3.Z = num3;
            return vector3;
        }

        public static void Maximize(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            float num1 = value1.X <= (double)value2.X ? value2.X : value1.X;
            result.X = num1;
            float num2 = value1.Y <= (double)value2.Y ? value2.Y : value1.Y;
            result.Y = num2;
            float num3 = value1.Z <= (double)value2.Z ? value2.Z : value1.Z;
            result.Z = num3;
        }

        public static Vector3 Maximize(Vector3 value1, Vector3 value2)
        {
            var vector3 = new Vector3();
            float num1 = value1.X <= (double)value2.X ? value2.X : value1.X;
            vector3.X = num1;
            float num2 = value1.Y <= (double)value2.Y ? value2.Y : value1.Y;
            vector3.Y = num2;
            float num3 = value1.Z <= (double)value2.Z ? value2.Z : value1.Z;
            vector3.Z = num3;
            return vector3;
        }

        public override string ToString()
        {
            var objArray = new object[3];
            float num1 = X;
            objArray[0] = num1.ToString(CultureInfo.CurrentCulture);
            float num2 = Y;
            objArray[1] = num2.ToString(CultureInfo.CurrentCulture);
            float num3 = Z;
            objArray[2] = num3.ToString(CultureInfo.CurrentCulture);
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2}", objArray);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + (Y.GetHashCode() + Z.GetHashCode());
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool Equals(ref Vector3 value1, ref Vector3 value2)
        {
            return value1.X == (double)value2.X && value1.Y == (double)value2.Y && value1.Z == (double)value2.Z;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool Equals(Vector3 other)
        {
            return X == (double)other.X && Y == (double)other.Y && Z == (double)other.Z;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            else
                return Equals((Vector3)obj);
        }


        private static readonly Vector3 up;
        private static readonly Vector3 down;
        private static readonly Vector3 left;
        private static readonly Vector3 right;
        private static readonly Vector3 forward;
        private static readonly Vector3 backward;
        private static readonly Vector3 one;


        public static Vector3 Up
        {
            get { return up; }
        }

        public static Vector3 Down
        {
            get { return down; }
        }

        public static Vector3 Left
        {
            get { return left; }
        }

        public static Vector3 Right
        {
            get { return right; }
        }

        public static Vector3 Forward
        {
            get { return forward; }
        }

        public static Vector3 Backward
        {
            get { return backward; }
        }

        public static Vector3 One
        {
            get { return one; }
        }


        static Vector3()
        {
            up = new Vector3(0, 1, 0);
            down = new Vector3(0, -1, 0);
            left = new Vector3(-1, 0, 0);
            right = new Vector3(1, 0, 0);
            forward = new Vector3(0, 0, -1);
            backward = new Vector3(0, 0, 1);
            one = new Vector3(1, 1, 1);
        }
    }
}