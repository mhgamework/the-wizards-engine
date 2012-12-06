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
            get
            {
                return Marshal.SizeOf(typeof(Vector2));
            }
        }

        public static Vector2 UnitY
        {
            get
            {
                return new Vector2(0.0f, 1f);
            }
        }

        public static Vector2 UnitX
        {
            get
            {
                return new Vector2(1f, 0.0f);
            }
        }

        public static Vector2 Zero
        {
            get
            {
                return new Vector2(0.0f, 0.0f);
            }
        }

        public float this[int index]
        {
            get
            {
                if (index == 0)
                    return this.X;
                if (index != 1)
                    throw new ArgumentOutOfRangeException("index", "Indices for Vector2 run from 0 to 1, inclusive.");
                else
                    return this.Y;
            }
            set
            {
                if (index != 0)
                {
                    if (index != 1)
                        throw new ArgumentOutOfRangeException("index", "Indices for Vector2 run from 0 to 1, inclusive.");
                    this.Y = value;
                }
                else
                    this.X = value;
            }
        }

        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2(float value)
        {
            this.X = value;
            this.Y = value;
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
            return (double)left.X == (double)right.X && (double)left.Y == (double)right.Y;
        }

        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return ((double)left.X != (double)right.X || (double)left.Y != (double)right.Y ? 0 : 1) == 0;
        }

        public float Length()
        {
            double num1 = (double)this.Y;
            double num2 = (double)this.X;
            double num3 = num2 * num2;
            double num4 = num1;
            double num5 = num4 * num4;
            return (float)System.Math.Sqrt(num3 + num5);
        }

        public float LengthSquared()
        {
            double num1 = (double)this.Y;
            double num2 = (double)this.X;
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
            double num1 = (double)this.Y;
            double num2 = (double)this.X;
            double num3 = num2 * num2;
            double num4 = num1;
            double num5 = num4 * num4;
            float num6 = (float)System.Math.Sqrt(num3 + num5);
            if ((double)num6 == 0.0)
                return;
            float num7 = 1f / num6;
            this.X *= num7;
            this.Y *= num7;
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

        public static void Barycentric(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, float amount1, float amount2, out Vector2 result)
        {
            Vector2 vector2;
            vector2.X = (float)(((double)value2.X - (double)value1.X) * (double)amount1 + (double)value1.X + ((double)value3.X - (double)value1.X) * (double)amount2);
            vector2.Y = (float)(((double)value2.Y - (double)value1.Y) * (double)amount1 + (double)value1.Y + ((double)value3.Y - (double)value1.Y) * (double)amount2);
            result = vector2;
        }

        public static Vector2 Barycentric(Vector2 value1, Vector2 value2, Vector2 value3, float amount1, float amount2)
        {
            return new Vector2()
            {
                X = (float)(((double)value2.X - (double)value1.X) * (double)amount1 + (double)value1.X + ((double)value3.X - (double)value1.X) * (double)amount2),
                Y = (float)(((double)value2.Y - (double)value1.Y) * (double)amount1 + (double)value1.Y + ((double)value3.Y - (double)value1.Y) * (double)amount2)
            };
        }

        public static void CatmullRom(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, ref Vector2 value4, float amount, out Vector2 result)
        {
            double num1 = (double)amount;
            float num2 = (float)(num1 * num1);
            float num3 = num2 * amount;
            result = new Vector2()
            {
                X = (float)((((double)value1.X * 2.0 - (double)value2.X * 5.0 + (double)value3.X * 4.0 - (double)value4.X) * (double)num2 + (((double)value3.X - (double)value1.X) * (double)amount + (double)value2.X * 2.0) + ((double)value2.X * 3.0 - (double)value1.X - (double)value3.X * 3.0 + (double)value4.X) * (double)num3) * 0.5),
                Y = (float)((((double)value1.Y * 2.0 - (double)value2.Y * 5.0 + (double)value3.Y * 4.0 - (double)value4.Y) * (double)num2 + (((double)value3.Y - (double)value1.Y) * (double)amount + (double)value2.Y * 2.0) + ((double)value2.Y * 3.0 - (double)value1.Y - (double)value3.Y * 3.0 + (double)value4.Y) * (double)num3) * 0.5)
            };
        }

        public static Vector2 CatmullRom(Vector2 value1, Vector2 value2, Vector2 value3, Vector2 value4, float amount)
        {
            Vector2 vector2 = new Vector2();
            double num1 = (double)amount;
            float num2 = (float)(num1 * num1);
            float num3 = num2 * amount;
            vector2.X = (float)((((double)value1.X * 2.0 - (double)value2.X * 5.0 + (double)value3.X * 4.0 - (double)value4.X) * (double)num2 + (((double)value3.X - (double)value1.X) * (double)amount + (double)value2.X * 2.0) + ((double)value2.X * 3.0 - (double)value1.X - (double)value3.X * 3.0 + (double)value4.X) * (double)num3) * 0.5);
            vector2.Y = (float)((((double)value1.Y * 2.0 - (double)value2.Y * 5.0 + (double)value3.Y * 4.0 - (double)value4.Y) * (double)num2 + (((double)value3.Y - (double)value1.Y) * (double)amount + (double)value2.Y * 2.0) + ((double)value2.Y * 3.0 - (double)value1.Y - (double)value3.Y * 3.0 + (double)value4.Y) * (double)num3) * 0.5);
            return vector2;
        }

        public static void Clamp(ref Vector2 value, ref Vector2 min, ref Vector2 max, out Vector2 result)
        {
            float num1 = value.X;
            float num2 = (double)num1 <= (double)max.X ? num1 : max.X;
            float num3 = (double)num2 >= (double)min.X ? num2 : min.X;
            float num4 = value.Y;
            float num5 = (double)num4 <= (double)max.Y ? num4 : max.Y;
            float num6 = (double)num5 >= (double)min.Y ? num5 : min.Y;
            Vector2 vector2;
            vector2.X = num3;
            vector2.Y = num6;
            result = vector2;
        }

        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
        {
            float num1 = value.X;
            float num2 = (double)num1 <= (double)max.X ? num1 : max.X;
            float num3 = (double)num2 >= (double)min.X ? num2 : min.X;
            float num4 = value.Y;
            float num5 = (double)num4 <= (double)max.Y ? num4 : max.Y;
            float num6 = (double)num5 >= (double)min.Y ? num5 : min.Y;
            Vector2 vector2;
            vector2.X = num3;
            vector2.Y = num6;
            return vector2;
        }

        public static void Hermite(ref Vector2 value1, ref Vector2 tangent1, ref Vector2 value2, ref Vector2 tangent2, float amount, out Vector2 result)
        {
            double num1 = (double)amount;
            float num2 = (float)(num1 * num1);
            float num3 = num2 * amount;
            double num4 = (double)num2 * 3.0;
            float num5 = (float)((double)num3 * 2.0 - num4 + 1.0);
            float num6 = (float)((double)num3 * -2.0 + num4);
            float num7 = num3 - num2 * 2f + amount;
            float num8 = num3 - num2;
            result = new Vector2()
            {
                X = (float)((double)value2.X * (double)num6 + (double)value1.X * (double)num5 + (double)tangent1.X * (double)num7 + (double)tangent2.X * (double)num8),
                Y = (float)((double)value2.Y * (double)num6 + (double)value1.Y * (double)num5 + (double)tangent1.Y * (double)num7 + (double)tangent2.Y * (double)num8)
            };
        }

        public static Vector2 Hermite(Vector2 value1, Vector2 tangent1, Vector2 value2, Vector2 tangent2, float amount)
        {
            Vector2 vector2 = new Vector2();
            double num1 = (double)amount;
            float num2 = (float)(num1 * num1);
            float num3 = num2 * amount;
            double num4 = (double)num2 * 3.0;
            float num5 = (float)((double)num3 * 2.0 - num4 + 1.0);
            float num6 = (float)((double)num3 * -2.0 + num4);
            float num7 = num3 - num2 * 2f + amount;
            float num8 = num3 - num2;
            vector2.X = (float)((double)value2.X * (double)num6 + (double)value1.X * (double)num5 + (double)tangent1.X * (double)num7 + (double)tangent2.X * (double)num8);
            vector2.Y = (float)((double)value2.Y * (double)num6 + (double)value1.Y * (double)num5 + (double)tangent1.Y * (double)num7 + (double)tangent2.Y * (double)num8);
            return vector2;
        }

        public static void Lerp(ref Vector2 start, ref Vector2 end, float amount, out Vector2 result)
        {
            result = new Vector2()
            {
                X = (end.X - start.X) * amount + start.X,
                Y = (end.Y - start.Y) * amount + start.Y
            };
        }

        public static Vector2 Lerp(Vector2 start, Vector2 end, float amount)
        {
            return new Vector2()
            {
                X = (end.X - start.X) * amount + start.X,
                Y = (end.Y - start.Y) * amount + start.Y
            };
        }

        public static void SmoothStep(ref Vector2 start, ref Vector2 end, float amount, out Vector2 result)
        {
            float num1 = (double)amount <= 1.0 ? ((double)amount >= 0.0 ? amount : 0.0f) : 1f;
            double num2 = (double)num1;
            double num3 = 3.0 - (double)num1 * 2.0;
            double num4 = num2;
            double num5 = num4 * num4;
            amount = (float)(num3 * num5);
            result = new Vector2()
            {
                X = (end.X - start.X) * amount + start.X,
                Y = (end.Y - start.Y) * amount + start.Y
            };
        }

        public static Vector2 SmoothStep(Vector2 start, Vector2 end, float amount)
        {
            Vector2 vector2 = new Vector2();
            float num1 = (double)amount <= 1.0 ? ((double)amount >= 0.0 ? amount : 0.0f) : 1f;
            double num2 = (double)num1;
            double num3 = 3.0 - (double)num1 * 2.0;
            double num4 = num2;
            double num5 = num4 * num4;
            amount = (float)(num3 * num5);
            vector2.X = (end.X - start.X) * amount + start.X;
            vector2.Y = (end.Y - start.Y) * amount + start.Y;
            return vector2;
        }

        public static float Distance(Vector2 value1, Vector2 value2)
        {
            float num1 = value1.X - value2.X;
            double num2 = (double)(value1.Y - value2.Y);
            double num3 = (double)num1;
            double num4 = num3 * num3;
            double num5 = num2;
            double num6 = num5 * num5;
            return (float)System.Math.Sqrt(num4 + num6);
        }

        public static float DistanceSquared(Vector2 value1, Vector2 value2)
        {
            float num1 = value1.X - value2.X;
            double num2 = (double)(value1.Y - value2.Y);
            double num3 = (double)num1;
            double num4 = num3 * num3;
            double num5 = num2;
            double num6 = num5 * num5;
            return (float)(num4 + num6);
        }

        public static float Dot(Vector2 left, Vector2 right)
        {
            return (float)((double)left.Y * (double)right.Y + (double)left.X * (double)right.X);
        }

        public static Vector4[] Transform(Vector2[] vectors, ref Matrix transformation)
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
                        X = (float)((double)vectors[index].Y * (double)transformation.M21 + (double)vectors[index].X * (double)transformation.M11) + transformation.M41,
                        Y = (float)((double)vectors[index].Y * (double)transformation.M22 + (double)vectors[index].X * (double)transformation.M12) + transformation.M42,
                        Z = (float)((double)vectors[index].Y * (double)transformation.M23 + (double)vectors[index].X * (double)transformation.M13) + transformation.M43,
                        W = (float)((double)vectors[index].Y * (double)transformation.M24 + (double)vectors[index].X * (double)transformation.M14) + transformation.M44
                    };
                    ++index;
                }
                while (index < length);
            }
            return vector4Array;
        }

        public static void Transform(ref Vector2 vector, ref Matrix transformation, out Vector4 result)
        {
            result = new Vector4()
            {
                X = (float)((double)vector.Y * (double)transformation.M21 + (double)vector.X * (double)transformation.M11) + transformation.M41,
                Y = (float)((double)vector.Y * (double)transformation.M22 + (double)vector.X * (double)transformation.M12) + transformation.M42,
                Z = (float)((double)vector.Y * (double)transformation.M23 + (double)vector.X * (double)transformation.M13) + transformation.M43,
                W = (float)((double)vector.Y * (double)transformation.M24 + (double)vector.X * (double)transformation.M14) + transformation.M44
            };
        }

        public static Vector4 Transform(Vector2 vector, Matrix transformation)
        {
            return new Vector4()
            {
                X = (float)((double)transformation.M21 * (double)vector.Y + (double)transformation.M11 * (double)vector.X) + transformation.M41,
                Y = (float)((double)transformation.M22 * (double)vector.Y + (double)transformation.M12 * (double)vector.X) + transformation.M42,
                Z = (float)((double)transformation.M23 * (double)vector.Y + (double)transformation.M13 * (double)vector.X) + transformation.M43,
                W = (float)((double)transformation.M24 * (double)vector.Y + (double)transformation.M14 * (double)vector.X) + transformation.M44
            };
        }

        public static Vector2[] TransformCoordinate(Vector2[] coordinates, ref Matrix transformation)
        {
            if (coordinates == null)
                throw new ArgumentNullException("coordinates");
            Vector4 vector4 = new Vector4();
            int length = coordinates.Length;
            Vector2[] vector2Array = new Vector2[length];
            int index = 0;
            if (0 < length)
            {
                do
                {
                    vector4.X = (float)((double)coordinates[index].Y * (double)transformation.M21 + (double)coordinates[index].X * (double)transformation.M11) + transformation.M41;
                    vector4.Y = (float)((double)coordinates[index].Y * (double)transformation.M22 + (double)coordinates[index].X * (double)transformation.M12) + transformation.M42;
                    vector4.Z = (float)((double)coordinates[index].Y * (double)transformation.M23 + (double)coordinates[index].X * (double)transformation.M13) + transformation.M43;
                    float num = (float)(1.0 / ((double)coordinates[index].Y * (double)transformation.M24 + (double)coordinates[index].X * (double)transformation.M14 + (double)transformation.M44));
                    vector4.W = num;
                    Vector2 vector2;
                    vector2.X = vector4.X * num;
                    vector2.Y = vector4.Y * num;
                    vector2Array[index] = vector2;
                    ++index;
                }
                while (index < length);
            }
            return vector2Array;
        }

        public static void TransformCoordinate(ref Vector2 coordinate, ref Matrix transformation, out Vector2 result)
        {
            Vector4 vector4 = new Vector4();
            vector4.X = (float)((double)coordinate.Y * (double)transformation.M21 + (double)coordinate.X * (double)transformation.M11) + transformation.M41;
            vector4.Y = (float)((double)coordinate.Y * (double)transformation.M22 + (double)coordinate.X * (double)transformation.M12) + transformation.M42;
            vector4.Z = (float)((double)coordinate.Y * (double)transformation.M23 + (double)coordinate.X * (double)transformation.M13) + transformation.M43;
            float num = (float)(1.0 / ((double)coordinate.Y * (double)transformation.M24 + (double)coordinate.X * (double)transformation.M14 + (double)transformation.M44));
            vector4.W = num;
            Vector2 vector2;
            vector2.X = vector4.X * num;
            vector2.Y = vector4.Y * num;
            result = vector2;
        }

        public static Vector2 TransformCoordinate(Vector2 coordinate, Matrix transformation)
        {
            Vector4 vector4 = new Vector4();
            vector4.X = (float)((double)transformation.M21 * (double)coordinate.Y + (double)transformation.M11 * (double)coordinate.X) + transformation.M41;
            vector4.Y = (float)((double)transformation.M22 * (double)coordinate.Y + (double)transformation.M12 * (double)coordinate.X) + transformation.M42;
            vector4.Z = (float)((double)transformation.M23 * (double)coordinate.Y + (double)transformation.M13 * (double)coordinate.X) + transformation.M43;
            float num = (float)(1.0 / ((double)transformation.M24 * (double)coordinate.Y + (double)transformation.M14 * (double)coordinate.X + (double)transformation.M44));
            vector4.W = num;
            Vector2 vector2;
            vector2.X = vector4.X * num;
            vector2.Y = vector4.Y * num;
            return vector2;
        }

        public static Vector2[] TransformNormal(Vector2[] normals, ref Matrix transformation)
        {
            if (normals == null)
                throw new ArgumentNullException("normals");
            int length = normals.Length;
            Vector2[] vector2Array = new Vector2[length];
            int index = 0;
            if (0 < length)
            {
                do
                {
                    vector2Array[index] = new Vector2()
                    {
                        X = (float)((double)normals[index].Y * (double)transformation.M21 + (double)normals[index].X * (double)transformation.M11),
                        Y = (float)((double)normals[index].Y * (double)transformation.M22 + (double)normals[index].X * (double)transformation.M12)
                    };
                    ++index;
                }
                while (index < length);
            }
            return vector2Array;
        }

        public static void TransformNormal(ref Vector2 normal, ref Matrix transformation, out Vector2 result)
        {
            result = new Vector2()
            {
                X = (float)((double)normal.Y * (double)transformation.M21 + (double)normal.X * (double)transformation.M11),
                Y = (float)((double)normal.Y * (double)transformation.M22 + (double)normal.X * (double)transformation.M12)
            };
        }

        public static Vector2 TransformNormal(Vector2 normal, Matrix transformation)
        {
            return new Vector2()
            {
                X = (float)((double)transformation.M21 * (double)normal.Y + (double)transformation.M11 * (double)normal.X),
                Y = (float)((double)transformation.M22 * (double)normal.Y + (double)transformation.M12 * (double)normal.X)
            };
        }

        public static void Minimize(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            Vector2 vector2 = new Vector2();
            float num1 = (double)value1.X >= (double)value2.X ? value2.X : value1.X;
            vector2.X = num1;
            float num2 = (double)value1.Y >= (double)value2.Y ? value2.Y : value1.Y;
            vector2.Y = num2;
            result = vector2;
        }

        public static Vector2 Minimize(Vector2 value1, Vector2 value2)
        {
            Vector2 vector2 = new Vector2();
            float num1 = (double)value1.X >= (double)value2.X ? value2.X : value1.X;
            vector2.X = num1;
            float num2 = (double)value1.Y >= (double)value2.Y ? value2.Y : value1.Y;
            vector2.Y = num2;
            return vector2;
        }

        public static void Maximize(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            Vector2 vector2 = new Vector2();
            float num1 = (double)value1.X <= (double)value2.X ? value2.X : value1.X;
            vector2.X = num1;
            float num2 = (double)value1.Y <= (double)value2.Y ? value2.Y : value1.Y;
            vector2.Y = num2;
            result = vector2;
        }

        public static Vector2 Maximize(Vector2 value1, Vector2 value2)
        {
            Vector2 vector2 = new Vector2();
            float num1 = (double)value1.X <= (double)value2.X ? value2.X : value1.X;
            vector2.X = num1;
            float num2 = (double)value1.Y <= (double)value2.Y ? value2.Y : value1.Y;
            vector2.Y = num2;
            return vector2;
        }

        public override  string ToString()
        {
            object[] objArray = new object[2];
            float num1 = this.X;
            objArray[0] = (object)num1.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num2 = this.Y;
            objArray[1] = (object)num2.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, "X:{0} Y:{1}", objArray);
        }

        public override  int GetHashCode()
        {
            return this.Y.GetHashCode() + this.X.GetHashCode();
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool Equals(ref Vector2 value1, ref Vector2 value2)
        {
            return (double)value1.X == (double)value2.X && (double)value1.Y == (double)value2.Y;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool Equals(Vector2 other)
        {
            return (double)this.X == (double)other.X && (double)this.Y == (double)other.Y;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override  bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;
            else
                return this.Equals((Vector2)obj);
        }
    }
}
