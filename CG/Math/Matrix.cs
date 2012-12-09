using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MHGameWork.TheWizards.CG.Math
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Matrix : IEquatable<Matrix>
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        [Browsable(false)]
        public bool IsIdentity
        {
            get
            {
                return (double)this.M11 == 1.0 && (double)this.M22 == 1.0 && ((double)this.M33 == 1.0 && (double)this.M44 == 1.0) && ((double)this.M12 == 0.0 && (double)this.M13 == 0.0 && ((double)this.M14 == 0.0 && (double)this.M21 == 0.0)) && ((double)this.M23 == 0.0 && (double)this.M24 == 0.0 && ((double)this.M31 == 0.0 && (double)this.M32 == 0.0) && ((double)this.M34 == 0.0 && (double)this.M41 == 0.0 && ((double)this.M42 == 0.0 && (double)this.M43 == 0.0)));
            }
        }

        public static Matrix Identity
        {
            get
            {
                return new Matrix()
                {
                    M11 = 1f,
                    M22 = 1f,
                    M33 = 1f,
                    M44 = 1f
                };
            }
        }

        [Browsable(false)]
        public float this[int row, int column]
        {
            get
            {
                if ((uint)row > 3U)
                    throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 3, inclusive.");
                if ((uint)column > 3U)
                    throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 3, inclusive.");
                switch (row * 4 + column)
                {
                    case 0:
                        return this.M11;
                    case 1:
                        return this.M12;
                    case 2:
                        return this.M13;
                    case 3:
                        return this.M14;
                    case 4:
                        return this.M21;
                    case 5:
                        return this.M22;
                    case 6:
                        return this.M23;
                    case 7:
                        return this.M24;
                    case 8:
                        return this.M31;
                    case 9:
                        return this.M32;
                    case 10:
                        return this.M33;
                    case 11:
                        return this.M34;
                    case 12:
                        return this.M41;
                    case 13:
                        return this.M42;
                    case 14:
                        return this.M43;
                    case 15:
                        return this.M44;
                    default:
                        return 0.0f;
                }
            }
            set
            {
                if ((uint)row > 3U)
                    throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 3, inclusive.");
                if ((uint)column > 3U)
                    throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 3, inclusive.");
                switch (row * 4 + column)
                {
                    case 0:
                        this.M11 = value;
                        break;
                    case 1:
                        this.M12 = value;
                        break;
                    case 2:
                        this.M13 = value;
                        break;
                    case 3:
                        this.M14 = value;
                        break;
                    case 4:
                        this.M21 = value;
                        break;
                    case 5:
                        this.M22 = value;
                        break;
                    case 6:
                        this.M23 = value;
                        break;
                    case 7:
                        this.M24 = value;
                        break;
                    case 8:
                        this.M31 = value;
                        break;
                    case 9:
                        this.M32 = value;
                        break;
                    case 10:
                        this.M33 = value;
                        break;
                    case 11:
                        this.M34 = value;
                        break;
                    case 12:
                        this.M41 = value;
                        break;
                    case 13:
                        this.M42 = value;
                        break;
                    case 14:
                        this.M43 = value;
                        break;
                    case 15:
                        this.M44 = value;
                        break;
                }
            }
        }

        public static Matrix operator -(Matrix left, Matrix right)
        {
            return new Matrix()
            {
                M11 = left.M11 - right.M11,
                M12 = left.M12 - right.M12,
                M13 = left.M13 - right.M13,
                M14 = left.M14 - right.M14,
                M21 = left.M21 - right.M21,
                M22 = left.M22 - right.M22,
                M23 = left.M23 - right.M23,
                M24 = left.M24 - right.M24,
                M31 = left.M31 - right.M31,
                M32 = left.M32 - right.M32,
                M33 = left.M33 - right.M33,
                M34 = left.M34 - right.M34,
                M41 = left.M41 - right.M41,
                M42 = left.M42 - right.M42,
                M43 = left.M43 - right.M43,
                M44 = left.M44 - right.M44
            };
        }

        public static Matrix operator -(Matrix matrix)
        {
            return new Matrix()
            {
                M11 = -matrix.M11,
                M12 = -matrix.M12,
                M13 = -matrix.M13,
                M14 = -matrix.M14,
                M21 = -matrix.M21,
                M22 = -matrix.M22,
                M23 = -matrix.M23,
                M24 = -matrix.M24,
                M31 = -matrix.M31,
                M32 = -matrix.M32,
                M33 = -matrix.M33,
                M34 = -matrix.M34,
                M41 = -matrix.M41,
                M42 = -matrix.M42,
                M43 = -matrix.M43,
                M44 = -matrix.M44
            };
        }

        public static Matrix operator +(Matrix left, Matrix right)
        {
            return new Matrix()
            {
                M11 = left.M11 + right.M11,
                M12 = left.M12 + right.M12,
                M13 = left.M13 + right.M13,
                M14 = left.M14 + right.M14,
                M21 = left.M21 + right.M21,
                M22 = left.M22 + right.M22,
                M23 = left.M23 + right.M23,
                M24 = left.M24 + right.M24,
                M31 = left.M31 + right.M31,
                M32 = left.M32 + right.M32,
                M33 = left.M33 + right.M33,
                M34 = left.M34 + right.M34,
                M41 = left.M41 + right.M41,
                M42 = left.M42 + right.M42,
                M43 = left.M43 + right.M43,
                M44 = left.M44 + right.M44
            };
        }

        public static Matrix operator /(Matrix left, float right)
        {
            return new Matrix()
            {
                M11 = left.M11 / right,
                M12 = left.M12 / right,
                M13 = left.M13 / right,
                M14 = left.M14 / right,
                M21 = left.M21 / right,
                M22 = left.M22 / right,
                M23 = left.M23 / right,
                M24 = left.M24 / right,
                M31 = left.M31 / right,
                M32 = left.M32 / right,
                M33 = left.M33 / right,
                M34 = left.M34 / right,
                M41 = left.M41 / right,
                M42 = left.M42 / right,
                M43 = left.M43 / right,
                M44 = left.M44 / right
            };
        }

        public static Matrix operator /(Matrix left, Matrix right)
        {
            return new Matrix()
            {
                M11 = left.M11 / right.M11,
                M12 = left.M12 / right.M12,
                M13 = left.M13 / right.M13,
                M14 = left.M14 / right.M14,
                M21 = left.M21 / right.M21,
                M22 = left.M22 / right.M22,
                M23 = left.M23 / right.M23,
                M24 = left.M24 / right.M24,
                M31 = left.M31 / right.M31,
                M32 = left.M32 / right.M32,
                M33 = left.M33 / right.M33,
                M34 = left.M34 / right.M34,
                M41 = left.M41 / right.M41,
                M42 = left.M42 / right.M42,
                M43 = left.M43 / right.M43,
                M44 = left.M44 / right.M44
            };
        }

        public static Matrix operator *(float left, Matrix right)
        {
            return right * left;
        }

        public static Matrix operator *(Matrix left, float right)
        {
            return new Matrix()
            {
                M11 = left.M11 * right,
                M12 = left.M12 * right,
                M13 = left.M13 * right,
                M14 = left.M14 * right,
                M21 = left.M21 * right,
                M22 = left.M22 * right,
                M23 = left.M23 * right,
                M24 = left.M24 * right,
                M31 = left.M31 * right,
                M32 = left.M32 * right,
                M33 = left.M33 * right,
                M34 = left.M34 * right,
                M41 = left.M41 * right,
                M42 = left.M42 * right,
                M43 = left.M43 * right,
                M44 = left.M44 * right
            };
        }

        public static Matrix operator *(Matrix left, Matrix right)
        {
            return new Matrix()
            {
                M11 = (float)((double)right.M21 * (double)left.M12 + (double)left.M11 * (double)right.M11 + (double)right.M31 * (double)left.M13 + (double)right.M41 * (double)left.M14),
                M12 = (float)((double)right.M22 * (double)left.M12 + (double)right.M12 * (double)left.M11 + (double)right.M32 * (double)left.M13 + (double)right.M42 * (double)left.M14),
                M13 = (float)((double)right.M23 * (double)left.M12 + (double)right.M13 * (double)left.M11 + (double)right.M33 * (double)left.M13 + (double)right.M43 * (double)left.M14),
                M14 = (float)((double)right.M24 * (double)left.M12 + (double)right.M14 * (double)left.M11 + (double)right.M34 * (double)left.M13 + (double)right.M44 * (double)left.M14),
                M21 = (float)((double)left.M22 * (double)right.M21 + (double)left.M21 * (double)right.M11 + (double)left.M23 * (double)right.M31 + (double)left.M24 * (double)right.M41),
                M22 = (float)((double)left.M22 * (double)right.M22 + (double)left.M21 * (double)right.M12 + (double)left.M23 * (double)right.M32 + (double)left.M24 * (double)right.M42),
                M23 = (float)((double)right.M23 * (double)left.M22 + (double)right.M13 * (double)left.M21 + (double)right.M33 * (double)left.M23 + (double)left.M24 * (double)right.M43),
                M24 = (float)((double)right.M24 * (double)left.M22 + (double)right.M14 * (double)left.M21 + (double)right.M34 * (double)left.M23 + (double)right.M44 * (double)left.M24),
                M31 = (float)((double)left.M32 * (double)right.M21 + (double)left.M31 * (double)right.M11 + (double)left.M33 * (double)right.M31 + (double)left.M34 * (double)right.M41),
                M32 = (float)((double)left.M32 * (double)right.M22 + (double)left.M31 * (double)right.M12 + (double)left.M33 * (double)right.M32 + (double)left.M34 * (double)right.M42),
                M33 = (float)((double)right.M23 * (double)left.M32 + (double)left.M31 * (double)right.M13 + (double)left.M33 * (double)right.M33 + (double)left.M34 * (double)right.M43),
                M34 = (float)((double)right.M24 * (double)left.M32 + (double)right.M14 * (double)left.M31 + (double)right.M34 * (double)left.M33 + (double)right.M44 * (double)left.M34),
                M41 = (float)((double)left.M42 * (double)right.M21 + (double)left.M41 * (double)right.M11 + (double)left.M43 * (double)right.M31 + (double)left.M44 * (double)right.M41),
                M42 = (float)((double)left.M42 * (double)right.M22 + (double)left.M41 * (double)right.M12 + (double)left.M43 * (double)right.M32 + (double)left.M44 * (double)right.M42),
                M43 = (float)((double)right.M23 * (double)left.M42 + (double)left.M41 * (double)right.M13 + (double)left.M43 * (double)right.M33 + (double)left.M44 * (double)right.M43),
                M44 = (float)((double)right.M24 * (double)left.M42 + (double)left.M41 * (double)right.M14 + (double)right.M34 * (double)left.M43 + (double)left.M44 * (double)right.M44)
            };
        }

        public static bool operator ==(Matrix left, Matrix right)
        {
            return Matrix.Equals(ref left, ref right);
        }

        public static bool operator !=(Matrix left, Matrix right)
        {
            return !Matrix.Equals(ref left, ref right);
        }


        public Vector4 get_Rows(int row)
        {
            return new Vector4(this[row, 0], this[row, 1], this[row, 2], this[row, 3]);
        }

        public void set_Rows(int row, Vector4 value)
        {
            this[row, 0] = value.X;
            this[row, 1] = value.Y;
            this[row, 2] = value.Z;
            this[row, 3] = value.W;
        }

        public Vector4 get_Columns(int column)
        {
            return new Vector4(this[0, column], this[1, column], this[2, column], this[3, column]);
        }

        public void set_Columns(int column, Vector4 value)
        {
            this[0, column] = value.X;
            this[1, column] = value.Y;
            this[2, column] = value.Z;
            this[3, column] = value.W;
        }

        public float[] ToArray()
        {
            return new float[16]
      {
        this.M11,
        this.M12,
        this.M13,
        this.M14,
        this.M21,
        this.M22,
        this.M23,
        this.M24,
        this.M31,
        this.M32,
        this.M33,
        this.M34,
        this.M41,
        this.M42,
        this.M43,
        this.M44
      };
        }


        public float Determinant()
        {
            float num1 = (float)((double)this.M44 * (double)this.M33 - (double)this.M43 * (double)this.M34);
            float num2 = (float)((double)this.M32 * (double)this.M44 - (double)this.M42 * (double)this.M34);
            float num3 = (float)((double)this.M32 * (double)this.M43 - (double)this.M42 * (double)this.M33);
            float num4 = (float)((double)this.M31 * (double)this.M44 - (double)this.M41 * (double)this.M34);
            float num5 = (float)((double)this.M31 * (double)this.M43 - (double)this.M41 * (double)this.M33);
            float num6 = (float)((double)this.M31 * (double)this.M42 - (double)this.M41 * (double)this.M32);
            return (float)(((double)this.M22 * (double)num1 - (double)this.M23 * (double)num2 + (double)this.M24 * (double)num3) * (double)this.M11 - ((double)this.M21 * (double)num1 - (double)this.M23 * (double)num4 + (double)this.M24 * (double)num5) * (double)this.M12 + ((double)this.M21 * (double)num2 - (double)this.M22 * (double)num4 + (double)this.M24 * (double)num6) * (double)this.M13 - ((double)this.M21 * (double)num3 - (double)this.M22 * (double)num5 + (double)this.M23 * (double)num6) * (double)this.M14);
        }

        public static void Add(ref Matrix left, ref Matrix right, out Matrix result)
        {
            result = new Matrix()
            {
                M11 = left.M11 + right.M11,
                M12 = left.M12 + right.M12,
                M13 = left.M13 + right.M13,
                M14 = left.M14 + right.M14,
                M21 = left.M21 + right.M21,
                M22 = left.M22 + right.M22,
                M23 = left.M23 + right.M23,
                M24 = left.M24 + right.M24,
                M31 = left.M31 + right.M31,
                M32 = left.M32 + right.M32,
                M33 = left.M33 + right.M33,
                M34 = left.M34 + right.M34,
                M41 = left.M41 + right.M41,
                M42 = left.M42 + right.M42,
                M43 = left.M43 + right.M43,
                M44 = left.M44 + right.M44
            };
        }

        public static Matrix Add(Matrix left, Matrix right)
        {
            return new Matrix()
            {
                M11 = left.M11 + right.M11,
                M12 = left.M12 + right.M12,
                M13 = left.M13 + right.M13,
                M14 = left.M14 + right.M14,
                M21 = left.M21 + right.M21,
                M22 = left.M22 + right.M22,
                M23 = left.M23 + right.M23,
                M24 = left.M24 + right.M24,
                M31 = left.M31 + right.M31,
                M32 = left.M32 + right.M32,
                M33 = left.M33 + right.M33,
                M34 = left.M34 + right.M34,
                M41 = left.M41 + right.M41,
                M42 = left.M42 + right.M42,
                M43 = left.M43 + right.M43,
                M44 = left.M44 + right.M44
            };
        }

        public static void Subtract(ref Matrix left, ref Matrix right, out Matrix result)
        {
            result = new Matrix()
            {
                M11 = left.M11 - right.M11,
                M12 = left.M12 - right.M12,
                M13 = left.M13 - right.M13,
                M14 = left.M14 - right.M14,
                M21 = left.M21 - right.M21,
                M22 = left.M22 - right.M22,
                M23 = left.M23 - right.M23,
                M24 = left.M24 - right.M24,
                M31 = left.M31 - right.M31,
                M32 = left.M32 - right.M32,
                M33 = left.M33 - right.M33,
                M34 = left.M34 - right.M34,
                M41 = left.M41 - right.M41,
                M42 = left.M42 - right.M42,
                M43 = left.M43 - right.M43,
                M44 = left.M44 - right.M44
            };
        }

        public static Matrix Subtract(Matrix left, Matrix right)
        {
            return new Matrix()
            {
                M11 = left.M11 - right.M11,
                M12 = left.M12 - right.M12,
                M13 = left.M13 - right.M13,
                M14 = left.M14 - right.M14,
                M21 = left.M21 - right.M21,
                M22 = left.M22 - right.M22,
                M23 = left.M23 - right.M23,
                M24 = left.M24 - right.M24,
                M31 = left.M31 - right.M31,
                M32 = left.M32 - right.M32,
                M33 = left.M33 - right.M33,
                M34 = left.M34 - right.M34,
                M41 = left.M41 - right.M41,
                M42 = left.M42 - right.M42,
                M43 = left.M43 - right.M43,
                M44 = left.M44 - right.M44
            };
        }

        public static void Multiply(ref Matrix left, float right, out Matrix result)
        {
            result = new Matrix()
            {
                M11 = left.M11 * right,
                M12 = left.M12 * right,
                M13 = left.M13 * right,
                M14 = left.M14 * right,
                M21 = left.M21 * right,
                M22 = left.M22 * right,
                M23 = left.M23 * right,
                M24 = left.M24 * right,
                M31 = left.M31 * right,
                M32 = left.M32 * right,
                M33 = left.M33 * right,
                M34 = left.M34 * right,
                M41 = left.M41 * right,
                M42 = left.M42 * right,
                M43 = left.M43 * right,
                M44 = left.M44 * right
            };
        }

        public static Matrix Multiply(Matrix left, float right)
        {
            return new Matrix()
            {
                M11 = left.M11 * right,
                M12 = left.M12 * right,
                M13 = left.M13 * right,
                M14 = left.M14 * right,
                M21 = left.M21 * right,
                M22 = left.M22 * right,
                M23 = left.M23 * right,
                M24 = left.M24 * right,
                M31 = left.M31 * right,
                M32 = left.M32 * right,
                M33 = left.M33 * right,
                M34 = left.M34 * right,
                M41 = left.M41 * right,
                M42 = left.M42 * right,
                M43 = left.M43 * right,
                M44 = left.M44 * right
            };
        }



        public static void Multiply(ref Matrix left, ref Matrix right, out Matrix result)
        {
            result = new Matrix()
            {
                M11 = (float)((double)left.M12 * (double)right.M21 + (double)left.M11 * (double)right.M11 + (double)left.M13 * (double)right.M31 + (double)left.M14 * (double)right.M41),
                M12 = (float)((double)left.M12 * (double)right.M22 + (double)left.M11 * (double)right.M12 + (double)left.M13 * (double)right.M32 + (double)left.M14 * (double)right.M42),
                M13 = (float)((double)left.M12 * (double)right.M23 + (double)left.M11 * (double)right.M13 + (double)left.M13 * (double)right.M33 + (double)left.M14 * (double)right.M43),
                M14 = (float)((double)left.M12 * (double)right.M24 + (double)left.M11 * (double)right.M14 + (double)left.M13 * (double)right.M34 + (double)left.M14 * (double)right.M44),
                M21 = (float)((double)left.M22 * (double)right.M21 + (double)left.M21 * (double)right.M11 + (double)left.M23 * (double)right.M31 + (double)left.M24 * (double)right.M41),
                M22 = (float)((double)left.M22 * (double)right.M22 + (double)left.M21 * (double)right.M12 + (double)left.M23 * (double)right.M32 + (double)left.M24 * (double)right.M42),
                M23 = (float)((double)left.M22 * (double)right.M23 + (double)left.M21 * (double)right.M13 + (double)left.M23 * (double)right.M33 + (double)left.M24 * (double)right.M43),
                M24 = (float)((double)left.M22 * (double)right.M24 + (double)left.M21 * (double)right.M14 + (double)left.M23 * (double)right.M34 + (double)left.M24 * (double)right.M44),
                M31 = (float)((double)left.M32 * (double)right.M21 + (double)left.M31 * (double)right.M11 + (double)left.M33 * (double)right.M31 + (double)left.M34 * (double)right.M41),
                M32 = (float)((double)left.M32 * (double)right.M22 + (double)left.M31 * (double)right.M12 + (double)left.M33 * (double)right.M32 + (double)left.M34 * (double)right.M42),
                M33 = (float)((double)left.M32 * (double)right.M23 + (double)left.M31 * (double)right.M13 + (double)left.M33 * (double)right.M33 + (double)left.M34 * (double)right.M43),
                M34 = (float)((double)left.M32 * (double)right.M24 + (double)left.M31 * (double)right.M14 + (double)left.M33 * (double)right.M34 + (double)left.M34 * (double)right.M44),
                M41 = (float)((double)left.M42 * (double)right.M21 + (double)left.M41 * (double)right.M11 + (double)left.M43 * (double)right.M31 + (double)left.M44 * (double)right.M41),
                M42 = (float)((double)left.M42 * (double)right.M22 + (double)left.M41 * (double)right.M12 + (double)left.M43 * (double)right.M32 + (double)left.M44 * (double)right.M42),
                M43 = (float)((double)left.M42 * (double)right.M23 + (double)left.M41 * (double)right.M13 + (double)left.M43 * (double)right.M33 + (double)left.M44 * (double)right.M43),
                M44 = (float)((double)left.M42 * (double)right.M24 + (double)left.M41 * (double)right.M14 + (double)left.M43 * (double)right.M34 + (double)left.M44 * (double)right.M44)
            };
        }

        public static Matrix Multiply(Matrix left, Matrix right)
        {
            return new Matrix()
            {
                M11 = (float)((double)right.M21 * (double)left.M12 + (double)left.M11 * (double)right.M11 + (double)right.M31 * (double)left.M13 + (double)right.M41 * (double)left.M14),
                M12 = (float)((double)right.M22 * (double)left.M12 + (double)right.M12 * (double)left.M11 + (double)right.M32 * (double)left.M13 + (double)right.M42 * (double)left.M14),
                M13 = (float)((double)right.M23 * (double)left.M12 + (double)right.M13 * (double)left.M11 + (double)right.M33 * (double)left.M13 + (double)right.M43 * (double)left.M14),
                M14 = (float)((double)right.M24 * (double)left.M12 + (double)right.M14 * (double)left.M11 + (double)right.M34 * (double)left.M13 + (double)right.M44 * (double)left.M14),
                M21 = (float)((double)left.M22 * (double)right.M21 + (double)left.M21 * (double)right.M11 + (double)left.M23 * (double)right.M31 + (double)left.M24 * (double)right.M41),
                M22 = (float)((double)left.M22 * (double)right.M22 + (double)left.M21 * (double)right.M12 + (double)left.M23 * (double)right.M32 + (double)left.M24 * (double)right.M42),
                M23 = (float)((double)right.M23 * (double)left.M22 + (double)right.M13 * (double)left.M21 + (double)right.M33 * (double)left.M23 + (double)left.M24 * (double)right.M43),
                M24 = (float)((double)right.M24 * (double)left.M22 + (double)right.M14 * (double)left.M21 + (double)right.M34 * (double)left.M23 + (double)right.M44 * (double)left.M24),
                M31 = (float)((double)left.M32 * (double)right.M21 + (double)left.M31 * (double)right.M11 + (double)left.M33 * (double)right.M31 + (double)left.M34 * (double)right.M41),
                M32 = (float)((double)left.M32 * (double)right.M22 + (double)left.M31 * (double)right.M12 + (double)left.M33 * (double)right.M32 + (double)left.M34 * (double)right.M42),
                M33 = (float)((double)right.M23 * (double)left.M32 + (double)left.M31 * (double)right.M13 + (double)left.M33 * (double)right.M33 + (double)left.M34 * (double)right.M43),
                M34 = (float)((double)right.M24 * (double)left.M32 + (double)right.M14 * (double)left.M31 + (double)right.M34 * (double)left.M33 + (double)right.M44 * (double)left.M34),
                M41 = (float)((double)left.M42 * (double)right.M21 + (double)left.M41 * (double)right.M11 + (double)left.M43 * (double)right.M31 + (double)left.M44 * (double)right.M41),
                M42 = (float)((double)left.M42 * (double)right.M22 + (double)left.M41 * (double)right.M12 + (double)left.M43 * (double)right.M32 + (double)left.M44 * (double)right.M42),
                M43 = (float)((double)right.M23 * (double)left.M42 + (double)left.M41 * (double)right.M13 + (double)left.M43 * (double)right.M33 + (double)left.M44 * (double)right.M43),
                M44 = (float)((double)right.M24 * (double)left.M42 + (double)left.M41 * (double)right.M14 + (double)right.M34 * (double)left.M43 + (double)left.M44 * (double)right.M44)
            };
        }

        public static void Divide(ref Matrix left, float right, out Matrix result)
        {
            float num = 1f / right;
            result = new Matrix()
            {
                M11 = left.M11 * num,
                M12 = left.M12 * num,
                M13 = left.M13 * num,
                M14 = left.M14 * num,
                M21 = left.M21 * num,
                M22 = left.M22 * num,
                M23 = left.M23 * num,
                M24 = left.M24 * num,
                M31 = left.M31 * num,
                M32 = left.M32 * num,
                M33 = left.M33 * num,
                M34 = left.M34 * num,
                M41 = left.M41 * num,
                M42 = left.M42 * num,
                M43 = left.M43 * num,
                M44 = left.M44 * num
            };
        }

        public static Matrix Divide(Matrix left, float right)
        {
            Matrix matrix = new Matrix();
            float num = 1f / right;
            matrix.M11 = left.M11 * num;
            matrix.M12 = left.M12 * num;
            matrix.M13 = left.M13 * num;
            matrix.M14 = left.M14 * num;
            matrix.M21 = left.M21 * num;
            matrix.M22 = left.M22 * num;
            matrix.M23 = left.M23 * num;
            matrix.M24 = left.M24 * num;
            matrix.M31 = left.M31 * num;
            matrix.M32 = left.M32 * num;
            matrix.M33 = left.M33 * num;
            matrix.M34 = left.M34 * num;
            matrix.M41 = left.M41 * num;
            matrix.M42 = left.M42 * num;
            matrix.M43 = left.M43 * num;
            matrix.M44 = left.M44 * num;
            return matrix;
        }

        public static void Divide(ref Matrix left, ref Matrix right, out Matrix result)
        {
            result = new Matrix()
            {
                M11 = left.M11 / right.M11,
                M12 = left.M12 / right.M12,
                M13 = left.M13 / right.M13,
                M14 = left.M14 / right.M14,
                M21 = left.M21 / right.M21,
                M22 = left.M22 / right.M22,
                M23 = left.M23 / right.M23,
                M24 = left.M24 / right.M24,
                M31 = left.M31 / right.M31,
                M32 = left.M32 / right.M32,
                M33 = left.M33 / right.M33,
                M34 = left.M34 / right.M34,
                M41 = left.M41 / right.M41,
                M42 = left.M42 / right.M42,
                M43 = left.M43 / right.M43,
                M44 = left.M44 / right.M44
            };
        }

        public static Matrix Divide(Matrix left, Matrix right)
        {
            return new Matrix()
            {
                M11 = left.M11 / right.M11,
                M12 = left.M12 / right.M12,
                M13 = left.M13 / right.M13,
                M14 = left.M14 / right.M14,
                M21 = left.M21 / right.M21,
                M22 = left.M22 / right.M22,
                M23 = left.M23 / right.M23,
                M24 = left.M24 / right.M24,
                M31 = left.M31 / right.M31,
                M32 = left.M32 / right.M32,
                M33 = left.M33 / right.M33,
                M34 = left.M34 / right.M34,
                M41 = left.M41 / right.M41,
                M42 = left.M42 / right.M42,
                M43 = left.M43 / right.M43,
                M44 = left.M44 / right.M44
            };
        }

        public static void Negate(ref Matrix matrix, out Matrix result)
        {
            result = new Matrix()
            {
                M11 = -matrix.M11,
                M12 = -matrix.M12,
                M13 = -matrix.M13,
                M14 = -matrix.M14,
                M21 = -matrix.M21,
                M22 = -matrix.M22,
                M23 = -matrix.M23,
                M24 = -matrix.M24,
                M31 = -matrix.M31,
                M32 = -matrix.M32,
                M33 = -matrix.M33,
                M34 = -matrix.M34,
                M41 = -matrix.M41,
                M42 = -matrix.M42,
                M43 = -matrix.M43,
                M44 = -matrix.M44
            };
        }

        public static Matrix Negate(Matrix matrix)
        {
            return new Matrix()
            {
                M11 = -matrix.M11,
                M12 = -matrix.M12,
                M13 = -matrix.M13,
                M14 = -matrix.M14,
                M21 = -matrix.M21,
                M22 = -matrix.M22,
                M23 = -matrix.M23,
                M24 = -matrix.M24,
                M31 = -matrix.M31,
                M32 = -matrix.M32,
                M33 = -matrix.M33,
                M34 = -matrix.M34,
                M41 = -matrix.M41,
                M42 = -matrix.M42,
                M43 = -matrix.M43,
                M44 = -matrix.M44
            };
        }

        public static void Lerp(ref Matrix start, ref Matrix end, float amount, out Matrix result)
        {
            result = new Matrix()
            {
                M11 = (end.M11 - start.M11) * amount + start.M11,
                M12 = (end.M12 - start.M12) * amount + start.M12,
                M13 = (end.M13 - start.M13) * amount + start.M13,
                M14 = (end.M14 - start.M14) * amount + start.M14,
                M21 = (end.M21 - start.M21) * amount + start.M21,
                M22 = (end.M22 - start.M22) * amount + start.M22,
                M23 = (end.M23 - start.M23) * amount + start.M23,
                M24 = (end.M24 - start.M24) * amount + start.M24,
                M31 = (end.M31 - start.M31) * amount + start.M31,
                M32 = (end.M32 - start.M32) * amount + start.M32,
                M33 = (end.M33 - start.M33) * amount + start.M33,
                M34 = (end.M34 - start.M34) * amount + start.M34,
                M41 = (end.M41 - start.M41) * amount + start.M41,
                M42 = (end.M42 - start.M42) * amount + start.M42,
                M43 = (end.M43 - start.M43) * amount + start.M43,
                M44 = (end.M44 - start.M44) * amount + start.M44
            };
        }

        public static Matrix Lerp(Matrix start, Matrix end, float amount)
        {
            return new Matrix()
            {
                M11 = (end.M11 - start.M11) * amount + start.M11,
                M12 = (end.M12 - start.M12) * amount + start.M12,
                M13 = (end.M13 - start.M13) * amount + start.M13,
                M14 = (end.M14 - start.M14) * amount + start.M14,
                M21 = (end.M21 - start.M21) * amount + start.M21,
                M22 = (end.M22 - start.M22) * amount + start.M22,
                M23 = (end.M23 - start.M23) * amount + start.M23,
                M24 = (end.M24 - start.M24) * amount + start.M24,
                M31 = (end.M31 - start.M31) * amount + start.M31,
                M32 = (end.M32 - start.M32) * amount + start.M32,
                M33 = (end.M33 - start.M33) * amount + start.M33,
                M34 = (end.M34 - start.M34) * amount + start.M34,
                M41 = (end.M41 - start.M41) * amount + start.M41,
                M42 = (end.M42 - start.M42) * amount + start.M42,
                M43 = (end.M43 - start.M43) * amount + start.M43,
                M44 = (end.M44 - start.M44) * amount + start.M44
            };
        }

        public static void Billboard(ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 cameraUpVector, ref Vector3 cameraForwardVector, out Matrix result)
        {
            Vector3 vector3_1 = objectPosition - cameraPosition;
            Vector3 result1 = new Vector3();
            Vector3 result2 = new Vector3();
            float num = vector3_1.LengthSquared();
            Vector3 vector3_2 = (double)num >= 9.99999974737875E-05 ? vector3_1 * (float)(1.0 / System.Math.Sqrt((double)num)) : -cameraForwardVector;
            Vector3.Cross(ref cameraUpVector, ref vector3_2, out result1);
            result1.Normalize();
            Vector3.Cross(ref vector3_2, ref result1, out result2);
            result.M11 = result1.X;
            result.M12 = result1.Y;
            result.M13 = result1.Z;
            result.M14 = 0.0f;
            result.M21 = result2.X;
            result.M22 = result2.Y;
            result.M23 = result2.Z;
            result.M24 = 0.0f;
            result.M31 = vector3_2.X;
            result.M32 = vector3_2.Y;
            result.M33 = vector3_2.Z;
            result.M34 = 0.0f;
            result.M41 = objectPosition.X;
            result.M42 = objectPosition.Y;
            result.M43 = objectPosition.Z;
            result.M44 = 1f;
        }

        public static Matrix Billboard(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3 cameraForwardVector)
        {
            Matrix matrix = new Matrix();
            Vector3 vector3_1 = objectPosition - cameraPosition;
            Vector3 result1 = new Vector3();
            Vector3 result2 = new Vector3();
            float num = vector3_1.LengthSquared();
            Vector3 vector3_2 = (double)num >= 9.99999974737875E-05 ? vector3_1 * (float)(1.0 / System.Math.Sqrt((double)num)) : -cameraForwardVector;
            Vector3.Cross(ref cameraUpVector, ref vector3_2, out result1);
            result1.Normalize();
            Vector3.Cross(ref vector3_2, ref result1, out result2);
            matrix.M11 = result1.X;
            matrix.M12 = result1.Y;
            matrix.M13 = result1.Z;
            matrix.M14 = 0.0f;
            matrix.M21 = result2.X;
            matrix.M22 = result2.Y;
            matrix.M23 = result2.Z;
            matrix.M24 = 0.0f;
            matrix.M31 = vector3_2.X;
            matrix.M32 = vector3_2.Y;
            matrix.M33 = vector3_2.Z;
            matrix.M34 = 0.0f;
            matrix.M41 = objectPosition.X;
            matrix.M42 = objectPosition.Y;
            matrix.M43 = objectPosition.Z;
            matrix.M44 = 1f;
            return matrix;
        }

        public static void RotationX(float angle, out Matrix result)
        {
            float num1 = (float)System.Math.Cos((double)angle);
            float num2 = (float)System.Math.Sin((double)angle);
            result.M11 = 1f;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;
            result.M21 = 0.0f;
            result.M22 = num1;
            result.M23 = num2;
            result.M24 = 0.0f;
            result.M31 = 0.0f;
            result.M32 = -num2;
            result.M33 = num1;
            result.M34 = 0.0f;
            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1f;
        }

        public static Matrix RotationX(float angle)
        {
            Matrix matrix = new Matrix();
            float num1 = (float)System.Math.Cos((double)angle);
            float num2 = (float)System.Math.Sin((double)angle);
            matrix.M11 = 1f;
            matrix.M12 = 0.0f;
            matrix.M13 = 0.0f;
            matrix.M14 = 0.0f;
            matrix.M21 = 0.0f;
            matrix.M22 = num1;
            matrix.M23 = num2;
            matrix.M24 = 0.0f;
            matrix.M31 = 0.0f;
            matrix.M32 = -num2;
            matrix.M33 = num1;
            matrix.M34 = 0.0f;
            matrix.M41 = 0.0f;
            matrix.M42 = 0.0f;
            matrix.M43 = 0.0f;
            matrix.M44 = 1f;
            return matrix;
        }

        public static void RotationY(float angle, out Matrix result)
        {
            float num1 = (float)System.Math.Cos((double)angle);
            float num2 = (float)System.Math.Sin((double)angle);
            result.M11 = num1;
            result.M12 = 0.0f;
            result.M13 = -num2;
            result.M14 = 0.0f;
            result.M21 = 0.0f;
            result.M22 = 1f;
            result.M23 = 0.0f;
            result.M24 = 0.0f;
            result.M31 = num2;
            result.M32 = 0.0f;
            result.M33 = num1;
            result.M34 = 0.0f;
            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1f;
        }

        public static Matrix RotationY(float angle)
        {
            Matrix matrix = new Matrix();
            float num1 = (float)System.Math.Cos((double)angle);
            float num2 = (float)System.Math.Sin((double)angle);
            matrix.M11 = num1;
            matrix.M12 = 0.0f;
            matrix.M13 = -num2;
            matrix.M14 = 0.0f;
            matrix.M21 = 0.0f;
            matrix.M22 = 1f;
            matrix.M23 = 0.0f;
            matrix.M24 = 0.0f;
            matrix.M31 = num2;
            matrix.M32 = 0.0f;
            matrix.M33 = num1;
            matrix.M34 = 0.0f;
            matrix.M41 = 0.0f;
            matrix.M42 = 0.0f;
            matrix.M43 = 0.0f;
            matrix.M44 = 1f;
            return matrix;
        }

        public static void RotationZ(float angle, out Matrix result)
        {
            float num1 = (float)System.Math.Cos((double)angle);
            float num2 = (float)System.Math.Sin((double)angle);
            result.M11 = num1;
            result.M12 = num2;
            result.M13 = 0.0f;
            result.M14 = 0.0f;
            result.M21 = -num2;
            result.M22 = num1;
            result.M23 = 0.0f;
            result.M24 = 0.0f;
            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = 1f;
            result.M34 = 0.0f;
            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1f;
        }

        public static Matrix RotationZ(float angle)
        {
            Matrix matrix = new Matrix();
            float num1 = (float)System.Math.Cos((double)angle);
            float num2 = (float)System.Math.Sin((double)angle);
            matrix.M11 = num1;
            matrix.M12 = num2;
            matrix.M13 = 0.0f;
            matrix.M14 = 0.0f;
            matrix.M21 = -num2;
            matrix.M22 = num1;
            matrix.M23 = 0.0f;
            matrix.M24 = 0.0f;
            matrix.M31 = 0.0f;
            matrix.M32 = 0.0f;
            matrix.M33 = 1f;
            matrix.M34 = 0.0f;
            matrix.M41 = 0.0f;
            matrix.M42 = 0.0f;
            matrix.M43 = 0.0f;
            matrix.M44 = 1f;
            return matrix;
        }

        public static void RotationAxis(ref Vector3 axis, float angle, out Matrix result)
        {
            if ((double)axis.LengthSquared() != 1.0)
                axis.Normalize();
            float num1 = axis.X;
            float num2 = axis.Y;
            float num3 = axis.Z;
            float num4 = (float)System.Math.Cos((double)angle);
            float num5 = (float)System.Math.Sin((double)angle);
            double num6 = (double)num1;
            float num7 = (float)(num6 * num6);
            double num8 = (double)num2;
            float num9 = (float)(num8 * num8);
            double num10 = (double)num3;
            float num11 = (float)(num10 * num10);
            float num12 = num2 * num1;
            float num13 = num3 * num1;
            float num14 = num3 * num2;
            result.M11 = (1f - num7) * num4 + num7;
            double num15 = (double)num12;
            double num16 = num15 - (double)num4 * num15;
            double num17 = (double)num5 * (double)num3;
            result.M12 = (float)(num17 + num16);
            double num18 = (double)num13;
            double num19 = num18 - (double)num4 * num18;
            double num20 = (double)num5 * (double)num2;
            result.M13 = (float)(num19 - num20);
            result.M14 = 0.0f;
            result.M21 = (float)(num16 - num17);
            result.M22 = (1f - num9) * num4 + num9;
            double num21 = (double)num14;
            double num22 = num21 - (double)num4 * num21;
            double num23 = (double)num5 * (double)num1;
            result.M23 = (float)(num23 + num22);
            result.M24 = 0.0f;
            result.M31 = (float)(num20 + num19);
            result.M32 = (float)(num22 - num23);
            result.M33 = (1f - num11) * num4 + num11;
            result.M34 = 0.0f;
            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1f;
        }

        public static Matrix RotationAxis(Vector3 axis, float angle)
        {
            if ((double)axis.LengthSquared() != 1.0)
                axis.Normalize();
            Matrix matrix = new Matrix();
            float num1 = axis.X;
            float num2 = axis.Y;
            float num3 = axis.Z;
            float num4 = (float)System.Math.Cos((double)angle);
            float num5 = (float)System.Math.Sin((double)angle);
            double num6 = (double)num1;
            float num7 = (float)(num6 * num6);
            double num8 = (double)num2;
            float num9 = (float)(num8 * num8);
            double num10 = (double)num3;
            float num11 = (float)(num10 * num10);
            float num12 = num2 * num1;
            float num13 = num3 * num1;
            float num14 = num3 * num2;
            matrix.M11 = (1f - num7) * num4 + num7;
            double num15 = (double)num12;
            double num16 = num15 - (double)num4 * num15;
            double num17 = (double)num5 * (double)num3;
            matrix.M12 = (float)(num17 + num16);
            double num18 = (double)num13;
            double num19 = num18 - (double)num4 * num18;
            double num20 = (double)num5 * (double)num2;
            matrix.M13 = (float)(num19 - num20);
            matrix.M14 = 0.0f;
            matrix.M21 = (float)(num16 - num17);
            matrix.M22 = (1f - num9) * num4 + num9;
            double num21 = (double)num14;
            double num22 = num21 - (double)num4 * num21;
            double num23 = (double)num5 * (double)num1;
            matrix.M23 = (float)(num23 + num22);
            matrix.M24 = 0.0f;
            matrix.M31 = (float)(num20 + num19);
            matrix.M32 = (float)(num22 - num23);
            matrix.M33 = (1f - num11) * num4 + num11;
            matrix.M34 = 0.0f;
            matrix.M41 = 0.0f;
            matrix.M42 = 0.0f;
            matrix.M43 = 0.0f;
            matrix.M44 = 1f;
            return matrix;
        }


        public static void Invert(ref Matrix matrix, out Matrix result)
        {
            float num17 = (float)((double)matrix.M33 * (double)matrix.M44 - (double)matrix.M34 * (double)matrix.M43);
            float num18 = (float)((double)matrix.M32 * (double)matrix.M44 - (double)matrix.M34 * (double)matrix.M42);
            float num19 = (float)((double)matrix.M32 * (double)matrix.M43 - (double)matrix.M33 * (double)matrix.M42);
            float num20 = (float)((double)matrix.M31 * (double)matrix.M44 - (double)matrix.M34 * (double)matrix.M41);
            float num21 = (float)((double)matrix.M31 * (double)matrix.M43 - (double)matrix.M33 * (double)matrix.M41);
            float num22 = (float)((double)matrix.M31 * (double)matrix.M42 - (double)matrix.M32 * (double)matrix.M41);
            float num23 = (float)((double)matrix.M22 * (double)num17 - (double)matrix.M23 * (double)num18 + (double)matrix.M24 * (double)num19);
            float num24 = (float)-((double)matrix.M21 * (double)num17 - (double)matrix.M23 * (double)num20 + (double)matrix.M24 * (double)num21);
            float num25 = (float)((double)matrix.M21 * (double)num18 - (double)matrix.M22 * (double)num20 + (double)matrix.M24 * (double)num22);
            float num26 = (float)-((double)matrix.M21 * (double)num19 - (double)matrix.M22 * (double)num21 + (double)matrix.M23 * (double)num22);
            float num27 = (float)(1.0 / ((double)matrix.M11 * (double)num23 + (double)matrix.M12 * (double)num24 + (double)matrix.M13 * (double)num25 + (double)matrix.M14 * (double)num26));
            result.M11 = num23 * num27;
            result.M21 = num24 * num27;
            result.M31 = num25 * num27;
            result.M41 = num26 * num27;
            result.M12 = (float)-((double)matrix.M12 * (double)num17 - (double)matrix.M13 * (double)num18 + (double)matrix.M14 * (double)num19) * num27;
            result.M22 = (float)((double)matrix.M11 * (double)num17 - (double)matrix.M13 * (double)num20 + (double)matrix.M14 * (double)num21) * num27;
            result.M32 = (float)-((double)matrix.M11 * (double)num18 - (double)matrix.M12 * (double)num20 + (double)matrix.M14 * (double)num22) * num27;
            result.M42 = (float)((double)matrix.M11 * (double)num19 - (double)matrix.M12 * (double)num21 + (double)matrix.M13 * (double)num22) * num27;
            float num28 = (float)((double)matrix.M23 * (double)matrix.M44 - (double)matrix.M24 * (double)matrix.M43);
            float num29 = (float)((double)matrix.M22 * (double)matrix.M44 - (double)matrix.M24 * (double)matrix.M42);
            float num30 = (float)((double)matrix.M22 * (double)matrix.M43 - (double)matrix.M23 * (double)matrix.M42);
            float num31 = (float)((double)matrix.M21 * (double)matrix.M44 - (double)matrix.M24 * (double)matrix.M41);
            float num32 = (float)((double)matrix.M21 * (double)matrix.M43 - (double)matrix.M23 * (double)matrix.M41);
            float num33 = (float)((double)matrix.M21 * (double)matrix.M42 - (double)matrix.M22 * (double)matrix.M41);
            result.M13 = (float)((double)matrix.M12 * (double)num28 - (double)matrix.M13 * (double)num29 + (double)matrix.M14 * (double)num30) * num27;
            result.M23 = (float)-((double)matrix.M11 * (double)num28 - (double)matrix.M13 * (double)num31 + (double)matrix.M14 * (double)num32) * num27;
            result.M33 = (float)((double)matrix.M11 * (double)num29 - (double)matrix.M12 * (double)num31 + (double)matrix.M14 * (double)num33) * num27;
            result.M43 = (float)-((double)matrix.M11 * (double)num30 - (double)matrix.M12 * (double)num32 + (double)matrix.M13 * (double)num33) * num27;
            float num34 = (float)((double)matrix.M23 * (double)matrix.M34 - (double)matrix.M24 * (double)matrix.M33);
            float num35 = (float)((double)matrix.M22 * (double)matrix.M34 - (double)matrix.M24 * (double)matrix.M32);
            float num36 = (float)((double)matrix.M22 * (double)matrix.M33 - (double)matrix.M23 * (double)matrix.M32);
            float num37 = (float)((double)matrix.M21 * (double)matrix.M34 - (double)matrix.M24 * (double)matrix.M31);
            float num38 = (float)((double)matrix.M21 * (double)matrix.M33 - (double)matrix.M23 * (double)matrix.M31);
            float num39 = (float)((double)matrix.M21 * (double)matrix.M32 - (double)matrix.M22 * (double)matrix.M31);
            result.M14 = (float)-((double)matrix.M12 * (double)num34 - (double)matrix.M13 * (double)num35 + (double)matrix.M14 * (double)num36) * num27;
            result.M24 = (float)((double)matrix.M11 * (double)num34 - (double)matrix.M13 * (double)num37 + (double)matrix.M14 * (double)num38) * num27;
            result.M34 = (float)-((double)matrix.M11 * (double)num35 - (double)matrix.M12 * (double)num37 + (double)matrix.M14 * (double)num39) * num27;
            result.M44 = (float)((double)matrix.M11 * (double)num36 - (double)matrix.M12 * (double)num38 + (double)matrix.M13 * (double)num39) * num27;
        }



        public static void Reflection(ref Plane plane, out Matrix result)
        {
            plane.Normalize();
            float num1 = plane.Normal.X;
            float num2 = plane.Normal.Y;
            float num3 = plane.Normal.Z;
            float num4 = num1 * -2f;
            float num5 = num2 * -2f;
            float num6 = num3 * -2f;
            result.M11 = (float)((double)num4 * (double)num1 + 1.0);
            result.M12 = num5 * num1;
            result.M13 = num6 * num1;
            result.M14 = 0.0f;
            result.M21 = num4 * num2;
            result.M22 = (float)((double)num5 * (double)num2 + 1.0);
            result.M23 = num6 * num2;
            result.M24 = 0.0f;
            result.M31 = num4 * num3;
            result.M32 = num5 * num3;
            result.M33 = (float)((double)num6 * (double)num3 + 1.0);
            result.M34 = 0.0f;
            result.M41 = plane.D * num4;
            result.M42 = plane.D * num5;
            result.M43 = plane.D * num6;
            result.M44 = 1f;
        }

        public static Matrix Reflection(Plane plane)
        {
            Matrix matrix = new Matrix();
            plane.Normalize();
            float num1 = plane.Normal.X;
            float num2 = plane.Normal.Y;
            float num3 = plane.Normal.Z;
            float num4 = num1 * -2f;
            float num5 = num2 * -2f;
            float num6 = num3 * -2f;
            matrix.M11 = (float)((double)num4 * (double)num1 + 1.0);
            matrix.M12 = num5 * num1;
            matrix.M13 = num6 * num1;
            matrix.M14 = 0.0f;
            matrix.M21 = num4 * num2;
            matrix.M22 = (float)((double)num5 * (double)num2 + 1.0);
            matrix.M23 = num6 * num2;
            matrix.M24 = 0.0f;
            matrix.M31 = num4 * num3;
            matrix.M32 = num5 * num3;
            matrix.M33 = (float)((double)num6 * (double)num3 + 1.0);
            matrix.M34 = 0.0f;
            matrix.M41 = plane.D * num4;
            matrix.M42 = plane.D * num5;
            matrix.M43 = plane.D * num6;
            matrix.M44 = 1f;
            return matrix;
        }

        public static void Scaling(ref Vector3 scale, out Matrix result)
        {
            result.M11 = scale.X;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;
            result.M21 = 0.0f;
            result.M22 = scale.Y;
            result.M23 = 0.0f;
            result.M24 = 0.0f;
            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = scale.Z;
            result.M34 = 0.0f;
            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1f;
        }

        public static Matrix Scaling(Vector3 scale)
        {
            return new Matrix()
            {
                M11 = scale.X,
                M12 = 0.0f,
                M13 = 0.0f,
                M14 = 0.0f,
                M21 = 0.0f,
                M22 = scale.Y,
                M23 = 0.0f,
                M24 = 0.0f,
                M31 = 0.0f,
                M32 = 0.0f,
                M33 = scale.Z,
                M34 = 0.0f,
                M41 = 0.0f,
                M42 = 0.0f,
                M43 = 0.0f,
                M44 = 1f
            };
        }

        public static void Scaling(float x, float y, float z, out Matrix result)
        {
            result.M11 = x;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;
            result.M21 = 0.0f;
            result.M22 = y;
            result.M23 = 0.0f;
            result.M24 = 0.0f;
            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = z;
            result.M34 = 0.0f;
            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1f;
        }

        public static Matrix Scaling(float x, float y, float z)
        {
            return new Matrix()
            {
                M11 = x,
                M12 = 0.0f,
                M13 = 0.0f,
                M14 = 0.0f,
                M21 = 0.0f,
                M22 = y,
                M23 = 0.0f,
                M24 = 0.0f,
                M31 = 0.0f,
                M32 = 0.0f,
                M33 = z,
                M34 = 0.0f,
                M41 = 0.0f,
                M42 = 0.0f,
                M43 = 0.0f,
                M44 = 1f
            };
        }

        public static void Shadow(ref Vector4 light, ref Plane plane, out Matrix result)
        {
            plane.Normalize();
            float num1 = (float)((double)plane.Normal.Y * (double)light.Y + (double)plane.Normal.X * (double)light.X + (double)plane.Normal.Z * (double)light.Z);
            float num2 = -plane.Normal.X;
            float num3 = -plane.Normal.Y;
            float num4 = -plane.Normal.Z;
            float num5 = -plane.D;
            result.M11 = light.X * num2 + num1;
            result.M21 = light.X * num3;
            result.M31 = light.X * num4;
            result.M41 = light.X * num5;
            result.M12 = light.Y * num2;
            result.M22 = light.Y * num3 + num1;
            result.M32 = light.Y * num4;
            result.M42 = light.Y * num5;
            result.M13 = light.Z * num2;
            result.M23 = light.Z * num3;
            result.M33 = light.Z * num4 + num1;
            result.M43 = light.Z * num5;
            result.M14 = 0.0f;
            result.M24 = 0.0f;
            result.M34 = 0.0f;
            result.M44 = num1;
        }

        public static Matrix Shadow(Vector4 light, Plane plane)
        {
            Matrix matrix = new Matrix();
            plane.Normalize();
            float num1 = (float)((double)plane.Normal.Y * (double)light.Y + (double)plane.Normal.X * (double)light.X + (double)plane.Normal.Z * (double)light.Z);
            float num2 = -plane.Normal.X;
            float num3 = -plane.Normal.Y;
            float num4 = -plane.Normal.Z;
            float num5 = -plane.D;
            matrix.M11 = light.X * num2 + num1;
            matrix.M21 = light.X * num3;
            matrix.M31 = light.X * num4;
            matrix.M41 = light.X * num5;
            matrix.M12 = light.Y * num2;
            matrix.M22 = light.Y * num3 + num1;
            matrix.M32 = light.Y * num4;
            matrix.M42 = light.Y * num5;
            matrix.M13 = light.Z * num2;
            matrix.M23 = light.Z * num3;
            matrix.M33 = light.Z * num4 + num1;
            matrix.M43 = light.Z * num5;
            matrix.M14 = 0.0f;
            matrix.M24 = 0.0f;
            matrix.M34 = 0.0f;
            matrix.M44 = num1;
            return matrix;
        }

        public static void Translation(ref Vector3 amount, out Matrix result)
        {
            result.M11 = 1f;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;
            result.M21 = 0.0f;
            result.M22 = 1f;
            result.M23 = 0.0f;
            result.M24 = 0.0f;
            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = 1f;
            result.M34 = 0.0f;
            result.M41 = amount.X;
            result.M42 = amount.Y;
            result.M43 = amount.Z;
            result.M44 = 1f;
        }

        public static Matrix Translation(Vector3 amount)
        {
            return new Matrix()
            {
                M11 = 1f,
                M12 = 0.0f,
                M13 = 0.0f,
                M14 = 0.0f,
                M21 = 0.0f,
                M22 = 1f,
                M23 = 0.0f,
                M24 = 0.0f,
                M31 = 0.0f,
                M32 = 0.0f,
                M33 = 1f,
                M34 = 0.0f,
                M41 = amount.X,
                M42 = amount.Y,
                M43 = amount.Z,
                M44 = 1f
            };
        }

        public static void Translation(float x, float y, float z, out Matrix result)
        {
            result.M11 = 1f;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;
            result.M21 = 0.0f;
            result.M22 = 1f;
            result.M23 = 0.0f;
            result.M24 = 0.0f;
            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = 1f;
            result.M34 = 0.0f;
            result.M41 = x;
            result.M42 = y;
            result.M43 = z;
            result.M44 = 1f;
        }

        public static Matrix Translation(float x, float y, float z)
        {
            return new Matrix()
            {
                M11 = 1f,
                M12 = 0.0f,
                M13 = 0.0f,
                M14 = 0.0f,
                M21 = 0.0f,
                M22 = 1f,
                M23 = 0.0f,
                M24 = 0.0f,
                M31 = 0.0f,
                M32 = 0.0f,
                M33 = 1f,
                M34 = 0.0f,
                M41 = x,
                M42 = y,
                M43 = z,
                M44 = 1f
            };
        }

        public static void Transpose(ref Matrix matrix, out Matrix result)
        {
            result = new Matrix()
            {
                M11 = matrix.M11,
                M12 = matrix.M21,
                M13 = matrix.M31,
                M14 = matrix.M41,
                M21 = matrix.M12,
                M22 = matrix.M22,
                M23 = matrix.M32,
                M24 = matrix.M42,
                M31 = matrix.M13,
                M32 = matrix.M23,
                M33 = matrix.M33,
                M34 = matrix.M43,
                M41 = matrix.M14,
                M42 = matrix.M24,
                M43 = matrix.M34,
                M44 = matrix.M44
            };
        }

        public static Matrix Transpose(Matrix matrix)
        {
            return new Matrix()
            {
                M11 = matrix.M11,
                M12 = matrix.M21,
                M13 = matrix.M31,
                M14 = matrix.M41,
                M21 = matrix.M12,
                M22 = matrix.M22,
                M23 = matrix.M32,
                M24 = matrix.M42,
                M31 = matrix.M13,
                M32 = matrix.M23,
                M33 = matrix.M33,
                M34 = matrix.M43,
                M41 = matrix.M14,
                M42 = matrix.M24,
                M43 = matrix.M34,
                M44 = matrix.M44
            };
        }



        public override  string ToString()
        {
            object[] objArray = new object[16];
            float num1 = this.M11;
            objArray[0] = (object)num1.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num2 = this.M12;
            objArray[1] = (object)num2.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num3 = this.M13;
            objArray[2] = (object)num3.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num4 = this.M14;
            objArray[3] = (object)num4.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num5 = this.M21;
            objArray[4] = (object)num5.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num6 = this.M22;
            objArray[5] = (object)num6.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num7 = this.M23;
            objArray[6] = (object)num7.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num8 = this.M24;
            objArray[7] = (object)num8.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num9 = this.M31;
            objArray[8] = (object)num9.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num10 = this.M32;
            objArray[9] = (object)num10.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num11 = this.M33;
            objArray[10] = (object)num11.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num12 = this.M34;
            objArray[11] = (object)num12.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num13 = this.M41;
            objArray[12] = (object)num13.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num14 = this.M42;
            objArray[13] = (object)num14.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num15 = this.M43;
            objArray[14] = (object)num15.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num16 = this.M44;
            objArray[15] = (object)num16.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, "[[M11:{0} M12:{1} M13:{2} M14:{3}] [M21:{4} M22:{5} M23:{6} M24:{7}] [M31:{8} M32:{9} M33:{10} M34:{11}] [M41:{12} M42:{13} M43:{14} M44:{15}]]", objArray);
        }

        public override  int GetHashCode()
        {
            return this.M11.GetHashCode() + (this.M43.GetHashCode() + this.M44.GetHashCode() + this.M42.GetHashCode() + this.M41.GetHashCode() + this.M34.GetHashCode() + this.M33.GetHashCode() + this.M32.GetHashCode() + this.M31.GetHashCode() + this.M24.GetHashCode() + this.M23.GetHashCode() + this.M22.GetHashCode() + this.M21.GetHashCode() + this.M14.GetHashCode() + this.M13.GetHashCode() + this.M12.GetHashCode());
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool Equals(ref Matrix value1, ref Matrix value2)
        {
            return (double)value1.M11 == (double)value2.M11 && (double)value1.M12 == (double)value2.M12 && ((double)value1.M13 == (double)value2.M13 && (double)value1.M14 == (double)value2.M14) && ((double)value1.M21 == (double)value2.M21 && (double)value1.M22 == (double)value2.M22 && ((double)value1.M23 == (double)value2.M23 && (double)value1.M24 == (double)value2.M24)) && ((double)value1.M31 == (double)value2.M31 && (double)value1.M32 == (double)value2.M32 && ((double)value1.M33 == (double)value2.M33 && (double)value1.M34 == (double)value2.M34) && ((double)value1.M41 == (double)value2.M41 && (double)value1.M42 == (double)value2.M42 && ((double)value1.M43 == (double)value2.M43 && (double)value1.M44 == (double)value2.M44)));
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool Equals(Matrix other)
        {
            return (double)this.M11 == (double)other.M11 && (double)this.M12 == (double)other.M12 && ((double)this.M13 == (double)other.M13 && (double)this.M14 == (double)other.M14) && ((double)this.M21 == (double)other.M21 && (double)this.M22 == (double)other.M22 && ((double)this.M23 == (double)other.M23 && (double)this.M24 == (double)other.M24)) && ((double)this.M31 == (double)other.M31 && (double)this.M32 == (double)other.M32 && ((double)this.M33 == (double)other.M33 && (double)this.M34 == (double)other.M34) && ((double)this.M41 == (double)other.M41 && (double)this.M42 == (double)other.M42 && ((double)this.M43 == (double)other.M43 && (double)this.M44 == (double)other.M44)));
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override  bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;
            else
                return this.Equals((Matrix)obj);
        }
    }

}
