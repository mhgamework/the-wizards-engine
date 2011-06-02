using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.TWClient;
using MHGameWork.TheWizards.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Common.GeoMipMap;
using System.IO;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.ServerClient.TWXNAEngine;
using NovodexWrapper;

namespace MHGameWork.TheWizards.ServerClient.Terrain.Rendering
{
    public class TerrainBlock : ITerrainBlockRenderable
    {

        public IndexBuffer IndexBuffer;

        public int DetailLevel
        {
            get { return detailLevel; }
        }

        public TerrainBlock West
        {
            get { return neighbors[(int)TerrainBlockEdge.West]; }
            set { neighbors[(int)TerrainBlockEdge.West] = value; }
        }
        public TerrainBlock East
        {
            get { return neighbors[(int)TerrainBlockEdge.East]; }
            set { neighbors[(int)TerrainBlockEdge.East] = value; }
        }

        public TerrainBlock North
        {
            get { return neighbors[(int)TerrainBlockEdge.North]; }
            set { neighbors[(int)TerrainBlockEdge.North] = value; }
        }
        public TerrainBlock South
        {
            get { return neighbors[(int)TerrainBlockEdge.South]; }
            set { neighbors[(int)TerrainBlockEdge.South] = value; }
        }

        protected TerrainBlock[] neighbors = new TerrainBlock[4];



        public VertexBuffer VertexBuffer;
        public TerrainMaterial Material;
        public int TotalVertices;

        public bool IsLoaded { get { return VertexBuffer != null && IndexBuffer != null; } }


        public TerrainBlock()
        {

        }

        private float[] minDistancesSquared;

        public float[] MinDistancesSquared
        {
            get { return minDistancesSquared; }
            set { minDistancesSquared = value; }
        }

        /// <summary>
        /// TODO: preprocess?
        /// </summary>
        public Vector3 Center
        {
            get { return center; }
            set { center = value; }
        }
        protected Vector3 center;






        /// <summary>
        /// TODO: Not sure we need the x and z coords.
        /// </summary>
        /// <param name="terrainGeomipmapRenderData"></param>
        /// <param name="_x"></param>
        /// <param name="_z"></param>
        public TerrainBlock(TerrainGeomipmapRenderData terrainGeomipmapRenderData, int _x, int _z)
        {
            this.terrainGeomipmapRenderData = terrainGeomipmapRenderData;
            x = _x;
            z = _z;
            minDistancesSquared = new float[this.terrainGeomipmapRenderData.MaxDetailLevel + 1];
            blockNumX = x / this.terrainGeomipmapRenderData.BlockSize;
            blockNumZ = z / this.terrainGeomipmapRenderData.BlockSize;
        }















        /// <summary>
        /// This creates and writes the data to create this block in preprocessed format.
        /// TODO: this method will probably be faster when just dumping the vertexbuffer data to the disk,
        /// instead of writing the vertices seperately.
        /// </summary>
        /// <param name="BW"></param>
        /// <param name="map"></param>
        /// <param name="projectionMatrix"></param>
        public void WritePreProcessedData(ByteWriter BW, HeightMap map, Matrix projectionMatrix, int materialIndex)
        {
            VertexMultitextured[] vertexes;

            Vector3 min;
            Vector3 max;

            vertexes = GenerateVerticesFromHeightmapSpecialWeightmap(map, out min, out max);


            TotalVertices = vertexes.Length;


            BW.Write(TotalVertices);
            BW.Write(min);
            BW.Write(max);
            for (int i = 0; i < vertexes.Length; i++)
            {
                BW.Write(vertexes[i].Position);
                BW.Write(vertexes[i].Normal);
                BW.Write(vertexes[i].TextureCoordinate);
            }

            throw new NotImplementedException();
            float[] localMinDistancesSquared = null;//TODO: CalculateMinDistances(projectionMatrix, map);

            BW.Write(localMinDistancesSquared.Length);

            for (int i = 0; i < localMinDistancesSquared.Length; i++)
            {
                BW.Write(localMinDistancesSquared[i]);
            }

            BW.Write(materialIndex);

        }

        /// <summary>
        /// Reads the preprocessed Data and loads the vertex buffer
        /// </summary>
        /// <param name="br"></param>
        public void ReadPreProcessedData(ByteReader br, List<TerrainMaterial> materials)
        {
            VertexMultitextured[] vertexes;

            Vector3 min;
            Vector3 max;


            TotalVertices = br.ReadInt32();
            vertexes = new VertexMultitextured[TotalVertices];
            tempVertices = vertexes;
            min = br.ReadVector3();
            max = br.ReadVector3();

            for (int i = 0; i < vertexes.Length; i++)
            {
                vertexes[i] = new VertexMultitextured();
                vertexes[i].Position = br.ReadVector3();
                vertexes[i].Normal = br.ReadVector3();
                vertexes[i].TextureCoordinate = br.ReadVector2();
            }

            //Debug verification
            //if ( vertexes[ 0 ].Position.X != x || vertexes[ 0 ].Position.Z != z ) throw new Exception();


            SetMinMax(min, max);



            if (VertexBuffer != null)
                VertexBuffer.Dispose();

            VertexBuffer = new VertexBuffer(TerrainGeomipmapRenderData.XNAGame.GraphicsDevice, typeof(VertexPositionNormalTexture),
                                             vertexes.Length, BufferUsage.None);
            VertexBuffer.SetData<VertexMultitextured>(vertexes);

            vertexes = null;

            //vertexBufferDirty = false;


            float[] localMinDistancesSquared = new float[br.ReadInt32()];

            for (int i = 0; i < localMinDistancesSquared.Length; i++)
            {
                localMinDistancesSquared[i] = br.ReadSingle();
            }

            MinDistancesSquared = localMinDistancesSquared;

            int materialIndex = br.ReadInt32();
            Material = materials[materialIndex];

            Center = min + (max - min) * 0.5f;

        }



        /// <summary>
        /// TODO: maybe in the wrong place
        /// </summary>
        public void Unload()
        {
            if (VertexBuffer != null)
                VertexBuffer.Dispose();

            if (IndexBuffer != null)
                IndexBuffer.Dispose();


            VertexBuffer = null;
            IndexBuffer = null;

        }






        private int blockNumX;
        private int blockNumZ;





        /// <summary>
        /// Generates vertices with the texturecoordinates set for the special weightmapformat, where every block has
        /// blocksize+1 texels (so no texels are shared between blocks) (the weightmap is thus larger than the heightmap)
        /// </summary>
        /// <param name="map"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public VertexMultitextured[] GenerateVerticesFromHeightmapSpecialWeightmap(HeightMap map, out Vector3 min, out Vector3 max)
        {
            VertexMultitextured[] vertexes = new VertexMultitextured[(terrainGeomipmapRenderData.BlockSize + 1) * (terrainGeomipmapRenderData.BlockSize + 1)];

            min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            //if ( blockNumX == 15 ) throw new Exception();

            // build vectors and normals
            for (int ix = 0; ix <= terrainGeomipmapRenderData.BlockSize; ix++)
            {
                //if ( ix == terrain.BlockSize ) throw new Exception();
                for (int iz = 0; iz <= terrainGeomipmapRenderData.BlockSize; iz++)
                {
                    int cx = this.x + ix;
                    int cz = this.z + iz;
                    //cx = ix;
                    //cz =  iz;

                    VertexMultitextured vert = new VertexMultitextured();

                    //vert.Position = terrain.GetLocalPosition( new Vector3( cx, map[ cx, cz ], cz ) );
                    vert.Position = new Vector3(cx, map[cx, cz], cz);

                    //if ( (BlockNumX * ( terrain.BlockSize + 1 ) + ix)  == 17 && (BlockNumZ * ( terrain.BlockSize + 1 ) + iz ) == 51) System.Diagnostics.Debugger.Break();

                    //Special texcoord for weightmaps
                    // EDIT: does not work
                    /*vert.TextureCoordinate = new Vector2(
                        (float)( BlockNumX * ( terrain.BlockSize + 1 ) + ix ) / ( (float)terrain.NumBlocksX * ( terrain.BlockSize + 1 ) - 1 ),
                        (float)( BlockNumZ * ( terrain.BlockSize + 1 ) + iz ) / ( (float)terrain.NumBlocksZ * ( terrain.BlockSize + 1 ) - 1 ) );*/

                    // Weightmap texcoord: every vertex is positioned in the center of a texel containing the weights for that texture
                    vert.TextureCoordinate = new Vector2(
                        (float)(BlockNumX * (terrainGeomipmapRenderData.BlockSize + 1) + ix + 0.5f) / ((float)terrainGeomipmapRenderData.NumBlocksX * (terrainGeomipmapRenderData.BlockSize + 1)),
                        (float)(BlockNumZ * (terrainGeomipmapRenderData.BlockSize + 1) + iz + 0.5f) / ((float)terrainGeomipmapRenderData.NumBlocksZ * (terrainGeomipmapRenderData.BlockSize + 1)));

                    /*vert.TextureCoordinate = new Vector2(
                       (float)( ix ) / ( (float)terrain.BlockSize ),
                       (float)( iz ) / ( (float)terrain.BlockSize ) );

                    vert.TextureCoordinate = new Vector2(
                        (float)( ix+0.5f ) / ( (float)terrain.NumBlocksX * ( terrain.BlockSize + 1 ) ),
                        (float)( iz +0.5f) / ( (float)terrain.NumBlocksZ * ( terrain.BlockSize + 1 ) ) );*/

                    throw new NotImplementedException();
                    //TODO: vert.Normal = CalculateAveragedNormal(map, cx, cz);// terrain.GetAveragedNormal( cx, cz );

                    min = Vector3.Min(min, vert.Position);
                    max = Vector3.Max(max, vert.Position);

                    //TODO: vertexes[IndexFromCoords(ix, iz)] = vert;
                }

            }


            return vertexes;

        }
























































        protected NxActor actor;

        //private TerrainMaterial material;

        //public TerrainMaterial Material
        //{
        //    get { return material; }
        //    set { material = value; }
        //}



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







        //public void AddLoadTasks( Engine.LoadingManager loadingManager, Engine.LoadingTaskType taskType )
        //{
        //    //if ( taskPreProcess == null )
        //    //{
        //    //    taskPreProcess = new Engine.LoadingTaskAdvanced( PreProcessTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.PreProccesing );
        //    //    taskNormal = new Engine.LoadingTaskAdvanced( LoadNormalTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal );
        //    //}

        //    switch ( taskType )
        //    {
        //        case MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.PreProccesing:
        //            break;
        //        case MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal:
        //            if ( tempNormalLoaded == false )
        //            {
        //                tempNormalLoaded = true;
        //                Terrain.Engine.LoadingManager.AddLoadTaskAdvanced( LoadNormalTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal );
        //            }
        //            break;
        //        case MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Detail:
        //            break;
        //        case MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Unload:
        //            if ( tempNormalLoaded == true )
        //            {
        //                tempNormalLoaded = false;
        //                Terrain.Engine.LoadingManager.AddLoadTaskAdvanced( UnloadTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Unload );
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //}




        //public virtual void LoadGraphicsContent( GraphicsDevice device, bool loadAllContent )
        //{
        //    //BuildVertexBufferFromFile( device );
        //    BuildVertexBufferFromHeightmap();
        //    SetAndCalculateMinDistances( Terrain.Engine.ActiveCamera.CameraInfo.ProjectionMatrix );
        //    BuildIndexBuffer( device );
        //}


        //public void BuildVertexBufferFromHeightmap()
        //{
        //    if ( vertexBufferOud != null )
        //        vertexBufferOud.Dispose();

        //    VertexMultitextured[] vertexes;

        //    Vector3 min;
        //    Vector3 max;

        //    vertexes = GenerateVerticesFromHeightmap( out min, out max );

        //    center = ( min + max ) * 0.5f;
        //    LocalBoundingBox = new BoundingBox( min, max );

        //    totalVerticesOud = vertexes.Length;

        //    if ( vertexBufferOud != null )
        //        vertexBufferOud.Dispose();

        //    vertexBufferOud = new VertexBuffer( Terrain.Device, typeof( VertexPositionNormalTexture ),
        //        vertexes.Length, BufferUsage.None );
        //    vertexBufferOud.SetData<VertexMultitextured>( vertexes );

        //    totalVerticesOud = vertexes.Length;
        //    vertexes = null;

        //    //vertexBufferDirty = false;
        //}



        protected VertexMultitextured[] GenerateVerticesFromHeightmap(HeightMap map, out Vector3 min, out Vector3 max)
        {
            VertexMultitextured[] vertexes = new VertexMultitextured[(terrainGeomipmapRenderData.BlockSize + 1) * (terrainGeomipmapRenderData.BlockSize + 1)];

            min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            max = new Vector3(float.MinValue, float.MinValue, float.MinValue);



            // build vectors and normals
            for (int ix = 0; ix <= terrainGeomipmapRenderData.BlockSize; ix++)
            {
                for (int iz = 0; iz <= terrainGeomipmapRenderData.BlockSize; iz++)
                {
                    int cx = this.x + ix;
                    int cz = this.z + iz;

                    VertexMultitextured vert = new VertexMultitextured();

                    //vert.Position = terrain.GetLocalPosition( new Vector3( cx, map[ cx, cz ], cz ) );
                    vert.Position = new Vector3(cx, map[cx, cz], cz);


                    //TODO: klopt deze texcoord wel?
                    vert.TextureCoordinate = new Vector2((float)cx / (float)terrainGeomipmapRenderData.SizeX, (float)cz / (float)terrainGeomipmapRenderData.SizeY);

                    throw new NotImplementedException();
                    //TODO: vert.Normal = CalculateAveragedNormal(map, cx, cz);// terrain.GetAveragedNormal( cx, cz );

                    min = Vector3.Min(min, vert.Position);
                    max = Vector3.Max(max, vert.Position);

                    //TODO: vertexes[IndexFromCoords(ix, iz)] = vert;
                }
            }


            return vertexes;

        }
        //protected VertexMultitextured[] GenerateVerticesFromHeightmap( out Vector3 min, out Vector3 max )
        //{
        //    return GenerateVerticesFromHeightmap( terrain.HeightMap, out min, out max );
        //}

        //protected VertexMultitextured[] GenerateVerticesFromHeightmap()
        //{
        //    Vector3 min;
        //    Vector3 max;
        //    return GenerateVerticesFromHeightmap( out min, out max );
        //}





        protected void Dispose(bool disposing)
        {
            lock (this)
            {

                West = null;
                East = null;
                North = null;
                South = null;
            }

            //base.Dispose( disposing );
        }







        public void OnBoundingChanged()
        {
            //QuadTreeNode.OnTerreinBlockBoundingChanged();
        }



        public void GenerateAutoWeights()
        {


            Color[] data = new Color[17 * 17];
            float A;
            float R;
            float G;
            float B;


            float ox = terrainGeomipmapRenderData.HeightMap.Width * 0.5f;
            float oz = terrainGeomipmapRenderData.HeightMap.Length * 0.5f;


            for (int tx = 0; tx < terrainGeomipmapRenderData.BlockSize + 1; tx++)
            {
                for (int tz = 0; tz < terrainGeomipmapRenderData.BlockSize + 1; tz++)
                {
                    int cx = tx + x;
                    int cz = tz + z;

                    G = CalculateWeight(terrainGeomipmapRenderData.HeightMap.GetHeight(cx, cz) * terrainGeomipmapRenderData.HeightScale, 0, 75);
                    R = CalculateWeight(terrainGeomipmapRenderData.HeightMap.GetHeight(cx, cz) * terrainGeomipmapRenderData.HeightScale, 50, 250);
                    B = CalculateWeight(terrainGeomipmapRenderData.HeightMap.GetHeight(cx, cz) * terrainGeomipmapRenderData.HeightScale, 175, 400);
                    A = CalculateWeight(terrainGeomipmapRenderData.HeightMap.GetHeight(cx, cz) * terrainGeomipmapRenderData.HeightScale, 300, 1000);

                    float totalWeight = R;
                    totalWeight += G;
                    totalWeight += B;
                    totalWeight += A;
                    R = R / totalWeight * 255;
                    G = G / totalWeight * 255;
                    B = B / totalWeight * 255;
                    A = A / totalWeight * 255;

                    R = (float)Math.Floor(R);
                    G = (float)Math.Floor(G);
                    B = (float)Math.Floor(B);
                    A = (float)Math.Floor(A);

                    A = 255 - R - G - B;

                    if (totalWeight == 0) { R = G = B = 0; A = 255; }

                    //TODO: data[IndexFromCoords(tx, tz)] = new Color((byte)R, (byte)G, (byte)B, (byte)A);
                }
            }






            TerrainGeomipmapRenderData.WeightMap.SetData<Color>(0, new Rectangle(x, z, terrainGeomipmapRenderData.BlockSize + 1, terrainGeomipmapRenderData.BlockSize + 1), data, 0, data.Length, SetDataOptions.None);

        }

        //protected void BuildVertexBufferFromFile( GraphicsDevice device )
        //{
        //    if ( vertexBufferOud != null )
        //        vertexBufferOud.Dispose();








        //    System.IO.FileStream fs = new System.IO.FileStream( terrain.Filename, System.IO.FileMode.Open, System.IO.FileAccess.Read );



        //    fs.Position = filePointer.Pos;

        //    MHGameWork.TheWizards.Common.ByteReader br = new MHGameWork.TheWizards.Common.ByteReader( fs );

        //    VertexMultitextured[] vertexes = new VertexMultitextured[ ( terrain.BlockSize + 1 ) * ( terrain.BlockSize + 1 ) ];




        //    Vector3 min;
        //    Vector3 max;



        //    totalVerticesOud = br.ReadInt32();
        //    min = br.ReadVector3();
        //    max = br.ReadVector3();



        //    for ( int i = 0; i < totalVerticesOud; i++ )
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




        //    if ( vertexBufferOud != null )
        //        vertexBufferOud.Dispose();

        //    vertexBufferOud = new VertexBuffer( device, typeof( VertexMultitextured ),
        //        vertexes.Length, BufferUsage.None );
        //    vertexBufferOud.SetData<VertexMultitextured>( vertexes );

        //    vertexes = null;





        //}

        public float CalculateWeight(float height, float min, float max)
        {
            //oorspronkelijke formule
            // 1 - Math.Abs( ( height - min - ( ( max - min ) / 2 ) ) / ( ( max - min ) / 2 ) )
            float test = MathHelper.Clamp(1 - Math.Abs((height - min - ((max - min) / 2)) / ((max - min) / 2)), 0, 1);

            //uitgerekende formule

            float result = MathHelper.Clamp(1 - Math.Abs((2 * height - min - max) / (max - min)), 0, 1);

            if (test != result) throw new Exception();
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


        //public byte[] ToBytes()
        //{
        //    MHGameWork.TheWizards.Common.ByteWriter BW = new MHGameWork.TheWizards.Common.ByteWriter();

        //    VertexMultitextured[] vertexes;

        //    Vector3 min;
        //    Vector3 max;

        //    vertexes = GenerateVerticesFromHeightmap( out min, out max );


        //    totalVerticesOud = vertexes.Length;


        //    BW.Write( totalVerticesOud );
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



        public void SetMinMax(Vector3 min, Vector3 max)
        {
            center = (min + max) * 0.5f;
            LocalBoundingBox = new BoundingBox(min, max);
        }

        VertexMultitextured[] tempVertices;












        #region ITerrainBlock Members

        private Wereld.QuadTreeNode quadtreeNode;

        public void AssignToQuadtreeNode(MHGameWork.TheWizards.Common.Wereld.IQuadtreeNode node)
        {
            if (node is Wereld.QuadTreeNode)
            {
                throw new ArgumentException("Only nodes that are ServerClient.Wereld.QuadTreeNode can be added to this blocks");
            }

            quadtreeNode = (Wereld.QuadTreeNode)node;

            throw new Exception("The method or operation is not implemented.");
        }

        public MHGameWork.TheWizards.Common.Wereld.IQuadtreeNode GetIQuadtreeNode()
        {
            return (MHGameWork.TheWizards.Common.Wereld.IQuadtreeNode)quadtreeNode;

        }

        #endregion

        #region ITerrainBlock Members


        public ITerrainBlock GetNeighbour(TerrainBlockEdge edge)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetNeighbour(TerrainBlockEdge edge, ITerrainBlock neighbour)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region ITerrainBlockRenderable Members

        public int DrawPrimitives()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RenderBatched()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion



        protected TerrainGeomipmapRenderData terrainGeomipmapRenderData;

        protected int detailLevel = 0;
        protected int x = -1;
        protected int z = -1;
        protected FilePointer filePointer;



        //New version



        protected Wereld.QuadTreeNode quadTreeNode;
        protected BoundingBox localBoundingBox = new BoundingBox();









        //public void SetAndCalculateMinDistances( Matrix projection )
        //{
        //    minDistancesSquared = CalculateMinDistances( projection, terrain.HeightMap );
        //    //for ( int i = 0; i < terrain.MaxDetailLevel + 1; i++ )
        //    //{
        //    //    float minDist = CalculateLevelMinDistance( i, projection );
        //    //    minDistancesSquared[ i ] = minDist * minDist;
        //    //}
        //}









        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }















        public FilePointer FilePointer
        { get { return filePointer; } set { filePointer = value; } }

        public TerrainGeomipmapRenderData TerrainGeomipmapRenderData
        {
            get { return terrainGeomipmapRenderData; }

        }


        public Wereld.QuadTreeNode QuadTreeNode
        { get { return quadTreeNode; } set { quadTreeNode = value; } }

        public BoundingBox LocalBoundingBox
        {
            get { return localBoundingBox; }
            set
            {
                localBoundingBox = value;
                if (localBoundingBox.Min.Y == localBoundingBox.Max.Y)
                {
                    //To mimic the very thin but existing terrain
                    localBoundingBox.Max += new Vector3(0, 0.0001f, 0);
                }
                OnBoundingChanged();
            }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                BoundingBox bb;

                bb = new BoundingBox(terrainGeomipmapRenderData.GetWorldPosition(localBoundingBox.Min),
                                      terrainGeomipmapRenderData.GetWorldPosition(localBoundingBox.Max));

                return bb;
            }

        }


    }
}