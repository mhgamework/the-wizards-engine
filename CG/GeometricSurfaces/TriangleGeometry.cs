using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.GeometricSurfaces
{
    public class TriangleGeometry : IGeometry
    {
        private readonly TangentVertex[] vertices;
        private readonly int startVertexIndex;

        public TriangleGeometry(TangentVertex[] vertices, int triangleIndex)
        {
            this.vertices = vertices;
            this.startVertexIndex = triangleIndex * 3;
        }

        public BoundingBox CalculateBoundingBox()
        {
            return
               BoundingBox.FromPoints(new[]
                                           {
                                               vertices[startVertexIndex].pos,
                                               vertices[startVertexIndex+1].pos,
                                               vertices[startVertexIndex+2].pos
                                           });
        }

        public void Intersects(ref RayTrace trace, ref TraceResult result)
        {
            var v1 = vertices[startVertexIndex].pos;
            var v2 = vertices[startVertexIndex + 1].pos;
            var v3 = vertices[startVertexIndex + 2].pos;

            var ray = trace.Ray;

            float? dist;
            float v;
            float u;
            Functions.RayIntersectsTriangle(ref ray, ref v1, ref v2, ref v3, out dist, out u, out v);
            trace.SetNullWhenNotInRange(ref dist);
            if (dist == null)
                return;

            result.Distance = dist;

            CalculateColor(ref trace, ref u, ref v, ref result);

        }

        public void CalculateColor(ref RayTrace trace, ref float U, ref float V, ref TraceResult input)
        {
            var Vertex1 = vertices[startVertexIndex];
            var Vertex2 = vertices[startVertexIndex + 1];
            var Vertex3 = vertices[startVertexIndex + 2];

            input.Normal = (Vertex2.normal * U + Vertex3.normal * V + Vertex1.normal * (1 - U - V));

            //input.Normal =
            //    Vector3.Normalize(-Vector3.Cross((raycast.Vertex1.Position - raycast.Vertex2.Position),
            //                                    (raycast.Vertex1.Position - raycast.Vertex3.Position)));

            //input.Normal = raycast.Vertex1.Normal;
            input.Normal = Vector3.Normalize(input.Normal); // Renormalize!
            input.Texcoord = (Vertex2.uv * U + Vertex3.uv * V + Vertex1.uv * (1 - U - V));


        }


    }
}
