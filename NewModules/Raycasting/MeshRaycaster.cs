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
        public static float? RaycastMesh(IMesh mesh, Ray ray, out Vector3 vertex1, out Vector3 vertex2, out Vector3 vertex3)
        {
            vertex1 = vertex2 = vertex3 = Vector3.Zero;

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
                        part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position),
                        partRay, out v1, out v2, out v3);

                // Does the ray intersect this triangle?
                if (intersection == null) continue;
                // If so, is it closer than any other previous triangle?
                if ((closestIntersection != null) && (intersection >= closestIntersection)) continue;

                // Store the distance to this triangle.
                closestIntersection = intersection;

                // Transform the three vertex positions into world space,
                // and store them into the output vertex parameters.
                Vector3.TransformCoordinate(ref v1, ref original, out vertex1);
                Vector3.TransformCoordinate(ref v2, ref original, out vertex2);
                Vector3.TransformCoordinate(ref v3, ref original, out vertex3);
            }

            return closestIntersection;
        }

        public static float? RaycastMeshPart(Vector3[] vertices, Ray ray, out Vector3 vertex1, out Vector3 vertex2,
                                              out Vector3 vertex3)
        {
            Vector3 normal;
            return RaycastMeshPart(vertices, ray, out vertex1, out vertex2, out vertex3, out normal);
        }
        public static float? RaycastMeshPart(Vector3[] vertices, Ray ray, out Vector3 vertex1, out Vector3 vertex2, out Vector3 vertex3, out Vector3 normal)
        {
            vertex1 = vertex2 = vertex3 = normal = Vector3.Zero;

            float? closestIntersection = null;

            var xnaRay = ray.xna();

            for (int i = 0; i < vertices.Length; i += 3)
            {
                // Perform a ray to triangle intersection test.
                float? intersection;
                var vert0 = vertices[i].xna();
                var vert1 = vertices[i + 1].xna();
                var vert2 = vertices[i + 2].xna();

                Functions.RayIntersectsTriangle(ref xnaRay, ref vert0, ref vert1, ref vert2, out intersection);


                // Does the ray intersect this triangle?

                if (intersection == null) continue;
                // If so, is it closer than any other previous triangle?
                if ((closestIntersection != null) && (intersection >= closestIntersection)) continue;

                // Store the distance to this triangle.
                closestIntersection = intersection;
                vertex1 = vertices[i];
                vertex2 = vertices[i + 1];
                vertex3 = vertices[i + 2];
                normal = Vector3.Normalize(Microsoft.Xna.Framework.Vector3.Cross(vert2 - vert0, vert1 - vert0).dx());


            }
            return closestIntersection;
        }
        public static float? RaycastMeshPart(Microsoft.Xna.Framework.Vector3[] vertices, Ray ray, out Vector3 vertex1, out Vector3 vertex2, out Vector3 vertex3)
        {
            vertex1 = vertex2 = vertex3 = Vector3.Zero;

            float? closestIntersection = null;

            var xnaRay = ray.xna();

            for (int i = 0; i < vertices.Length; i += 3)
            {
                // Perform a ray to triangle intersection test.
                float? intersection;

                Functions.RayIntersectsTriangle(ref xnaRay, ref vertices[i], ref vertices[i + 1], ref vertices[i + 2], out intersection);


                // Does the ray intersect this triangle?

                if (intersection == null) continue;
                // If so, is it closer than any other previous triangle?
                if ((closestIntersection != null) && (intersection >= closestIntersection)) continue;

                // Store the distance to this triangle.
                closestIntersection = intersection;
                vertex1 = vertices[i].dx();
                vertex2 = vertices[i + 1].dx();
                vertex3 = vertices[i + 2].dx();


            }
            return closestIntersection;
        }

    }
}
