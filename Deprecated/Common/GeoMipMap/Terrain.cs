using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MHGameWork.TheWizards.Common;

namespace MHGameWork.TheWizards.Common.GeoMipMap
{
    public abstract class Terrain : IDisposable // : DrawableGameComponent
    {
        private float scale = 1f;
        private float heightScale = 1f;//1024f / 8;
        protected int visibleTriangles = 0;
        public float centerX; //TODO : instelle
        public float centerZ;//TODO : instelle



        //New version

        protected string filenameOud;
        protected string heightMapFilename;
        protected string lightMapFilename;
        protected string weightMapFilename;

        protected int sizeX;
        protected int sizeY;

        //private QuadTreeNode trunk;
        protected Wereld.QuadTreeNode quadTreeNode;
        protected TerrainBlock[][] blocks;
        protected int blockSize;
        private int numBlocksX;


        private int numBlocksY;



        protected HeightMapOud heightMap;
        protected LightMap lightMap;
        protected WeightMap weightMap;

        protected Matrix worldMatrix;

        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }
        public Vector3 localCameraPosition;



        public Terrain()
            : base()
        {

            worldMatrix = Matrix.Identity;
        }

        ~Terrain()
        {
            Dispose( false );
        }

        public void Dispose()
        {
            Dispose( true );
            System.GC.SuppressFinalize( this );
        }
        protected virtual void Dispose( bool disposing )
        {
            // lock (this) ???

            if ( disposing )
            {
                DisposeTerrain();


                if ( heightMap != null )
                    heightMap.Dispose();
                heightMap = null;




            }


        }

        protected void DisposeTerrain()
        {
            DisposeBlocks();
        }

        protected void DisposeBlocks()
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
        public void SetQuadTreeNode( Wereld.QuadTreeNode node )
        {
            quadTreeNode = node;
            Vector3 terrainTopLeft = new Vector3(
                        node.BoundingBox.Min.X,
                        0,
                        node.BoundingBox.Min.Z );

            worldMatrix = Matrix.CreateTranslation( terrainTopLeft );
        }
        //Terrain Creation
        //  Here the blocks are built, and data for the terrain is created

        /// <summary>
        /// Filename van dit terrein, zonder extensie. Hiermee worden verschillende files voor
        /// heightmap, lightmap, weightmap, blockdata, ... aangemaakt.
        /// </summary>
        public void SetFilename( string nFilename )
        {
            //filename = engine.XNAGame._content.RootDirectory + "\\" + nFilename;
            filenameOud = nFilename;
            heightMapFilename = filenameOud + "HeightMap.twf";
            lightMapFilename = filenameOud + "LightMap.twf";
            weightMapFilename = filenameOud + "WeightMap.twf";
        }

        /// <summary>
        /// DEPRECATED. Creates a new, empty terrain.
        /// 
        /// </summary>
        /// <param name="nBlockSize"></param>
        /// <param name="nNumBlocksX"></param>
        /// <param name="nNumBlocksY"></param>
        public void CreateTerrain( int nBlockSize, int nNumBlocksX, int nNumBlocksY )
        {
            DisposeTerrain();



            BuildBlocks( nBlockSize, nNumBlocksX, nNumBlocksY );

            heightMap = new HeightMapOud( sizeX, sizeY );
            lightMap = new LightMap( sizeX, sizeY );
            weightMap = new WeightMap( sizeX, sizeY );

            for ( int ix = 0; ix < sizeX; ix++ )
            {
                for ( int iy = 0; iy < sizeY; iy++ )
                {
                    lightMap.SetSample( ix, iy, 255 );
                }
            }

            SaveMaps();

            //lightMapTexture = new Texture2D( device, sizeX, sizeY, 1, ResourceUsage.None, SurfaceFormat.Luminance8 );
            //weightMapTexture = new Texture2D( device, sizeX, sizeY, 1, ResourceUsage.None, SurfaceFormat.Color );

            //byte[] data = new byte[ sizeX * SizeY ];


            //lightMapTexture.SetData<byte>( data );
            //( 0, new Rectangle( x, z, terrain.BlockSize + 1, terrain.BlockSize + 1 ), data, 0, data.Length, SetDataOptions.None );


        }

        public void SaveMaps()
        {
            heightMap.Save( heightMapFilename );
            lightMap.Save( lightMapFilename );
            weightMap.Save( weightMapFilename );
        }


        /// <summary>
        /// Creates the terrain blocks and binds them to the corresponding quadtreenode
        /// </summary>
        public void BuildBlocks( int nBlockSize, int nNumBlocksX, int nNumBlocksY )
        {

            blockSize = nBlockSize;
            numBlocksX = nNumBlocksX;
            numBlocksY = nNumBlocksY;
            sizeX = nBlockSize * nNumBlocksX;
            sizeY = nBlockSize * nNumBlocksY;
            //centerX = sizeX / 2;
            //centerZ = sizeY / 2;
            centerX = 0;
            centerZ = 0;

            /*// get number of blocks
            numBlocksX = size / blockSize;
            numBlocksY = size / blockSize;*/

            blocks = new TerrainBlock[ numBlocksX ][];

            // create blocks
            for ( int x = 0; x < numBlocksX; x++ )
            {
                blocks[ x ] = new TerrainBlock[ numBlocksY ];

                for ( int z = 0; z < numBlocksY; z++ )
                {
                    TerrainBlock patch = CreateBlockOld( x * blockSize, z * blockSize );

                    blocks[ x ][ z ] = patch;

                    if ( z > 0 )
                    {
                        patch.North = blocks[ x ][ z - 1 ];
                        blocks[ x ][ z - 1 ].South = patch;
                    }

                    if ( x > 0 )
                    {
                        patch.West = blocks[ x - 1 ][ z ];
                        blocks[ x - 1 ][ z ].East = patch;
                    }
                }
            }

            AssignBlocksToQuadtree( 0, 0, numBlocksX, numBlocksY, quadTreeNode );

            //trunk = BuildTreeNode( 0, 0, numBlocksX, numBlocksY ) as QuadTreeNode;
        }

        protected abstract TerrainBlock CreateBlockOld( int x, int z );


        public void AssignBlocksToQuadtree( int x, int z, int width, int length, Wereld.QuadTreeNode node )
        {
            if ( width == 0 || length == 0 )
                return;

            if ( width == 1 && length == 1 )
            {
                AssignBlockToNode( blocks[ x ][ z ], node );
                return;
            }


            int left = (int)Math.Round( width * 0.5f );
            int right = width - left;
            int top = (int)Math.Round( length * 0.5f );
            int bottom = length - top;



            node.Split();

            AssignBlocksToQuadtree( x, z, left, top, node.UpperLeft );
            AssignBlocksToQuadtree( x + left, z, right, top, node.UpperRight );
            AssignBlocksToQuadtree( x, z + top, left, bottom, node.LowerLeft );
            AssignBlocksToQuadtree( x + left, z + top, right, bottom, node.LowerRight );


        }

        public abstract void AssignBlockToNode( TerrainBlock block, Wereld.QuadTreeNode node );


        public void RaiseTerrain( Vector2 center, float range, float strength )
        {
            Vector3 tempcenter = new Vector3( center.X, 0, center.Y );
            tempcenter = Vector3.Transform( tempcenter, Matrix.Invert( worldMatrix ) );
            center = new Vector2( tempcenter.X, tempcenter.Z );
            //RaiseTerrain( center, range, strength, trunk );
            RaiseTerrain( center, range, strength, quadTreeNode );
        }

        public void RaiseTerrain( Vector2 center, float range, float strength, Wereld.QuadTreeNode node )
        {

            //Vector3 min = node.BoundingBox.Min;
            //Vector3 max = node.BoundingBox.Max;
            //min.Y = 0;
            //max.Y = 0;

            //BoundingBox b = new BoundingBox( min, max );
            //BoundingSphere brush = new BoundingSphere( new Vector3( center.X, 0, center.Y ), range );

            //ContainmentType result;

            //b.Contains( ref brush, out result );
            //if ( result == ContainmentType.Disjoint ) return;


            //Type type = node.GetType();

            //if ( type == typeof( QuadTreeNode ) )
            //{
            //    QuadTreeNode tree = (QuadTreeNode)node;

            //    if ( tree.UpperLeft != null )
            //        RaiseTerrain( center, range, strength, tree.UpperLeft );

            //    if ( tree.UpperRight != null )
            //        RaiseTerrain( center, range, strength, tree.UpperRight );

            //    if ( tree.LowerLeft != null )
            //        RaiseTerrain( center, range, strength, tree.LowerLeft );

            //    if ( tree.LowerRight != null )
            //        RaiseTerrain( center, range, strength, tree.LowerRight );
            //}
            //else if ( type == typeof( TerrainBlock ) )
            //{
            //    TerrainBlock block = (TerrainBlock)node;

            //    //some vertices are in more then one block. Therefore influences the following vertices:
            //    //
            //    // ----...----
            //    // -VVV...VVVV
            //    // -VVV...VVVV
            //    // ...........
            //    // -VVV...VVVV
            //    // -VVV...VVVV
            //    //
            //    //the top blocks also have the top row
            //    //the left blocks have the left row
            //    //the top left block has the topleft vertex


            //    int startX = block.X;
            //    int startY = block.Z;

            //    if ( block.X != 0 ) startX += 1;
            //    if ( block.Z != 0 ) startY += 1;



            //    for ( int ix = startX; ix < block.X + blockSize + 1; ix++ )
            //    {
            //        for ( int iy = startY; iy < block.Z + blockSize + 1; iy++ )
            //        {
            //            float height = heightMap.GetHeight( ix, iy );

            //            float dist = Vector3.Distance( new Vector3( center.X, 0, center.Y ), GetLocalPosition( new Vector3( ix, 0, iy ) ) );

            //            if ( dist <= range )
            //            {
            //                float factor = 1 - dist / range;
            //                heightMap.SetHeight( ix, iy, height + factor * strength );
            //            }
            //        }
            //    }

            //    block.SetVertexBufferDirty();


            //}
        }













        public Terrain( string nFilename )
            : base()
        {
            filenameOud = nFilename;
            blockSize = 16;
        }

        public Terrain( int nSize )
            : base()
        {
            sizeX = nSize;
            sizeY = nSize;
            blockSize = 16;
        }






        public Vector3 GetAveragedNormal( int x, int z )
        {
            Vector3 normal = new Vector3();

            // top left
            if ( x > 0 && z > 0 )
                normal += GetNormal( x - 1, z - 1 );

            // top center
            if ( z > 0 )
                normal += GetNormal( x, z - 1 );

            // top right
            if ( x < heightMap.Width && z > 0 )
                normal += GetNormal( x + 1, z - 1 );

            // middle left
            if ( x > 0 )
                normal += GetNormal( x - 1, z );

            // middle center
            normal += GetNormal( x, z );

            // middle right
            if ( x < heightMap.Width )
                normal += GetNormal( x + 1, z );

            // lower left
            if ( x > 0 && z < heightMap.Length )
                normal += GetNormal( x - 1, z + 1 );

            // lower center
            if ( z < heightMap.Length )
                normal += GetNormal( x, z + 1 );

            // lower right
            if ( x < heightMap.Width && z < heightMap.Length )
                normal += GetNormal( x + 1, z + 1 );

            return Vector3.Normalize( normal );
        }

        public Vector3 GetNormal( int x, int z )
        {
            Vector3 v1 = new Vector3( x * scale, heightMap[ x, z + 1 ] * heightScale, ( z + 1 ) * scale );
            Vector3 v2 = new Vector3( x * scale, heightMap[ x, z - 1 ] * heightScale, ( z - 1 ) * scale );
            Vector3 v3 = new Vector3( ( x + 1 ) * scale, heightMap[ x + 1, z ] * heightScale, z * scale );
            Vector3 v4 = new Vector3( ( x - 1 ) * scale, heightMap[ x - 1, z ] * heightScale, z * scale );

            return Vector3.Normalize( Vector3.Cross( v1 - v2, v3 - v4 ) );
        }

        public Vector3 GetLocalPosition( Vector3 vertexPosition )
        {
            return new Vector3( ( vertexPosition.X - centerX ) * Scale,
                                        vertexPosition.Y * HeightScale,
                                ( vertexPosition.Z - centerZ ) * Scale );
        }

        public Vector3 GetWorldPosition( Vector3 localPosition )
        {
            return Vector3.Transform( localPosition, worldMatrix );
        }


        /// <summary>
        /// DEPRECATED!!! THERE SHOULD BE NO HEIGHTMAP IN THIS CLASS
        /// </summary>
        public HeightMapOud HeightMap
        {
            get { return heightMap; }
            set
            {
                heightMap = value;
            }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public float HeightScale
        {
            get { return heightScale; }
            set { heightScale = value; }
        }

        public Wereld.QuadTreeNode QuadTreeNode
        {
            get { return quadTreeNode; }
        }
        public int Width
        {
            get
            {
                if ( heightMap != null )
                    return heightMap.Width;

                return 0;
            }
        }

        public int Length
        {
            get
            {
                if ( heightMap != null )
                    return heightMap.Length;

                return 0;
            }
        }

        public int BlockSize
        {
            get { return blockSize; }
        }
        public int SizeX
        {
            get { return sizeX; }
        }
        public int SizeY
        {
            get { return sizeY; }
        }

        public int MaxDetailLevel
        {
            //get { return ( BlockSize >> 2 ) - 1; }
            get { return 4; }
        }

        public int VisibleTriangles
        {
            get { return visibleTriangles; }
        }

        public string Filename
        { get { return filenameOud; } set { filenameOud = value; } }
        public int NumBlocksY
        {
            get { return numBlocksY; }
            //set { numBlocksY = value; }
        }
        public int NumBlocksX
        {
            get { return numBlocksX; }
            //set { numBlocksX = value; }
        }

    }
}

