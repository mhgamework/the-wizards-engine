// Type: SlimDX.Vector2
// Assembly: SlimDX, Version=4.0.13.43, Culture=neutral, PublicKeyToken=b1b0c32fd1ffe4f9
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_32\SlimDX\v4.0_4.0.13.43__b1b0c32fd1ffe4f9\SlimDX.dll

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MHGameWork.TheWizards.CG.Math
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector2 : IEquatable<Vector2>
    {
        public float X;
        public float Y;

        public static int SizeInBytes
        {
            get { return Marshal.SizeOf(typeof(Vector2)); }
        }

        public static Vector2 UnitY
        {
            get { return new Vector2(0.0f, 1f); }
        }

        public static Vector2 UnitX
        {
            get { return new Vector2(1f, 0.0f); }
        }

        public static Vector2 Zero
        {
            get { return new Vector2(0.0f, 0.0f); }
        }

        public float this[int index]
        {
            get
            {
                if (index == 0)
                    return X;
                if (index != 1)
                    throw new ArgumentOutOfRangeException("index", "Indices for Vector2 run from 0 to 1, inclusive.");
                else
                    return Y;
            }
            set
            {
                if (index != 0)
                {
                    if (index != 1)
                        throw new ArgumentOutOfRangeException("index", "Indices for Vector2 run from 0 to 1, inclusive.");
                    Y = value;
                }
                else
                    X = value;
            }
        }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2(float value)
        {
            X = value;
            Y = value;
        }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            Vector2 vector2;
            vector2.X = left.X + right.X;
            vector2.Y = left.Y + right.Y;
            return vector2;
        }

        public static Vector2 operator -(Vector2 value)
        {
            float num1 = -value.X;
            float num2 = -value.Y;
            Vector2 vector2;
            vector2.X = num1;
            vector2.Y = num2;
            return vector2;
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            Vector2 vector2;
            vector2.X = left.X - right.X;
            vector2.Y = left.Y - right.Y;
            return vector2;
        }

        public static Vector2 operator *(float scale, Vector2 vector)
        {
            Vector2 vector2_1 = vector;
            Vector2 vector2_2;
            vector2_2.X = vector2_1.X * scale;
            vector2_2.Y = vector2_1.Y * scale;
            return vector2_2;
        }

        public static Vector2 operator *(Vector2 vector, float scale)
        {
            Vector2 vector2;
            vector2.X = vector.X * scale;
            vector2.Y = vector.Y * scale;
            return vector2;
        }

        public static Vector2 operator /(Vector2 vector, float scale)
        {
            Vector2 vector2;
            vector2.X = vector.X / scale;
            vector2.Y = vector.Y / scale;
            return vector2;
        }

        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.X == (double)right.X && left.Y == (double)right.Y;
        }

        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return (left.X != (double)right.X || left.Y != (double)right.Y ? 0 : 1) == 0;
        }

        public float Length()
        {
            double num1 = Y;
            double num2 = X;
            double num3 = num2 * num2;
            double num4 = num1;
            double num5 = num4 * num4;
            return (float)System.Math.Sqrt(num3 + num5);
        }

        public float LengthSquared()
        {
            double num1 = Y;
            double num2 = X;
            double num3 = num2 * num2;
            double num4 = num1;
            double num5 = num4 * num4;
            return (float)(num3 + num5);
        }

        public static void Normalize(ref Vector2 vector, out Vector2 result)
        {
            Vector2 vector2_1 = vector;
            vector2_1.Normalize();
            Vector2 vector2_2 = vector2_1;
            result = vector2_2;
        }

        public static Vector2 Normalize(Vector2 vector)
        {
            vector.Normalize();
            return vector;
        }

        public void Normalize()
        {
            double num1 = Y;
            double num2 = X;
            double num3 = num2 * num2;
            double num4 = num1;
            double num5 = num4 * num4;
            var num6 = (float)System.Math.Sqrt(num3 + num5);
            if (num6 == 0.0)
                return;
            float num7 = 1f / num6;
            X *= num7;
            Y *= num7;
        }

        public static void Add(ref Vector2 left, ref Vector2 right, out Vector2 result)
        {
            Vector2 vector2;
            vector2.X = left.X + right.X;
            vector2.Y = left.Y + right.Y;
            result = vector2;
        }

        public static Vector2 Add(Vector2 left, Vector2 right)
        {
            Vector2 vector2;
            vector2.X = left.X + right.X;
            vector2.Y = left.Y + right.Y;
            return vector2;
        }

        public static void Subtract(ref Vector2 left, ref Vector2 right, out Vector2 result)
        {
            Vector2 vector2;
            vector2.X = left.X - right.X;
            vector2.Y = left.Y - right.Y;
            result = vector2;
        }

        public static Vector2 Subtract(Vector2 left, Vector2 right)
        {
            Vector2 vector2;
            vector2.X = left.X - right.X;
            vector2.Y = left.Y - right.Y;
            return vector2;
        }

        public static void Multiply(ref Vector2 vector, float scale, out Vector2 result)
        {
            Vector2 vector2;
            vector2.X = vector.X * scale;
            vector2.Y = vector.Y * scale;
            result = vector2;
        }

        public static Vector2 Multiply(Vector2 value, float scale)
        {
            Vector2 vector2;
            vector2.X = value.X * scale;
            vector2.Y = value.Y * scale;
            return vector2;
        }

        public static void Modulate(ref Vector2 left, ref Vector2 right, out Vector2 result)
        {
            Vector2 vector2;
            vector2.X = left.X * right.X;
            vector2.Y = left.Y * right.Y;
            result = vector2;
        }

        public static Vector2 Modulate(Vector2 left, Vector2 right)
        {
            Vector2 vector2;
            vector2.X = left.X * right.X;
            vector2.Y = left.Y * right.Y;
            return vector2;
        }

        public static void Divide(ref Vector2 vector, float scale, out Vector2 result)
        {
            Vector2 vector2;
            vector2.X = vector.X / scale;
            vector2.Y = vector.Y / scale;
            result = vector2;
        }

        public static Vector2 Divide(Vector2 value, float scale)
        {
            Vector2 vector2;
            vector2.X = value.X / scale;
            vector2.Y = value.Y / scale;
            return vector2;
        }

        public static void Negate(ref Vector2 value, out Vector2 result)
        {
            float num1 = -value.X;
            float num2 = -value.Y;
            Vector2 vector2;
            vector2.X = num1;
            vector2.Y = num2;
            result = vector2;
        }

        public static Vector2 Negate(Vector2 value)
        {
            float num1 = -value.X;
            float num2 = -value.Y;
            Vector2 vector2;
            vector2.X = num1;
            vector2.Y = num2;
            return vector2;
        }

     
        public static float Dot(Vector2 left, Vector2 right)
        {
            return (float)(left.Y * (double)right.Y + left.X * (double)right.X);
        }

     

      
        public override string ToString()
        {
            var objArray = new object[2];
            float num1 = X;
            objArray[0] = num1.ToString(CultureInfo.CurrentCulture);
            float num2 = Y;
            objArray[1] = num2.ToString(CultureInfo.CurrentCulture);
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1}", objArray);
        }

        public override int GetHashCode()
        {
            return Y.GetHashCode() + X.GetHashCode();
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool Equals(ref Vector2 value1, ref Vector2 value2)
        {
            return value1.X == (double)value2.X && value1.Y == (double)value2.Y;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool Equals(Vector2 other)
        {
            return X == (double)other.X && Y == (double)other.Y;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            else
                return Equals((Vector2)obj);
        }
    }
}