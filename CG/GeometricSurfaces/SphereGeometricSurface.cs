using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.GeometricSurfaces
{
    public class SphereGeometricSurface : IGeometricSurface
    {
        private readonly float radius;

        public SphereGeometricSurface(float radius)
        {
            this.radius = radius;
        }

        public BoundingBox CalculateBoundingBox()
        {
            return new BoundingBox(Vector3.One * -radius, Vector3.One * radius);
        }

        public void Intersects(ref RayTrace trace, ref TraceResult result)
        {
            float? dist;
            IntersectsSphere(ref trace.Ray, out dist);
            trace.SetNullWhenNotInRange(ref dist);

            if (!dist.HasValue)
                return;

            result.Distance = dist;

            result.GeometryInput.Position = trace.Ray.Position + trace.Ray.Direction * dist.Value;
            result.GeometryInput.Normal = Vector3.Normalize(result.GeometryInput.Position);
        }

        public void IntersectsSphere(ref Ray ray, out float? result)
        {
            float x = -ray.Position.X;
            float y = -ray.Position.Y;
            float z = -ray.Position.Z;
            float length = (float)((double)x * (double)x + (double)y * (double)y + (double)z * (double)z);
            float radiusSquare = radius * radius;
            if (false && (double)length <= (double)radiusSquare)
            {
                result = new float?(0.0f);
            }
            else
            {
                result = new float?();
                float dot = (float)((double)x * (double)ray.Direction.X + (double)y * (double)ray.Direction.Y + (double)z * (double)ray.Direction.Z);
                /*if ((double)dot < 0.0)
                    return;*/
                float diff = length - dot * dot;
                if ((double)diff > (double)radiusSquare)
                    return;
                float root = (float)System.Math.Sqrt((double)radiusSquare - (double)diff);
                result = new float?(dot - root);
                if (result < 0.001)
                    result = dot + root;
            }
        }


    }
}
