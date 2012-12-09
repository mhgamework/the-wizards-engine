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
                return M11 == 1.0 && M22 == 1.0 && (M33 == 1.0 && M44 == 1.0) &&
                       (M12 == 0.0 && M13 == 0.0 && (M14 == 0.0 && M21 == 0.0)) &&
                       (M23 == 0.0 && M24 == 0.0 && (M31 == 0.0 && M32 == 0.0) &&
                        (M34 == 0.0 && M41 == 0.0 && (M42 == 0.0 && M43 == 0.0)));
            }
        }

        public static Matrix Identity
        {
            get
            {
                return new Matrix
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
                    throw new ArgumentOutOfRangeException();
                if ((uint)column > 3U)
                    throw new ArgumentOutOfRangeException();
                switch (row * 4 + column)
                {
                    case 0:
                        return M11;
                    case 1:
                        return M12;
                    case 2:
                        return M13;
                    case 3:
                        return M14;
                    case 4:
                        return M21;
                    case 5:
                        return M22;
                    case 6:
                        return M23;
                    case 7:
                        return M24;
                    case 8:
                        return M31;
                    case 9:
                        return M32;
                    case 10:
                        return M33;
                    case 11:
                        return M34;
                    case 12:
                        return M41;
                    case 13:
                        return M42;
                    case 14:
                        return M43;
                    case 15:
                        return M44;
                    default:
                        return 0.0f;
                }
            }
            set
            {
                if ((uint)row > 3U)
                    throw new ArgumentOutOfRangeException();
                if ((uint)column > 3U)
                    throw new ArgumentOutOfRangeException();
                switch (row * 4 + column)
                {
                    case 0:
                        M11 = value;
                        break;
                    case 1:
                        M12 = value;
                        break;
                    case 2:
                        M13 = value;
                        break;
                    case 3:
                        M14 = value;
                        break;
                    case 4:
                        M21 = value;
                        break;
                    case 5:
                        M22 = value;
                        break;
                    case 6:
                        M23 = value;
                        break;
                    case 7:
                        M24 = value;
                        break;
                    case 8:
                        M31 = value;
                        break;
                    case 9:
                        M32 = value;
                        break;
                    case 10:
                        M33 = value;
                        break;
                    case 11:
                        M34 = value;
                        break;
                    case 12:
                        M41 = value;
                        break;
                    case 13:
                        M42 = value;
                        break;
                    case 14:
                        M43 = value;
                        break;
                    case 15:
                        M44 = value;
                        break;
                }
            }
        }

        public static Matrix operator -(Matrix left, Matrix right)
        {
            return new Matrix
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
            return new Matrix
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
            return new Matrix
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
            return new Matrix
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
            return new Matrix
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
            return new Matrix
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
            return new Matrix
                       {
                           M11 =
                               (float)
                               (right.M21 * (double)left.M12 + left.M11 * (double)right.M11 + right.M31 * (double)left.M13 +
                                right.M41 * (double)left.M14),
                           M12 =
                               (float)
                               (right.M22 * (double)left.M12 + right.M12 * (double)left.M11 + right.M32 * (double)left.M13 +
                                right.M42 * (double)left.M14),
                           M13 =
                               (float)
                               (right.M23 * (double)left.M12 + right.M13 * (double)left.M11 + right.M33 * (double)left.M13 +
                                right.M43 * (double)left.M14),
                           M14 =
                               (float)
                               (right.M24 * (double)left.M12 + right.M14 * (double)left.M11 + right.M34 * (double)left.M13 +
                                right.M44 * (double)left.M14),
                           M21 =
                               (float)
                               (left.M22 * (double)right.M21 + left.M21 * (double)right.M11 + left.M23 * (double)right.M31 +
                                left.M24 * (double)right.M41),
                           M22 =
                               (float)
                               (left.M22 * (double)right.M22 + left.M21 * (double)right.M12 + left.M23 * (double)right.M32 +
                                left.M24 * (double)right.M42),
                           M23 =
                               (float)
                               (right.M23 * (double)left.M22 + right.M13 * (double)left.M21 + right.M33 * (double)left.M23 +
                                left.M24 * (double)right.M43),
                           M24 =
                               (float)
                               (right.M24 * (double)left.M22 + right.M14 * (double)left.M21 + right.M34 * (double)left.M23 +
                                right.M44 * (double)left.M24),
                           M31 =
                               (float)
                               (left.M32 * (double)right.M21 + left.M31 * (double)right.M11 + left.M33 * (double)right.M31 +
                                left.M34 * (double)right.M41),
                           M32 =
                               (float)
                               (left.M32 * (double)right.M22 + left.M31 * (double)right.M12 + left.M33 * (double)right.M32 +
                                left.M34 * (double)right.M42),
                           M33 =
                               (float)
                               (right.M23 * (double)left.M32 + left.M31 * (double)right.M13 + left.M33 * (double)right.M33 +
                                left.M34 * (double)right.M43),
                           M34 =
                               (float)
                               (right.M24 * (double)left.M32 + right.M14 * (double)left.M31 + right.M34 * (double)left.M33 +
                                right.M44 * (double)left.M34),
                           M41 =
                               (float)
                               (left.M42 * (double)right.M21 + left.M41 * (double)right.M11 + left.M43 * (double)right.M31 +
                                left.M44 * (double)right.M41),
                           M42 =
                               (float)
                               (left.M42 * (double)right.M22 + left.M41 * (double)right.M12 + left.M43 * (double)right.M32 +
                                left.M44 * (double)right.M42),
                           M43 =
                               (float)
                               (right.M23 * (double)left.M42 + left.M41 * (double)right.M13 + left.M43 * (double)right.M33 +
                                left.M44 * (double)right.M43),
                           M44 =
                               (float)
                               (right.M24 * (double)left.M42 + left.M41 * (double)right.M14 + right.M34 * (double)left.M43 +
                                left.M44 * (double)right.M44)
                       };
        }

        public static bool operator ==(Matrix left, Matrix right)
        {
            return Equals(ref left, ref right);
        }

        public static bool operator !=(Matrix left, Matrix right)
        {
            return !Equals(ref left, ref right);
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
                           M11,
                           M12,
                           M13,
                           M14,
                           M21,
                           M22,
                           M23,
                           M24,
                           M31,
                           M32,
                           M33,
                           M34,
                           M41,
                           M42,
                           M43,
                           M44
                       };
        }


        public float Determinant()
        {
            var num1 = (float)(M44 * (double)M33 - M43 * (double)M34);
            var num2 = (float)(M32 * (double)M44 - M42 * (double)M34);
            var num3 = (float)(M32 * (double)M43 - M42 * (double)M33);
            var num4 = (float)(M31 * (double)M44 - M41 * (double)M34);
            var num5 = (float)(M31 * (double)M43 - M41 * (double)M33);
            var num6 = (float)(M31 * (double)M42 - M41 * (double)M32);
            return
                (float)
                ((M22 * (double)num1 - M23 * (double)num2 + M24 * (double)num3) * M11 -
                 (M21 * (double)num1 - M23 * (double)num4 + M24 * (double)num5) * M12 +
                 (M21 * (double)num2 - M22 * (double)num4 + M24 * (double)num6) * M13 -
                 (M21 * (double)num3 - M22 * (double)num5 + M23 * (double)num6) * M14);
        }

        public static void Add(ref Matrix left, ref Matrix right, out Matrix result)
        {
            result = new Matrix
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
            return new Matrix
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
            result = new Matrix
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
            return new Matrix
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
            result = new Matrix
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
            return new Matrix
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
            result = new Matrix
                         {
                             M11 =
                                 (float)
                                 (left.M12 * (double)right.M21 + left.M11 * (double)right.M11 +
                                  left.M13 * (double)right.M31 + left.M14 * (double)right.M41),
                             M12 =
                                 (float)
                                 (left.M12 * (double)right.M22 + left.M11 * (double)right.M12 +
                                  left.M13 * (double)right.M32 + left.M14 * (double)right.M42),
                             M13 =
                                 (float)
                                 (left.M12 * (double)right.M23 + left.M11 * (double)right.M13 +
                                  left.M13 * (double)right.M33 + left.M14 * (double)right.M43),
                             M14 =
                                 (float)
                                 (left.M12 * (double)right.M24 + left.M11 * (double)right.M14 +
                                  left.M13 * (double)right.M34 + left.M14 * (double)right.M44),
                             M21 =
                                 (float)
                                 (left.M22 * (double)right.M21 + left.M21 * (double)right.M11 +
                                  left.M23 * (double)right.M31 + left.M24 * (double)right.M41),
                             M22 =
                                 (float)
                                 (left.M22 * (double)right.M22 + left.M21 * (double)right.M12 +
                                  left.M23 * (double)right.M32 + left.M24 * (double)right.M42),
                             M23 =
                                 (float)
                                 (left.M22 * (double)right.M23 + left.M21 * (double)right.M13 +
                                  left.M23 * (double)right.M33 + left.M24 * (double)right.M43),
                             M24 =
                                 (float)
                                 (left.M22 * (double)right.M24 + left.M21 * (double)right.M14 +
                                  left.M23 * (double)right.M34 + left.M24 * (double)right.M44),
                             M31 =
                                 (float)
                                 (left.M32 * (double)right.M21 + left.M31 * (double)right.M11 +
                                  left.M33 * (double)right.M31 + left.M34 * (double)right.M41),
                             M32 =
                                 (float)
                                 (left.M32 * (double)right.M22 + left.M31 * (double)right.M12 +
                                  left.M33 * (double)right.M32 + left.M34 * (double)right.M42),
                             M33 =
                                 (float)
                                 (left.M32 * (double)right.M23 + left.M31 * (double)right.M13 +
                                  left.M33 * (double)right.M33 + left.M34 * (double)right.M43),
                             M34 =
                                 (float)
                                 (left.M32 * (double)right.M24 + left.M31 * (double)right.M14 +
                                  left.M33 * (double)right.M34 + left.M34 * (double)right.M44),
                             M41 =
                                 (float)
                                 (left.M42 * (double)right.M21 + left.M41 * (double)right.M11 +
                                  left.M43 * (double)right.M31 + left.M44 * (double)right.M41),
                             M42 =
                                 (float)
                                 (left.M42 * (double)right.M22 + left.M41 * (double)right.M12 +
                                  left.M43 * (double)right.M32 + left.M44 * (double)right.M42),
                             M43 =
                                 (float)
                                 (left.M42 * (double)right.M23 + left.M41 * (double)right.M13 +
                                  left.M43 * (double)right.M33 + left.M44 * (double)right.M43),
                             M44 =
                                 (float)
                                 (left.M42 * (double)right.M24 + left.M41 * (double)right.M14 +
                                  left.M43 * (double)right.M34 + left.M44 * (double)right.M44)
                         };
        }

        public static Matrix Multiply(Matrix left, Matrix right)
        {
            return new Matrix
                       {
                           M11 =
                               (float)
                               (right.M21 * (double)left.M12 + left.M11 * (double)right.M11 + right.M31 * (double)left.M13 +
                                right.M41 * (double)left.M14),
                           M12 =
                               (float)
                               (right.M22 * (double)left.M12 + right.M12 * (double)left.M11 + right.M32 * (double)left.M13 +
                                right.M42 * (double)left.M14),
                           M13 =
                               (float)
                               (right.M23 * (double)left.M12 + right.M13 * (double)left.M11 + right.M33 * (double)left.M13 +
                                right.M43 * (double)left.M14),
                           M14 =
                               (float)
                               (right.M24 * (double)left.M12 + right.M14 * (double)left.M11 + right.M34 * (double)left.M13 +
                                right.M44 * (double)left.M14),
                           M21 =
                               (float)
                               (left.M22 * (double)right.M21 + left.M21 * (double)right.M11 + left.M23 * (double)right.M31 +
                                left.M24 * (double)right.M41),
                           M22 =
                               (float)
                               (left.M22 * (double)right.M22 + left.M21 * (double)right.M12 + left.M23 * (double)right.M32 +
                                left.M24 * (double)right.M42),
                           M23 =
                               (float)
                               (right.M23 * (double)left.M22 + right.M13 * (double)left.M21 + right.M33 * (double)left.M23 +
                                left.M24 * (double)right.M43),
                           M24 =
                               (float)
                               (right.M24 * (double)left.M22 + right.M14 * (double)left.M21 + right.M34 * (double)left.M23 +
                                right.M44 * (double)left.M24),
                           M31 =
                               (float)
                               (left.M32 * (double)right.M21 + left.M31 * (double)right.M11 + left.M33 * (double)right.M31 +
                                left.M34 * (double)right.M41),
                           M32 =
                               (float)
                               (left.M32 * (double)right.M22 + left.M31 * (double)right.M12 + left.M33 * (double)right.M32 +
                                left.M34 * (double)right.M42),
                           M33 =
                               (float)
                               (right.M23 * (double)left.M32 + left.M31 * (double)right.M13 + left.M33 * (double)right.M33 +
                                left.M34 * (double)right.M43),
                           M34 =
                               (float)
                               (right.M24 * (double)left.M32 + right.M14 * (double)left.M31 + right.M34 * (double)left.M33 +
                                right.M44 * (double)left.M34),
                           M41 =
                               (float)
                               (left.M42 * (double)right.M21 + left.M41 * (double)right.M11 + left.M43 * (double)right.M31 +
                                left.M44 * (double)right.M41),
                           M42 =
                               (float)
                               (left.M42 * (double)right.M22 + left.M41 * (double)right.M12 + left.M43 * (double)right.M32 +
                                left.M44 * (double)right.M42),
                           M43 =
                               (float)
                               (right.M23 * (double)left.M42 + left.M41 * (double)right.M13 + left.M43 * (double)right.M33 +
                                left.M44 * (double)right.M43),
                           M44 =
                               (float)
                               (right.M24 * (double)left.M42 + left.M41 * (double)right.M14 + right.M34 * (double)left.M43 +
                                left.M44 * (double)right.M44)
                       };
        }

        public static void Divide(ref Matrix left, float right, out Matrix result)
        {
            float num = 1f / right;
            result = new Matrix
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
            var matrix = new Matrix();
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
            result = new Matrix
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
            return new Matrix
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
            result = new Matrix
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
            return new Matrix
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
            result = new Matrix
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
            return new Matrix
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



        public static void RotationAxis(ref Vector3 axis, float angle, out Matrix result)
        {
            if (axis.LengthSquared() != 1.0)
                axis.Normalize();
            float num1 = axis.X;
            float num2 = axis.Y;
            float num3 = axis.Z;
            var num4 = (float)System.Math.Cos(angle);
            var num5 = (float)System.Math.Sin(angle);
            double num6 = num1;
            var num7 = (float)(num6 * num6);
            double num8 = num2;
            var num9 = (float)(num8 * num8);
            double num10 = num3;
            var num11 = (float)(num10 * num10);
            float num12 = num2 * num1;
            float num13 = num3 * num1;
            float num14 = num3 * num2;
            result.M11 = (1f - num7) * num4 + num7;
            double num15 = num12;
            double num16 = num15 - num4 * num15;
            double num17 = num5 * (double)num3;
            result.M12 = (float)(num17 + num16);
            double num18 = num13;
            double num19 = num18 - num4 * num18;
            double num20 = num5 * (double)num2;
            result.M13 = (float)(num19 - num20);
            result.M14 = 0.0f;
            result.M21 = (float)(num16 - num17);
            result.M22 = (1f - num9) * num4 + num9;
            double num21 = num14;
            double num22 = num21 - num4 * num21;
            double num23 = num5 * (double)num1;
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
            if (axis.LengthSquared() != 1.0)
                axis.Normalize();
            var matrix = new Matrix();
            float num1 = axis.X;
            float num2 = axis.Y;
            float num3 = axis.Z;
            var num4 = (float)System.Math.Cos(angle);
            var num5 = (float)System.Math.Sin(angle);
            double num6 = num1;
            var num7 = (float)(num6 * num6);
            double num8 = num2;
            var num9 = (float)(num8 * num8);
            double num10 = num3;
            var num11 = (float)(num10 * num10);
            float num12 = num2 * num1;
            float num13 = num3 * num1;
            float num14 = num3 * num2;
            matrix.M11 = (1f - num7) * num4 + num7;
            double num15 = num12;
            double num16 = num15 - num4 * num15;
            double num17 = num5 * (double)num3;
            matrix.M12 = (float)(num17 + num16);
            double num18 = num13;
            double num19 = num18 - num4 * num18;
            double num20 = num5 * (double)num2;
            matrix.M13 = (float)(num19 - num20);
            matrix.M14 = 0.0f;
            matrix.M21 = (float)(num16 - num17);
            matrix.M22 = (1f - num9) * num4 + num9;
            double num21 = num14;
            double num22 = num21 - num4 * num21;
            double num23 = num5 * (double)num1;
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
            var num17 = (float)(matrix.M33 * (double)matrix.M44 - matrix.M34 * (double)matrix.M43);
            var num18 = (float)(matrix.M32 * (double)matrix.M44 - matrix.M34 * (double)matrix.M42);
            var num19 = (float)(matrix.M32 * (double)matrix.M43 - matrix.M33 * (double)matrix.M42);
            var num20 = (float)(matrix.M31 * (double)matrix.M44 - matrix.M34 * (double)matrix.M41);
            var num21 = (float)(matrix.M31 * (double)matrix.M43 - matrix.M33 * (double)matrix.M41);
            var num22 = (float)(matrix.M31 * (double)matrix.M42 - matrix.M32 * (double)matrix.M41);
            var num23 = (float)(matrix.M22 * (double)num17 - matrix.M23 * (double)num18 + matrix.M24 * (double)num19);
            var num24 = (float)-(matrix.M21 * (double)num17 - matrix.M23 * (double)num20 + matrix.M24 * (double)num21);
            var num25 = (float)(matrix.M21 * (double)num18 - matrix.M22 * (double)num20 + matrix.M24 * (double)num22);
            var num26 = (float)-(matrix.M21 * (double)num19 - matrix.M22 * (double)num21 + matrix.M23 * (double)num22);
            var num27 =
                (float)
                (1.0 /
                 (matrix.M11 * (double)num23 + matrix.M12 * (double)num24 + matrix.M13 * (double)num25 +
                  matrix.M14 * (double)num26));
            result.M11 = num23 * num27;
            result.M21 = num24 * num27;
            result.M31 = num25 * num27;
            result.M41 = num26 * num27;
            result.M12 = (float)-(matrix.M12 * (double)num17 - matrix.M13 * (double)num18 + matrix.M14 * (double)num19) *
                         num27;
            result.M22 = (float)(matrix.M11 * (double)num17 - matrix.M13 * (double)num20 + matrix.M14 * (double)num21) *
                         num27;
            result.M32 = (float)-(matrix.M11 * (double)num18 - matrix.M12 * (double)num20 + matrix.M14 * (double)num22) *
                         num27;
            result.M42 = (float)(matrix.M11 * (double)num19 - matrix.M12 * (double)num21 + matrix.M13 * (double)num22) *
                         num27;
            var num28 = (float)(matrix.M23 * (double)matrix.M44 - matrix.M24 * (double)matrix.M43);
            var num29 = (float)(matrix.M22 * (double)matrix.M44 - matrix.M24 * (double)matrix.M42);
            var num30 = (float)(matrix.M22 * (double)matrix.M43 - matrix.M23 * (double)matrix.M42);
            var num31 = (float)(matrix.M21 * (double)matrix.M44 - matrix.M24 * (double)matrix.M41);
            var num32 = (float)(matrix.M21 * (double)matrix.M43 - matrix.M23 * (double)matrix.M41);
            var num33 = (float)(matrix.M21 * (double)matrix.M42 - matrix.M22 * (double)matrix.M41);
            result.M13 = (float)(matrix.M12 * (double)num28 - matrix.M13 * (double)num29 + matrix.M14 * (double)num30) *
                         num27;
            result.M23 = (float)-(matrix.M11 * (double)num28 - matrix.M13 * (double)num31 + matrix.M14 * (double)num32) *
                         num27;
            result.M33 = (float)(matrix.M11 * (double)num29 - matrix.M12 * (double)num31 + matrix.M14 * (double)num33) *
                         num27;
            result.M43 = (float)-(matrix.M11 * (double)num30 - matrix.M12 * (double)num32 + matrix.M13 * (double)num33) *
                         num27;
            var num34 = (float)(matrix.M23 * (double)matrix.M34 - matrix.M24 * (double)matrix.M33);
            var num35 = (float)(matrix.M22 * (double)matrix.M34 - matrix.M24 * (double)matrix.M32);
            var num36 = (float)(matrix.M22 * (double)matrix.M33 - matrix.M23 * (double)matrix.M32);
            var num37 = (float)(matrix.M21 * (double)matrix.M34 - matrix.M24 * (double)matrix.M31);
            var num38 = (float)(matrix.M21 * (double)matrix.M33 - matrix.M23 * (double)matrix.M31);
            var num39 = (float)(matrix.M21 * (double)matrix.M32 - matrix.M22 * (double)matrix.M31);
            result.M14 = (float)-(matrix.M12 * (double)num34 - matrix.M13 * (double)num35 + matrix.M14 * (double)num36) *
                         num27;
            result.M24 = (float)(matrix.M11 * (double)num34 - matrix.M13 * (double)num37 + matrix.M14 * (double)num38) *
                         num27;
            result.M34 = (float)-(matrix.M11 * (double)num35 - matrix.M12 * (double)num37 + matrix.M14 * (double)num39) *
                         num27;
            result.M44 = (float)(matrix.M11 * (double)num36 - matrix.M12 * (double)num38 + matrix.M13 * (double)num39) *
                         num27;
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
            return new Matrix
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
            return new Matrix
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
            return new Matrix
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
            return new Matrix
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
            result = new Matrix
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
            return new Matrix
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


        public override string ToString()
        {
            var objArray = new object[16];
            float num1 = M11;
            objArray[0] = num1.ToString(CultureInfo.CurrentCulture);
            float num2 = M12;
            objArray[1] = num2.ToString(CultureInfo.CurrentCulture);
            float num3 = M13;
            objArray[2] = num3.ToString(CultureInfo.CurrentCulture);
            float num4 = M14;
            objArray[3] = num4.ToString(CultureInfo.CurrentCulture);
            float num5 = M21;
            objArray[4] = num5.ToString(CultureInfo.CurrentCulture);
            float num6 = M22;
            objArray[5] = num6.ToString(CultureInfo.CurrentCulture);
            float num7 = M23;
            objArray[6] = num7.ToString(CultureInfo.CurrentCulture);
            float num8 = M24;
            objArray[7] = num8.ToString(CultureInfo.CurrentCulture);
            float num9 = M31;
            objArray[8] = num9.ToString(CultureInfo.CurrentCulture);
            float num10 = M32;
            objArray[9] = num10.ToString(CultureInfo.CurrentCulture);
            float num11 = M33;
            objArray[10] = num11.ToString(CultureInfo.CurrentCulture);
            float num12 = M34;
            objArray[11] = num12.ToString(CultureInfo.CurrentCulture);
            float num13 = M41;
            objArray[12] = num13.ToString(CultureInfo.CurrentCulture);
            float num14 = M42;
            objArray[13] = num14.ToString(CultureInfo.CurrentCulture);
            float num15 = M43;
            objArray[14] = num15.ToString(CultureInfo.CurrentCulture);
            float num16 = M44;
            objArray[15] = num16.ToString(CultureInfo.CurrentCulture);
            return string.Format(CultureInfo.CurrentCulture,
                                 "[[M11:{0} M12:{1} M13:{2} M14:{3}] [M21:{4} M22:{5} M23:{6} M24:{7}] [M31:{8} M32:{9} M33:{10} M34:{11}] [M41:{12} M42:{13} M43:{14} M44:{15}]]",
                                 objArray);
        }

        public override int GetHashCode()
        {
            return M11.GetHashCode() +
                   (M43.GetHashCode() + M44.GetHashCode() + M42.GetHashCode() + M41.GetHashCode() + M34.GetHashCode() +
                    M33.GetHashCode() + M32.GetHashCode() + M31.GetHashCode() + M24.GetHashCode() + M23.GetHashCode() +
                    M22.GetHashCode() + M21.GetHashCode() + M14.GetHashCode() + M13.GetHashCode() + M12.GetHashCode());
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool Equals(ref Matrix value1, ref Matrix value2)
        {
            return value1.M11 == (double)value2.M11 && value1.M12 == (double)value2.M12 &&
                   (value1.M13 == (double)value2.M13 && value1.M14 == (double)value2.M14) &&
                   (value1.M21 == (double)value2.M21 && value1.M22 == (double)value2.M22 &&
                    (value1.M23 == (double)value2.M23 && value1.M24 == (double)value2.M24)) &&
                   (value1.M31 == (double)value2.M31 && value1.M32 == (double)value2.M32 &&
                    (value1.M33 == (double)value2.M33 && value1.M34 == (double)value2.M34) &&
                    (value1.M41 == (double)value2.M41 && value1.M42 == (double)value2.M42 &&
                     (value1.M43 == (double)value2.M43 && value1.M44 == (double)value2.M44)));
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool Equals(Matrix other)
        {
            return M11 == (double)other.M11 && M12 == (double)other.M12 &&
                   (M13 == (double)other.M13 && M14 == (double)other.M14) &&
                   (M21 == (double)other.M21 && M22 == (double)other.M22 &&
                    (M23 == (double)other.M23 && M24 == (double)other.M24)) &&
                   (M31 == (double)other.M31 && M32 == (double)other.M32 &&
                    (M33 == (double)other.M33 && M34 == (double)other.M34) &&
                    (M41 == (double)other.M41 && M42 == (double)other.M42 &&
                     (M43 == (double)other.M43 && M44 == (double)other.M44)));
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            else
                return Equals((Matrix)obj);
        }
    }
}