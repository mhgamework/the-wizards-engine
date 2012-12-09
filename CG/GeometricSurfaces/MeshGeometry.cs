using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;
using System.Linq;

namespace MHGameWork.TheWizards.CG.GeometricSurfaces
{
    public class MeshGeometry : IGeometry
    {
        private TangentVertex[] vertices;

        public MeshGeometry(TangentVertex[] vertices)
        {
            this.vertices = vertices;
        }

        public BoundingBox CalculateBoundingBox()
        {
            return
               BoundingBox.FromPoints(vertices.Select(t => t.pos).ToArray());
        }

        public void Intersects(ref RayTrace trace, ref TraceResult result)
        {
            var newResult = new TraceResult();

            for (int i = 0; i < vertices.Length; i += 3)
            {
                var v1 = vertices[i].pos;
                var v2 = vertices[i + 1].pos;
                var v3 = vertices[i + 2].pos;

                var ray = trace.Ray;

                float? dist;
                float v;
                float u;
                Functions.RayIntersectsTriangle(ref ray, ref v1, ref v2, ref v3, out dist, out u, out v);
                trace.SetNullWhenNotInRange(ref dist);
                if (dist == null)
                    break;

                newResult.Distance = dist;
                TriangleGeometry.SetGeometryInfoOnResult(ref vertices, i, ref trace, ref u, ref v, ref newResult);

                result.AddResult(ref newResult);
            }

        }

    }
}
