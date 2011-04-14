using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
namespace MHGameWork.TheWizards.ServerClient
{
    public static class MathFunctions
    {
        public static float DistancePointRay( Vector3 point, Ray ray )
        {
            point -= ray.Position;
            float dot = Vector3.Dot( point, ray.Direction );
            Vector3 projectedPoint = ray.Direction * dot;

            return Vector3.Distance( projectedPoint, point );
        }

        public static Vector3 ProjectOnPlane( Vector3 point, Plane p )
        {
            p.Normalize();
            // first find a point on the plane.
            Vector3 planePoint = -p.Normal * p.D;
            // now find the vector from the planePoint through the point on the axis
            Vector3 localAxisPoint = point - planePoint;

            // Project the vector from the plane origin to the axisPoint 
            //  on the plane's normal
            float dot = Vector3.Dot( localAxisPoint, p.Normal );

            point += -p.Normal * dot;

            return point;

        }

        /// <summary>
        /// Returns a plane parallel to p through point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="p">The plane that the returned plane should be parallel with</param>
        /// <returns></returns>
        public static Plane PlaneThroughPoint( Vector3 point, Plane p )
        {
            p.Normalize();
            // first find a point on the plane.
            Vector3 planePoint = -p.Normal * p.D;
            // now find the vector from the planePoint through the point on the axis
            Vector3 localAxisPoint = point - planePoint;

            // Project the vector from the plane origin to the axisPoint 
            //  on the plane's normal
            float dot = Vector3.Dot( localAxisPoint, p.Normal );

            // Now move the plane along the normal. The point on the axis should be on the plane
            p.D -= dot;

            return p;

        }

    }
}
