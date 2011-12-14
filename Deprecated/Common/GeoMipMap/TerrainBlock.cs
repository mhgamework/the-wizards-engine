using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Common;

namespace MHGameWork.TheWizards.Common.GeoMipMap
{
    public abstract class TerrainBlock : IDisposable //: TreeNode
    {
        protected Terrain terrain;
        protected TerrainBlock[] neighbors = new TerrainBlock[ 4 ];
        protected Vector3 center;
        private int totalBaseTriangles = 0;

        public int TotalBaseTriangles
        {
            get { return totalBaseTriangles; }
            set { totalBaseTriangles = value; }
        }
        private int totalEdgeTriangles = 0;

        public int TotalEdgeTriangles
        {
            get { return totalEdgeTriangles; }
            set { totalEdgeTriangles = value; }
        }
        protected int totalEdgeOffset = 0;
        protected int detailLevel = 0;
        protected int x = -1;
        protected int z = -1;
        protected FilePointer filePointer;



        //New version

        private float[] minDistancesSquared;

        public float[] MinDistancesSquared
        {
            get { return minDistancesSquared; }
            set { minDistancesSquared = value; }
        }

        protected Wereld.QuadTreeNode quadTreeNode;
        protected BoundingBox localBoundingBox = new BoundingBox();


        public TerrainBlock( Terrain terrain, int x, int z )
        {
            this.terrain = terrain;
            this.x = x;
            this.z = z;
            minDistancesSquared = new float[ terrain.MaxDetailLevel + 1 ];
        }










        public void SetAndCalculateMinDistances( Matrix projection )
        {
            minDistancesSquared = CalculateMinDistances( projection, terrain.HeightMap );
            //for ( int i = 0; i < terrain.MaxDetailLevel + 1; i++ )
            //{
            //    float minDist = CalculateLevelMinDistance( i, projection );
            //    minDistancesSquared[ i ] = minDist * minDist;
            //}
        }

        public float[] CalculateMinDistances( Matrix projection, HeightMapOud map )
        {
            float[] localMinDistancesSquared = new float[ terrain.MaxDetailLevel + 1 ];

            for ( int i = 0; i < terrain.MaxDetailLevel + 1; i++ )
            {
                float minDist = CalculateLevelMinDistance( i, projection, map );
                localMinDistancesSquared[ i ] = minDist * minDist;
            }

            return localMinDistancesSquared;

        }

        public float CalculateLevelMinDistance( int level, Matrix projection, HeightMapOud map )
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

        public float CalculateLevelError( int level, HeightMapOud map )
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


            for ( int quadZ = 0; quadZ < terrain.BlockSize; quadZ += stepping )
            {
                for ( int quadX = 0; quadX < terrain.BlockSize; quadX += stepping )
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











        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            lock ( this )
            {

                West = null;
                East = null;
                North = null;
                South = null;
            }

            //base.Dispose( disposing );
        }






        public ushort IndexFromCoords( int x, int z )
        {
            return (ushort)( z * ( terrain.BlockSize + 1 ) + x );
        }





        public abstract byte[] ToBytes();


        protected void BuildBaseIndexes( List<ushort> indexes )
        {
            totalBaseTriangles = 0;

            int stepping = 1 << DetailLevel;
            int total = terrain.BlockSize;

            // build indexes for inner polygons
            for ( int ix = stepping; ix < total - stepping; ix += stepping )
            {
                for ( int iz = stepping; iz < total - stepping; iz += stepping )
                {
                    // triangle 1
                    indexes.Add( IndexFromCoords( ix, iz ) );
                    indexes.Add( IndexFromCoords( ix + stepping, iz ) );
                    indexes.Add( IndexFromCoords( ix, iz + stepping ) );
                    totalBaseTriangles++;

                    // triangle 2
                    indexes.Add( IndexFromCoords( ix + stepping, iz ) );
                    indexes.Add( IndexFromCoords( ix + stepping, iz + stepping ) );
                    indexes.Add( IndexFromCoords( ix, iz + stepping ) );
                    totalBaseTriangles++;
                }
            }

            totalEdgeOffset = indexes.Count;
        }

        protected void BuildEdgeIndexes( List<ushort> indexes )
        {
            int startCount = indexes.Count;

            totalEdgeTriangles = 0;
            totalEdgeTriangles += BuildEdgeIndexes( indexes, TerrainBlockEdge.West );
            totalEdgeTriangles += BuildEdgeIndexes( indexes, TerrainBlockEdge.East );
            totalEdgeTriangles += BuildEdgeIndexes( indexes, TerrainBlockEdge.North );
            totalEdgeTriangles += BuildEdgeIndexes( indexes, TerrainBlockEdge.South );
        }

        protected int BuildEdgeIndexes( List<ushort> indexes, TerrainBlockEdge edge )
        {
            TerrainBlock neighbor = neighbors[ (int)edge ];

            if ( neighbor == null || neighbor.DetailLevel <= DetailLevel )
                return BuildBasicEdgeIndexes( indexes, edge );
            else
                return BuildStitchedEdgeIndexes( indexes, edge );
        }

        protected int BuildBasicEdgeIndexes( List<ushort> indexes, TerrainBlockEdge edge )
        {
            int triangles = 0;
            int stepping = 1 << DetailLevel;
            int total = terrain.BlockSize;

            int startX = 0;
            int endX = 0;
            int startZ = 0;
            int endZ = 0;

            switch ( edge )
            {
                case TerrainBlockEdge.West:
                    startX = 0;
                    endX = startX;
                    startZ = 0;
                    endZ = total - stepping;
                    break;

                case TerrainBlockEdge.East:
                    startX = total - stepping;
                    endX = startX;
                    startZ = 0;
                    endZ = total - stepping;
                    break;

                case TerrainBlockEdge.North:
                    startX = 0;
                    endX = total - stepping;
                    startZ = 0;
                    endZ = startZ;
                    break;

                case TerrainBlockEdge.South:
                    startX = 0;
                    endX = total - stepping;
                    startZ = total - stepping;
                    endZ = startZ;
                    break;
            }

            for ( int ix = startX; ix <= endX; ix += stepping )
            {
                for ( int iz = startZ; iz <= endZ; iz += stepping )
                {
                    if ( edge == TerrainBlockEdge.West )
                    {
                        if ( iz == startZ )
                        {
                            indexes.Add( IndexFromCoords( ix, iz ) );
                            indexes.Add( IndexFromCoords( ix + stepping, iz + stepping ) );
                            indexes.Add( IndexFromCoords( ix, iz + stepping ) );
                            triangles++;

                            continue;
                        }

                        if ( iz == endZ )
                        {
                            indexes.Add( IndexFromCoords( ix, iz ) );
                            indexes.Add( IndexFromCoords( ix + stepping, iz ) );
                            indexes.Add( IndexFromCoords( ix, iz + stepping ) );
                            triangles++;

                            continue;
                        }
                    }
                    else if ( edge == TerrainBlockEdge.East )
                    {
                        if ( iz == startZ )
                        {
                            indexes.Add( IndexFromCoords( ix + stepping, iz ) );
                            indexes.Add( IndexFromCoords( ix + stepping, iz + stepping ) );
                            indexes.Add( IndexFromCoords( ix, iz + stepping ) );
                            triangles++;

                            continue;
                        }

                        if ( iz == endZ )
                        {
                            indexes.Add( IndexFromCoords( ix, iz ) );
                            indexes.Add( IndexFromCoords( ix + stepping, iz ) );
                            indexes.Add( IndexFromCoords( ix + stepping, iz + stepping ) );
                            triangles++;

                            continue;
                        }
                    }
                    else if ( edge == TerrainBlockEdge.North )
                    {
                        if ( ix == startX )
                        {
                            indexes.Add( IndexFromCoords( ix, iz ) );
                            indexes.Add( IndexFromCoords( ix + stepping, iz ) );
                            indexes.Add( IndexFromCoords( ix + stepping, iz + stepping ) );
                            triangles++;

                            continue;
                        }

                        if ( ix == endX )
                        {
                            indexes.Add( IndexFromCoords( ix, iz ) );
                            indexes.Add( IndexFromCoords( ix + stepping, iz ) );
                            indexes.Add( IndexFromCoords( ix, iz + stepping ) );
                            triangles++;

                            continue;
                        }
                    }

                    else if ( edge == TerrainBlockEdge.South )
                    {
                        if ( ix == startX )
                        {
                            indexes.Add( IndexFromCoords( ix + stepping, iz ) );
                            indexes.Add( IndexFromCoords( ix + stepping, iz + stepping ) );
                            indexes.Add( IndexFromCoords( ix, iz + stepping ) );
                            triangles++;

                            continue;
                        }

                        if ( ix == endX )
                        {
                            indexes.Add( IndexFromCoords( ix, iz ) );
                            indexes.Add( IndexFromCoords( ix + stepping, iz + stepping ) );
                            indexes.Add( IndexFromCoords( ix, iz + stepping ) );
                            triangles++;

                            continue;
                        }
                    }

                    indexes.Add( IndexFromCoords( ix, iz ) );
                    indexes.Add( IndexFromCoords( ix + stepping, iz ) );
                    indexes.Add( IndexFromCoords( ix, iz + stepping ) );
                    triangles++;

                    indexes.Add( IndexFromCoords( ix + stepping, iz ) );
                    indexes.Add( IndexFromCoords( ix + stepping, iz + stepping ) );
                    indexes.Add( IndexFromCoords( ix, iz + stepping ) );
                    triangles++;
                }
            }

            return triangles;
        }

        protected int BuildStitchedEdgeIndexes( List<ushort> indexes, TerrainBlockEdge edge )
        {
            TerrainBlock neighbor = neighbors[ (int)edge ];
            int sourceStep = 1 << DetailLevel;
            int destStep = 1 << neighbor.DetailLevel;
            //if (destStep > sourceStep) throw new Exception();
            int destHalfStep = destStep >> 1;
            int triangles = 0;
            int startPos = 0;
            int endPos = 0;
            int insidePos = 0;
            int insideStep = 0;
            bool horizontal = false;

            switch ( edge )
            {
                case TerrainBlockEdge.West:
                    startPos = terrain.BlockSize;
                    insideStep = sourceStep;
                    sourceStep = -sourceStep;
                    destStep = -destStep;
                    destHalfStep = -destHalfStep;
                    break;

                case TerrainBlockEdge.East:
                    endPos = terrain.BlockSize;
                    insidePos = terrain.BlockSize;
                    insideStep = -sourceStep;
                    break;

                case TerrainBlockEdge.North:
                    endPos = terrain.BlockSize;
                    insideStep = sourceStep;
                    horizontal = true;
                    break;

                case TerrainBlockEdge.South:
                    startPos = terrain.BlockSize;
                    insidePos = terrain.BlockSize;
                    insideStep = -sourceStep;
                    sourceStep = -sourceStep;
                    destStep = -destStep;
                    destHalfStep = -destHalfStep;
                    horizontal = true;
                    break;
            };

            for ( int pos1 = startPos; pos1 != endPos; pos1 += destStep )
            {
                for ( int pos2 = 0; pos2 != destHalfStep; pos2 += sourceStep )
                {
                    if ( pos1 != startPos || pos2 != 0 )
                    {
                        if ( horizontal )
                        {
                            indexes.Add( IndexFromCoords( pos1, insidePos ) );
                            indexes.Add( IndexFromCoords( pos1 + pos2 + sourceStep, insidePos + insideStep ) );
                            indexes.Add( IndexFromCoords( pos1 + pos2, insidePos + insideStep ) );
                            triangles++;
                        }
                        else
                        {
                            indexes.Add( IndexFromCoords( insidePos, pos1 ) );
                            indexes.Add( IndexFromCoords( insidePos + insideStep, pos1 + pos2 + sourceStep ) );
                            indexes.Add( IndexFromCoords( insidePos + insideStep, pos1 + pos2 ) );
                            triangles++;
                        }
                    }
                }

                if ( horizontal )
                {
                    indexes.Add( IndexFromCoords( pos1, insidePos ) );
                    indexes.Add( IndexFromCoords( pos1 + destStep, insidePos ) );
                    indexes.Add( IndexFromCoords( pos1 + destHalfStep, insidePos + insideStep ) );
                    triangles++;
                }
                else
                {
                    indexes.Add( IndexFromCoords( insidePos, pos1 ) );
                    indexes.Add( IndexFromCoords( insidePos, pos1 + destStep ) );
                    indexes.Add( IndexFromCoords( insidePos + insideStep, pos1 + destHalfStep ) );
                    triangles++;
                }

                for ( int pos2 = destHalfStep; pos2 != destStep; pos2 += sourceStep )
                {
                    if ( pos1 != endPos - destStep || pos2 != destStep - sourceStep )
                    {
                        if ( horizontal )
                        {
                            indexes.Add( IndexFromCoords( pos1 + destStep, insidePos ) );
                            indexes.Add( IndexFromCoords( pos1 + pos2 + sourceStep, insidePos + insideStep ) );
                            indexes.Add( IndexFromCoords( pos1 + pos2, insidePos + insideStep ) );
                            triangles++;
                        }
                        else
                        {
                            indexes.Add( IndexFromCoords( insidePos, pos1 + destStep ) );
                            indexes.Add( IndexFromCoords( insidePos + insideStep, pos1 + pos2 + sourceStep ) );
                            indexes.Add( IndexFromCoords( insidePos + insideStep, pos1 + pos2 ) );
                            triangles++;
                        }
                    }
                }
            }

            return triangles;
        }


        public virtual void OnBoundingChanged()
        {
        }



        public Vector3 Center
        {
            get { return center; }
            set { center = value; }
        }

        public int X
        {
            get { return x; }
        }

        public int Z
        {
            get { return z; }
        }

        public int DetailLevel
        {
            get { return detailLevel; }
        }

        public TerrainBlock West
        {
            get { return neighbors[ (int)TerrainBlockEdge.West ]; }
            set { neighbors[ (int)TerrainBlockEdge.West ] = value; }
        }

        public TerrainBlock East
        {
            get { return neighbors[ (int)TerrainBlockEdge.East ]; }
            set { neighbors[ (int)TerrainBlockEdge.East ] = value; }
        }

        public TerrainBlock North
        {
            get { return neighbors[ (int)TerrainBlockEdge.North ]; }
            set { neighbors[ (int)TerrainBlockEdge.North ] = value; }
        }

        public TerrainBlock South
        {
            get { return neighbors[ (int)TerrainBlockEdge.South ]; }
            set { neighbors[ (int)TerrainBlockEdge.South ] = value; }
        }

        public FilePointer FilePointer
        { get { return filePointer; } set { filePointer = value; } }

        public Terrain Terrain
        {
            get { return terrain; }

        }


        public Wereld.QuadTreeNode QuadTreeNode
        { get { return quadTreeNode; } set { quadTreeNode = value; } }

        public BoundingBox LocalBoundingBox
        {
            get { return localBoundingBox; }
            set
            {
                localBoundingBox = value;
                if ( localBoundingBox.Min.Y == localBoundingBox.Max.Y )
                {
                    //To mimic the very thin but existing terrain
                    localBoundingBox.Max += new Vector3( 0, 0.0001f, 0 );
                }
                OnBoundingChanged();
            }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                BoundingBox bb;

                bb = new BoundingBox( terrain.GetWorldPosition( localBoundingBox.Min ),
                                       terrain.GetWorldPosition( localBoundingBox.Max ) );

                return bb;
            }

        }


    }
}

