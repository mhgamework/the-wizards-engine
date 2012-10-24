using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Raycasting
{
    /// <summary>
    /// This class is responsible for Raycasting an IMesh. Raycasting is done on a per-triangle level.
    /// </summary>
    public class MeshRaycaster
    {
        public static float? RaycastMesh(IMesh mesh, Ray ray, out  MeshRaycastResult result)
        {
            result = new MeshRaycastResult();
            var tempResult = new MeshRaycastResult();

            // Keep track of the closest triangle we found so far,
            // so we can always return the closest one.
            float? closestIntersection = null;

            var xnaRay = ray.xna();


            foreach (var part in mesh.GetCoreData().Parts)
            {
                var original = part.ObjectMatrix.dx();
                var invert = Matrix.Invert(original);
                var partRay = new Ray(Vector3.TransformCoordinate(ray.Position, invert),
                                      Vector3.TransformNormal(ray.Direction, invert));


                Vector3 v1, v2, v3;
                var intersection =
                    RaycastMeshPart(
                        part,
                        partRay, out tempResult);

                // Does the ray intersect this triangle?
                if (intersection == null) continue;
                // If so, is it closer than any other previous triangle?
                if ((closestIntersection != null) && (intersection >= closestIntersection)) continue;

                // Store the distance to this triangle.
                closestIntersection = intersection;
                result = tempResult;

                // Transform the three vertex positions into world space,
                // and store them into the output vertex parameters.
                //TODO: transform normals!!
                Vector3.TransformCoordinate(ref result.Vertex1.Position, ref original, out result.Vertex1.Position);
                Vector3.TransformCoordinate(ref result.Vertex2.Position, ref original, out result.Vertex2.Position);
                Vector3.TransformCoordinate(ref result.Vertex2.Position, ref original, out result.Vertex3.Position);
            }

            return closestIntersection;
        }

        public static float? RaycastMeshPart(MeshCoreData.Part part, Ray ray, out MeshRaycastResult result)
        {

            Microsoft.Xna.Framework.Vector3[] vertices = part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            Microsoft.Xna.Framework.Vector3[] normals = part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
            Microsoft.Xna.Framework.Vector2[] texcoords = part.MeshPart.GetGeometryData().GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);

            result = new MeshRaycastResult();
            

            float? closestIntersection = null;

            var xnaRay = ray.xna();

            for (int i = 0; i < vertices.Length; i += 3)
            {
                // Perform a ray to triangle intersection test.
                float? intersection;

                float u;
                float v;
                Functions.RayIntersectsTriangle(ref xnaRay, ref vertices[i], ref vertices[i + 1], ref vertices[i + 2], out intersection, out u, out v);


                // Does the ray intersect this triangle?

                if (intersection == null) continue;
                // If so, is it closer than any other previous triangle?
                if ((closestIntersection != null) && (intersection >= closestIntersection)) continue;

                // Store the distance to this triangle.
                closestIntersection = intersection;
                result.Vertex1 = new VertexPositionNormalTexture
                {
                    Position = vertices[i].dx(),
                    Normal = normals[i].dx(),
                    Texcoord = texcoords[i].dx()
                };
                result.Vertex2 = new VertexPositionNormalTexture
                {
                    Position = vertices[i + 1].dx(),
                    Normal = normals[i + 1].dx(),
                    Texcoord = texcoords[i + 1].dx()
                };
                result.Vertex3 = new VertexPositionNormalTexture
                {
                    Position = vertices[i + 2].dx(),
                    Normal = normals[i + 2].dx(),
                    Texcoord = texcoords[i + 2].dx()
                };

                result.U = u;
                result.V = v;



            }
            return closestIntersection;
        }

    }
}
