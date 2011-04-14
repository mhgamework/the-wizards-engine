using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Terrain;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class EditorTerrain
    {
        private TerrainFullData fullData;

        public TerrainFullData FullData
        {
            get { return fullData; }
            set { fullData = value; }
        }

        public EditorTerrain( TerrainManagerService managerService, TaggedTerrain terr )
        {
            fullData = terr.GetFullData();
        }

        public float? Raycast( Ray ray, out Vector3 vertex1, out Vector3 vertex2, out Vector3 vertex3 )
        {
            vertex1 = vertex2 = vertex3 = Vector3.Zero;

            // Transform ray to local space
            ray.Position -= fullData.Position;

            // For wild guessing

            BoundingBox bb = new BoundingBox( new Vector3( 0, -40000, 0 ), new Vector3( fullData.SizeX, 40000, fullData.SizeZ ) );
            if ( ray.Intersects( bb ).HasValue == false ) return null;

            // Taken from Beginning XNA 2.0 Game Programming From Novice to Professional on page 296

            // Do a linear search

            // A good ray step is half of the blockScale
            float stepLength = 8f;
            Vector3 rayStep = ray.Direction * stepLength;// * blockScale * 0.5f;
            Vector3 rayStartPosition = ray.Position;
            // Linear search - Loop until find a point inside and outside the terrain
            Vector3 lastRayPosition;
            float height;
            /*= ray.Position;
            ray.Position += rayStep;
            float height = GetHeight( ray.Position );*/


            do
            {
                if ( ray.Position.X < 0 || ray.Position.Z < 0
                    || ray.Position.X > fullData.SizeX
                    || ray.Position.Z > fullData.SizeZ )
                {
                    if ( ray.Intersects( bb ).HasValue == false ) return null; // Check if the ray is still moving towards the terrain

                }


                lastRayPosition = ray.Position;
                ray.Position += rayStep;

                height = fullData.HeightMap.CalculateHeight( ray.Position.X, ray.Position.Z );



            }
            //while ( ray.Position.Y > height && height >= 0 ); // Detect if under the terrain, this will mean we had to intersect it somewhere 
            while ( ray.Position.Y > height ); // Detect if under the terrain, this will mean we had to intersect it somewhere 

            if ( ray.Position.X < 0 || ray.Position.Z < 0
                   || ray.Position.X > fullData.SizeX
                   || ray.Position.Z > fullData.SizeZ )
            {
                // We are below the borders of the terrain, and outside it, so no intersection
                return null;

            }
            // Perform binary search

            Vector3 startPosition = lastRayPosition;
            Vector3 endPosition = ray.Position;
            // Binary search with 32 steps. Try to find the exact collision point
            for ( int i = 0; i < 32; i++ )
            {
                // Binary search pass
                Vector3 middlePoint = ( startPosition + endPosition ) * 0.5f;


                //MHGW: original:
                //if ( middlePoint.Y < height ) endPosition = middlePoint;
                //else startPosition = middlePoint;

                height = fullData.HeightMap.CalculateHeight( middlePoint.X, middlePoint.Z );

                if ( middlePoint.Y < height ) endPosition = middlePoint;
                else startPosition = middlePoint;
            }

            Vector3 collisionPoint = ( startPosition + endPosition ) * 0.5f;

            return Vector3.Distance( rayStartPosition, collisionPoint );
















            /*//TODO: do a real raycast
            Plane groundPlane = new Plane( Vector3.Up, 0 );

            float? dist = ray.Intersects( groundPlane );

            if ( !dist.HasValue ) return null;

            Vector3 pos = ray.Position + ray.Direction * dist.Value;

            if ( pos.X < fullData.Position.X || pos.Z < fullData.Position.Z
                || pos.X > fullData.Position.X + fullData.SizeX || pos.Z > fullData.Position.Z + fullData.SizeZ )
                return null;

            return dist.Value;*/


        }
    }
}
