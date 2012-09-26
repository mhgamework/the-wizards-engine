using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Raycast
{
    /// <summary>
    /// To be renamed!
    /// This class is responsible for providing all supported types of raycasting in TW. 
    /// If necessarry this class could be expanded to a system where 'raycasters' can be added for new types of raycastable objects
    /// </summary>
    public class MainRaycaster
    {
        public void Start(Ray ray)
        {

        }
        public void Add(IMesh mesh, Matrix worldMatrix)
        {
            if (fullData == null) throw new Exception("EditorFullModelData not loaded!");

            vertex1 = vertex2 = vertex3 = Vector3.Zero;

            // The input ray is in world space, but our model data is stored in object
            // space. We would normally have to transform all the model data by the
            // modelTransform matrix, moving it into world space before we test it
            // against the ray. That transform can be slow if there are a lot of
            // triangles in the model, however, so instead we do the opposite.
            // Transforming our ray by the inverse modelTransform moves it into object
            // space, where we can test it directly against our model data. Since there
            // is only one ray but typically many triangles, doing things this way
            // around can be much faster.

            Matrix modelMatrix = fullData.ObjectMatrix * modelWorldMatrix;
            Matrix inverseTransform = Matrix.Invert(modelMatrix);

            ray.Position = Vector3.Transform(ray.Position, inverseTransform);
            ray.Direction = Vector3.TransformNormal(ray.Direction, inverseTransform);


            //ray.Direction.Normalize();


            //Little trick: the ray direction is not normalized, but the algorithm seems to work using that value, but the bounding check doesn't
            Ray ray2 = ray;
            ray2.Direction.Normalize();

            if (!fullData.BoundingSphere.Intersects(ray2).HasValue)
            {
                // If the ray does not intersect the bounding sphere, we cannot
                // possibly have picked this model, so there is no need to even
                // bother looking at the individual triangle data.
                //insideBoundingSphere = false;

                return null;
            }


            // The bounding sphere test passed, so we need to do a full
            // triangle picking test.
            //insideBoundingSphere = true;

            // Keep track of the closest triangle we found so far,
            // so we can always return the closest one.
            float? closestIntersection = null;

            // Loop over the vertex data, 3 at a time (3 vertices = 1 triangle).
            Vector3[] vertices = fullData.Positions;

            for (int i = 0; i < vertices.Length; i += 3)
            {
                // Perform a ray to triangle intersection test.
                float? intersection;

                Functions.RayIntersectsTriangle(ref ray,
                                                                        ref vertices[i],
                                                                        ref vertices[i + 1],
                                                                        ref vertices[i + 2],
                                                                        out intersection);


                // Does the ray intersect this triangle?
                if (intersection != null)
                {
                    // If so, is it closer than any other previous triangle?
                    if ((closestIntersection == null) ||
                         (intersection < closestIntersection))
                    {
                        // Store the distance to this triangle.
                        closestIntersection = intersection;

                        // Transform the three vertex positions into world space,
                        // and store them into the output vertex parameters.
                        Vector3.Transform(ref vertices[i],
                                           ref modelMatrix, out vertex1);

                        Vector3.Transform(ref vertices[i + 1],
                                           ref modelMatrix, out vertex2);

                        Vector3.Transform(ref vertices[i + 2],
                                           ref modelMatrix, out vertex3);
                    }
                }
            }

            return closestIntersection;
        }
    }
}
