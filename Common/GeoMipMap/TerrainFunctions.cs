using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Common.GeoMipMap
{
    public static class TerrainFunctions
    {
        /// <summary>
        /// Creates an array of blocks for the given terrain.
        /// </summary>
        public static ITerrainBlock[][] BuildBlocks( ITerrain terrain, int blockSize, int numBlocksX, int numBlocksY )
        {
            ITerrainBlock[][] blocks;

            blocks = new ITerrainBlock[ numBlocksX ][];

            // create blocks
            for ( int x = 0; x < numBlocksX; x++ )
            {
                blocks[ x ] = new ITerrainBlock[ numBlocksY ];

                for ( int z = 0; z < numBlocksY; z++ )
                {
                    ITerrainBlock patch = terrain.CreateBlock( x * blockSize, z * blockSize );

                    blocks[ x ][ z ] = patch;

                    if ( z > 0 )
                    {
                        patch.SetNeighbour( TerrainBlockEdge.North, blocks[ x ][ z - 1 ] );
                        blocks[ x ][ z - 1 ].SetNeighbour( TerrainBlockEdge.South, patch );
                    }

                    if ( x > 0 )
                    {
                        patch.SetNeighbour( TerrainBlockEdge.West, blocks[ x - 1 ][ z ] );
                        blocks[ x - 1 ][ z ].SetNeighbour( TerrainBlockEdge.East, patch );
                    }
                }
            }

            return blocks;
        }

        //protected abstract TerrainBlock CreateBlock( int x, int z );

        public static void AssignBlocksToQuadtree( ITerrainBlock[][] blocks, Wereld.IQuadtreeNode node )
        {
            AssignBlocksToQuadtreeRecur( blocks, 0, 0, blocks.Length, blocks[ 0 ].Length, node );
        }


        private static void AssignBlocksToQuadtreeRecur( ITerrainBlock[][] blocks, int x, int z, int width, int length, Wereld.IQuadtreeNode node )
        {
            if ( width == 0 || length == 0 )
                return;

            if ( width == 1 && length == 1 )
            {
                blocks[ x ][ z ].AssignToQuadtreeNode( node );
                return;
            }


            int left = (int)Math.Round( width * 0.5f );
            int right = width - left;
            int top = (int)Math.Round( length * 0.5f );
            int bottom = length - top;



            node.Split();

            AssignBlocksToQuadtreeRecur( blocks, x, z, left, top, node.GetIChild( Wereld.QuadtreeChildDirection.TopLeft ) );
            AssignBlocksToQuadtreeRecur( blocks, x + left, z, right, top, node.GetIChild( Wereld.QuadtreeChildDirection.TopRight ) );
            AssignBlocksToQuadtreeRecur( blocks, x, z + top, left, bottom, node.GetIChild( Wereld.QuadtreeChildDirection.BottomLeft ) );
            AssignBlocksToQuadtreeRecur( blocks, x + left, z + top, right, bottom, node.GetIChild( Wereld.QuadtreeChildDirection.BottomRight ) );


        }

        public static void DisposeBlocks( ITerrainBlock[][] blocks )
        {

            if ( blocks != null )
            {
                for ( int x = 0; x < blocks.Length; x++ )
                {
                    if ( blocks[ x ] == null )
                        continue;

                    for ( int z = 0; z < blocks[ x ].Length; z++ )
                    {
                        if ( blocks[ x ][ z ] == null )
                            continue;

                        blocks[ x ][ z ].Dispose();
                        blocks[ x ][ z ] = null;
                    }

                    blocks[ x ] = null;
                }
            }


            /*if ( trunk != null )
                trunk.Dispose();*/



            blocks = null;
            //trunk = null;
        }


        public static Vector3 GetAveragedNormal( HeightMapOud heightMap, int x, int z )
        {
            Vector3 normal = new Vector3();

            // top left
            if ( x > 0 && z > 0 )
                normal += GetNormal( heightMap, x - 1, z - 1 );

            // top center
            if ( z > 0 )
                normal += GetNormal( heightMap, x, z - 1 );

            // top right
            if ( x < heightMap.Width && z > 0 )
                normal += GetNormal( heightMap, x + 1, z - 1 );

            // middle left
            if ( x > 0 )
                normal += GetNormal( heightMap, x - 1, z );

            // middle center
            normal += GetNormal( heightMap, x, z );

            // middle right
            if ( x < heightMap.Width )
                normal += GetNormal( heightMap, x + 1, z );

            // lower left
            if ( x > 0 && z < heightMap.Length )
                normal += GetNormal( heightMap, x - 1, z + 1 );

            // lower center
            if ( z < heightMap.Length )
                normal += GetNormal( heightMap, x, z + 1 );

            // lower right
            if ( x < heightMap.Width && z < heightMap.Length )
                normal += GetNormal( heightMap, x + 1, z + 1 );

            return Vector3.Normalize( normal );
        }


        public static Vector3 GetNormal( HeightMapOud heightMap, int x, int z )
        {
            Vector3 v1 = new Vector3( x, heightMap[ x, z + 1 ], ( z + 1 ) );
            Vector3 v2 = new Vector3( x, heightMap[ x, z - 1 ], ( z - 1 ) );
            Vector3 v3 = new Vector3( ( x + 1 ), heightMap[ x + 1, z ], z );
            Vector3 v4 = new Vector3( ( x - 1 ), heightMap[ x - 1, z ], z );

            return Vector3.Normalize( Vector3.Cross( v1 - v2, v3 - v4 ) );
        }

    }
}
