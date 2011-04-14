using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.TWClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NovodexWrapper;
using MHGameWork.TheWizards.Common.GeoMipMap;
using System.IO;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.ServerClient.TWXNAEngine;

namespace MHGameWork.TheWizards.ServerClient.Terrain.Preprocesser
{
    public class PreprocessedTerrainBlock
    {

        public VertexMultitextured[] Vertices;

        public Vector3 Min;
        public Vector3 Max;


        public float[] LocalMinDistancesSquared;



        public int MaterialIndex;


        public TerrainFullData TerrainFullData;

        public int BlockNumX;
        public int BlockNumZ;

        public PreprocessedTerrainBlock( TerrainFullData fullData, int _blockNumX, int _blockNumZ )
        {
            TerrainFullData = fullData;
            BlockNumX = _blockNumX;
            BlockNumZ = _blockNumZ;
        }






        public void CalculatePreProcessedData( Matrix projectionMatrix, int _materialIndex )
        {

            GenerateVerticesFromHeightmapSpecialWeightmap( TerrainFullData );



            LocalMinDistancesSquared = CalculateMinDistances( projectionMatrix, TerrainFullData.HeightMap );

            MaterialIndex = _materialIndex;

        }





        /// <summary>
        /// Generates vertices with the texturecoordinates set for the special weightmapformat, where every block has
        /// blocksize+1 texels (so no texels are shared between blocks) (the weightmap is thus larger than the heightmap)
        /// </summary>
        public void GenerateVerticesFromHeightmapSpecialWeightmap( TerrainFullData fullData )
        {
            VertexMultitextured[] vertexes = new VertexMultitextured[ ( fullData.BlockSize + 1 ) * ( fullData.BlockSize + 1 ) ];

            Vector3 min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
            Vector3 max = new Vector3( float.MinValue, float.MinValue, float.MinValue );

            int x = BlockNumX * fullData.BlockSize;
            int z = BlockNumZ * fullData.BlockSize;


            // build vectors and normals
            for ( int ix = 0; ix <= fullData.BlockSize; ix++ )
            {
                for ( int iz = 0; iz <= fullData.BlockSize; iz++ )
                {
                    int cx = x + ix;
                    int cz = z + iz;
                    //cx = ix;
                    //cz =  iz;

                    VertexMultitextured vert = new VertexMultitextured();

                    vert.Position = new Vector3( cx, fullData.HeightMap[ cx, cz ], cz );

                    // Weightmap texcoord: every vertex is positioned in the center of a texel containing the weights for that texture
                    vert.TextureCoordinate = new Vector2(
                        (float)( BlockNumX * ( fullData.BlockSize + 1 ) + ix + 0.5f ) / ( (float)fullData.NumBlocksX * ( fullData.BlockSize + 1 ) ),
                        (float)( BlockNumZ * ( fullData.BlockSize + 1 ) + iz + 0.5f ) / ( (float)fullData.NumBlocksZ * ( fullData.BlockSize + 1 ) ) );

                    vert.Normal = CalculateAveragedNormal( fullData.HeightMap, cx, cz );

                    min = Vector3.Min( min, vert.Position );
                    max = Vector3.Max( max, vert.Position );

                    vertexes[ IndexFromCoords( ix, iz ) ] = vert;
                }

            }


            this.Vertices = vertexes;
            // In terrain local space
            this.Min = min;
            this.Max = max;


        }


        public static Vector3 CalculateAveragedNormal( TerrainHeightMap map, int x, int z )
        {
            Vector3 normal = new Vector3();

            // top left
            if ( x > 0 && z > 0 )
                normal += CalculateNormal( map, x - 1, z - 1 );

            // top center
            if ( z > 0 )
                normal += CalculateNormal( map, x, z - 1 );

            // top right
            if ( x < map.Width && z > 0 )
                normal += CalculateNormal( map, x + 1, z - 1 );

            // middle left
            if ( x > 0 )
                normal += CalculateNormal( map, x - 1, z );

            // middle center
            normal += CalculateNormal( map, x, z );

            // middle right
            if ( x < map.Width )
                normal += CalculateNormal( map, x + 1, z );

            // lower left
            if ( x > 0 && z < map.Length )
                normal += CalculateNormal( map, x - 1, z + 1 );

            // lower center
            if ( z < map.Length )
                normal += CalculateNormal( map, x, z + 1 );

            // lower right
            if ( x < map.Width && z < map.Length )
                normal += CalculateNormal( map, x + 1, z + 1 );

            return Vector3.Normalize( normal );
        }

        public static Vector3 CalculateNormal( TerrainHeightMap map, int x, int z )
        {
            float scale = 1;
            float heightScale = 1;
            Vector3 v1 = new Vector3( x * scale, map[ x, z + 1 ] * heightScale, ( z + 1 ) * scale );
            Vector3 v2 = new Vector3( x * scale, map[ x, z - 1 ] * heightScale, ( z - 1 ) * scale );
            Vector3 v3 = new Vector3( ( x + 1 ) * scale, map[ x + 1, z ] * heightScale, z * scale );
            Vector3 v4 = new Vector3( ( x - 1 ) * scale, map[ x - 1, z ] * heightScale, z * scale );

            return Vector3.Normalize( Vector3.Cross( v1 - v2, v3 - v4 ) );
        }


        public int CalculateMaxDetailLevel()
        {

            //int maxlevel = (int)( Math.Log( TerrainFullData.BlockSize ) / Math.Log( 2 ) );

            return ( TerrainFullData.BlockSize >> 2 ) - 1;
        }

        public float[] CalculateMinDistances( Matrix projection, TerrainHeightMap map )
        {
            int maxlevel = CalculateMaxDetailLevel();
            float[] localMinDistancesSquared = new float[ maxlevel + 1 ];

            for ( int i = 0; i < maxlevel + 1; i++ )
            {
                float minDist = CalculateLevelMinDistance( i, projection, map );
                localMinDistancesSquared[ i ] = minDist * minDist;
            }

            return localMinDistancesSquared;

        }

        public float CalculateLevelMinDistance( int level, Matrix projection, TerrainHeightMap map )
        {
            float error = CalculateLevelError( level, map );

            //Willem de Boer:
            // Dn = error * C
            // C = A / T
            // A = n / |t|
            // T = ( 2 * threshold ) / verticalResolution



            //Relfection on class Matrix:
            // matrix.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            // matrix.M43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
            // ==>    M43 / M33 = nearPlaneDistance
            //
            // matrix.M22 = 1f / ((float) Math.Tan((double) (fieldOfView * 0.5f)));
            // matrix.M22 = (2f * nearPlaneDistance) / height;
            // (http://www.avl.iu.edu/~ewernert/b581/lectures/12.2/index.html):
            // top = near * tan(PI/180 * viewAngle/2) 
            // top = near * 1f / matrix.M22
            // top = near / M22








            float threshold = 10;
            float n = projection.M43 / projection.M33;
            float t = n / projection.M22;  // 768f / 2f;
            float verticalResolution = 768f;


            //lijkt hetzelfde te zijn als m11
            float A = n / Math.Abs( t );

            float T = ( 2 * threshold ) / verticalResolution;

            float C = A / T;

            float Dn = error * C;



            /*float threshold = 6;


            float Dn = 0;

            Vector3 vProj1 = Vector3.Zero;
            Vector3 vProj2 = new Vector3( 0, threshold, 0 );
            Matrix inverseProj = Matrix.Invert( projection );

            Vector3 v1 = Vector3.Transform( vProj1, inverseProj );
            Vector3 v2 = Vector3.Transform( vProj2, inverseProj );

            float dist = Vector3.Distance( v1, v2 );*/







            return Dn;


        }

        public float CalculateLevelError( int level, TerrainHeightMap map )
        {
            int stepping = 1 << level;

            float maxError = 0;

            //We go through all the quads in the selected level en interpolate a height value for every vertex that is left out


            int cx;
            int cz;
            float tl; //top left
            float tr; //top right
            float bl; //bottom left
            float br; //bottom right
            float e; //error

            int x = BlockNumX * TerrainFullData.BlockSize;
            int z = BlockNumZ * TerrainFullData.BlockSize;


            for ( int quadZ = 0; quadZ < TerrainFullData.BlockSize; quadZ += stepping )
            {
                for ( int quadX = 0; quadX < TerrainFullData.BlockSize; quadX += stepping )
                {

                    cx = x + quadX;
                    cz = z + quadZ;
                    tl = map.GetHeight( cx, cz );
                    tr = map.GetHeight( cx + stepping, cz );
                    bl = map.GetHeight( cx, cz + stepping );
                    br = map.GetHeight( cx + stepping, cz + stepping );


                    for ( int iz = 0; iz <= stepping; iz++ )
                    {
                        for ( int ix = 0; ix <= stepping; ix++ )
                        {
                            //We could skip the corners but the error is 0 on those points anyway
                            float lerpX = MathHelper.Lerp( tl, tr, (float)ix / (float)stepping );
                            float lerpZ = MathHelper.Lerp( bl, br, (float)iz / (float)stepping );
                            float lerp = ( lerpX + lerpZ ) * 0.5f;

                            e = Math.Abs( map.GetHeight( cx + ix, cz + iz ) - lerp );
                            maxError = MathHelper.Max( maxError, e );


                        }
                    }

                }
            }

            return maxError;

        }












        public ushort IndexFromCoords( int x, int z )
        {
            return (ushort)( z * ( TerrainFullData.BlockSize + 1 ) + x );
        }







    }
}

