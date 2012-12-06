using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using SlimDX;

namespace SlimDX
{
    [Serializable]
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        public Vector3 Maximum;
        public Vector3 Minimum;

        public BoundingBox(Vector3 minimum, Vector3 maximum)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        public static bool operator ==(BoundingBox left, BoundingBox right)
        {
            return BoundingBox.Equals(ref left, ref right);
        }

        public static bool operator !=(BoundingBox left, BoundingBox right)
        {
            return !BoundingBox.Equals(ref left, ref right);
        }

        public Vector3[] GetCorners()
        {
            Vector3[] vector3Array = new Vector3[8];
            Vector3 vector3_1 = new Vector3(this.Minimum.X, this.Maximum.Y, this.Maximum.Z);
            vector3Array[0] = vector3_1;
            Vector3 vector3_2 = new Vector3(this.Maximum.X, this.Maximum.Y, this.Maximum.Z);
            vector3Array[1] = vector3_2;
            Vector3 vector3_3 = new Vector3(this.Maximum.X, this.Minimum.Y, this.Maximum.Z);
            vector3Array[2] = vector3_3;
            Vector3 vector3_4 = new Vector3(this.Minimum.X, this.Minimum.Y, this.Maximum.Z);
            vector3Array[3] = vector3_4;
            Vector3 vector3_5 = new Vector3(this.Minimum.X, this.Maximum.Y, this.Minimum.Z);
            vector3Array[4] = vector3_5;
            Vector3 vector3_6 = new Vector3(this.Maximum.X, this.Maximum.Y, this.Minimum.Z);
            vector3Array[5] = vector3_6;
            Vector3 vector3_7 = new Vector3(this.Maximum.X, this.Minimum.Y, this.Minimum.Z);
            vector3Array[6] = vector3_7;
            Vector3 vector3_8 = new Vector3(this.Minimum.X, this.Minimum.Y, this.Minimum.Z);
            vector3Array[7] = vector3_8;
            return vector3Array;
        }

        //public static ContainmentType Contains(BoundingBox box, Vector3 vector)
        //{
        //  return (double) box.Minimum.X <= (double) vector.X && (double) vector.X <= (double) box.Maximum.X && ((double) box.Minimum.Y <= (double) vector.Y && (double) vector.Y <= (double) box.Maximum.Y) && ((double) box.Minimum.Z <= (double) vector.Z && (double) vector.Z <= (double) box.Maximum.Z) ? ContainmentType.Contains : ContainmentType.Disjoint;
        //}

        //public static ContainmentType Contains(BoundingBox box, BoundingSphere sphere)
        //{
        //  Vector3 result = new Vector3();
        //  Vector3.Clamp(ref sphere.Center, ref box.Minimum, ref box.Maximum, out result);
        //  float num1 = sphere.Center.X - result.X;
        //  float num2 = sphere.Center.Y - result.Y;
        //  float num3 = sphere.Center.Z - result.Z;
        //  float num4 = sphere.Radius;
        //  double num5 = (double) num2;
        //  double num6 = (double) num1;
        //  double num7 = (double) num3;
        //  double num8 = (double) num4;
        //  double num9 = num6;
        //  double num10 = num9 * num9;
        //  double num11 = num5;
        //  double num12 = num11 * num11;
        //  double num13 = num10 + num12;
        //  double num14 = num7;
        //  double num15 = num14 * num14;
        //  double num16 = num13 + num15;
        //  double num17 = num8;
        //  double num18 = num17 * num17;
        //  if (num16 > num18)
        //    return ContainmentType.Disjoint;
        //  if ((double) box.Minimum.X + (double) num4 <= (double) sphere.Center.X && (double) sphere.Center.X <= (double) box.Maximum.X - (double) num4)
        //  {
        //    double num19 = (double) box.Maximum.X - (double) box.Minimum.X;
        //    if (num19 > (double) num4 && (double) box.Minimum.Y + (double) num4 <= (double) sphere.Center.Y && ((double) sphere.Center.Y <= (double) box.Maximum.Y - (double) num4 && (double) box.Maximum.Y - (double) box.Minimum.Y > (double) num4) && ((double) box.Minimum.Z + (double) num4 <= (double) sphere.Center.Z && (double) sphere.Center.Z <= (double) box.Maximum.Z - (double) num4 && num19 > (double) num4))
        //      return ContainmentType.Contains;
        //  }
        //  return ContainmentType.Intersects;
        //}

        //public static ContainmentType Contains(BoundingBox box1, BoundingBox box2)
        //{
        //  if ((double) box1.Maximum.X < (double) box2.Minimum.X || (double) box1.Minimum.X > (double) box2.Maximum.X || ((double) box1.Maximum.Y < (double) box2.Minimum.Y || (double) box1.Minimum.Y > (double) box2.Maximum.Y) || ((double) box1.Maximum.Z < (double) box2.Minimum.Z || (double) box1.Minimum.Z > (double) box2.Maximum.Z))
        //    return ContainmentType.Disjoint;
        //  return (double) box1.Minimum.X <= (double) box2.Minimum.X && (double) box2.Maximum.X <= (double) box1.Maximum.X && ((double) box1.Minimum.Y <= (double) box2.Minimum.Y && (double) box2.Maximum.Y <= (double) box1.Maximum.Y) && ((double) box1.Minimum.Z <= (double) box2.Minimum.Z && (double) box2.Maximum.Z <= (double) box1.Maximum.Z) ? ContainmentType.Contains : ContainmentType.Intersects;
        //}

        //public static unsafe BoundingBox FromPoints(DataStream points, int count, int stride)
        //{
        //  BoundingBox boundingBox = new BoundingBox();
        //  if (Result.Record<SlimDXException>(\u003CModule\u003E.D3DXComputeBoundingBox((D3DXVECTOR3*) points.PositionPointer, (uint) count, (uint) stride, (D3DXVECTOR3*) (int) &boundingBox.Minimum, (D3DXVECTOR3*) (int) &boundingBox.Maximum), (object) null, (object) null).IsFailure)
        //    return new BoundingBox();
        //  else
        //    return boundingBox;
        //}

        public static BoundingBox FromPoints(Vector3[] points)
        {
            if (points == null || points.Length <= 0)
                throw new ArgumentNullException("points");
            Vector3 result1 = new Vector3(float.MaxValue);
            Vector3 result2 = new Vector3(float.MinValue);
            int index = 0;
            if (0 < points.Length)
            {
                do
                {
                    Vector3 vector3 = points[index];
                    Vector3.Minimize(ref result1, ref vector3, out result1);
                    Vector3.Maximize(ref result2, ref vector3, out result2);
                    ++index;
                }
                while (index < points.Length);
            }
            Vector3 vector3_1 = result2;
            Vector3 vector3_2 = result1;
            BoundingBox boundingBox;
            boundingBox.Minimum = vector3_2;
            boundingBox.Maximum = vector3_1;
            return boundingBox;
        }

        //public static BoundingBox FromSphere(BoundingSphere sphere)
        //{
        //  BoundingBox boundingBox = new BoundingBox();
        //  Vector3 vector3_1 = new Vector3(sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius);
        //  boundingBox.Minimum = vector3_1;
        //  Vector3 vector3_2 = new Vector3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius);
        //  boundingBox.Maximum = vector3_2;
        //  return boundingBox;
        //}

        public static BoundingBox Merge(BoundingBox box1, BoundingBox box2)
        {
            BoundingBox boundingBox = new BoundingBox();
            Vector3.Minimize(ref box1.Minimum, ref box2.Minimum, out boundingBox.Minimum);
            Vector3.Maximize(ref box1.Maximum, ref box2.Maximum, out boundingBox.Maximum);
            return boundingBox;
        }

        //public static PlaneIntersectionType Intersects(BoundingBox box, Plane plane)
        //{
        //  return Plane.Intersects(plane, box);
        //}

        //[return: MarshalAs(UnmanagedType.U1)]
        //public static bool Intersects(BoundingBox box, Ray ray, out float distance)
        //{
        //  return Ray.Intersects(ray, box, out distance);
        //}

        //[return: MarshalAs(UnmanagedType.U1)]
        //public static bool Intersects(BoundingBox box, BoundingSphere sphere)
        //{
        //  Vector3 result = new Vector3();
        //  Vector3.Clamp(ref sphere.Center, ref box.Minimum, ref box.Maximum, out result);
        //  float num1 = sphere.Center.X - result.X;
        //  float num2 = sphere.Center.Y - result.Y;
        //  float num3 = sphere.Center.Z - result.Z;
        //  double num4 = (double) num2;
        //  double num5 = (double) num1;
        //  double num6 = (double) num3;
        //  double num7 = (double) sphere.Radius;
        //  double num8 = num5;
        //  double num9 = num8 * num8;
        //  double num10 = num4;
        //  double num11 = num10 * num10;
        //  double num12 = num9 + num11;
        //  double num13 = num6;
        //  double num14 = num13 * num13;
        //  double num15 = num12 + num14;
        //  double num16 = num7;
        //  double num17 = num16 * num16;
        //  return num15 <= num17;
        //}

        //[return: MarshalAs(UnmanagedType.U1)]
        //public static bool Intersects(BoundingBox box1, BoundingBox box2)
        //{
        //  if ((double) box1.Maximum.X >= (double) box2.Minimum.X && (double) box1.Minimum.X <= (double) box2.Maximum.X && ((double) box1.Maximum.Y >= (double) box2.Minimum.Y && (double) box1.Minimum.Y <= (double) box2.Maximum.Y))
        //    return (double) box1.Maximum.Z >= (double) box2.Minimum.Z && (double) box1.Minimum.Z <= (double) box2.Maximum.Z;
        //  else
        //    return false;
        //}

        public static void MergeWith(ref BoundingBox box1, ref BoundingBox box2, ref BoundingBox merged)
        {
            if (box1.Minimum == box1.Maximum)
                merged = box2;
            else if (box2.Minimum == box2.Maximum)
                merged = box1;
            else
                merged = BoundingBox.Merge(box1, box2);
        }

        public void Intersects(ref Ray ray, out float? result)
        {
            result = new float?();
            float num1 = 0.0f;
            float num2 = float.MaxValue;
            if ((double)Math.Abs(ray.Direction.X) < 9.99999997475243E-07)
            {
                if ((double)ray.Position.X < (double)this.Minimum.X || (double)ray.Position.X > (double)this.Maximum.X)
                    return;
            }
            else
            {
                float num3 = 1f / ray.Direction.X;
                float num4 = (this.Minimum.X - ray.Position.X) * num3;
                float num5 = (this.Maximum.X - ray.Position.X) * num3;
                if ((double)num4 > (double)num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num1 = MathHelper.Max(num4, num1);
                num2 = MathHelper.Min(num5, num2);
                if ((double)num1 > (double)num2)
                    return;
            }
            if ((double)Math.Abs(ray.Direction.Y) < 9.99999997475243E-07)
            {
                if ((double)ray.Position.Y < (double)this.Minimum.Y || (double)ray.Position.Y > (double)this.Maximum.Y)
                    return;
            }
            else
            {
                float num3 = 1f / ray.Direction.Y;
                float num4 = (this.Minimum.Y - ray.Position.Y) * num3;
                float num5 = (this.Maximum.Y - ray.Position.Y) * num3;
                if ((double)num4 > (double)num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num1 = MathHelper.Max(num4, num1);
                num2 = MathHelper.Min(num5, num2);
                if ((double)num1 > (double)num2)
                    return;
            }
            if ((double)Math.Abs(ray.Direction.Z) < 9.99999997475243E-07)
            {
                if ((double)ray.Position.Z < (double)this.Minimum.Z || (double)ray.Position.Z > (double)this.Maximum.Z)
                    return;
            }
            else
            {
                float num3 = 1f / ray.Direction.Z;
                float num4 = (this.Minimum.Z - ray.Position.Z) * num3;
                float num5 = (this.Maximum.Z - ray.Position.Z) * num3;
                if ((double)num4 > (double)num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num1 = MathHelper.Max(num4, num1);
                float num7 = MathHelper.Min(num5, num2);
                if ((double)num1 > (double)num7)
                    return;
            }
            result = new float?(num1);
        }

        public override string ToString()
        {
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, "Minimum:{0} Maximum:{1}", new object[2]
      {
        (object) this.Minimum.ToString(),
        (object) this.Maximum.ToString()
      });
        }

        public override int GetHashCode()
        {
            return this.Maximum.GetHashCode() + this.Minimum.GetHashCode();
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool Equals(ref BoundingBox value1, ref BoundingBox value2)
        {
            return value1.Minimum == value2.Minimum && value1.Maximum == value2.Maximum;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool Equals(BoundingBox other)
        {
            return this.Minimum == other.Minimum && this.Maximum == other.Maximum;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;
            else
                return this.Equals((BoundingBox)obj);
        }
    }


}
