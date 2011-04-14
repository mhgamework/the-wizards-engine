using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NovodexWrapper;
using MHGameWork.TheWizards.Common.GeoMipMap;
using VertexMultitextured = MHGameWork.TheWizards.ServerClient.XNAGeoMipMap.VertexMultitextured;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class TerrainBlock
    {
        private XNAGeoMipMap.TerrainBlock baseBlock;
        private TerrainOud terrainOud;

        public TerrainOud TerrainOud
        {
            get { return terrainOud; }
            set { terrainOud = value; }
        }

        private bool vertexBufferDirty;
        private bool heightMapDirty;


        private VertexBuffer viewVertexBuffer;

        public VertexBuffer ViewVertexBuffer
        {
            get { return viewVertexBuffer; }
            set { viewVertexBuffer = value; }
        }

        private TerrainMaterial viewMaterial;

        public TerrainMaterial ViewMaterial
        {
            get { return viewMaterial; }
            set { viewMaterial = value; }
        }

        /// <summary>
        /// Is true when the heightmap data for this block differs from the server's heightmap data
        /// </summary>
        public bool HeightMapDirty
        {
            get { return heightMapDirty; }
            set { heightMapDirty = value; }
        }

        private bool serverDirty;
        public bool ServerDirty
        {
            get { return serverDirty; }
            set { serverDirty = value; }
        }
        public XNAGeoMipMap.TerrainBlock BaseBlock
        {
            get { return baseBlock; }
            //set { baseBlock = value; }
        }
        protected TerrainBlock[] neighbors = new TerrainBlock[ 4 ];

        //private VertexBuffer vertexBuffer;
        //private IndexBuffer indexBuffer;
        //private Vector3 center;
        //private int totalVertexes = 0;
        //private NxActor actor;





        public TerrainBlock( TerrainOud nTerrainOud, XNAGeoMipMap.TerrainBlock nBaseBlock )
        {
            terrainOud = nTerrainOud;
            baseBlock = nBaseBlock;
        }








        public virtual void LoadGraphicsContent( GraphicsDevice device, bool loadAllContent )
        {
            //BuildVertexBufferFromFile( device );
            //BuildVertexBufferFromHeightmap();
            //CalculateMinDistances( Terrain.Engine.ActiveCamera.CameraInfo.ProjectionMatrix );
            //BuildIndexBuffer( device );
        }
















        protected void Dispose( bool disposing )
        {

        }



        public void SetVertexBufferDirty()
        {
            vertexBufferDirty = true;
        }

        public void SetHeightMapDirty()
        {
            serverDirty = true;
            heightMapDirty = true;
        }

        //public virtual void UnloadGraphicsContent( bool unloadAllContent )
        //{
        //    if ( vertexBuffer != null )
        //        vertexBuffer.Dispose();

        //    if ( indexBuffer != null )
        //        indexBuffer.Dispose();


        //    vertexBuffer = null;
        //    indexBuffer = null;

        //}

        //public void ChangeDetailLevel( int mipLevel, bool force )
        //{
        //    if ( mipLevel > terrain.MaxDetailLevel )
        //        mipLevel = terrain.MaxDetailLevel;

        //    if ( mipLevel == detailLevel && force != true )
        //        return;

        //    detailLevel = mipLevel;

        //    BuildBaseIndexes();
        //    BuildEdgeIndexes();

        //    if ( West != null )
        //        West.BuildEdgeIndexes();

        //    if ( East != null )
        //        East.BuildEdgeIndexes();

        //    if ( North != null )
        //        North.BuildEdgeIndexes();

        //    if ( South != null )
        //        South.BuildEdgeIndexes();
        //}


        //public void OnBoundingChanged()
        //{
        //    base.OnBoundingChanged();
        //    QuadTreeNode.OnTerreinBlockBoundingChanged();
        //}


        //public void GenerateLightmap()
        //{
        //    TerrainLightmapGenerator gen = new TerrainLightmapGenerator( terrain );

        //    GenerateLightmap( gen );

        //}

        //public void GenerateLightmap( TerrainLightmapGenerator gen )
        //{
        //    byte[] data = gen.Generate( x, z );
        //    Terrain.LightMap.SetData<byte>( 0, new Rectangle( x, z, terrain.BlockSize + 1, terrain.BlockSize + 1 ), data, 0, data.Length, SetDataOptions.None );

        //}

        //public void GenerateAutoWeights()
        //{


        //    Color[] data = new Color[ 17 * 17 ];
        //    float A;
        //    float R;
        //    float G;
        //    float B;


        //    float ox = terrain.HeightMap.Width * 0.5f;
        //    float oz = terrain.HeightMap.Length * 0.5f;


        //    for ( int tx = 0; tx < terrain.BlockSize + 1; tx++ )
        //    {
        //        for ( int tz = 0; tz < terrain.BlockSize + 1; tz++ )
        //        {
        //            int cx = tx + x;
        //            int cz = tz + z;

        //            G = CalculateWeight( terrain.HeightMap.GetHeight( cx, cz ) * terrain.HeightScale, 0, 75 );
        //            R = CalculateWeight( terrain.HeightMap.GetHeight( cx, cz ) * terrain.HeightScale, 50, 250 );
        //            B = CalculateWeight( terrain.HeightMap.GetHeight( cx, cz ) * terrain.HeightScale, 175, 400 );
        //            A = CalculateWeight( terrain.HeightMap.GetHeight( cx, cz ) * terrain.HeightScale, 300, 1000 );

        //            float totalWeight = R;
        //            totalWeight += G;
        //            totalWeight += B;
        //            totalWeight += A;
        //            R = R / totalWeight * 255;
        //            G = G / totalWeight * 255;
        //            B = B / totalWeight * 255;
        //            A = A / totalWeight * 255;

        //            R = (float)Math.Floor( R );
        //            G = (float)Math.Floor( G );
        //            B = (float)Math.Floor( B );
        //            A = (float)Math.Floor( A );

        //            A = 255 - R - G - B;

        //            if ( totalWeight == 0 ) { R = G = B = 0; A = 255; }

        //            data[ IndexFromCoords( tx, tz ) ] = new Color( (byte)R, (byte)G, (byte)B, (byte)A );
        //        }
        //    }






        //    Terrain.WeightMap.SetData<Color>( 0, new Rectangle( x, z, terrain.BlockSize + 1, terrain.BlockSize + 1 ), data, 0, data.Length, SetDataOptions.None );

        //}

        //protected void BuildVertexBufferFromFile( GraphicsDevice device )
        //{
        //    if ( vertexBuffer != null )
        //        vertexBuffer.Dispose();








        //    System.IO.FileStream fs = new System.IO.FileStream( terrain.Filename, System.IO.FileMode.Open, System.IO.FileAccess.Read );



        //    fs.Position = filePointer.Pos;

        //    MHGameWork.TheWizards.Common.ByteReader br = new MHGameWork.TheWizards.Common.ByteReader( fs );

        //    VertexMultitextured[] vertexes = new VertexMultitextured[ ( terrain.BlockSize + 1 ) * ( terrain.BlockSize + 1 ) ];




        //    Vector3 min;
        //    Vector3 max;



        //    totalVertexes = br.ReadInt32();
        //    min = br.ReadVector3();
        //    max = br.ReadVector3();



        //    for ( int i = 0; i < totalVertexes; i++ )
        //    {
        //        VertexMultitextured vert = new VertexMultitextured();
        //        vert.Position = br.ReadVector3();
        //        vert.Normal = br.ReadVector3();
        //        vert.TextureCoordinate = br.ReadVector2();

        //        vertexes[ i ] = vert;
        //    }

        //    br.Close();
        //    fs.Close();




        //    center = ( min + max ) * 0.5f;
        //    localBoundingBox = new BoundingBox( min, max );




        //    if ( vertexBuffer != null )
        //        vertexBuffer.Dispose();

        //    vertexBuffer = new VertexBuffer( device, typeof( VertexMultitextured ),
        //        vertexes.Length, BufferUsage.None );
        //    vertexBuffer.SetData<VertexMultitextured>( vertexes );

        //    vertexes = null;





        //}

        //public float CalculateWeight( float height, float min, float max )
        //{
        //    //oorspronkelijke formule
        //    // 1 - Math.Abs( ( height - min - ( ( max - min ) / 2 ) ) / ( ( max - min ) / 2 ) )
        //    float test = MathHelper.Clamp( 1 - Math.Abs( ( height - min - ( ( max - min ) / 2 ) ) / ( ( max - min ) / 2 ) ), 0, 1 );

        //    //uitgerekende formule

        //    float result = MathHelper.Clamp( 1 - Math.Abs( ( 2 * height - min - max ) / ( max - min ) ), 0, 1 );

        //    if ( test != result ) throw new Exception();
        //    return result;
        //}

        ////public void PaintWeight(int x, int y, int range, int texNum)
        ////{
        ////    BoundingSphere sphere = new BoundingSphere( new Vector3( x, 0, y ), range );
        ////    BoundingBox box = new BoundingBox( new Vector3( this.x, 0, this.z ), new Vector3( this.x + 16, 0, this.z + 16 ) );
        ////    if ( !sphere.Intersects( box ) ) return;
        ////    x -= this.x;
        ////    y -= this.z;
        ////    //if ( x < 0 || x > 16 ) return;
        ////    //if ( y < 0 || y > 16 ) return;
        ////    Color[] data = new Color[ 17 * 17 ];
        ////    weightTexture.GetData<Color>( data, 0, 17 * 17 );


        ////    for ( int ix = 0; ix < 17; ix++ )
        ////    {
        ////        for ( int iy = 0; iy < 17; iy++ )
        ////        {
        ////            Vector2 diff = new Vector2( ix, iy ) - new Vector2( x, y );
        ////            float dist = diff.Length();
        ////            if ( dist < range )
        ////            {
        ////                float factor = 1 - ( dist / range );
        ////                factor *= 255;
        ////                Color c = data[ IndexFromCoords( ix, iy ) ];
        ////                float a = c.A;
        ////                float r = c.R;
        ////                float g = c.G;
        ////                float b = c.B;

        ////                //Deel elke kleur door het nieuwe totaal * 255
        ////                a = a / ( 255 + factor ) * 255;
        ////                r = r / ( 255 + factor ) * 255;
        ////                g = g / ( 255 + factor ) * 255;
        ////                b = b / ( 255 + factor ) * 255;

        ////                a = (float)Math.Floor( a );
        ////                r = (float)Math.Floor( r );
        ////                g = (float)Math.Floor( g );
        ////                b = (float)Math.Floor( b );

        ////                //Zorgt dat de som exact 255 is, de overschot gaat naar de gekozen weight

        ////                switch ( texNum )
        ////                {
        ////                    case 0:
        ////                        r = 255 - g - b - a;
        ////                        break;
        ////                    case 1:
        ////                        g = 255 - r - b - a;
        ////                        break;
        ////                    case 2:
        ////                        b = 255 - r - g - a;
        ////                        break;
        ////                    case 3:
        ////                        a = 255 - r - g - b;
        ////                        break;
        ////                }




        ////                //r = (byte)Math.Floor( 255 * factor );
        ////                //a = (byte)( 1 - r );
        ////                data[ IndexFromCoords( ix, iy ) ] = new Color( (byte)r, (byte)g, (byte)b, (byte)a );

        ////            }
        ////        }
        ////    }

        ////    /*byte a;
        ////    byte r;
        ////    byte g;
        ////    byte b;
        ////    Color c;*/
        ////    //c = data[ IndexFromCoords( x, y ) ];
        ////    //data[ IndexFromCoords( x, y ) ] = new Color( 255, 0, 0, 0 );

        ////    /*data[ IndexFromCoords( x, y ) ].A = 0;*/

        ////    weightTexture.SetData<Color>( data );


        ////}

        ///*public byte[] ToBytes()
        //{
        //    MHGameWork.TheWizards.Common.ByteWriter BW = new MHGameWork.TheWizards.Common.ByteWriter();

        //    VertexPositionNormalTexture[] vertexes = new VertexPositionNormalTexture[ ( terrain.BlockSize + 1 ) * ( terrain.BlockSize + 1 ) ];


        //    Vector3 min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
        //    Vector3 max = new Vector3( float.MinValue, float.MinValue, float.MinValue );

        //    float ox = terrain.HeightMap.Width * 0.5f;
        //    float oz = terrain.HeightMap.Length * 0.5f;

        //    // build vectors and normals
        //    for ( int x = 0; x <= terrain.BlockSize; x++ )
        //    {
        //        for ( int z = 0; z <= terrain.BlockSize; z++ )
        //        {
        //            int cx = this.x + x;
        //            int cz = this.z + z;

        //            Vector3 position = new Vector3(
        //                ( cx - ox ) * terrain.Scale,
        //                terrain.HeightMap[ cx, cz ] * terrain.HeightScale,
        //                ( cz - oz ) * terrain.Scale );

        //            Vector2 textureCoords = new Vector2(
        //                (float)x / ( (float)terrain.BlockSize + 0.51f ),
        //                (float)z / ( (float)terrain.BlockSize + 0.51f ) );

        //            Vector3 normal = terrain.GetAveragedNormal( cx, cz );

        //            min = Vector3.Min( min, position );
        //            max = Vector3.Max( max, position );

        //            vertexes[ IndexFromCoords( x, z ) ] = new VertexPositionNormalTexture( position, normal, textureCoords );
        //        }
        //    }

        //    center = ( min + max ) * 0.5f;
        //    BoundingBox = new BoundingBox( min, max );

        //    totalVertexes = vertexes.Length;


        //    BW.Write( totalVertexes );
        //    BW.Write( min );
        //    BW.Write( max );
        //    for ( int i = 0; i < vertexes.Length; i++ )
        //    {
        //        BW.Write( vertexes[ i ].Position );
        //        BW.Write( vertexes[ i ].Normal );
        //        BW.Write( vertexes[ i ].TextureCoordinate );
        //    }

        //    return BW.ToBytesAndClose();
        //}*/


        //public byte[] ToBytes()
        //{
        //    MHGameWork.TheWizards.Common.ByteWriter BW = new MHGameWork.TheWizards.Common.ByteWriter();

        //    VertexMultitextured[] vertexes;

        //    Vector3 min;
        //    Vector3 max;

        //    vertexes = GenerateVerticesFromHeightmap( out min, out max );


        //    totalVertexes = vertexes.Length;


        //    BW.Write( totalVertexes );
        //    BW.Write( min );
        //    BW.Write( max );
        //    for ( int i = 0; i < vertexes.Length; i++ )
        //    {
        //        BW.Write( vertexes[ i ].Position );
        //        BW.Write( vertexes[ i ].Normal );
        //        BW.Write( vertexes[ i ].TextureCoordinate );
        //    }

        //    return BW.ToBytesAndClose();
        //}


        //public void BuildIndexBuffer( GraphicsDevice device )
        //{
        //    if ( indexBuffer != null )
        //        indexBuffer.Dispose();

        //    // allocate index buffer for maximum amount of room needed
        //    indexBuffer = new IndexBuffer( device, typeof( ushort ),
        //        ( terrain.BlockSize + 1 ) * ( terrain.BlockSize + 1 ) * 6, BufferUsage.None );

        //    List<ushort> indexes = new List<ushort>();

        //    BuildBaseIndexes( indexes );
        //    BuildEdgeIndexes( indexes );

        //    if ( indexes.Count > 0 )
        //        indexBuffer.SetData<ushort>( indexes.ToArray() );
        //}

        //public void BuildBaseIndexes()
        //{
        //    List<ushort> indexes = new List<ushort>();

        //    BuildBaseIndexes( indexes );

        //    if ( indexes.Count > 0 )
        //        indexBuffer.SetData<ushort>( indexes.ToArray() );
        //}



        //public void BuildEdgeIndexes()
        //{
        //    List<ushort> indexes = new List<ushort>();

        //    BuildEdgeIndexes( indexes );

        //    if ( indexes.Count > 0 )
        //    {
        //        indexBuffer.SetData<ushort>( totalEdgeOffset * sizeof( ushort ),
        //            indexes.ToArray(), 0, indexes.Count );
        //        //TODO: CHECK IF THIS EDIT IS OK!!
        //        /*indexBuffer.SetData<ushort>( totalEdgeOffset * sizeof( ushort ),
        //            indexes.ToArray(), 0, indexes.Count, SetDataOptions.None );*/
        //    }
        //}


        //public virtual void Update()
        //{
        //    if ( Terrain.Engine.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.N ) )
        //    {
        //        float distance = Vector3.Distance( terrain.localCameraPosition, center );
        //        float ratio = 500f / (float)terrain.MaxDetailLevel;

        //        ChangeDetailLevel( (int)Math.Round( distance / ratio ), false );
        //    }
        //    else
        //    {
        //        int level = 0;

        //        float distSquared = Vector3.DistanceSquared( terrain.localCameraPosition, center );
        //        for ( int i = 0; i <= terrain.MaxDetailLevel; i++ )
        //        {
        //            if ( distSquared > minDistancesSquared[ i ] ) level = i;

        //        }



        //        ChangeDetailLevel( level, false );
        //    }
        //}

        //public virtual int Draw( GraphicsDevice device )
        //{
        //    if ( vertexBuffer == null ) return 0;
        //    int totalTriangles = totalBaseTriangles + totalEdgeTriangles;

        //    if ( totalTriangles <= 0 )
        //        return 0;

        //    device.Vertices[ 0 ].SetSource( vertexBuffer, 0, VertexMultitextured.SizeInBytes );
        //    device.Indices = indexBuffer;
        //    /*device.Textures[ 2 ] = terrain.GetTextureOld( 0 );
        //    device.Textures[ 3 ] = terrain.GetTextureOld( 1 );
        //    device.Textures[ 4 ] = terrain.GetTextureOld( 2 );
        //    device.Textures[ 5 ] = terrain.GetTextureOld( 3 );*/


        //    device.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, totalVertexes, 0, totalTriangles );


        //    Terrain.Statistics.DrawCalls += 1;

        //    return totalTriangles;
        //}










        // Functions




        public void BuildVertexBufferFromHeightmap( HeightMapOud heightMap )
        {
            if ( baseBlock.VertexBuffer != null )
                baseBlock.VertexBuffer.Dispose();

            VertexMultitextured[] vertexes;

            Vector3 min;
            Vector3 max;

            vertexes = GenerateVerticesFromHeightmap( heightMap, out min, out max );

            baseBlock.Center = ( min + max ) * 0.5f;
            baseBlock.LocalBoundingBox = new BoundingBox( min, max );

            baseBlock.TotalVertexes = vertexes.Length;

            if ( baseBlock.VertexBuffer != null )
                baseBlock.VertexBuffer.Dispose();

            baseBlock.VertexBuffer = new VertexBuffer( baseBlock.Terrain.Device, typeof( VertexMultitextured ),
                vertexes.Length, BufferUsage.None );
            baseBlock.VertexBuffer.SetData<VertexMultitextured>( vertexes );

            baseBlock.TotalVertexes = vertexes.Length;
            vertexes = null;

            vertexBufferDirty = false;
        }

        protected XNAGeoMipMap.VertexMultitextured[] GenerateVerticesFromHeightmap( HeightMapOud map, out Vector3 min, out Vector3 max )
        {
            VertexMultitextured[] vertexes = new VertexMultitextured[ ( BlockSize + 1 ) * ( BlockSize + 1 ) ];

            min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
            max = new Vector3( float.MinValue, float.MinValue, float.MinValue );



            // build vectors and normals
            for ( int x = 0; x <= BlockSize; x++ )
            {
                for ( int z = 0; z <= BlockSize; z++ )
                {
                    int cx = baseBlock.X + x;
                    int cz = baseBlock.Z + z;

                    VertexMultitextured vert = new VertexMultitextured();

                    vert.Position = new Vector3( cx, map[ cx, cz ], cz ); //terrain.GetLocalPosition( new Vector3( cx, map[ cx, cz ], cz ) );


                    //TODO: klopt deze texcoord wel?
                    vert.TextureCoordinate = new Vector2( (float)cx / (float)terrainOud.SizeX, (float)cz / (float)terrainOud.SizeY );

                    vert.Normal = GetAveragedNormal( map, cx, cz ); //terrain.GetAveragedNormal( cx, cz );

                    min = Vector3.Min( min, vert.Position );
                    max = Vector3.Max( max, vert.Position );

                    vertexes[ baseBlock.IndexFromCoords( x, z ) ] = vert;
                }
            }


            return vertexes;

        }

        protected XNAGeoMipMap.VertexMultitextured[] GenerateVerticesFromHeightmap( HeightMapOud heightMap )
        {
            Vector3 min;
            Vector3 max;
            return GenerateVerticesFromHeightmap( heightMap, out min, out max );
        }




        /// <summary>
        /// to be placed correctly
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 GetAveragedNormal( HeightMapOud map, int x, int z )
        {
            Vector3 normal = new Vector3();

            // top left
            if ( x > 0 && z > 0 )
                normal += GetNormal( map, x - 1, z - 1 );

            // top center
            if ( z > 0 )
                normal += GetNormal( map, x, z - 1 );

            // top right
            if ( x < map.Width && z > 0 )
                normal += GetNormal( map, x + 1, z - 1 );

            // middle left
            if ( x > 0 )
                normal += GetNormal( map, x - 1, z );

            // middle center
            normal += GetNormal( map, x, z );

            // middle right
            if ( x < map.Width )
                normal += GetNormal( map, x + 1, z );

            // lower left
            if ( x > 0 && z < map.Length )
                normal += GetNormal( map, x - 1, z + 1 );

            // lower center
            if ( z < map.Length )
                normal += GetNormal( map, x, z + 1 );

            // lower right
            if ( x < map.Width && z < map.Length )
                normal += GetNormal( map, x + 1, z + 1 );

            return Vector3.Normalize( normal );
        }

        /// <summary>
        /// to be place correctly
        /// </summary>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 GetNormal( HeightMapOud map, int x, int z )
        {
            Vector3 v1 = new Vector3( x, map[ x, z + 1 ], ( z + 1 ) );
            Vector3 v2 = new Vector3( x, map[ x, z - 1 ], ( z - 1 ) );
            Vector3 v3 = new Vector3( ( x + 1 ), map[ x + 1, z ], z );
            Vector3 v4 = new Vector3( ( x - 1 ), map[ x - 1, z ], z );

            return Vector3.Normalize( Vector3.Cross( v1 - v2, v3 - v4 ) );
        }



        public void CalculateMinDistances( HeightMapOud heightMap, Matrix projection )
        {
            for ( int i = 0; i < baseBlock.Terrain.MaxDetailLevel + 1; i++ )
            {
                float minDist = CalculateLevelMinDistance( heightMap, i, projection );
                baseBlock.MinDistancesSquared[ i ] = minDist * minDist;
            }
        }

        public float CalculateLevelMinDistance( HeightMapOud heightMap, int level, Matrix projection )
        {
            float error = CalculateLevelError( heightMap, level );

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

        public float CalculateLevelError( HeightMapOud heightMap, int level )
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


            for ( int quadZ = 0; quadZ < BlockSize; quadZ += stepping )
            {
                for ( int quadX = 0; quadX < BlockSize; quadX += stepping )
                {

                    cx = BaseBlock.X + quadX;
                    cz = baseBlock.Z + quadZ;
                    tl = heightMap.GetHeight( cx, cz );
                    tr = heightMap.GetHeight( cx + stepping, cz );
                    bl = heightMap.GetHeight( cx, cz + stepping );
                    br = heightMap.GetHeight( cx + stepping, cz + stepping );


                    for ( int iz = 0; iz <= stepping; iz++ )
                    {
                        for ( int ix = 0; ix <= stepping; ix++ )
                        {
                            //We could skip the corners but the error is 0 on those points anyway
                            float lerpX = MathHelper.Lerp( tl, tr, (float)ix / (float)stepping );
                            float lerpZ = MathHelper.Lerp( bl, br, (float)iz / (float)stepping );
                            float lerp = ( lerpX + lerpZ ) * 0.5f;

                            e = Math.Abs( heightMap.GetHeight( cx + ix, cz + iz ) - lerp );
                            maxError = MathHelper.Max( maxError, e );


                        }
                    }

                }
            }

            return maxError;

        }


        public void DrawViewBatched()
        {
            if ( viewMaterial == null ) return;
            ViewMaterial.BatchBlock( this );
        }


        public int BlockSize
        {
            get { return terrainOud.BaseTerrain.BlockSize; }
        }


        //public new Terrain Terrain
        //{
        //    get { return (Terrain)terrain; }

        //}

        //public new Wereld.QuadTreeNode QuadTreeNode
        //{
        //    get { return (Wereld.QuadTreeNode)base.QuadTreeNode; }
        //    set { base.QuadTreeNode = value; }
        //}

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

        public int X
        { get { return BaseBlock.X; } }

        public int Z
        { get { return BaseBlock.Z; } }

        public bool VertexBufferDirty { get { return vertexBufferDirty; } set { vertexBufferDirty = value; } }

    }
}

