using System;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.MathExtra
{
    /// <summary>
    /// Doubtfull whether this namespace should exist, it is not clearly a module.
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Returns a rotation quaternion that rotates the original vector 
        /// so that it becomes target vector
        /// </summary>
        /// <returns></returns>
        public static Quaternion CreateRotationBetweenVectors(Vector3 original, Vector3 target)
        {
            // This of course can be faster, but the coding is fastest this way!
            // First get rotation from original to forward , then from target to forward, then mix!

            Quaternion qOri = CreateFromLookDir(original);
            Quaternion qTarget = CreateFromLookDir(target);

            // F * A = O
            // F * B = T
            // O * X = T
            // F * A * X = F * B
            // X = A^(-1) * B

            // Quaternion total = qTarget * Quaternion.Inverse(qOri);

            // Dus, natuurlijk werkt dit niet. Maar gelukkig blijkt het volgende te werken

            Quaternion total = qTarget * Quaternion.Inverse(qOri);

#if (DEBUG)
            Vector3 newDir = Vector3.Transform(original, total);
            if (!VectorsEqual(newDir, target)) throw new Exception("This algoritm doesnt work!");
#endif

            return total;
        }

        /// <summary>
        /// Returns a rotation quaternion that makes the camera in its current position
        /// look at a given point.
        /// </summary>
        /// <returns></returns>
        public static Quaternion CreateFromLookDir(Vector3 dir)
        {
            dir.Normalize();

            // now some magic trigoniometry
            // TODO: This probably can be done faster

            // dir.y ^ 2 + radius ^ 2 = dir.lengthsquared
            float radius = (float)Math.Sqrt(1 - dir.Y * dir.Y);

            float angleY;
            float angleX;
            if (radius < 0.0001)
            {
                if (dir.Y > 0)
                {
                    angleY = 0;
                    angleX = MathHelper.PiOver2;
                }
                else
                {
                    angleY = 0;
                    angleX = -MathHelper.PiOver2;
                }

            }
            else
            {

                angleY = (float)Math.Acos(MathHelper.Clamp(-dir.Z / radius, -1, 1));
                angleX = (float)Math.Asin(MathHelper.Clamp(dir.Y, -1, 1));



                if (dir.X > 0) angleY = -angleY;
                //if ( dir.Z > 0 && dir.Y > 0 ) angleX = angleX + MathHelper.PiOver2;
                //if ( dir.Z > 0 && dir.Y < 0 ) angleX = angleX - MathHelper.PiOver2;

            }

            Quaternion q;
            q = Quaternion.CreateFromYawPitchRoll(angleY, angleX, 0);

            //Since i was stupid enough to design this algoritm with the base vector vector.Right
            // i need to add this line to get vector3.forward
            //q = q * Quaternion.CreateFromAxisAngle( Vector3.Up, -MathHelper.PiOver2 );

#if (DEBUG)
            Vector3 newDir = Vector3.Transform(Vector3.Forward, q);
            if (!VectorsEqual(dir, newDir)) throw new Exception("This algoritm doesnt work!");
#endif
            return q;
        }

        private static bool VectorsEqual(Vector3 v1, Vector3 v2)
        {
            Vector3 diff = v1 - v2;
            if (Math.Abs(diff.X) > 0.01) return false;
            if (Math.Abs(diff.Y) > 0.01) return false;
            if (Math.Abs(diff.Z) > 0.01) return false;

            return true;
        }
        /*private static bool QuaternionsEqual( Quaternion q1, Quaternion q2 )
        {
            // My quess is that i need to normalize the quats first
            q1.Normalize();
            q2.Normalize();
            Quaternion diff = q1 - q2;
            
            if ( Math.Abs( diff.X ) > 0.01 ) return false;
            if ( Math.Abs( diff.Y ) > 0.01 ) return false;
            if ( Math.Abs( diff.Z ) > 0.01 ) return false;
            if ( Math.Abs( diff.W ) > 0.01 ) return false;

            return true;
        }*/

        /*
  
*/
        /// <summary>
        ///  Calculate the line segment PaPb that is the shortest route between
        /// two lines P1P2 and P3P4. Calculate also the values of mua and mub where
        /// Pa = P1 + mua (P2 - P1)
        /// Pb = P3 + mub (P4 - P3)
        /// Return FALSE if no solution exists.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="pa"></param>
        /// <param name="pb"></param>
        /// <param name="mua"></param>
        /// <param name="mub"></param>
        /// <returns></returns>
        public static bool LineLineIntersect(
           Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 pa, out  Vector3 pb,
           out float mua, out  float mub)
        {
            //TODO: recode this to conventional vector math and use the Ray class

            pb = Vector3.Zero;
            pa = Vector3.Zero;
            mua = float.NaN;
            mub = float.NaN;


            float epsilon = 0.0001f;

            Vector3 p13, p43, p21;
            float d1343, d4321, d1321, d4343, d2121;
            float numer, denom;

            p13.X = p1.X - p3.X;
            p13.Y = p1.Y - p3.Y;
            p13.Z = p1.Z - p3.Z;
            p43.X = p4.X - p3.X;
            p43.Y = p4.Y - p3.Y;
            p43.Z = p4.Z - p3.Z;
            if (Math.Abs(p43.X) < epsilon && Math.Abs(p43.Y) < epsilon && Math.Abs(p43.Z) < epsilon)
                return (false);
            p21.X = p2.X - p1.X;
            p21.Y = p2.Y - p1.Y;
            p21.Z = p2.Z - p1.Z;
            if (Math.Abs(p21.X) < epsilon && Math.Abs(p21.Y) < epsilon && Math.Abs(p21.Z) < epsilon)
                return (false);

            d1343 = p13.X * p43.X + p13.Y * p43.Y + p13.Z * p43.Z;
            d4321 = p43.X * p21.X + p43.Y * p21.Y + p43.Z * p21.Z;
            d1321 = p13.X * p21.X + p13.Y * p21.Y + p13.Z * p21.Z;
            d4343 = p43.X * p43.X + p43.Y * p43.Y + p43.Z * p43.Z;
            d2121 = p21.X * p21.X + p21.Y * p21.Y + p21.Z * p21.Z;

            denom = d2121 * d4343 - d4321 * d4321;
            if (Math.Abs(denom) < epsilon)
                return (false);
            numer = d1343 * d4321 - d1321 * d4343;

            mua = numer / denom;
            mub = (d1343 + d4321 * (mua)) / d4343;

            pa.X = p1.X + mua * p21.X;
            pa.Y = p1.Y + mua * p21.Y;
            pa.Z = p1.Z + mua * p21.Z;
            pb.X = p3.X + mub * p43.X;
            pb.Y = p3.Y + mub * p43.Y;
            pb.Z = p3.Z + mub * p43.Z;

            return (true);
        }


        public static Vector3 QuatToEuler(Quaternion q1)
        {
            double heading, attitude, bank;
            Vector3 Euler;
            double test = q1.X * q1.Y + q1.Z * q1.W;
            if (test > 0.499)
            { // singularity at north pole
                heading = 2 * Math.Atan2(q1.X, q1.W);
                attitude = Math.PI / 2;
                bank = 0;
            }
            else if (test < -0.499)
            { // singularity at south pole
                heading = -2 * Math.Atan2(q1.X, q1.W);
                attitude = -Math.PI / 2;
                bank = 0;
            }
            else
            {
                double sqx = q1.X * q1.X;
                double sqy = q1.Y * q1.Y;
                double sqz = q1.Z * q1.Z;
                heading = Math.Atan2(2 * q1.Y * q1.W - 2 * q1.X * q1.Z, 1 - 2 * sqy - 2 * sqz);
                attitude = Math.Asin(2 * test);
                bank = Math.Atan2(2 * q1.X * q1.W - 2 * q1.Y * q1.Z, 1 - 2 * sqx - 2 * sqz);

            }

            Euler = new Vector3((float)attitude, (float)heading, (float)bank);
            throw new InvalidOperationException("A debug check should be finished here first!");
            /*#if DEBUG
                        Quaternion qResult = Quaternion.CreateFromYawPitchRoll( Euler.Y, Euler.X, Euler.Z );
                        if ( !QuaternionsEqual( q1, qResult ) ) throw new Exception( "Error in the algoritm" );
            #endif*/

            return Euler;
        }

        /// <summary>
        /// Checks whether a ray intersects a triangle. This uses the algorithm
        /// developed by Tomas Moller and Ben Trumbore, which was published in the
        /// Journal of Graphics Tools, volume 2, "Fast, Minimum Storage Ray-Triangle
        /// Intersection".
        /// 
        /// This method is implemented using the pass-by-reference versions of the
        /// XNA math functions. Using these overloads is generally not recommended,
        /// because they make the code less readable than the normal pass-by-value
        /// versions. This method can be called very frequently in a tight inner loop,
        /// however, so in this particular case the performance benefits from passing
        /// everything by reference outweigh the loss of readability.
        /// </summary>
        public static void RayIntersectsTriangle(ref Ray ray,
                                          ref Vector3 vertex1,
                                          ref Vector3 vertex2,
                                          ref Vector3 vertex3, out float? result)
        {
            // Compute vectors along two edges of the triangle.
            Vector3 edge1, edge2;

            Vector3.Subtract(ref vertex2, ref vertex1, out edge1);
            Vector3.Subtract(ref vertex3, ref vertex1, out edge2);

            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref ray.Direction, ref edge2, out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                result = null;
                return;
            }

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref ray.Position, ref vertex1, out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                result = null;
                return;
            }

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref edge1, out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref ray.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                result = null;
                return;
            }

            // Compute the distance along the ray to the triangle.
            float rayDistance;
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                result = null;
                return;
            }

            result = rayDistance;
        }
        /// <summary>
        /// This use MHGW Shiznitz based on the moller (see above)
        /// </summary>
        public static void RayIntersectsSquare(ref Ray ray,
                                          ref Vector3 vertex1,
                                          ref Vector3 vertex2,
                                          ref Vector3 vertex3,
                                          ref Vector3 vertex4, out float? result)
        {
            // Compute vectors along two edges of the triangle.
            Vector3 edge1, edge2;

            Vector3.Subtract(ref vertex2, ref vertex1, out edge1);
            Vector3.Subtract(ref vertex3, ref vertex1, out edge2);

            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref ray.Direction, ref edge2, out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                result = null;
                return;
            }

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref ray.Position, ref vertex1, out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                result = null;
                return;
            }

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref edge1, out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref ray.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleV > 1)
            {
                result = null;
                return;
            }

            // Compute the distance along the ray to the triangle.
            float rayDistance;
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                result = null;
                return;
            }

            result = rayDistance;
        }

        public static Ray CreateRayFromViewInverse(Matrix viewInverse)
        {
            return new Ray { Position = viewInverse.Translation, Direction = viewInverse.Forward };
        }
    }
}
