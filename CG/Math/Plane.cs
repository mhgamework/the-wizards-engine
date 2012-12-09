using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MHGameWork.TheWizards.CG.Math
{
    [Serializable]
    public struct Plane : IEquatable<Plane>
    {
        public float D;
        public Vector3 Normal;

        public Plane(Vector4 value)
        {
            Normal = new Vector3(value.X, value.Y, value.Z);
            D = value.W;
        }

        public Plane(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            float num1 = point2.X - point1.X;
            float num2 = point2.Y - point1.Y;
            float num3 = point2.Z - point1.Z;
            float num4 = point3.X - point1.X;
            float num5 = point3.Y - point1.Y;
            float num6 = point3.Z - point1.Z;
            var num7 = (float)(num6 * (double)num2 - num5 * (double)num3);
            var num8 = (float)(num4 * (double)num3 - num6 * (double)num1);
            var num9 = (float)(num5 * (double)num1 - num4 * (double)num2);
            double num10 = num8;
            double num11 = num7;
            double num12 = num9;
            double num13 = 1.0;
            double num14 = num11;
            double num15 = num14 * num14;
            double num16 = num10;
            double num17 = num16 * num16;
            double num18 = num15 + num17;
            double num19 = num12;
            double num20 = num19 * num19;
            double num21 = System.Math.Sqrt(num18 + num20);
            var num22 = (float)(num13 / num21);
            float num23 = num22 * num7;
            Normal.X = num23;
            float num24 = num22 * num8;
            Normal.Y = num24;
            float num25 = num22 * num9;
            Normal.Z = num25;
            D = (float)-(point1.Y * (double)num24 + point1.X * (double)num23 + point1.Z * (double)num25);
        }

        public Plane(Vector3 point, Vector3 normal)
        {
            Normal = normal;
            D = -Vector3.Dot(normal, point);
        }

        public Plane(Vector3 normal, float d)
        {
            Normal = normal;
            D = d;
        }

        public Plane(float a, float b, float c, float d)
        {
            Normal = new Vector3(a, b, c);
            D = d;
        }

        #region IEquatable<Plane> Members

        [return: MarshalAs(UnmanagedType.U1)]
        public bool Equals(Plane other)
        {
            return Normal == other.Normal && D == (double)other.D;
        }

        #endregion

        public static bool operator ==(Plane left, Plane right)
        {
            return Equals(ref left, ref right);
        }

        public static bool operator !=(Plane left, Plane right)
        {
            return !Equals(ref left, ref right);
        }

        public static float Dot(Plane plane, Vector4 point)
        {
            return
                (float)
                (plane.Normal.Y * (double)point.Y + plane.Normal.X * (double)point.X + plane.Normal.Z * (double)point.Z +
                 point.W * (double)plane.D);
        }

        public static float DotCoordinate(Plane plane, Vector3 point)
        {
            return
                (float)
                (plane.Normal.Y * (double)point.Y + plane.Normal.X * (double)point.X + plane.Normal.Z * (double)point.Z) +
                plane.D;
        }

        public static float DotNormal(Plane plane, Vector3 point)
        {
            return
                (float)
                (plane.Normal.Y * (double)point.Y + plane.Normal.X * (double)point.X + plane.Normal.Z * (double)point.Z);
        }



        public override string ToString()
        {
            var objArray = new object[2]
                               {
                                   Normal.ToString(),
                                   null
                               };
            float num = D;
            objArray[1] = num.ToString(CultureInfo.CurrentCulture);
            return string.Format(CultureInfo.CurrentCulture, "Normal:{0} D:{1}", objArray);
        }

        public override int GetHashCode()
        {
            return Normal.GetHashCode() + D.GetHashCode();
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool Equals(ref Plane value1, ref Plane value2)
        {
            return value1.Normal == value2.Normal && value1.D == (double)value2.D;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            else
                return Equals((Plane)obj);
        }
    }
}