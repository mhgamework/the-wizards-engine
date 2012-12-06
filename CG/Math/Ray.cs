// Type: SlimDX.Ray
// Assembly: SlimDX, Version=4.0.13.43, Culture=neutral, PublicKeyToken=b1b0c32fd1ffe4f9
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_32\SlimDX\v4.0_4.0.13.43__b1b0c32fd1ffe4f9\SlimDX.dll

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MHGameWork.TheWizards.CG.Math
{
  [Serializable]
  public struct Ray : IEquatable<Ray>
  {
    public Vector3 Position;
    public Vector3 Direction;

    public Ray(Vector3 position, Vector3 direction)
    {
      this.Position = position;
      this.Direction = direction;
    }

    public static bool operator ==(Ray left, Ray right)
    {
      return Ray.Equals(ref left, ref right);
    }

    public static bool operator !=(Ray left, Ray right)
    {
      return !Ray.Equals(ref left, ref right);
    }

    public void Intersects(ref BoundingBox box, out float? result)
    {
        box.Intersects( ref this, out result);
    }
    
    public override  string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Position:{0} Direction:{1}", new object[2]
      {
        (object) this.Position.ToString(),
        (object) this.Direction.ToString()
      });
    }

    public override  int GetHashCode()
    {
      return this.Direction.GetHashCode() + this.Position.GetHashCode();
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public static bool Equals(ref Ray value1, ref Ray value2)
    {
      return value1.Position == value2.Position && value1.Direction == value2.Direction;
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public bool Equals(Ray other)
    {
      return this.Position == other.Position && this.Direction == other.Direction;
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public override  bool Equals(object obj)
    {
      if (obj == null || obj.GetType() != this.GetType())
        return false;
      else
        return this.Equals((Ray) obj);
    }
  }
}
