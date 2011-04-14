using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient
{
    public struct Torus
    {
        public Vector3 Center;
        //public Vector3 Normal;
        /// <summary>
        /// When you transform the UnitZ by this orientation you get the torus' normal
        /// </summary>
        public Quaternion Orientation;
        /// <summary>
        /// Outer Radius.
        /// Radius from the center to the circle.
        /// </summary>
        public float Radius1;
        /// <summary>
        /// Inner Radius
        /// </summary>
        public float Radius2;

        public Torus( Vector3 center, Quaternion orientation, float radius1, float radius2 )
        {
            Center = center;
            Orientation = orientation;
            Radius1 = radius1;
            Radius2 = radius2;

        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="ray"></param>
        /// <remarks>Based on an article by Max Wagner at http://www.emeyex.com/site/projects/raytorus.pdf
        /// </remarks>
        /// <returns></returns>
        public float? Intersects( Ray ray )
        {
            //Move the ray so torus is at origin
            ray.Position -= Center;
            //Now transform the ray so the torus is in the xy plane for the algorithm to work.
            //Dit was van toen ik torus een normal gaf, aangezien ik dit elke keer bereken,
            //is het belachlijk om de quaternion zelf niet gewoon op te slaan
            //Quaternion q = MathExtra.Functions.CreateRotationBetweenVectors( Normal, Vector3.UnitZ );

            ray.Position = Vector3.Transform( ray.Position, Orientation );
            ray.Direction = Vector3.Transform( ray.Direction, Orientation );

            // Using double for more precision
            double alfa = Vector3.Dot( ray.Direction, ray.Direction );
            double beta = 2 * Vector3.Dot( ray.Position, ray.Direction );
            double gamma = Vector3.Dot( ray.Position, ray.Position ) - Radius2 * Radius2 - Radius1 * Radius1;

            //Polynomial: a4 * t^4 + a3 * t^3 + a2 * t^2 + a1 * t^1 + a0 = 0
            double a4 = alfa * alfa;
            double a3 = 2 * alfa * beta;
            double a2 = beta * beta + 2 * alfa * gamma + 4 * Radius1 * Radius1 * ray.Direction.Z * ray.Direction.Z;
            double a1 = 2 * beta * gamma + 8 * Radius1 * Radius1 * ray.Position.Z * ray.Direction.Z;
            double a0 = gamma * gamma + 4 * Radius1 * Radius1 * ray.Position.Z * ray.Position.Z - 4 * Radius1 * Radius1 * Radius2 * Radius2;

            /* Now get the 4 roots */

            MathExtra.Polynomial4 p = new MHGameWork.TheWizards.ServerClient.MathExtra.Polynomial4( a0, a1, a2, a3, a4 );

            //TODO: the distances
            double minDistance = -4000;
            double maxDistance = 4000;
            double precision = 0.00001;
            double secondaryPrecision = 0.0001;
            int iterations = 200000;
            //TODO: whopping number of iterations
            double[] roots = new double[ 4 ];
            roots[ 0 ] = MathExtra.Solve.solve( p, 1, minDistance, maxDistance, precision, iterations );
            roots[ 1 ] = MathExtra.Solve.solve( p, 2, minDistance, maxDistance, precision, iterations );
            roots[ 2 ] = MathExtra.Solve.solve( p, 3, minDistance, maxDistance, precision, iterations );
            roots[ 3 ] = MathExtra.Solve.solve( p, 4, minDistance, maxDistance, precision, iterations );

            // find closest intersection
            double closest = double.NaN;
            for ( int i = 0; i < 4; i++ )
            {
                // Ignore negative roots, only get intersections in front of the ray.
                if ( roots[ i ] < -secondaryPrecision ) continue;
                //Because of the solver method sometimes returning the bounds of the interval, we double check if the root is zero
                if ( Math.Abs( p.valueAt( roots[ i ] ) ) < secondaryPrecision )
                {
                    //This actually is zero!
                    if ( double.IsNaN( closest ) )
                        closest = roots[ i ];
                    else if ( roots[ i ] < closest ) closest = roots[ i ];
                }
            }

            //Since we only rotated and translated our coordinate system at the start of this method,
            //the distances we calculated are the same as before our coordinate system change (since no scaling occured)

            if ( double.IsNaN( closest ) ) return null; else return (float)closest;


        }


        /// <summary>
        /// Not working approach with vectors (self written of course :P)
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        //public float? Intersects( Ray ray )
        //{

        //    // Move ray to origin
        //    Vector3 localCenter = Center - ray.Position;

        //    // Get the normal of the plane that goes through the ray and the center of the torus.
        //    // we can use cross product since we placed the ray's position at the origin.
        //    Vector3 planeNormal = Vector3.Cross( localCenter, ray.Direction );
        //    planeNormal.Normalize();

        //    // Now we look for the line that is formed by the intersection of this plane and the plane
        //    //  in which the torus is located.
        //    Vector3 intersectionLineDir = Vector3.Cross( Normal, planeNormal );
        //    intersectionLineDir.Normalize();

        //    // Now we have 2 possible points on the circle inside the torus. Just calculate the distance
        //    //  to each of the points and use the closest.
        //    Vector3 p1 = Center + intersectionLineDir * Radius1;
        //    Vector3 p2 = Center + intersectionLineDir * -Radius1;

        //    // We can simply this by just calculating the intersection distance to the spheres with 
        //    //  p1 en p2 the centres and radius2
        //    BoundingSphere sphere = new BoundingSphere();
        //    sphere.Radius = Radius2;

        //    sphere.Center = p1;
        //    float? dist1 = ray.Intersects( sphere );

        //    sphere.Center = p2;
        //    float? dist2 = ray.Intersects( sphere );

        //    TestXNAGame.Instance.LineManager3D.AddCenteredBox( p1, 0.2f, Color.Red );
        //    TestXNAGame.Instance.LineManager3D.AddCenteredBox( p2, 0.2f, Color.Red );

        //    if ( !dist1.HasValue ) return dist2;
        //    if ( !dist2.HasValue ) return dist1;
        //    if ( dist1.Value > dist2.Value ) return dist2; else return dist1;
        //}

        public static void TestCalculateDistanceLineTorus()
        {
            TestXNAGame game = new TestXNAGame( "TestCalculateDistanceLineTorus" );

            //bool rayTypeCamera = true;

            game.RenderAxis = false;

            Torus torus = new Torus();
            torus.Center = new Vector3( 4, 0, 0 );
            torus.Orientation = Quaternion.Identity;//Quaternion.CreateFromYawPitchRoll( MathHelper.PiOver2, 0, 0 );
            torus.Radius1 = 3;
            torus.Radius2 = 1f;

            Ray ray = new Ray();
            ray.Position = new Vector3( 0, 0, 0 );
            ray.Direction = new Vector3( 0, 0, 1 );

            game.renderCode = delegate
            {
                if ( !game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.R ) )
                {
                    game.Mouse.CursorEnabled = false;
                    if ( game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Up ) )
                    {
                        ray.Position += ray.Direction * 0.01f;
                    }
                    if ( game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Down ) )
                    {
                        ray.Position += ray.Direction * -0.01f;
                    }
                    if ( game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.E ) )
                    {
                        ray.Position += Vector3.Up * 0.01f;
                    }
                    if ( game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.A ) )
                    {
                        ray.Position += Vector3.Up * -0.01f;
                    }
                    if ( game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Left ) )
                    {
                        ray.Direction = Vector3.Transform( ray.Direction, Matrix.CreateRotationY( 0.01f ) );
                        ray.Direction.Normalize();
                    }
                    if ( game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Right ) )
                    {
                        ray.Direction = Vector3.Transform( ray.Direction, Matrix.CreateRotationY( -0.01f ) );
                        ray.Direction.Normalize();
                    }
                }
                else
                {
                    game.Mouse.CursorEnabled = true;
                    ray = game.GetWereldViewRay( game.Mouse.CursorPositionVector );
                }

                float? dist = torus.Intersects( ray );

                if ( dist.HasValue ) game.Window.Title = dist.ToString();

                if ( dist.HasValue ) game.LineManager3D.AddCenteredBox( ray.Position + ray.Direction * dist.Value, 0.2f, Color.Yellow );

                game.LineManager3D.AddCenteredBox( ray.Position, 0.2f, Color.Green );
                game.LineManager3D.AddLine( ray.Position, ray.Position + ray.Direction * 20, Color.Green );
                game.LineManager3D.AddLine( ray.Position, ray.Position + ray.Direction * -20, Color.LightGreen );


                // cross with any vector to get perpendicular vector

                Vector3 Normal = Vector3.Transform( Vector3.UnitZ, torus.Orientation );

                game.LineManager3D.AddCenteredBox( torus.Center, 0.2f, Color.Red );
                game.LineManager3D.AddLine( torus.Center, torus.Center + Normal, Color.Red );

                Vector3 randomVector;
                for ( float iFloat = 0; iFloat < MathHelper.TwoPi; iFloat += MathHelper.PiOver4 * 0.2f )
                {
                    randomVector = new Vector3( (float)Math.Sin( iFloat ), (float)Math.Cos( iFloat ), 0 );
                    //game.LineManager3D.AddLine( torus.Center, randomVector, Color.Black );

                    randomVector.Normalize();

                    Vector3 torusTangent = Vector3.Cross( Normal, randomVector );
                    //game.LineManager3D.AddLine( torus.Center, torusTangent, Color.Yellow );

                    float halfRadius2 = torus.Radius2 * 0.5f;

                    game.LineManager3D.AddLine( torus.Center + torusTangent * ( torus.Radius1 - torus.Radius2 ), torus.Center + torusTangent * ( torus.Radius1 + torus.Radius2 ), Color.Black );

                    game.LineManager3D.AddLine( torus.Center + torusTangent * torus.Radius1, torus.Center + torusTangent * torus.Radius1 + Normal * torus.Radius2, Color.Black );
                    game.LineManager3D.AddLine( torus.Center + torusTangent * torus.Radius1, torus.Center + torusTangent * torus.Radius1 + Normal * -torus.Radius2, Color.Black );

                    game.LineManager3D.AddLine( torus.Center + torusTangent * -( torus.Radius1 - torus.Radius2 ), torus.Center + torusTangent * -( torus.Radius1 + torus.Radius2 ), Color.Black );

                    game.LineManager3D.AddLine( torus.Center + torusTangent * -torus.Radius1, torus.Center + torusTangent * -torus.Radius1 + Normal * torus.Radius2, Color.Black );
                    game.LineManager3D.AddLine( torus.Center + torusTangent * -torus.Radius1, torus.Center + torusTangent * -torus.Radius1 + Normal * -torus.Radius2, Color.Black );
                }
            };

            game.Run();

        }
    }
}
