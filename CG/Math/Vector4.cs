// Type: SlimDX.Vector4
// Assembly: SlimDX, Version=4.0.13.43, Culture=neutral, PublicKeyToken=b1b0c32fd1ffe4f9
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_32\SlimDX\v4.0_4.0.13.43__b1b0c32fd1ffe4f9\SlimDX.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SlimDX
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
            get
            {
                return Marshal.SizeOf(typeof(Vector4));
            }
        }

        public static Vector4 UnitW
        {
            get
            {
                return new Vector4(0.0f, 0.0f, 0.0f, 1f);
            }
        }

        public static Vector4 UnitZ
        {
            get
            {
                return new Vector4(0.0f, 0.0f, 1f, 0.0f);
            }
        }

        public static Vector4 UnitY
        {
            get
            {
                return new Vector4(0.0f, 1f, 0.0f, 0.0f);
            }
        }

        public static Vector4 UnitX
        {
            get
            {
                return new Vector4(1f, 0.0f, 0.0f, 0.0f);
            }
        }

        public static Vector4 Zero
        {
            get
            {
                return new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            }
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.X;
                    case 1:
                        return this.Y;
                    case 2:
                        return this.Z;
                    case 3:
                        return this.W;
                    default:
                        throw new ArgumentOutOfRangeException("index", "Indices for Vector4 run from 0 to 3, inclusive.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.X = value;
                        break;
                    case 1:
                        this.Y = value;
                        break;
                    case 2:
                        this.Z = value;
                        break;
                    case 3:
                        this.W = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("index", "Indices for Vector4 run from 0 to 3, inclusive.");
                }
            }
        }

        public Vector4(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Vector4(Vector3 value, float w)
        {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = value.Z;
            this.W = w;
        }

        public Vector4(Vector2 value, float z, float w)
        {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
            this.W = w;
        }

        public Vector4(float value)
        {
            this.X = value;
            this.Y = value;
            this.Z = value;
            this.W = value;
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
            return Vector4.Equals(ref left, ref right);
        }

        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return !Vector4.Equals(ref left, ref right);
        }

        public float Length()
        {
            double num1 = (double)this.Y;
            double num2 = (double)this.X;
            double num3 = (double)this.Z;
            double num4 = (double)this.W;
            double num5 = num2;
            double num6 = num5 * num5;
            double num7 = num1;
            double num8 = num7 * num7;
            double num9 = num6 + num8;
            double num10 = num3;
            double num11 = num10 * num10;
            double num12 = num9 + num11;
            double num13 = num4;
            double num14 = num13 * num13;
            return (float)Math.Sqrt(num12 + num14);
        }

        public float LengthSquared()
        {
            double num1 = (double)this.Y;
            double num2 = (double)this.X;
            double num3 = (double)this.Z;
            double num4 = (double)this.W;
            double num5 = num2;
            double num6 = num5 * num5;
            double num7 = num1;
            double num8 = num7 * num7;
            double num9 = num6 + num8;
            double num10 = num3;
            double num11 = num10 * num10;
            double num12 = num9 + num11;
            double num13 = num4;
            double num14 = num13 * num13;
            return (float)(num12 + num14);
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
            float num1 = this.Length();
            if ((double)num1 == 0.0)
                return;
            float num2 = 1f / num1;
            this.X *= num2;
            this.Y *= num2;
            this.Z *= num2;
            this.W *= num2;
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

        public static void Divide(ref Vector4 vector, float scale, out Vector4 result)
        {
            Vector4 vector4;
            vector4.X = vector.X / scale;
            vector4.Y = vector.Y / scale;
            vector4.Z = vector.Z / scale;
            vector4.W = vector.W / scale;
            result = vector4;
        }

        public static Vector4 Divide(Vector4 value, float scale)
        {
            Vector4 vector4;
            vector4.X = value.X / scale;
            vector4.Y = value.Y / scale;
            vector4.Z = value.Z / scale;
            vector4.W = value.W / scale;
            return vector4;
        }

        public static void Negate(ref Vector4 value, out Vector4 result)
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
            result = vector4;
        }

        public static Vector4 Negate(Vector4 value)
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

        public static void Barycentric(ref Vector4 value1, ref Vector4 value2, ref Vector4 value3, float amount1, float amount2, out Vector4 result)
        {
            Vector4 vector4;
            vector4.X = (float)(((double)value2.X - (double)value1.X) * (double)amount1 + (double)value1.X + ((double)value3.X - (double)value1.X) * (double)amount2);
            vector4.Y = (float)(((double)value2.Y - (double)value1.Y) * (double)amount1 + (double)value1.Y + ((double)value3.Y - (double)value1.Y) * (double)amount2);
            vector4.Z = (float)(((double)value2.Z - (double)value1.Z) * (double)amount1 + (double)value1.Z + ((double)value3.Z - (double)value1.Z) * (double)amount2);
            vector4.W = (float)(((double)value2.W - (double)value1.W) * (double)amount1 + (double)value1.W + ((double)value3.W - (double)value1.W) * (double)amount2);
            result = vector4;
        }

        public static Vector4 Barycentric(Vector4 value1, Vector4 value2, Vector4 value3, float amount1, float amount2)
        {
            return new Vector4()
            {
                X = (float)(((double)value2.X - (double)value1.X) * (double)amount1 + (double)value1.X + ((double)value3.X - (double)value1.X) * (double)amount2),
                Y = (float)(((double)value2.Y - (double)value1.Y) * (double)amount1 + (double)value1.Y + ((double)value3.Y - (double)value1.Y) * (double)amount2),
                Z = (float)(((double)value2.Z - (double)value1.Z) * (double)amount1 + (double)value1.Z + ((double)value3.Z - (double)value1.Z) * (double)amount2),
                W = (float)(((double)value2.W - (double)value1.W) * (double)amount1 + (double)value1.W + ((double)value3.W - (double)value1.W) * (double)amount2)
            };
        }

        public static void CatmullRom(ref Vector4 value1, ref Vector4 value2, ref Vector4 value3, ref Vector4 value4, float amount, out Vector4 result)
        {
            double num1 = (double)amount;
            float num2 = (float)(num1 * num1);
            float num3 = num2 * amount;
            result = new Vector4()
            {
                X = (float)((((double)value1.X * 2.0 - (double)value2.X * 5.0 + (double)value3.X * 4.0 - (double)value4.X) * (double)num2 + (((double)value3.X - (double)value1.X) * (double)amount + (double)value2.X * 2.0) + ((double)value2.X * 3.0 - (double)value1.X - (double)value3.X * 3.0 + (double)value4.X) * (double)num3) * 0.5),
                Y = (float)((((double)value1.Y * 2.0 - (double)value2.Y * 5.0 + (double)value3.Y * 4.0 - (double)value4.Y) * (double)num2 + (((double)value3.Y - (double)value1.Y) * (double)amount + (double)value2.Y * 2.0) + ((double)value2.Y * 3.0 - (double)value1.Y - (double)value3.Y * 3.0 + (double)value4.Y) * (double)num3) * 0.5),
                Z = (float)((((double)value1.Z * 2.0 - (double)value2.Z * 5.0 + (double)value3.Z * 4.0 - (double)value4.Z) * (double)num2 + (((double)value3.Z - (double)value1.Z) * (double)amount + (double)value2.Z * 2.0) + ((double)value2.Z * 3.0 - (double)value1.Z - (double)value3.Z * 3.0 + (double)value4.Z) * (double)num3) * 0.5),
                W = (float)((((double)value1.W * 2.0 - (double)value2.W * 5.0 + (double)value3.W * 4.0 - (double)value4.W) * (double)num2 + (((double)value3.W - (double)value1.W) * (double)amount + (double)value2.W * 2.0) + ((double)value2.W * 3.0 - (double)value1.W - (double)value3.W * 3.0 + (double)value4.W) * (double)num3) * 0.5)
            };
        }

        public static Vector4 CatmullRom(Vector4 value1, Vector4 value2, Vector4 value3, Vector4 value4, float amount)
        {
            Vector4 vector4 = new Vector4();
            double num1 = (double)amount;
            float num2 = (float)(num1 * num1);
            float num3 = num2 * amount;
            vector4.X = (float)((((double)value1.X * 2.0 - (double)value2.X * 5.0 + (double)value3.X * 4.0 - (double)value4.X) * (double)num2 + (((double)value3.X - (double)value1.X) * (double)amount + (double)value2.X * 2.0) + ((double)value2.X * 3.0 - (double)value1.X - (double)value3.X * 3.0 + (double)value4.X) * (double)num3) * 0.5);
            vector4.Y = (float)((((double)value1.Y * 2.0 - (double)value2.Y * 5.0 + (double)value3.Y * 4.0 - (double)value4.Y) * (double)num2 + (((double)value3.Y - (double)value1.Y) * (double)amount + (double)value2.Y * 2.0) + ((double)value2.Y * 3.0 - (double)value1.Y - (double)value3.Y * 3.0 + (double)value4.Y) * (double)num3) * 0.5);
            vector4.Z = (float)((((double)value1.Z * 2.0 - (double)value2.Z * 5.0 + (double)value3.Z * 4.0 - (double)value4.Z) * (double)num2 + (((double)value3.Z - (double)value1.Z) * (double)amount + (double)value2.Z * 2.0) + ((double)value2.Z * 3.0 - (double)value1.Z - (double)value3.Z * 3.0 + (double)value4.Z) * (double)num3) * 0.5);
            vector4.W = (float)((((double)value1.W * 2.0 - (double)value2.W * 5.0 + (double)value3.W * 4.0 - (double)value4.W) * (double)num2 + (((double)value3.W - (double)value1.W) * (double)amount + (double)value2.W * 2.0) + ((double)value2.W * 3.0 - (double)value1.W - (double)value3.W * 3.0 + (double)value4.W) * (double)num3) * 0.5);
            return vector4;
        }

        public static void Clamp(ref Vector4 value, ref Vector4 min, ref Vector4 max, out Vector4 result)
        {
            float num1 = value.X;
            float num2 = (double)num1 <= (double)max.X ? num1 : max.X;
            float num3 = (double)num2 >= (double)min.X ? num2 : min.X;
            float num4 = value.Y;
            float num5 = (double)num4 <= (double)max.Y ? num4 : max.Y;
            float num6 = (double)num5 >= (double)min.Y ? num5 : min.Y;
            float num7 = value.Z;
            float num8 = (double)num7 <= (double)max.Z ? num7 : max.Z;
            float num9 = (double)num8 >= (double)min.Z ? num8 : min.Z;
            float num10 = value.W;
            float num11 = (double)num10 <= (double)max.W ? num10 : max.W;
            float num12 = (double)num11 >= (double)min.W ? num11 : min.W;
            Vector4 vector4;
            vector4.X = num3;
            vector4.Y = num6;
            vector4.Z = num9;
            vector4.W = num12;
            result = vector4;
        }

        public static Vector4 Clamp(Vector4 value, Vector4 min, Vector4 max)
        {
            float num1 = value.X;
            float num2 = (double)num1 <= (double)max.X ? num1 : max.X;
            float num3 = (double)num2 >= (double)min.X ? num2 : min.X;
            float num4 = value.Y;
            float num5 = (double)num4 <= (double)max.Y ? num4 : max.Y;
            float num6 = (double)num5 >= (double)min.Y ? num5 : min.Y;
            float num7 = value.Z;
            float num8 = (double)num7 <= (double)max.Z ? num7 : max.Z;
            float num9 = (double)num8 >= (double)min.Z ? num8 : min.Z;
            float num10 = value.W;
            float num11 = (double)num10 <= (double)max.W ? num10 : max.W;
            float num12 = (double)num11 >= (double)min.W ? num11 : min.W;
            Vector4 vector4;
            vector4.X = num3;
            vector4.Y = num6;
            vector4.Z = num9;
            vector4.W = num12;
            return vector4;
        }

        public static void Hermite(ref Vector4 value1, ref Vector4 tangent1, ref Vector4 value2, ref Vector4 tangent2, float amount, out Vector4 result)
        {
            double num1 = (double)amount;
            float num2 = (float)(num1 * num1);
            float num3 = num2 * amount;
            double num4 = (double)num2 * 3.0;
            float num5 = (float)((double)num3 * 2.0 - num4 + 1.0);
            float num6 = (float)((double)num3 * -2.0 + num4);
            float num7 = num3 - num2 * 2f + amount;
            float num8 = num3 - num2;
            result.X = (float)((double)value2.X * (double)num6 + (double)value1.X * (double)num5 + (double)tangent1.X * (double)num7 + (double)tangent2.X * (double)num8);
            result.Y = (float)((double)value2.Y * (double)num6 + (double)value1.Y * (double)num5 + (double)tangent1.Y * (double)num7 + (double)tangent2.Y * (double)num8);
            result.Z = (float)((double)value2.Z * (double)num6 + (double)value1.Z * (double)num5 + (double)tangent1.Z * (double)num7 + (double)tangent2.Z * (double)num8);
            result.W = (float)((double)value2.W * (double)num6 + (double)value1.W * (double)num5 + (double)tangent1.W * (double)num7 + (double)tangent2.W * (double)num8);
        }

        public static Vector4 Hermite(Vector4 value1, Vector4 tangent1, Vector4 value2, Vector4 tangent2, float amount)
        {
            Vector4 vector4 = new Vector4();
            double num1 = (double)amount;
            float num2 = (float)(num1 * num1);
            float num3 = num2 * amount;
            double num4 = (double)num2 * 3.0;
            float num5 = (float)((double)num3 * 2.0 - num4 + 1.0);
            float num6 = (float)((double)num3 * -2.0 + num4);
            float num7 = num3 - num2 * 2f + amount;
            float num8 = num3 - num2;
            vector4.X = (float)((double)value2.X * (double)num6 + (double)value1.X * (double)num5 + (double)tangent1.X * (double)num7 + (double)tangent2.X * (double)num8);
            vector4.Y = (float)((double)value2.Y * (double)num6 + (double)value1.Y * (double)num5 + (double)tangent1.Y * (double)num7 + (double)tangent2.Y * (double)num8);
            vector4.Z = (float)((double)value2.Z * (double)num6 + (double)value1.Z * (double)num5 + (double)tangent1.Z * (double)num7 + (double)tangent2.Z * (double)num8);
            vector4.W = (float)((double)value2.W * (double)num6 + (double)value1.W * (double)num5 + (double)tangent1.W * (double)num7 + (double)tangent2.W * (double)num8);
            return vector4;
        }

        public static void Lerp(ref Vector4 start, ref Vector4 end, float amount, out Vector4 result)
        {
            result.X = (end.X - start.X) * amount + start.X;
            result.Y = (end.Y - start.Y) * amount + start.Y;
            result.Z = (end.Z - start.Z) * amount + start.Z;
            result.W = (end.W - start.W) * amount + start.W;
        }

        public static Vector4 Lerp(Vector4 start, Vector4 end, float amount)
        {
            return new Vector4()
            {
                X = (end.X - start.X) * amount + start.X,
                Y = (end.Y - start.Y) * amount + start.Y,
                Z = (end.Z - start.Z) * amount + start.Z,
                W = (end.W - start.W) * amount + start.W
            };
        }

        public static void SmoothStep(ref Vector4 start, ref Vector4 end, float amount, out Vector4 result)
        {
            float num1 = (double)amount <= 1.0 ? ((double)amount >= 0.0 ? amount : 0.0f) : 1f;
            double num2 = (double)num1;
            double num3 = 3.0 - (double)num1 * 2.0;
            double num4 = num2;
            double num5 = num4 * num4;
            amount = (float)(num3 * num5);
            result.X = (end.X - start.X) * amount + start.X;
            result.Y = (end.Y - start.Y) * amount + start.Y;
            result.Z = (end.Z - start.Z) * amount + start.Z;
            result.W = (end.W - start.W) * amount + start.W;
        }

        public static Vector4 SmoothStep(Vector4 start, Vector4 end, float amount)
        {
            Vector4 vector4 = new Vector4();
            float num1 = (double)amount <= 1.0 ? ((double)amount >= 0.0 ? amount : 0.0f) : 1f;
            double num2 = (double)num1;
            double num3 = 3.0 - (double)num1 * 2.0;
            double num4 = num2;
            double num5 = num4 * num4;
            amount = (float)(num3 * num5);
            vector4.X = (end.X - start.X) * amount + start.X;
            vector4.Y = (end.Y - start.Y) * amount + start.Y;
            vector4.Z = (end.Z - start.Z) * amount + start.Z;
            vector4.W = (end.W - start.W) * amount + start.W;
            return vector4;
        }

        public static float Distance(Vector4 value1, Vector4 value2)
        {
            float num1 = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            float num4 = value1.W - value2.W;
            double num5 = (double)num2;
            double num6 = (double)num1;
            double num7 = (double)num3;
            double num8 = (double)num4;
            double num9 = num6;
            double num10 = num9 * num9;
            double num11 = num5;
            double num12 = num11 * num11;
            double num13 = num10 + num12;
            double num14 = num7;
            double num15 = num14 * num14;
            double num16 = num13 + num15;
            double num17 = num8;
            double num18 = num17 * num17;
            return (float)Math.Sqrt(num16 + num18);
        }

        public static float DistanceSquared(Vector4 value1, Vector4 value2)
        {
            float num1 = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            float num4 = value1.W - value2.W;
            double num5 = (double)num2;
            double num6 = (double)num1;
            double num7 = (double)num3;
            double num8 = (double)num4;
            double num9 = num6;
            double num10 = num9 * num9;
            double num11 = num5;
            double num12 = num11 * num11;
            double num13 = num10 + num12;
            double num14 = num7;
            double num15 = num14 * num14;
            double num16 = num13 + num15;
            double num17 = num8;
            double num18 = num17 * num17;
            return (float)(num16 + num18);
        }

        public static float Dot(Vector4 left, Vector4 right)
        {
            return (float)((double)left.Y * (double)right.Y + (double)left.X * (double)right.X + (double)left.Z * (double)right.Z + (double)left.W * (double)right.W);
        }


        public static Vector4[] Transform(Vector4[] vectors, ref Matrix transformation)
        {
            if (vectors == null)
                throw new ArgumentNullException("vectors");
            int length = vectors.Length;
            Vector4[] vector4Array = new Vector4[length];
            int index = 0;
            if (0 < length)
            {
                do
                {
                    vector4Array[index] = new Vector4()
                    {
                        X = (float)((double)vectors[index].Y * (double)transformation.M21 + (double)vectors[index].X * (double)transformation.M11 + (double)vectors[index].Z * (double)transformation.M31 + (double)vectors[index].W * (double)transformation.M41),
                        Y = (float)((double)vectors[index].Y * (double)transformation.M22 + (double)vectors[index].X * (double)transformation.M12 + (double)vectors[index].Z * (double)transformation.M32 + (double)vectors[index].W * (double)transformation.M42),
                        Z = (float)((double)vectors[index].Y * (double)transformation.M23 + (double)vectors[index].X * (double)transformation.M13 + (double)vectors[index].Z * (double)transformation.M33 + (double)vectors[index].W * (double)transformation.M43),
                        W = (float)((double)vectors[index].Y * (double)transformation.M24 + (double)vectors[index].X * (double)transformation.M14 + (double)vectors[index].Z * (double)transformation.M34 + (double)vectors[index].W * (double)transformation.M44)
                    };
                    ++index;
                }
                while (index < length);
            }
            return vector4Array;
        }

        public static void Transform(ref Vector4 vector, ref Matrix transformation, out Vector4 result)
        {
            result = new Vector4()
            {
                X = (float)((double)vector.Y * (double)transformation.M21 + (double)vector.X * (double)transformation.M11 + (double)vector.Z * (double)transformation.M31 + (double)vector.W * (double)transformation.M41),
                Y = (float)((double)vector.Y * (double)transformation.M22 + (double)vector.X * (double)transformation.M12 + (double)vector.Z * (double)transformation.M32 + (double)vector.W * (double)transformation.M42),
                Z = (float)((double)vector.Y * (double)transformation.M23 + (double)vector.X * (double)transformation.M13 + (double)vector.Z * (double)transformation.M33 + (double)vector.W * (double)transformation.M43),
                W = (float)((double)vector.Y * (double)transformation.M24 + (double)vector.X * (double)transformation.M14 + (double)vector.Z * (double)transformation.M34 + (double)vector.W * (double)transformation.M44)
            };
        }

        public static Vector4 Transform(Vector4 vector, Matrix transformation)
        {
            return new Vector4()
            {
                X = (float)((double)transformation.M21 * (double)vector.Y + (double)transformation.M11 * (double)vector.X + (double)transformation.M31 * (double)vector.Z + (double)transformation.M41 * (double)vector.W),
                Y = (float)((double)transformation.M22 * (double)vector.Y + (double)transformation.M12 * (double)vector.X + (double)transformation.M32 * (double)vector.Z + (double)transformation.M42 * (double)vector.W),
                Z = (float)((double)transformation.M23 * (double)vector.Y + (double)transformation.M13 * (double)vector.X + (double)transformation.M33 * (double)vector.Z + (double)transformation.M43 * (double)vector.W),
                W = (float)((double)transformation.M24 * (double)vector.Y + (double)transformation.M14 * (double)vector.X + (double)transformation.M34 * (double)vector.Z + (double)transformation.M44 * (double)vector.W)
            };
        }

        public static void Minimize(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            float num1 = (double)value1.X >= (double)value2.X ? value2.X : value1.X;
            result.X = num1;
            float num2 = (double)value1.Y >= (double)value2.Y ? value2.Y : value1.Y;
            result.Y = num2;
            float num3 = (double)value1.Z >= (double)value2.Z ? value2.Z : value1.Z;
            result.Z = num3;
            float num4 = (double)value1.W >= (double)value2.W ? value2.W : value1.W;
            result.W = num4;
        }

        public static Vector4 Minimize(Vector4 value1, Vector4 value2)
        {
            Vector4 vector4 = new Vector4();
            float num1 = (double)value1.X >= (double)value2.X ? value2.X : value1.X;
            vector4.X = num1;
            float num2 = (double)value1.Y >= (double)value2.Y ? value2.Y : value1.Y;
            vector4.Y = num2;
            float num3 = (double)value1.Z >= (double)value2.Z ? value2.Z : value1.Z;
            vector4.Z = num3;
            float num4 = (double)value1.W >= (double)value2.W ? value2.W : value1.W;
            vector4.W = num4;
            return vector4;
        }

        public static void Maximize(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            float num1 = (double)value1.X <= (double)value2.X ? value2.X : value1.X;
            result.X = num1;
            float num2 = (double)value1.Y <= (double)value2.Y ? value2.Y : value1.Y;
            result.Y = num2;
            float num3 = (double)value1.Z <= (double)value2.Z ? value2.Z : value1.Z;
            result.Z = num3;
            float num4 = (double)value1.W <= (double)value2.W ? value2.W : value1.W;
            result.W = num4;
        }

        public static Vector4 Maximize(Vector4 value1, Vector4 value2)
        {
            Vector4 vector4 = new Vector4();
            float num1 = (double)value1.X <= (double)value2.X ? value2.X : value1.X;
            vector4.X = num1;
            float num2 = (double)value1.Y <= (double)value2.Y ? value2.Y : value1.Y;
            vector4.Y = num2;
            float num3 = (double)value1.Z <= (double)value2.Z ? value2.Z : value1.Z;
            vector4.Z = num3;
            float num4 = (double)value1.W <= (double)value2.W ? value2.W : value1.W;
            vector4.W = num4;
            return vector4;
        }

        public override  string ToString()
        {
            object[] objArray = new object[4];
            float num1 = this.X;
            objArray[0] = (object)num1.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num2 = this.Y;
            objArray[1] = (object)num2.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num3 = this.Z;
            objArray[2] = (object)num3.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num4 = this.W;
            objArray[3] = (object)num4.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", objArray);
        }

        public override  int GetHashCode()
        {
            return this.X.GetHashCode() + (this.Z.GetHashCode() + this.W.GetHashCode() + this.Y.GetHashCode());
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool Equals(ref Vector4 value1, ref Vector4 value2)
        {
            return (double)value1.X == (double)value2.X && (double)value1.Y == (double)value2.Y && ((double)value1.Z == (double)value2.Z && (double)value1.W == (double)value2.W);
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool Equals(Vector4 other)
        {
            return (double)this.X == (double)other.X && (double)this.Y == (double)other.Y && ((double)this.Z == (double)other.Z && (double)this.W == (double)other.W);
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override  bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;
            else
                return this.Equals((Vector4)obj);
        }
    }
}
