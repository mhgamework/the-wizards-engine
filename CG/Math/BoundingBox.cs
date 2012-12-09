using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MHGameWork.TheWizards.CG.Math
{
    [Serializable]
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        public Vector3 Maximum;
        public Vector3 Minimum;

        public BoundingBox(Vector3 minimum, Vector3 maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        #region IEquatable<BoundingBox> Members

        [return: MarshalAs(UnmanagedType.U1)]
        public bool Equals(BoundingBox other)
        {
            return Minimum == other.Minimum && Maximum == other.Maximum;
        }

        #endregion

        public static bool operator ==(BoundingBox left, BoundingBox right)
        {
            return Equals(ref left, ref right);
        }

        public static bool operator !=(BoundingBox left, BoundingBox right)
        {
            return !Equals(ref left, ref right);
        }

      
        public static BoundingBox FromPoints(Vector3[] points)
        {
            if (points == null || points.Length <= 0)
                throw new ArgumentNullException("points");
            var result1 = new Vector3(float.MaxValue);
            var result2 = new Vector3(float.MinValue);
            int index = 0;
            if (0 < points.Length)
            {
                do
                {
                    Vector3 vector3 = points[index];
                    Vector3.Minimize(ref result1, ref vector3, out result1);
                    Vector3.Maximize(ref result2, ref vector3, out result2);
                    ++index;
                } while (index < points.Length);
            }
            Vector3 vector3_1 = result2;
            Vector3 vector3_2 = result1;
            BoundingBox boundingBox;
            boundingBox.Minimum = vector3_2;
            boundingBox.Maximum = vector3_1;
            return boundingBox;
        }

       

        public static BoundingBox Merge(BoundingBox box1, BoundingBox box2)
        {
            var boundingBox = new BoundingBox();
            Vector3.Minimize(ref box1.Minimum, ref box2.Minimum, out boundingBox.Minimum);
            Vector3.Maximize(ref box1.Maximum, ref box2.Maximum, out boundingBox.Maximum);
            return boundingBox;
        }


        public BoundingBox MergeWith(BoundingBox box2)
        {
            BoundingBox merged;
            if (Minimum == Maximum)
                merged = box2;
            else if (box2.Minimum == box2.Maximum)
                merged = this;
            else
                merged = Merge(this, box2);

            return merged;
        }

        public static void MergeWith(ref BoundingBox box1, ref BoundingBox box2, ref BoundingBox merged)
        {
            if (box1.Minimum == box1.Maximum)
                merged = box2;
            else if (box2.Minimum == box2.Maximum)
                merged = box1;
            else
                merged = Merge(box1, box2);
        }

        public void Intersects(ref Ray ray, out float? result)
        {
            result = new float?();
            float num1 = 0.0f;
            float num2 = float.MaxValue;
            if (System.Math.Abs(ray.Direction.X) < 9.99999997475243E-07)
            {
                if (ray.Position.X < (double) Minimum.X || ray.Position.X > (double) Maximum.X)
                    return;
            }
            else
            {
                float num3 = 1f/ray.Direction.X;
                float num4 = (Minimum.X - ray.Position.X)*num3;
                float num5 = (Maximum.X - ray.Position.X)*num3;
                if (num4 > (double) num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num1 = MathHelper.Max(num4, num1);
                num2 = MathHelper.Min(num5, num2);
                if (num1 > (double) num2)
                    return;
            }
            if (System.Math.Abs(ray.Direction.Y) < 9.99999997475243E-07)
            {
                if (ray.Position.Y < (double) Minimum.Y || ray.Position.Y > (double) Maximum.Y)
                    return;
            }
            else
            {
                float num3 = 1f/ray.Direction.Y;
                float num4 = (Minimum.Y - ray.Position.Y)*num3;
                float num5 = (Maximum.Y - ray.Position.Y)*num3;
                if (num4 > (double) num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num1 = MathHelper.Max(num4, num1);
                num2 = MathHelper.Min(num5, num2);
                if (num1 > (double) num2)
                    return;
            }
            if (System.Math.Abs(ray.Direction.Z) < 9.99999997475243E-07)
            {
                if (ray.Position.Z < (double) Minimum.Z || ray.Position.Z > (double) Maximum.Z)
                    return;
            }
            else
            {
                float num3 = 1f/ray.Direction.Z;
                float num4 = (Minimum.Z - ray.Position.Z)*num3;
                float num5 = (Maximum.Z - ray.Position.Z)*num3;
                if (num4 > (double) num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num1 = MathHelper.Max(num4, num1);
                float num7 = MathHelper.Min(num5, num2);
                if (num1 > (double) num7)
                    return;
            }
            result = new float?(num1);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Minimum:{0} Maximum:{1}", new object[2]
                                                                                            {
                                                                                                Minimum.ToString(),
                                                                                                Maximum.ToString()
                                                                                            });
        }

        public override int GetHashCode()
        {
            return Maximum.GetHashCode() + Minimum.GetHashCode();
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool Equals(ref BoundingBox value1, ref BoundingBox value2)
        {
            return value1.Minimum == value2.Minimum && value1.Maximum == value2.Maximum;
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            else
                return Equals((BoundingBox) obj);
        }
    }
}