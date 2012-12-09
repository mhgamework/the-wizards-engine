using System;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.GeometricSurfaces
{
    public class PlaneGeometry : IGeometry
    {
        private Plane plane;

        public PlaneGeometry(Plane plane)
        {
            this.plane = plane;
        }

        public BoundingBox CalculateBoundingBox()
        {
            //TODO: what is zis?
            return new BoundingBox(Vector3.One*float.MinValue, Vector3.One*float.MaxValue);
        }

        public void Intersects(ref RayTrace trace, ref TraceResult result)
        {
            float? dist;
            IntersectsPlane(ref trace.Ray, out dist);
            trace.SetNullWhenNotInRange(ref dist);

            if ( !dist.HasValue)
                return;

            result.Distance = dist;

            result.Normal = plane.Normal;
        }

        //TODO:
        public void IntersectsPlane(ref Ray ray, out float? result)
        {
            float num1 = (float)((double)plane.Normal.X * (double)ray.Direction.X + (double)plane.Normal.Y * (double)ray.Direction.Y + (double)plane.Normal.Z * (double)ray.Direction.Z);
            if ((double)System.Math.Abs(num1) < 9.99999974737875E-06)
            {
                result = new float?();
            }
            else
            {
                float num2 = (float)((double)plane.Normal.X * (double)ray.Position.X + (double)plane.Normal.Y * (double)ray.Position.Y + (double)plane.Normal.Z * (double)ray.Position.Z);
                float num3 = (-plane.D - num2) / num1;
                if ((double)num3 < 0.0)
                {
                    if ((double)num3 < -9.99999974737875E-06)
                    {
                        result = new float?();
                        return;
                    }
                    else
                        result = new float?(0.0f);
                }
                result = new float?(num3);
            }
        }

  
    }
}
