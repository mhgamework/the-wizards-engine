using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NovodexWrapper;
using MHGameWork.TheWizards.Common.GeoMipMap;
using System.IO;
using MHGameWork.TheWizards.Common;

namespace MHGameWork.TheWizards.ServerClient.XNAGeoMipMap
{
    public class TerrainBlock : Common.GeoMipMap.TerrainBlock, ITerrainBlockRenderable  //: TreeNode
    {
        private VertexBuffer vertexBuffer;

        public VertexBuffer VertexBuffer
        {
            get { return vertexBuffer; }
            set { vertexBuffer = value; }
        }
        protected IndexBuffer indexBuffer;

        public IndexBuffer IndexBuffer
        {
            get { return indexBuffer; }
            set { indexBuffer = value; }
        }
        //private Vector3 center;
        private int totalVertexes = 0;

        public int TotalVertexes
        {
            get { return totalVertexes; }
            set { totalVertexes = value; }
        }
        protected NxActor actor;

        //private TerrainMaterial material;

        //public TerrainMaterial Material
        //{
        //    get { return material; }
        //    set { material = value; }
        //}

        private int blockNumX;
        private int blockNumZ;

        /// <summary>
        /// The x-coordinate of this block, but in block numbers.
        /// So BlockNumX actually equals this.x / BlockSize
        /// </summary>
        public int BlockNumX
        {
            get { return blockNumX; }
        }
        /// <summary>
        /// The z-coordinate of this block, but in block numbers.
        /// So BlockNumX actually equals this.z / BlockSize
        /// </summary>
        public int BlockNumZ
        {
            get { return blockNumZ; }
        }

        private bool tempNormalLoaded;

        private int version;

        public int Version
        {
            get { return version; }
            set { version = value; }
        }


        public TerrainBlock( Terrain terrain, int x, int z )
            : base( terrain, x, z )
        {
            blockNumX = x / terrain.BlockSize;
            blockNumZ = z / terrain.BlockSize;
        }




        public void AddLoadTasks( Engine.LoadingManager loadingManager, Engine.LoadingTaskType taskType )
        {
            //if ( taskPreProcess == null )
            //{
            //    taskPreProcess = new Engine.LoadingTaskAdvanced( PreProcessTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.PreProccesing );
            //    taskNormal = new Engine.LoadingTaskAdvanced( LoadNormalTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal );
            //}

            switch ( taskType )
            {
                case MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.PreProccesing:
                    break;
                case MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal:
                    if ( tempNormalLoaded == false )
                    {
                        tempNormalLoaded = true;
                        Terrain.Engine.LoadingManager.AddLoadTaskAdvanced( LoadNormalTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal );
                    }
                    break;
                case MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Detail:
                    break;
                case MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Unload:
                    if ( tempNormalLoaded == true )
                    {
                        tempNormalLoaded = false;
                        Terrain.Engine.LoadingManager.AddLoadTaskAdvanced( UnloadTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Unload );
                    }
                    break;
                default:
                    break;
            }
        }

        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState LoadNormalTask( Engine.LoadingTaskType taskType )
        {
            if ( Terrain.Device == null )
                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Idle;

            string filename = System.Windows.Forms.Application.StartupPath + @"\SavedData\Terrains\TerrainPreProcessed" + Terrain.ID.ToString( "000" ) + ".txt";

            if ( System.IO.File.Exists( filename ) )
            {
                FileStream fs = System.IO.File.Open( filename, FileMode.Open, FileAccess.Read, FileShare.None );
                ByteReader br = new ByteReader( fs );

                try
                {

                    //Get the position of the blockdata for this block
                    fs.Seek( ( BlockNumX * Terrain.NumBlocksY + blockNumZ ) * 4, SeekOrigin.Begin );
                    int blockPointer = br.ReadInt32();
                    fs.Seek( blockPointer, SeekOrigin.Begin );


                    ReadPreProcessedData( br );
                    BuildIndexBuffer( Terrain.Device );


                }
                finally
                {

                    br.Close();
                    fs.Close();
                }

                //return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            }

            tempNormalLoaded = true;

            return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Completed;

        }
        public MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState UnloadTask( Engine.LoadingTaskType taskType )
        {
            if ( vertexBuffer != null )
                vertexBuffer.Dispose();
            vertexBuffer = null;
            if ( indexBuffer != null )
                indexBuffer.Dispose();
            indexBuffer = null;

            tempNormalLoaded = false;
            return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Completed;

        }


        public virtual void LoadGraphicsContent( GraphicsDevice device, bool loadAllContent )
        {
            //BuildVertexBufferFromFile( device );
            BuildVertexBufferFromHeightmap();
            SetAndCalculateMinDistances( Terrain.Engine.ActiveCamera.CameraInfo.ProjectionMatrix );
            BuildIndexBuffer( device );
        }


        public void BuildVertexBufferFromHeightmap()
        {
            if ( vertexBuffer != null )
                vertexBuffer.Dispose();

            VertexMultitextured[] vertexes;

            Vector3 min;
            Vector3 max;

            vertexes = GenerateVerticesFromHeightmap( out min, out max );

            center = ( min + max ) * 0.5f;
            LocalBoundingBox = new BoundingBox( min, max );

            totalVertexes = vertexes.Length;

            if ( vertexBuffer != null )
                vertexBuffer.Dispose();

            vertexBuffer = new VertexBuffer( Terrain.Device, typeof( VertexPositionNormalTexture ),
                vertexes.Length, BufferUsage.None );
            vertexBuffer.SetData<VertexMultitextured>( vertexes );

            totalVertexes = vertexes.Length;
            vertexes = null;

            //vertexBufferDirty = false;
        }

        public VertexMultitextured[] GenerateVerticesFromHeightmap_New( HeightMapOud map, out Vector3 min, out Vector3 max )
        {
            VertexMultitextured[] vertexes = new VertexMultitextured[ ( terrain.BlockSize + 1 ) * ( terrain.BlockSize + 1 ) ];

            min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
            max = new Vector3( float.MinValue, float.MinValue, float.MinValue );

            //if ( blockNumX == 15 ) throw new Exception();

            // build vectors and normals
            for ( int ix = 0; ix <= terrain.BlockSize; ix++ )
            {
                //if ( ix == terrain.BlockSize ) throw new Exception();
                for ( int iz = 0; iz <= terrain.BlockSize; iz++ )
                {
                    int cx = this.x + ix;
                    int cz = this.z + iz;

                    VertexMultitextured vert = new VertexMultitextured();

                    //vert.Position = terrain.GetLocalPosition( new Vector3( cx, map[ cx, cz ], cz ) );
                    vert.Position = new Vector3( cx, map[ cx, cz ], cz );


                    //Special texcoord for weightmaps
                    vert.TextureCoordinate = new Vector2(
                        (float)( BlockNumX * ( terrain.BlockSize + 1 ) + ix ) / ( (float)terrain.NumBlocksX * ( terrain.BlockSize + 1 ) - 1 ),
                        (float)( BlockNumZ * ( terrain.BlockSize + 1 ) + iz ) / ( (float)terrain.NumBlocksY * ( terrain.BlockSize + 1 ) - 1 ) );

                    vert.Normal = CalculateAveragedNormal( map, cx, cz );// terrain.GetAveragedNormal( cx, cz );

                    min = Vector3.Min( min, vert.Position );
                    max = Vector3.Max( max, vert.Position );

                    vertexes[ IndexFromCoords( ix, iz ) ] = vert;
                }

            }


            return vertexes;

        }

        protected VertexMultitextured[] GenerateVerticesFromHeightmap( HeightMapOud map, out Vector3 min, out Vector3 max )
        {
            VertexMultitextured[] vertexes = new VertexMultitextured[ ( terrain.BlockSize + 1 ) * ( terrain.BlockSize + 1 ) ];

            min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
            max = new Vector3( float.MinValue, float.MinValue, float.MinValue );



            // build vectors and normals
            for ( int ix = 0; ix <= terrain.BlockSize; ix++ )
            {
                for ( int iz = 0; iz <= terrain.BlockSize; iz++ )
                {
                    int cx = this.x + ix;
                    int cz = this.z + iz;

                    VertexMultitextured vert = new VertexMultitextured();

                    //vert.Position = terrain.GetLocalPosition( new Vector3( cx, map[ cx, cz ], cz ) );
                    vert.Position = new Vector3( cx, map[ cx, cz ], cz );


                    //TODO: klopt deze texcoord wel?
                    vert.TextureCoordinate = new Vector2( (float)cx / (float)terrain.SizeX, (float)cz / (float)terrain.SizeY );

                    vert.Normal = CalculateAveragedNormal( map, cx, cz );// terrain.GetAveragedNormal( cx, cz );

                    min = Vector3.Min( min, vert.Position );
                    max = Vector3.Max( max, vert.Position );

                    vertexes[ IndexFromCoords( ix, iz ) ] = vert;
                }
            }


            return vertexes;

        }
        protected VertexMultitextured[] GenerateVerticesFromHeightmap( out Vector3 min, out Vector3 max )
        {
            return GenerateVerticesFromHeightmap( terrain.HeightMap, out min, out max );
        }

        protected VertexMultitextured[] GenerateVerticesFromHeightmap()
        {
            Vector3 min;
            Vector3 max;
            return GenerateVerticesFromHeightmap( out min, out max );
        }


        public static Vector3 CalculateAveragedNormal( HeightMapOud map, int x, int z )
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

        public static Vector3 CalculateNormal( HeightMapOud map, int x, int z )
        {
            float scale = 1;
            float heightScale = 1;
            Vector3 v1 = new Vector3( x * scale, map[ x, z + 1 ] * heightScale, ( z + 1 ) * scale );
            Vector3 v2 = new Vector3( x * scale, map[ x, z - 1 ] * heightScale, ( z - 1 ) * scale );
            Vector3 v3 = new Vector3( ( x + 1 ) * scale, map[ x + 1, z ] * heightScale, z * scale );
            Vector3 v4 = new Vector3( ( x - 1 ) * scale, map[ x - 1, z ] * heightScale, z * scale );

            return Vector3.Normalize( Vector3.Cross( v1 - v2, v3 - v4 ) );
        }



        protected override void Dispose( bool disposing )
        {

            base.Dispose( disposing );
        }



        public virtual void UnloadGraphicsContent( bool unloadAllContent )
        {
            if ( vertexBuffer != null )
                vertexBuffer.Dispose();

            if ( indexBuffer != null )
                indexBuffer.Dispose();


            vertexBuffer = null;
            indexBuffer = null;

        }

        public void ChangeDetailLevel( int mipLevel, bool force )
        {
            if ( mipLevel > terrain.MaxDetailLevel )
                mipLevel = terrain.MaxDetailLevel;

            if ( mipLevel == detailLevel && force != true )
                return;

            detailLevel = mipLevel;

            BuildBaseIndexes();
            BuildEdgeIndexes();

            if ( West != null )
                West.BuildEdgeIndexes();

            if ( East != null )
                East.BuildEdgeIndexes();

            if ( North != null )
                North.BuildEdgeIndexes();

            if ( South != null )
                South.BuildEdgeIndexes();
        }


        public override void OnBoundingChanged()
        {
            base.OnBoundingChanged();
            QuadTreeNode.OnTerreinBlockBoundingChanged();
        }


        public void GenerateLightmap()
        {
            TerrainLightmapGenerator gen = new TerrainLightmapGenerator( terrain );

            GenerateLightmap( gen );

        }

        public void GenerateLightmap( TerrainLightmapGenerator gen )
        {
            byte[] data = gen.Generate( x, z );
            Terrain.LightMap.SetData<byte>( 0, new Rectangle( x, z, terrain.BlockSize + 1, terrain.BlockSize + 1 ), data, 0, data.Length, SetDataOptions.None );

        }

        public void GenerateAutoWeights()
        {


            Color[] data = new Color[ 17 * 17 ];
            float A;
            float R;
            float G;
            float B;


            float ox = terrain.HeightMap.Width * 0.5f;
            float oz = terrain.HeightMap.Length * 0.5f;


            for ( int tx = 0; tx < terrain.BlockSize + 1; tx++ )
            {
                for ( int tz = 0; tz < terrain.BlockSize + 1; tz++ )
                {
                    int cx = tx + x;
                    int cz = tz + z;

                    G = CalculateWeight( terrain.HeightMap.GetHeight( cx, cz ) * terrain.HeightScale, 0, 75 );
                    R = CalculateWeight( terrain.HeightMap.GetHeight( cx, cz ) * terrain.HeightScale, 50, 250 );
                    B = CalculateWeight( terrain.HeightMap.GetHeight( cx, cz ) * terrain.HeightScale, 175, 400 );
                    A = CalculateWeight( terrain.HeightMap.GetHeight( cx, cz ) * terrain.HeightScale, 300, 1000 );

                    float totalWeight = R;
                    totalWeight += G;
                    totalWeight += B;
                    totalWeight += A;
                    R = R / totalWeight * 255;
                    G = G / totalWeight * 255;
                    B = B / totalWeight * 255;
                    A = A / totalWeight * 255;

                    R = (float)Math.Floor( R );
                    G = (float)Math.Floor( G );
                    B = (float)Math.Floor( B );
                    A = (float)Math.Floor( A );

                    A = 255 - R - G - B;

                    if ( totalWeight == 0 ) { R = G = B = 0; A = 255; }

                    data[ IndexFromCoords( tx, tz ) ] = new Color( (byte)R, (byte)G, (byte)B, (byte)A );
                }
            }






            Terrain.WeightMap.SetData<Color>( 0, new Rectangle( x, z, terrain.BlockSize + 1, terrain.BlockSize + 1 ), data, 0, data.Length, SetDataOptions.None );

        }

        protected void BuildVertexBufferFromFile( GraphicsDevice device )
        {
            if ( vertexBuffer != null )
                vertexBuffer.Dispose();








            System.IO.FileStream fs = new System.IO.FileStream( terrain.Filename, System.IO.FileMode.Open, System.IO.FileAccess.Read );



            fs.Position = filePointer.Pos;

            MHGameWork.TheWizards.Common.ByteReader br = new MHGameWork.TheWizards.Common.ByteReader( fs );

            VertexMultitextured[] vertexes = new VertexMultitextured[ ( terrain.BlockSize + 1 ) * ( terrain.BlockSize + 1 ) ];




            Vector3 min;
            Vector3 max;



            totalVertexes = br.ReadInt32();
            min = br.ReadVector3();
            max = br.ReadVector3();



            for ( int i = 0; i < totalVertexes; i++ )
            {
                VertexMultitextured vert = new VertexMultitextured();
                vert.Position = br.ReadVector3();
                vert.Normal = br.ReadVector3();
                vert.TextureCoordinate = br.ReadVector2();

                vertexes[ i ] = vert;
            }

            br.Close();
            fs.Close();




            center = ( min + max ) * 0.5f;
            localBoundingBox = new BoundingBox( min, max );




            if ( vertexBuffer != null )
                vertexBuffer.Dispose();

            vertexBuffer = new VertexBuffer( device, typeof( VertexMultitextured ),
                vertexes.Length, BufferUsage.None );
            vertexBuffer.SetData<VertexMultitextured>( vertexes );

            vertexes = null;





        }

        public float CalculateWeight( float height, float min, float max )
        {
            //oorspronkelijke formule
            // 1 - Math.Abs( ( height - min - ( ( max - min ) / 2 ) ) / ( ( max - min ) / 2 ) )
            float test = MathHelper.Clamp( 1 - Math.Abs( ( height - min - ( ( max - min ) / 2 ) ) / ( ( max - min ) / 2 ) ), 0, 1 );

            //uitgerekende formule

            float result = MathHelper.Clamp( 1 - Math.Abs( ( 2 * height - min - max ) / ( max - min ) ), 0, 1 );

            if ( test != result ) throw new Exception();
            return result;
        }

        //public void PaintWeight(int x, int y, int range, int texNum)
        //{
        //    BoundingSphere sphere = new BoundingSphere( new Vector3( x, 0, y ), range );
        //    BoundingBox box = new BoundingBox( new Vector3( this.x, 0, this.z ), new Vector3( this.x + 16, 0, this.z + 16 ) );
        //    if ( !sphere.Intersects( box ) ) return;
        //    x -= this.x;
        //    y -= this.z;
        //    //if ( x < 0 || x > 16 ) return;
        //    //if ( y < 0 || y > 16 ) return;
        //    Color[] data = new Color[ 17 * 17 ];
        //    weightTexture.GetData<Color>( data, 0, 17 * 17 );


        //    for ( int ix = 0; ix < 17; ix++ )
        //    {
        //        for ( int iy = 0; iy < 17; iy++ )
        //        {
        //            Vector2 diff = new Vector2( ix, iy ) - new Vector2( x, y );
        //            float dist = diff.Length();
        //            if ( dist < range )
        //            {
        //                float factor = 1 - ( dist / range );
        //                factor *= 255;
        //                Color c = data[ IndexFromCoords( ix, iy ) ];
        //                float a = c.A;
        //                float r = c.R;
        //                float g = c.G;
        //                float b = c.B;

        //                //Deel elke kleur door het nieuwe totaal * 255
        //                a = a / ( 255 + factor ) * 255;
        //                r = r / ( 255 + factor ) * 255;
        //                g = g / ( 255 + factor ) * 255;
        //                b = b / ( 255 + factor ) * 255;

        //                a = (float)Math.Floor( a );
        //                r = (float)Math.Floor( r );
        //                g = (float)Math.Floor( g );
        //                b = (float)Math.Floor( b );

        //                //Zorgt dat de som exact 255 is, de overschot gaat naar de gekozen weight

        //                switch ( texNum )
        //                {
        //                    case 0:
        //                        r = 255 - g - b - a;
        //                        break;
        //                    case 1:
        //                        g = 255 - r - b - a;
        //                        break;
        //                    case 2:
        //                        b = 255 - r - g - a;
        //                        break;
        //                    case 3:
        //                        a = 255 - r - g - b;
        //                        break;
        //                }




        //                //r = (byte)Math.Floor( 255 * factor );
        //                //a = (byte)( 1 - r );
        //                data[ IndexFromCoords( ix, iy ) ] = new Color( (byte)r, (byte)g, (byte)b, (byte)a );

        //            }
        //        }
        //    }

        //    /*byte a;
        //    byte r;
        //    byte g;
        //    byte b;
        //    Color c;*/
        //    //c = data[ IndexFromCoords( x, y ) ];
        //    //data[ IndexFromCoords( x, y ) ] = new Color( 255, 0, 0, 0 );

        //    /*data[ IndexFromCoords( x, y ) ].A = 0;*/

        //    weightTexture.SetData<Color>( data );


        //}

        /*public byte[] ToBytes()
        {
            MHGameWork.TheWizards.Common.ByteWriter BW = new MHGameWork.TheWizards.Common.ByteWriter();

            VertexPositionNormalTexture[] vertexes = new VertexPositionNormalTexture[ ( terrain.BlockSize + 1 ) * ( terrain.BlockSize + 1 ) ];


            Vector3 min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
            Vector3 max = new Vector3( float.MinValue, float.MinValue, float.MinValue );

            float ox = terrain.HeightMap.Width * 0.5f;
            float oz = terrain.HeightMap.Length * 0.5f;

            // build vectors and normals
            for ( int x = 0; x <= terrain.BlockSize; x++ )
            {
                for ( int z = 0; z <= terrain.BlockSize; z++ )
                {
                    int cx = this.x + x;
                    int cz = this.z + z;

                    Vector3 position = new Vector3(
                        ( cx - ox ) * terrain.Scale,
                        terrain.HeightMap[ cx, cz ] * terrain.HeightScale,
                        ( cz - oz ) * terrain.Scale );

                    Vector2 textureCoords = new Vector2(
                        (float)x / ( (float)terrain.BlockSize + 0.51f ),
                        (float)z / ( (float)terrain.BlockSize + 0.51f ) );

                    Vector3 normal = terrain.GetAveragedNormal( cx, cz );

                    min = Vector3.Min( min, position );
                    max = Vector3.Max( max, position );

                    vertexes[ IndexFromCoords( x, z ) ] = new VertexPositionNormalTexture( position, normal, textureCoords );
                }
            }

            center = ( min + max ) * 0.5f;
            BoundingBox = new BoundingBox( min, max );

            totalVertexes = vertexes.Length;


            BW.Write( totalVertexes );
            BW.Write( min );
            BW.Write( max );
            for ( int i = 0; i < vertexes.Length; i++ )
            {
                BW.Write( vertexes[ i ].Position );
                BW.Write( vertexes[ i ].Normal );
                BW.Write( vertexes[ i ].TextureCoordinate );
            }

            return BW.ToBytesAndClose();
        }*/


        public override byte[] ToBytes()
        {
            MHGameWork.TheWizards.Common.ByteWriter BW = new MHGameWork.TheWizards.Common.ByteWriter();

            VertexMultitextured[] vertexes;

            Vector3 min;
            Vector3 max;

            vertexes = GenerateVerticesFromHeightmap( out min, out max );


            totalVertexes = vertexes.Length;


            BW.Write( totalVertexes );
            BW.Write( min );
            BW.Write( max );
            for ( int i = 0; i < vertexes.Length; i++ )
            {
                BW.Write( vertexes[ i ].Position );
                BW.Write( vertexes[ i ].Normal );
                BW.Write( vertexes[ i ].TextureCoordinate );
            }

            return BW.ToBytesAndClose();
        }

        public void WritePreProcessedData( MHGameWork.TheWizards.Common.ByteWriter BW, HeightMapOud map, Matrix projectionMatrix )
        {
            VertexMultitextured[] vertexes;

            Vector3 min;
            Vector3 max;

            vertexes = GenerateVerticesFromHeightmap( map, out min, out max );


            totalVertexes = vertexes.Length;


            BW.Write( totalVertexes );
            BW.Write( min );
            BW.Write( max );
            for ( int i = 0; i < vertexes.Length; i++ )
            {
                BW.Write( vertexes[ i ].Position );
                BW.Write( vertexes[ i ].Normal );
                BW.Write( vertexes[ i ].TextureCoordinate );
            }

            float[] localMinDistancesSquared = CalculateMinDistances( projectionMatrix, map );

            BW.Write( localMinDistancesSquared.Length );

            for ( int i = 0; i < localMinDistancesSquared.Length; i++ )
            {
                BW.Write( localMinDistancesSquared[ i ] );
            }

        }

        VertexMultitextured[] tempVertices;
        public void ReadPreProcessedData( MHGameWork.TheWizards.Common.ByteReader br )
        {
            VertexMultitextured[] vertexes;

            Vector3 min;
            Vector3 max;


            totalVertexes = br.ReadInt32();
            vertexes = new VertexMultitextured[ totalVertexes ];
            tempVertices = vertexes;
            min = br.ReadVector3();
            max = br.ReadVector3();

            for ( int i = 0; i < vertexes.Length; i++ )
            {
                vertexes[ i ] = new VertexMultitextured();
                vertexes[ i ].Position = br.ReadVector3();
                vertexes[ i ].Normal = br.ReadVector3();
                vertexes[ i ].TextureCoordinate = br.ReadVector2();
            }

            //Debug verification
            if ( vertexes[ 0 ].Position.X != x || vertexes[ 0 ].Position.Z != z ) throw new Exception();


            SetMinMax( min, max );



            if ( vertexBuffer != null )
                vertexBuffer.Dispose();

            vertexBuffer = new VertexBuffer( Terrain.Device, typeof( VertexPositionNormalTexture ),
                vertexes.Length, BufferUsage.None );
            vertexBuffer.SetData<VertexMultitextured>( vertexes );

            vertexes = null;

            //vertexBufferDirty = false;


            float[] localMinDistancesSquared = new float[ br.ReadInt32() ];

            for ( int i = 0; i < localMinDistancesSquared.Length; i++ )
            {
                localMinDistancesSquared[ i ] = br.ReadSingle();
            }

            MinDistancesSquared = localMinDistancesSquared;

        }

        public void SetMinMax( Vector3 min, Vector3 max )
        {
            center = ( min + max ) * 0.5f;
            LocalBoundingBox = new BoundingBox( min, max );
        }

        public void BuildIndexBuffer( GraphicsDevice device )
        {
            if ( indexBuffer != null )
                indexBuffer.Dispose();

            // allocate index buffer for maximum amount of room needed
            indexBuffer = new IndexBuffer( device, typeof( ushort ),
                ( terrain.BlockSize + 1 ) * ( terrain.BlockSize + 1 ) * 6, BufferUsage.None );

            List<ushort> indexes = new List<ushort>();

            BuildBaseIndexes( indexes );
            BuildEdgeIndexes( indexes );

            if ( indexes.Count > 0 )
                indexBuffer.SetData<ushort>( indexes.ToArray() );
        }

        public void BuildBaseIndexes()
        {
            List<ushort> indexes = new List<ushort>();

            BuildBaseIndexes( indexes );

            if ( indexes.Count > 0 )
                indexBuffer.SetData<ushort>( indexes.ToArray() );
        }



        public void BuildEdgeIndexes()
        {
            if ( indexBuffer == null ) return;
            List<ushort> indexes = new List<ushort>();

            BuildEdgeIndexes( indexes );

            if ( indexes.Count > 0 )
            {
                indexBuffer.SetData<ushort>( totalEdgeOffset * sizeof( ushort ),
                    indexes.ToArray(), 0, indexes.Count );
                //TODO: CHECK IF THIS EDIT IS OK!!
                /*indexBuffer.SetData<ushort>( totalEdgeOffset * sizeof( ushort ),
                    indexes.ToArray(), 0, indexes.Count, SetDataOptions.None );*/
            }
        }


        public virtual void Update()
        {
            if ( indexBuffer == null ) return;
            if ( vertexBuffer == null ) return;
            if ( Terrain.Engine.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.N ) )
            {
                float distance = Vector3.Distance( terrain.localCameraPosition, center );
                float ratio = 500f / (float)terrain.MaxDetailLevel;

                ChangeDetailLevel( (int)Math.Round( distance / ratio ), false );
            }
            else
            {
                int level = 0;

                float distSquared = Vector3.DistanceSquared( terrain.localCameraPosition, center );
                for ( int i = 0; i <= terrain.MaxDetailLevel; i++ )
                {
                    if ( distSquared > MinDistancesSquared[ i ] ) level = i;

                }

                ChangeDetailLevel( level, false );
            }
        }

        public virtual int Draw( GraphicsDevice device )
        {

            if ( vertexBuffer == null || indexBuffer == null ) return 0;
            int totalTriangles = TotalBaseTriangles + TotalEdgeTriangles;

            if ( totalTriangles <= 0 )
                return 0;
            //if ( x > 400 ) return 0;
            device.Vertices[ 0 ].SetSource( vertexBuffer, 0, VertexMultitextured.SizeInBytes );
            device.Indices = indexBuffer;
            /*device.Textures[ 2 ] = terrain.GetTextureOld( 0 );
            device.Textures[ 3 ] = terrain.GetTextureOld( 1 );
            device.Textures[ 4 ] = terrain.GetTextureOld( 2 );
            device.Textures[ 5 ] = terrain.GetTextureOld( 3 );*/


            device.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, totalVertexes, 0, totalTriangles );


            Terrain.Statistics.DrawCalls += 1;

            return totalTriangles;
        }


        public void LoadHeightField()
        {
            UnloadHeightField();
            actor = CreateHeightField();
        }

        public void UnloadHeightField()
        {
            if ( actor != null ) actor.destroy();
            actor = null;

        }

        public NxActor CreateHeightField()
        {
            NxHeightFieldDesc heightFieldDesc = NxHeightFieldDesc.Default;
            heightFieldDesc.nbColumns = (uint)terrain.BlockSize + 1;
            heightFieldDesc.nbRows = (uint)terrain.BlockSize + 1;
            heightFieldDesc.verticalExtent = -1000;
            heightFieldDesc.convexEdgeThreshold = 0;
            heightFieldDesc.sampleStride = 3 * 4;
            heightFieldDesc.setSampleDimensions( terrain.BlockSize + 1, terrain.BlockSize + 1 );

            float ox = terrain.HeightMap.Width * 0.5f;
            float oz = terrain.HeightMap.Length * 0.5f;

            for ( int ix = 0; ix < terrain.BlockSize + 1; ix++ )
            {
                for ( int iz = 0; iz < terrain.BlockSize + 1; iz++ )
                {
                    int cx = this.x + ix;
                    int cz = this.z + iz;
                    bool flag = ( ix % 2 ) == 0;
                    if ( iz % 2 != 0 ) flag = !flag;
                    flag = false;
                    heightFieldDesc.setSample( ix, iz, new NxHeightFieldSample( (short)( terrain.HeightMap[ cx, cz ] * 5000 ), 0, flag, 0 ) );
                }
            }
            NxHeightField heightField = ServerClientMainOud.instance.ServerMain.PhysicsSDK.createHeightField( heightFieldDesc );

            heightFieldDesc = null;

            NxHeightFieldShapeDesc shapeDesc = new NxHeightFieldShapeDesc();

            shapeDesc.HeightField = heightField;
            shapeDesc.heightScale = terrain.HeightScale / 5000;
            shapeDesc.rowScale = terrain.Scale;
            shapeDesc.columnScale = terrain.Scale;
            shapeDesc.materialIndexHighBits = 0;
            shapeDesc.holeMaterial = 2;

            NxActorDesc actorDesc = new NxActorDesc();
            actorDesc.addShapeDesc( shapeDesc );
            actorDesc.globalPose = Matrix.CreateTranslation( new Vector3( ( this.x - ox ) * terrain.Scale, 0, ( this.z - oz ) * terrain.Scale ) );
            return ServerClientMainOud.instance.ServerMain.PhysicsScene.createActor( actorDesc );

        }


        public new Terrain Terrain
        {
            get { return (Terrain)terrain; }

        }
        public new TerrainBlock West
        {
            get { return (TerrainBlock)base.West; }
            set { base.West = value; }
        }

        public new TerrainBlock East
        {
            get { return (TerrainBlock)base.East; }
            set { base.East = value; }
        }

        public new TerrainBlock North
        {
            get { return (TerrainBlock)base.North; }
            set { base.North = value; }
        }

        public new TerrainBlock South
        {
            get { return (TerrainBlock)base.South; }
            set { base.South = value; }
        }

        public new Wereld.QuadTreeNode QuadTreeNode
        {
            get { return (Wereld.QuadTreeNode)base.QuadTreeNode; }
            set { base.QuadTreeNode = value; }
        }

        #region ITerrainBlock Members

        private Wereld.QuadTreeNode quadtreeNode;

        public void AssignToQuadtreeNode( MHGameWork.TheWizards.Common.Wereld.IQuadtreeNode node )
        {
            if ( node is Wereld.QuadTreeNode )
            {
                throw new ArgumentException( "Only nodes that are ServerClient.Wereld.QuadTreeNode can be added to this blocks" );
            }

            quadtreeNode = (Wereld.QuadTreeNode)node;

            throw new Exception( "The method or operation is not implemented." );
        }

        public MHGameWork.TheWizards.Common.Wereld.IQuadtreeNode GetIQuadtreeNode()
        {
            return (MHGameWork.TheWizards.Common.Wereld.IQuadtreeNode)quadtreeNode;

        }

        #endregion

        #region ITerrainBlock Members


        public ITerrainBlock GetNeighbour( TerrainBlockEdge edge )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public void SetNeighbour( TerrainBlockEdge edge, ITerrainBlock neighbour )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        #region ITerrainBlockRenderable Members

        public int DrawPrimitives()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public void RenderBatched()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion


    }
}

