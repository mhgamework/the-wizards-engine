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

        public static int SizeInBytes
        {
            get
            {
                return Marshal.SizeOf(typeof(Vector3));
            }
        }

        public static Vector3 UnitZ
        {
            get
            {
                return new Vector3(0.0f, 0.0f, 1f);
            }
        }

        public static Vector3 UnitY
        {
            get
            {
                return new Vector3(0.0f, 1f, 0.0f);
            }
        }

        public static Vector3 UnitX
        {
            get
            {
                return new Vector3(1f, 0.0f, 0.0f);
            }
        }

        public static Vector3 Zero
        {
            get
            {
                return new Vector3(0.0f, 0.0f, 0.0f);
            }
        }

        public float this[int index]
        {
            get
            {
                if (index == 0)
                    return this.X;
                if (index == 1)
                    return this.Y;
                if (index != 2)
                    throw new ArgumentOutOfRangeException("index", "Indices for Vector3 run from 0 to 2, inclusive.");
                else
                    return this.Z;
            }
            set
            {
                if (index != 0)
                {
                    if (index != 1)
                    {
                        if (index != 2)
                            throw new ArgumentOutOfRangeException("index", "Indices for Vector3 run from 0 to 2, inclusive.");
                        this.Z = value;
                    }
                    else
                        this.Y = value;
                }
                else
                    this.X = value;
            }
        }

        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3(Vector2 value, float z)
        {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
        }

        public Vector3(float value)
        {
            this.X = value;
            this.Y = value;
            this.Z = value;
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
            float num1 = -value.X;
            float num2 = -value.Y;
            float num3 = -value.Z;
            Vector3 vector3;
            vector3.X = num1;
            vector3.Y = num2;
            vector3.Z = num3;
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
            Vector3 vector3;
            vector3.X = vector.X / scale;
            vector3.Y = vector.Y / scale;
            vector3.Z = vector.Z / scale;
            return vector3;
        }

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return Vector3.Equals(ref left, ref right);
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !Vector3.Equals(ref left, ref right);
        }

        public float Length()
        {
            double num1 = (double)this.Y;
            double num2 = (double)this.X;
            double num3 = (double)this.Z;
            double num4 = num2;
            double num5 = num4 * num4;
            double num6 = num1;
            double num7 = num6 * num6;
            double num8 = num5 + num7;
            double num9 = num3;
            double num10 = num9 * num9;
            return (float)System.Math.Sqrt(num8 + num10);
        }

        public float LengthSquared()
        {
            double num1 = (double)this.Y;
            double num2 = (double)this.X;
            double num3 = (double)this.Z;
            double num4 = num2;
            double num5 = num4 * num4;
            double num6 = num1;
            double num7 = num6 * num6;
            double num8 = num5 + num7;
            double num9 = num3;
            double num10 = num9 * num9;
            return (float)(num8 + num10);
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
            float num1 = this.Length();
            if ((double)num1 == 0.0)
                return;
            float num2 = 1f / num1;
            this.X *= num2;
            this.Y *= num2;
            this.Z *= num2;
        }

        public static void Add(ref Vector3 left, ref Vector3 right, out Vector3 result)
        {
            Vector3 vector3;
            vector3.X = left.X + right.X;
            vector3.Y = left.Y + right.Y;
            vector3.Z = left.Z + right.Z;
            result = vector3;
        }

        public static Vector3 Add(Vector3 left, Vector3 right)
        {
            Vector3 vector3;
            vector3.X = left.X + right.X;
            vector3.Y = left.Y + right.Y;
            vector3.Z = left.Z + right.Z;
            return vector3;
        }

        public static void Subtract(ref Vector3 left, ref Vector3 right, out Vector3 result)
        {
            Vector3 vector3;
            vector3.X = left.X - right.X;
            vector3.Y = left.Y - right.Y;
            vector3.Z = left.Z - right.Z;
            result = vector3;
        }

        public static Vector3 Subtract(Vector3 left, Vector3 right)
        {
            Vector3 vector3;
            vector3.X = left.X - right.X;
            vector3.Y = left.Y - right.Y;
            vector3.Z = left.Z - right.Z;
            return vector3;
        }

        public static void Multiply(ref Vector3 vector, float scale, out Vector3 result)
        {
            Vector3 vector3;
            vector3.X = vector.X * scale;
            vector3.Y = vector.Y * scale;
            vector3.Z = vector.Z * scale;
            result = vector3;
        }

        public static Vector3 Multiply(Vector3 value, float scale)
        {
            Vector3 vector3;
            vector3.X = value.X * scale;
            vector3.Y = value.Y * scale;
            vector3.Z = value.Z * scale;
            return vector3;
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

        public static void Divide(ref Vector3 vector, float scale, out Vector3 result)
        {
            Vector3 vector3;
            vector3.X = vector.X / scale;
            vector3.Y = vector.Y / scale;
            vector3.Z = vector.Z / scale;
            result = vector3;
        }

        public static Vector3 Divide(Vector3 value, float scale)
        {
            Vector3 vector3;
            vector3.X = value.X / scale;
            vector3.Y = value.Y / scale;
            vector3.Z = value.Z / scale;
            return vector3;
        }

        public static void Negate(ref Vector3 value, out Vector3 result)
        {
            float num1 = -value.X;
            float num2 = -value.Y;
            float num3 = -value.Z;
            Vector3 vector3;
            vector3.X = num1;
            vector3.Y = num2;
            vector3.Z = num3;
            result = vector3;
        }

        public static Vector3 Negate(Vector3 value)
        {
            float num1 = -value.X;
            float num2 = -value.Y;
            float num3 = -value.Z;
            Vector3 vector3;
            vector3.X = num1;
            vector3.Y = num2;
            vector3.Z = num3;
            return vector3;
        }

        public static void Barycentric(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, float amount1, float amount2, out Vector3 result)
        {
            Vector3 vector3;
            vector3.X = (float)(((double)value2.X - (double)value1.X) * (double)amount1 + (double)value1.X + ((double)value3.X - (double)value1.X) * (double)amount2);
            vector3.Y = (float)(((double)value2.Y - (double)value1.Y) * (double)amount1 + (double)value1.Y + ((double)value3.Y - (double)value1.Y) * (double)amount2);
            vector3.Z = (float)(((double)value2.Z - (double)value1.Z) * (double)amount1 + (double)value1.Z + ((double)value3.Z - (double)value1.Z) * (double)amount2);
            result = vector3;
        }

        public static Vector3 Barycentric(Vector3 value1, Vector3 value2, Vector3 value3, float amount1, float amount2)
        {
            return new Vector3()
            {
                X = (float)(((double)value2.X - (double)value1.X) * (double)amount1 + (double)value1.X + ((double)value3.X - (double)value1.X) * (double)amount2),
                Y = (float)(((double)value2.Y - (double)value1.Y) * (double)amount1 + (double)value1.Y + ((double)value3.Y - (double)value1.Y) * (double)amount2),
                Z = (float)(((double)value2.Z - (double)value1.Z) * (double)amount1 + (double)value1.Z + ((double)value3.Z - (double)value1.Z) * (double)amount2)
            };
        }

        public static void CatmullRom(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, ref Vector3 value4, float amount, out Vector3 result)
        {
            double num1 = (double)amount;
            float num2 = (float)(num1 * num1);
            float num3 = num2 * amount;
            result = new Vector3()
            {
                X = (float)((((double)value1.X * 2.0 - (double)value2.X * 5.0 + (double)value3.X * 4.0 - (double)value4.X) * (double)num2 + (((double)value3.X - (double)value1.X) * (double)amount + (double)value2.X * 2.0) + ((double)value2.X * 3.0 - (double)value1.X - (double)value3.X * 3.0 + (double)value4.X) * (double)num3) * 0.5),
                Y = (float)((((double)value1.Y * 2.0 - (double)value2.Y * 5.0 + (double)value3.Y * 4.0 - (double)value4.Y) * (double)num2 + (((double)value3.Y - (double)value1.Y) * (double)amount + (double)value2.Y * 2.0) + ((double)value2.Y * 3.0 - (double)value1.Y - (double)value3.Y * 3.0 + (double)value4.Y) * (double)num3) * 0.5),
                Z = (float)((((double)value1.Z * 2.0 - (double)value2.Z * 5.0 + (double)value3.Z * 4.0 - (double)value4.Z) * (double)num2 + (((double)value3.Z - (double)value1.Z) * (double)amount + (double)value2.Z * 2.0) + ((double)value2.Z * 3.0 - (double)value1.Z - (double)value3.Z * 3.0 + (double)value4.Z) * (double)num3) * 0.5)
            };
        }

        public static Vector3 CatmullRom(Vector3 value1, Vector3 value2, Vector3 value3, Vector3 value4, float amount)
        {
            Vector3 vector3 = new Vector3();
            double num1 = (double)amount;
            float num2 = (float)(num1 * num1);
            float num3 = num2 * amount;
            vector3.X = (float)((((double)value1.X * 2.0 - (double)value2.X * 5.0 + (double)value3.X * 4.0 - (double)value4.X) * (double)num2 + (((double)value3.X - (double)value1.X) * (double)amount + (double)value2.X * 2.0) + ((double)value2.X * 3.0 - (double)value1.X - (double)value3.X * 3.0 + (double)value4.X) * (double)num3) * 0.5);
            vector3.Y = (float)((((double)value1.Y * 2.0 - (double)value2.Y * 5.0 + (double)value3.Y * 4.0 - (double)value4.Y) * (double)num2 + (((double)value3.Y - (double)value1.Y) * (double)amount + (double)value2.Y * 2.0) + ((double)value2.Y * 3.0 - (double)value1.Y - (double)value3.Y * 3.0 + (double)value4.Y) * (double)num3) * 0.5);
            vector3.Z = (float)((((double)value1.Z * 2.0 - (double)value2.Z * 5.0 + (double)value3.Z * 4.0 - (double)value4.Z) * (double)num2 + (((double)value3.Z - (double)value1.Z) * (double)amount + (double)value2.Z * 2.0) + ((double)value2.Z * 3.0 - (double)value1.Z - (double)value3.Z * 3.0 + (double)value4.Z) * (double)num3) * 0.5);
            return vector3;
        }

        public static void Clamp(ref Vector3 value, ref Vector3 min, ref Vector3 max, out Vector3 result)
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
            Vector3 vector3;
            vector3.X = num3;
            vector3.Y = num6;
            vector3.Z = num9;
            result = vector3;
        }

        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
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
            Vector3 vector3;
            vector3.X = num3;
            vector3.Y = num6;
            vector3.Z = num9;
            return vector3;
        }

        public static void Hermite(ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2, float amount, out Vector3 result)
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
        }

        public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float amount)
        {
            Vector3 vector3 = new Vector3();
            double num1 = (double)amount;
            float num2 = (float)(num1 * num1);
            float num3 = num2 * amount;
            double num4 = (double)num2 * 3.0;
            float num5 = (float)((double)num3 * 2.0 - num4 + 1.0);
            float num6 = (float)((double)num3 * -2.0 + num4);
            float num7 = num3 - num2 * 2f + amount;
            float num8 = num3 - num2;
            vector3.X = (float)((double)value2.X * (double)num6 + (double)value1.X * (double)num5 + (double)tangent1.X * (double)num7 + (double)tangent2.X * (double)num8);
            vector3.Y = (float)((double)value2.Y * (double)num6 + (double)value1.Y * (double)num5 + (double)tangent1.Y * (double)num7 + (double)tangent2.Y * (double)num8);
            vector3.Z = (float)((double)value2.Z * (double)num6 + (double)value1.Z * (double)num5 + (double)tangent1.Z * (double)num7 + (double)tangent2.Z * (double)num8);
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
            return new Vector3()
            {
                X = (end.X - start.X) * amount + start.X,
                Y = (end.Y - start.Y) * amount + start.Y,
                Z = (end.Z - start.Z) * amount + start.Z
            };
        }

        public static void SmoothStep(ref Vector3 start, ref Vector3 end, float amount, out Vector3 result)
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
        }

        public static Vector3 SmoothStep(Vector3 start, Vector3 end, float amount)
        {
            Vector3 vector3 = new Vector3();
            float num1 = (double)amount <= 1.0 ? ((double)amount >= 0.0 ? amount : 0.0f) : 1f;
            double num2 = (double)num1;
            double num3 = 3.0 - (double)num1 * 2.0;
            double num4 = num2;
            double num5 = num4 * num4;
            amount = (float)(num3 * num5);
            vector3.X = (end.X - start.X) * amount + start.X;
            vector3.Y = (end.Y - start.Y) * amount + start.Y;
            vector3.Z = (end.Z - start.Z) * amount + start.Z;
            return vector3;
        }

        public static float Distance(Vector3 value1, Vector3 value2)
        {
            float num1 = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            double num4 = (double)num2;
            double num5 = (double)num1;
            double num6 = (double)num3;
            double num7 = num5;
            double num8 = num7 * num7;
            double num9 = num4;
            double num10 = num9 * num9;
            double num11 = num8 + num10;
            double num12 = num6;
            double num13 = num12 * num12;
            return (float)System.Math.Sqrt(num11 + num13);
        }

        public static float DistanceSquared(Vector3 value1, Vector3 value2)
        {
            float num1 = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            double num4 = (double)num2;
            double num5 = (double)num1;
            double num6 = (double)num3;
            double num7 = num5;
            double num8 = num7 * num7;
            double num9 = num4;
            double num10 = num9 * num9;
            double num11 = num8 + num10;
            double num12 = num6;
            double num13 = num12 * num12;
            return (float)(num11 + num13);
        }

        public static float Dot(Vector3 left, Vector3 right)
        {
            float ret;
            Dot(ref left, ref right, out ret);
            return ret;
        }
        public static void Dot(ref Vector3 left, ref Vector3 right, out float determinant)
        {
            determinant = (float)((double)left.Y * (double)right.Y + (double)left.X * (double)right.X + (double)left.Z * (double)right.Z);
        }

        public static void Cross(ref Vector3 left, ref Vector3 right, out Vector3 result)
        {
            result = new Vector3()
            {
                X = (float)((double)left.Y * (double)right.Z - (double)left.Z * (double)right.Y),
                Y = (float)((double)left.Z * (double)right.X - (double)left.X * (double)right.Z),
                Z = (float)((double)left.X * (double)right.Y - (double)left.Y * (double)right.X)
            };
        }

        public static Vector3 Cross(Vector3 left, Vector3 right)
        {
            return new Vector3()
            {
                X = (float)((double)right.Z * (double)left.Y - (double)left.Z * (double)right.Y),
                Y = (float)((double)left.Z * (double)right.X - (double)right.Z * (double)left.X),
                Z = (float)((double)right.Y * (double)left.X - (double)left.Y * (double)right.X)
            };
        }

        public static void Reflect(ref Vector3 vector, ref Vector3 normal, out Vector3 result)
        {
            double num = ((double)vector.Y * (double)normal.Y + (double)vector.X * (double)normal.X + (double)vector.Z * (double)normal.Z) * 2.0;
            result.X = vector.X - normal.X * (float)num;
            result.Y = vector.Y - normal.Y * (float)num;
            result.Z = vector.Z - normal.Z * (float)num;
        }

        public static Vector3 Reflect(Vector3 vector, Vector3 normal)
        {
            Vector3 vector3 = new Vector3();
            double num = ((double)vector.Y * (double)normal.Y + (double)vector.X * (double)normal.X + (double)vector.Z * (double)normal.Z) * 2.0;
            vector3.X = vector.X - normal.X * (float)num;
            vector3.Y = vector.Y - normal.Y * (float)num;
            vector3.Z = vector.Z - normal.Z * (float)num;
            return vector3;
        }

        public static Vector4[] Transform(Vector3[] vectors, ref Matrix transformation)
        {
            Vector4[] vectorsOut = new Vector4[vectors.Length];
            throw new NotImplementedException(); // Vector3.Transform(vectors, ref transformation, vectorsOut, 0, 0);
            return vectorsOut;
        }

        public static void Transform(Vector3[] vectorsIn, ref Matrix transformation, Vector4[] vectorsOut)
        {
            throw new NotImplementedException(); // Vector3.Transform(vectorsIn, ref transformation, vectorsOut, 0, 0);
        }


        public static void Transform(ref Vector3 vector, ref Matrix transformation, out Vector4 result)
        {
            Vector4 vector4 = new Vector4();
            result = vector4;
            result.X = (float)((double)vector.Y * (double)transformation.M21 + (double)vector.X * (double)transformation.M11 + (double)vector.Z * (double)transformation.M31) + transformation.M41;
            result.Y = (float)((double)vector.Y * (double)transformation.M22 + (double)vector.X * (double)transformation.M12 + (double)vector.Z * (double)transformation.M32) + transformation.M42;
            result.Z = (float)((double)vector.Y * (double)transformation.M23 + (double)vector.X * (double)transformation.M13 + (double)vector.Z * (double)transformation.M33) + transformation.M43;
            result.W = (float)((double)transformation.M24 * (double)vector.Y + (double)transformation.M14 * (double)vector.X + (double)vector.Z * (double)transformation.M34) + transformation.M44;
        }

        public static Vector4 Transform(Vector3 vector, Matrix transformation)
        {
            return new Vector4()
            {
                X = (float)((double)transformation.M21 * (double)vector.Y + (double)transformation.M11 * (double)vector.X + (double)transformation.M31 * (double)vector.Z) + transformation.M41,
                Y = (float)((double)transformation.M22 * (double)vector.Y + (double)transformation.M12 * (double)vector.X + (double)transformation.M32 * (double)vector.Z) + transformation.M42,
                Z = (float)((double)transformation.M23 * (double)vector.Y + (double)transformation.M13 * (double)vector.X + (double)transformation.M33 * (double)vector.Z) + transformation.M43,
                W = (float)((double)transformation.M24 * (double)vector.Y + (double)transformation.M14 * (double)vector.X + (double)transformation.M34 * (double)vector.Z) + transformation.M44
            };
        }

        public static Vector3[] TransformCoordinate(Vector3[] coordinates, ref Matrix transformation)
        {
            if (coordinates == null)
                throw new ArgumentNullException("coordinates");
            Vector4 vector4 = new Vector4();
            Vector3[] coordinatesOut = new Vector3[coordinates.Length];
            throw new NotImplementedException(); //Vector3.TransformCoordinate(coordinates, ref transformation, coordinatesOut, 0, 0);
            return coordinatesOut;
        }

        public static void TransformCoordinate(Vector3[] coordinatesIn, ref Matrix transformation, Vector3[] coordinatesOut)
        {
            throw new NotImplementedException(); // Vector3.TransformCoordinate(coordinatesIn, ref transformation, coordinatesOut, 0, 0);
        }

        public static void TransformCoordinate(ref Vector3 coordinate, ref Matrix transformation, out Vector3 result)
        {
            Vector4 vector4 = new Vector4();
            vector4.X = (float)((double)coordinate.Y * (double)transformation.M21 + (double)coordinate.X * (double)transformation.M11 + (double)coordinate.Z * (double)transformation.M31) + transformation.M41;
            vector4.Y = (float)((double)coordinate.Y * (double)transformation.M22 + (double)coordinate.X * (double)transformation.M12 + (double)coordinate.Z * (double)transformation.M32) + transformation.M42;
            vector4.Z = (float)((double)coordinate.Y * (double)transformation.M23 + (double)coordinate.X * (double)transformation.M13 + (double)coordinate.Z * (double)transformation.M33) + transformation.M43;
            float num = (float)(1.0 / ((double)transformation.M24 * (double)coordinate.Y + (double)transformation.M14 * (double)coordinate.X + (double)coordinate.Z * (double)transformation.M34 + (double)transformation.M44));
            vector4.W = num;
            Vector3 vector3;
            vector3.X = vector4.X * num;
            vector3.Y = vector4.Y * num;
            vector3.Z = vector4.Z * num;
            result = vector3;
        }

        public static Vector3 TransformCoordinate(Vector3 coordinate, Matrix transformation)
        {
            Vector4 vector4 = new Vector4();
            vector4.X = (float)((double)transformation.M21 * (double)coordinate.Y + (double)transformation.M11 * (double)coordinate.X + (double)transformation.M31 * (double)coordinate.Z) + transformation.M41;
            vector4.Y = (float)((double)transformation.M22 * (double)coordinate.Y + (double)transformation.M12 * (double)coordinate.X + (double)transformation.M32 * (double)coordinate.Z) + transformation.M42;
            vector4.Z = (float)((double)transformation.M23 * (double)coordinate.Y + (double)transformation.M13 * (double)coordinate.X + (double)transformation.M33 * (double)coordinate.Z) + transformation.M43;
            float num = (float)(1.0 / ((double)transformation.M24 * (double)coordinate.Y + (double)transformation.M14 * (double)coordinate.X + (double)transformation.M34 * (double)coordinate.Z + (double)transformation.M44));
            vector4.W = num;
            Vector3 vector3;
            vector3.X = vector4.X * num;
            vector3.Y = vector4.Y * num;
            vector3.Z = vector4.Z * num;
            return vector3;
        }

        public static Vector3[] TransformNormal(Vector3[] normals, ref Matrix transformation)
        {
            if (normals == null)
                throw new ArgumentNullException("normals");
            Vector3[] normalsOut = new Vector3[normals.Length];
            throw new NotImplementedException(); //  Vector3.TransformNormal(normals, ref transformation, normalsOut, 0, 0);
            return normalsOut;
        }

        public static void TransformNormal(Vector3[] normalsIn, ref Matrix transformation, Vector3[] normalsOut)
        {
            throw new NotImplementedException(); // Vector3.TransformNormal(normalsIn, ref transformation, normalsOut, 0, 0);
        }

        public static void TransformNormal(ref Vector3 normal, ref Matrix transformation, out Vector3 result)
        {
            result.X = (float)((double)normal.Y * (double)transformation.M21 + (double)normal.X * (double)transformation.M11 + (double)normal.Z * (double)transformation.M31);
            result.Y = (float)((double)normal.Y * (double)transformation.M22 + (double)normal.X * (double)transformation.M12 + (double)normal.Z * (double)transformation.M32);
            result.Z = (float)((double)normal.Y * (double)transformation.M23 + (double)normal.X * (double)transformation.M13 + (double)normal.Z * (double)transformation.M33);
        }

        public static Vector3 TransformNormal(Vector3 normal, Matrix transformation)
        {
            return new Vector3()
            {
                X = (float)((double)transformation.M21 * (double)normal.Y + (double)transformation.M11 * (double)normal.X + (double)transformation.M31 * (double)normal.Z),
                Y = (float)((double)transformation.M22 * (double)normal.Y + (double)transformation.M12 * (double)normal.X + (double)transformation.M32 * (double)normal.Z),
                Z = (float)((double)transformation.M23 * (double)normal.Y + (double)transformation.M13 * (double)normal.X + (double)transformation.M33 * (double)normal.Z)
            };
        }

        public static void Project(ref Vector3 vector, float x, float y, float width, float height, float minZ, float maxZ, ref Matrix worldViewProjection, out Vector3 result)
        {
            Vector3 result1 = new Vector3();
            Vector3.TransformCoordinate(ref vector, ref worldViewProjection, out result1);
            Vector3 vector3;
            vector3.X = (float)(((double)result1.X + 1.0) * 0.5) * width + x;
            vector3.Y = (float)((1.0 - (double)result1.Y) * 0.5) * height + y;
            vector3.Z = result1.Z * (maxZ - minZ) + minZ;
            result = vector3;
        }

        public static Vector3 Project(Vector3 vector, float x, float y, float width, float height, float minZ, float maxZ, Matrix worldViewProjection)
        {
            Vector3.TransformCoordinate(ref vector, ref worldViewProjection, out vector);
            Vector3 vector3;
            vector3.X = (float)(((double)vector.X + 1.0) * 0.5) * width + x;
            vector3.Y = (float)((1.0 - (double)vector.Y) * 0.5) * height + y;
            vector3.Z = vector.Z * (maxZ - minZ) + minZ;
            return vector3;
        }

        public static void Unproject(ref Vector3 vector, float x, float y, float width, float height, float minZ, float maxZ, ref Matrix worldViewProjection, out Vector3 result)
        {
            Vector3 result1 = new Vector3();
            Matrix result2 = new Matrix();
            Matrix.Invert(ref worldViewProjection, out result2);
            result1.X = (float)(((double)vector.X - (double)x) / (double)width * 2.0 - 1.0);
            result1.Y = (float)-(((double)vector.Y - (double)y) / (double)height * 2.0 - 1.0);
            result1.Z = (float)(((double)vector.Z - (double)minZ) / ((double)maxZ - (double)minZ));
            Vector3.TransformCoordinate(ref result1, ref result2, out result1);
            result = result1;
        }

        public static Vector3 Unproject(Vector3 vector, float x, float y, float width, float height, float minZ, float maxZ, Matrix worldViewProjection)
        {
            Vector3 result1 = new Vector3();
            Matrix result2 = new Matrix();
            Matrix.Invert(ref worldViewProjection, out result2);
            result1.X = (float)(((double)vector.X - (double)x) / (double)width * 2.0 - 1.0);
            result1.Y = (float)-(((double)vector.Y - (double)y) / (double)height * 2.0 - 1.0);
            result1.Z = (float)(((double)vector.Z - (double)minZ) / ((double)maxZ - (double)minZ));
            Vector3.TransformCoordinate(ref result1, ref result2, out result1);
            return result1;
        }

        public static void Minimize(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            float num1 = (double)value1.X >= (double)value2.X ? value2.X : value1.X;
            result.X = num1;
            float num2 = (double)value1.Y >= (double)value2.Y ? value2.Y : value1.Y;
            result.Y = num2;
            float num3 = (double)value1.Z >= (double)value2.Z ? value2.Z : value1.Z;
            result.Z = num3;
        }

        public static Vector3 Minimize(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3 = new Vector3();
            float num1 = (double)value1.X >= (double)value2.X ? value2.X : value1.X;
            vector3.X = num1;
            float num2 = (double)value1.Y >= (double)value2.Y ? value2.Y : value1.Y;
            vector3.Y = num2;
            float num3 = (double)value1.Z >= (double)value2.Z ? value2.Z : value1.Z;
            vector3.Z = num3;
            return vector3;
        }

        public static void Maximize(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            float num1 = (double)value1.X <= (double)value2.X ? value2.X : value1.X;
            result.X = num1;
            float num2 = (double)value1.Y <= (double)value2.Y ? value2.Y : value1.Y;
            result.Y = num2;
            float num3 = (double)value1.Z <= (double)value2.Z ? value2.Z : value1.Z;
            result.Z = num3;
        }

        public static Vector3 Maximize(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3 = new Vector3();
            float num1 = (double)value1.X <= (double)value2.X ? value2.X : value1.X;
            vector3.X = num1;
            float num2 = (double)value1.Y <= (double)value2.Y ? value2.Y : value1.Y;
            vector3.Y = num2;
            float num3 = (double)value1.Z <= (double)value2.Z ? value2.Z : value1.Z;
            vector3.Z = num3;
            return vector3;
        }

        public override string ToString()
        {
            object[] objArray = new object[3];
            float num1 = this.X;
            objArray[0] = (object)num1.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num2 = this.Y;
            objArray[1] = (object)num2.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num3 = this.Z;
            objArray[2] = (object)num3.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2}", objArray);
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() + (this.Y.GetHashCode() + this.Z.GetHashCode());
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool Equals(ref Vector3 value1, ref Vector3 value2)
        {
            return (double)value1.X == (double)value2.X && (double)value1.Y == (double)value2.Y && (double)value1.Z == (double)value2.Z;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool Equals(Vector3 other)
        {
            return (double)this.X == (double)other.X && (double)this.Y == (double)other.Y && (double)this.Z == (double)other.Z;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;
            else
                return this.Equals((Vector3)obj);
        }
    }
}
