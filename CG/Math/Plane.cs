using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SlimDX;

namespace SlimDX
{

  [Serializable]
  public struct Plane : IEquatable<Plane>
  {
    public Vector3 Normal;
    public float D;

    public Plane(Vector4 value)
    {
      this.Normal = new Vector3(value.X, value.Y, value.Z);
      this.D = value.W;
    }

    public Plane(Vector3 point1, Vector3 point2, Vector3 point3)
    {
      float num1 = point2.X - point1.X;
      float num2 = point2.Y - point1.Y;
      float num3 = point2.Z - point1.Z;
      float num4 = point3.X - point1.X;
      float num5 = point3.Y - point1.Y;
      float num6 = point3.Z - point1.Z;
      float num7 = (float) ((double) num6 * (double) num2 - (double) num5 * (double) num3);
      float num8 = (float) ((double) num4 * (double) num3 - (double) num6 * (double) num1);
      float num9 = (float) ((double) num5 * (double) num1 - (double) num4 * (double) num2);
      double num10 = (double) num8;
      double num11 = (double) num7;
      double num12 = (double) num9;
      double num13 = 1.0;
      double num14 = num11;
      double num15 = num14 * num14;
      double num16 = num10;
      double num17 = num16 * num16;
      double num18 = num15 + num17;
      double num19 = num12;
      double num20 = num19 * num19;
      double num21 = System.Math.Sqrt(num18 + num20);
      float num22 = (float) (num13 / num21);
      float num23 = num22 * num7;
      this.Normal.X = num23;
      float num24 = num22 * num8;
      this.Normal.Y = num24;
      float num25 = num22 * num9;
      this.Normal.Z = num25;
      this.D = (float) -((double) point1.Y * (double) num24 + (double) point1.X * (double) num23 + (double) point1.Z * (double) num25);
    }

    public Plane(Vector3 point, Vector3 normal)
    {
      this.Normal = normal;
      this.D = -Vector3.Dot(normal, point);
    }

    public Plane(Vector3 normal, float d)
    {
      this.Normal = normal;
      this.D = d;
    }

    public Plane(float a, float b, float c, float d)
    {
      this.Normal = new Vector3(a, b, c);
      this.D = d;
    }

    public static bool operator ==(Plane left, Plane right)
    {
      return Plane.Equals(ref left, ref right);
    }

    public static bool operator !=(Plane left, Plane right)
    {
      return !Plane.Equals(ref left, ref right);
    }

    public static float Dot(Plane plane, Vector4 point)
    {
      return (float) ((double) plane.Normal.Y * (double) point.Y + (double) plane.Normal.X * (double) point.X + (double) plane.Normal.Z * (double) point.Z + (double) point.W * (double) plane.D);
    }

    public static float DotCoordinate(Plane plane, Vector3 point)
    {
      return (float) ((double) plane.Normal.Y * (double) point.Y + (double) plane.Normal.X * (double) point.X + (double) plane.Normal.Z * (double) point.Z) + plane.D;
    }

    public static float DotNormal(Plane plane, Vector3 point)
    {
      return (float) ((double) plane.Normal.Y * (double) point.Y + (double) plane.Normal.X * (double) point.X + (double) plane.Normal.Z * (double) point.Z);
    }

    public static void Normalize(ref Plane plane, out Plane result)
    {
      double num1 = (double) plane.Normal.Y;
      double num2 = (double) plane.Normal.X;
      double num3 = (double) plane.Normal.Z;
      double num4 = 1.0;
      double num5 = num2;
      double num6 = num5 * num5;
      double num7 = num1;
      double num8 = num7 * num7;
      double num9 = num6 + num8;
      double num10 = num3;
      double num11 = num10 * num10;
      double num12 = System.Math.Sqrt(num9 + num11);
      float num13 = (float) (num4 / num12);
      float num14 = plane.D * num13;
      Vector3 vector3 = new Vector3(plane.Normal.X * num13, plane.Normal.Y * num13, plane.Normal.Z * num13);
      Plane plane1;
      plane1.Normal = vector3;
      plane1.D = num14;
      result = plane1;
    }

    public static Plane Normalize(Plane plane)
    {
      double num1 = (double) plane.Normal.Y;
      double num2 = (double) plane.Normal.X;
      double num3 = (double) plane.Normal.Z;
      double num4 = 1.0;
      double num5 = num2;
      double num6 = num5 * num5;
      double num7 = num1;
      double num8 = num7 * num7;
      double num9 = num6 + num8;
      double num10 = num3;
      double num11 = num10 * num10;
      double num12 = System.Math.Sqrt(num9 + num11);
      float num13 = (float) (num4 / num12);
      float num14 = plane.D * num13;
      Vector3 vector3 = new Vector3(plane.Normal.X * num13, plane.Normal.Y * num13, plane.Normal.Z * num13);
      Plane plane1;
      plane1.Normal = vector3;
      plane1.D = num14;
      return plane1;
    }

    public void Normalize()
    {
      double num1 = (double) this.Normal.Y;
      double num2 = (double) this.Normal.X;
      double num3 = (double) this.Normal.Z;
      double num4 = 1.0;
      double num5 = num2;
      double num6 = num5 * num5;
      double num7 = num1;
      double num8 = num7 * num7;
      double num9 = num6 + num8;
      double num10 = num3;
      double num11 = num10 * num10;
      double num12 = System.Math.Sqrt(num9 + num11);
      float num13 = (float) (num4 / num12);
      this.Normal.X = this.Normal.X * num13;
      this.Normal.Y = this.Normal.Y * num13;
      this.Normal.Z = this.Normal.Z * num13;
      this.D *= num13;
    }

    

    public static Plane[] Transform(Plane[] planes, ref Matrix transformation)
    {
      if (planes == null)
        throw new ArgumentNullException("planes");
      int length = planes.Length;
      Plane[] planeArray = new Plane[length];
      Matrix matrix = Matrix.Invert(transformation);
      int index = 0;
      if (0 < length)
      {
        do
        {
          float num1 = planes[index].Normal.X;
          float num2 = planes[index].Normal.Y;
          float num3 = planes[index].Normal.Z;
          float num4 = planes[index].D;
          planeArray[index] = new Plane()
          {
            Normal = {
              X = (float) ((double) matrix.M12 * (double) num2 + (double) matrix.M11 * (double) num1 + (double) matrix.M13 * (double) num3 + (double) matrix.M14 * (double) num4),
              Y = (float) ((double) matrix.M22 * (double) num2 + (double) matrix.M21 * (double) num1 + (double) matrix.M23 * (double) num3 + (double) matrix.M24 * (double) num4),
              Z = (float) ((double) matrix.M32 * (double) num2 + (double) matrix.M31 * (double) num1 + (double) matrix.M33 * (double) num3 + (double) matrix.M34 * (double) num4)
            },
            D = (float) ((double) matrix.M42 * (double) num2 + (double) matrix.M41 * (double) num1 + (double) matrix.M43 * (double) num3 + (double) matrix.M44 * (double) num4)
          };
          ++index;
        }
        while (index < length);
      }
      return planeArray;
    }

    public static void Transform(ref Plane plane, ref Matrix transformation, out Plane result)
    {
      float num1 = plane.Normal.X;
      float num2 = plane.Normal.Y;
      float num3 = plane.Normal.Z;
      float num4 = plane.D;
      Matrix matrix = Matrix.Invert(transformation);
      result = new Plane()
      {
        Normal = {
          X = (float) ((double) matrix.M12 * (double) num2 + (double) matrix.M11 * (double) num1 + (double) matrix.M13 * (double) num3 + (double) matrix.M14 * (double) num4),
          Y = (float) ((double) matrix.M22 * (double) num2 + (double) matrix.M21 * (double) num1 + (double) matrix.M23 * (double) num3 + (double) matrix.M24 * (double) num4),
          Z = (float) ((double) matrix.M32 * (double) num2 + (double) matrix.M31 * (double) num1 + (double) matrix.M33 * (double) num3 + (double) matrix.M34 * (double) num4)
        },
        D = (float) ((double) matrix.M42 * (double) num2 + (double) matrix.M41 * (double) num1 + (double) matrix.M43 * (double) num3 + (double) matrix.M44 * (double) num4)
      };
    }

    public static Plane Transform(Plane plane, Matrix transformation)
    {
      Plane plane1 = new Plane();
      float num1 = plane.Normal.X;
      float num2 = plane.Normal.Y;
      float num3 = plane.Normal.Z;
      float num4 = plane.D;
      transformation.Invert();
      plane1.Normal.X = (float) ((double) transformation.M12 * (double) num2 + (double) transformation.M11 * (double) num1 + (double) transformation.M13 * (double) num3 + (double) transformation.M14 * (double) num4);
      plane1.Normal.Y = (float) ((double) transformation.M22 * (double) num2 + (double) transformation.M21 * (double) num1 + (double) transformation.M23 * (double) num3 + (double) transformation.M24 * (double) num4);
      plane1.Normal.Z = (float) ((double) transformation.M32 * (double) num2 + (double) transformation.M31 * (double) num1 + (double) transformation.M33 * (double) num3 + (double) transformation.M34 * (double) num4);
      plane1.D = (float) ((double) transformation.M42 * (double) num2 + (double) transformation.M41 * (double) num1 + (double) transformation.M43 * (double) num3 + (double) transformation.M44 * (double) num4);
      return plane1;
    }

   

    public override  string ToString()
    {
      object[] objArray = new object[2]
      {
        (object) this.Normal.ToString(),
        null
      };
      float num = this.D;
      objArray[1] = (object) num.ToString((IFormatProvider) CultureInfo.CurrentCulture);
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Normal:{0} D:{1}", objArray);
    }

    public override  int GetHashCode()
    {
      return this.Normal.GetHashCode() + this.D.GetHashCode();
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public static bool Equals(ref Plane value1, ref Plane value2)
    {
      return value1.Normal == value2.Normal && (double) value1.D == (double) value2.D;
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public bool Equals(Plane other)
    {
      return this.Normal == other.Normal && (double) this.D == (double) other.D;
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public override  bool Equals(object obj)
    {
      if (obj == null || obj.GetType() != this.GetType())
        return false;
      else
        return this.Equals((Plane) obj);
    }
  }
}

