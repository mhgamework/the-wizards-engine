// Type: SlimDX.BoundingSphere
// Assembly: SlimDX, Version=4.0.13.43, Culture=neutral, PublicKeyToken=b1b0c32fd1ffe4f9
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_32\SlimDX\v4.0_4.0.13.43__b1b0c32fd1ffe4f9\SlimDX.dll

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MHGameWork.TheWizards.CG.Math
{
    [Serializable]
    public struct BoundingSphere : IEquatable<BoundingSphere>
    {
        public Vector3 Center;
        public float Radius;

        public BoundingSphere(Vector3 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        public static bool operator ==(BoundingSphere left, BoundingSphere right)
        {
            return BoundingSphere.Equals(ref left, ref right);
        }

        public static bool operator !=(BoundingSphere left, BoundingSphere right)
        {
            return !BoundingSphere.Equals(ref left, ref right);
        }

    //    public static BoundingSphere FromBox(BoundingBox box)
    //{
    //  BoundingSphere boundingSphere = new BoundingSphere();
    //  Vector3.Lerp(ref box.Minimum, ref box.Maximum, 0.5f, out boundingSphere.Center);
    //  float num1 = box.Minimum.X - box.Maximum.X;
    //  float num2 = box.Minimum.Y - box.Maximum.Y;
    //  float num3 = box.Minimum.Z - box.Maximum.Z;
    //  double num4 = (double) num2;
    //  double num5 = (double) num1;
    //  double num6 = (double) num3;
    //  // ISSUE: explicit reference operation
    //  // ISSUE: variable of a reference type
    //  BoundingSphere& local = @boundingSphere;
    //  double num7 = num5;
    //  double num8 = num7 * num7;
    //  double num9 = num4;
    //  double num10 = num9 * num9;
    //  double num11 = num8 + num10;
    //  double num12 = num6;
    //  double num13 = num12 * num12;
    //  double num14 = Math.Sqrt(num11 + num13) * 0.5;
    //  // ISSUE: explicit reference operation
    //  (^local).Radius = (float) num14;
    //  return boundingSphere;
    //}

        public static BoundingSphere Merge(BoundingSphere sphere1, BoundingSphere sphere2)
        {
            BoundingSphere boundingSphere = new BoundingSphere();
            Vector3 vector3_1 = sphere2.Center - sphere1.Center;
            float num1 = vector3_1.Length();
            float val1 = sphere1.Radius;
            float num2 = sphere2.Radius;
            if ((double)num2 + (double)val1 >= (double)num1)
            {
                if ((double)val1 - (double)num2 >= (double)num1)
                    return sphere1;
                if ((double)num2 - (double)val1 >= (double)num1)
                    return sphere2;
            }
            Vector3 vector3_2 = vector3_1 * (1f / num1);
            float num3 = System.Math.Min(-val1, num1 - num2);
            float num4 = (float)(((double)System.Math.Max(val1, num2 + num1) - (double)num3) * 0.5);
            double num5 = (double)(num4 + num3);
            Vector3 vector3_3 = vector3_2 * (float)num5;
            Vector3 vector3_4 = sphere1.Center + vector3_3;
            boundingSphere.Center = vector3_4;
            boundingSphere.Radius = num4;
            return boundingSphere;
        }

        public override string ToString()
        {
            object[] objArray = new object[2]
      {
        (object) this.Center.ToString(),
        null
      };
            float num = this.Radius;
            objArray[1] = (object)num.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, "Center:{0} Radius:{1}", objArray);
        }

        public override int GetHashCode()
        {
            return this.Center.GetHashCode() + this.Radius.GetHashCode();
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool Equals(ref BoundingSphere value1, ref BoundingSphere value2)
        {
            return value1.Center == value2.Center && (double)value1.Radius == (double)value2.Radius;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool Equals(BoundingSphere other)
        {
            return this.Center == other.Center && (double)this.Radius == (double)other.Radius;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;
            else
                return this.Equals((BoundingSphere)obj);
        }
    }
}
