using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.GeometricSurfaces
{
    public class SphereGeometry : IGeometry
    {
        private readonly float radius;
        public Vector3 Position = new Vector3();

        public SphereGeometry(float radius)
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
            IntersectsSphere(ref trace, out dist);
            trace.SetNullWhenNotInRange(ref dist);

            if (!dist.HasValue)
                return;

            result.Distance = dist;

            var hit = trace.Ray.Position + trace.Ray.Direction * dist.Value;
            result.Normal = Vector3.Normalize(hit);
        }

        public void IntersectsSphere(ref RayTrace trace, out float? result)
        {
            var e = trace.Ray.Position;
            var d = trace.Ray.Direction;
            var c = Position;


            var eMinC = e - c;
            float dDotD = Vector3.Dot(d, d);
            float leftTerm = Vector3.Dot(d, eMinC);
            float rightTerm = dDotD * (Vector3.Dot(eMinC, eMinC) - radius * radius);
            
            float discrSquared = leftTerm*leftTerm - rightTerm;
            
            result = new float?();
            if (discrSquared < 0) return;
            float root = (float)System.Math.Sqrt(discrSquared);
            result = -leftTerm - root;
            if (result < trace.Start)
                result = -leftTerm + root;

            result = result/dDotD;
        }


    }
}
