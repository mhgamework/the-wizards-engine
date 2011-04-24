﻿using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.TWClient;
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
        public int BlockSize { get; set; }

        public ushort IndexFromCoords(int x, int z)
        {
            return (ushort)(z * (BlockSize + 1) + x);
        }

        public IndexBuffer IndexBuffer;

        public int DetailLevel
        {
            get { return detailLevel; }
        }


        protected void BuildBaseIndexes(List<ushort> indexes)
        {
            totalBaseTriangles = 0;

            int stepping = 1 << DetailLevel;
            int total = BlockSize;

            // build indexes for inner polygons
            for (int ix = stepping; ix < total - stepping; ix += stepping)
            {
                for (int iz = stepping; iz < total - stepping; iz += stepping)
                {
                    // triangle 1
                    indexes.Add(IndexFromCoords(ix, iz));
                    indexes.Add(IndexFromCoords(ix + stepping, iz));
                    indexes.Add(IndexFromCoords(ix, iz + stepping));
                    totalBaseTriangles++;

                    // triangle 2
                    indexes.Add(IndexFromCoords(ix + stepping, iz));
                    indexes.Add(IndexFromCoords(ix + stepping, iz + stepping));
                    indexes.Add(IndexFromCoords(ix, iz + stepping));
                    totalBaseTriangles++;
                }
            }

            totalEdgeOffset = indexes.Count;
        }
        protected void BuildEdgeIndexes(List<ushort> indexes)
        {
            int startCount = indexes.Count;

            totalEdgeTriangles = 0;
            totalEdgeTriangles += BuildEdgeIndexes(indexes, TerrainBlockEdge.West);
            totalEdgeTriangles += BuildEdgeIndexes(indexes, TerrainBlockEdge.East);
            totalEdgeTriangles += BuildEdgeIndexes(indexes, TerrainBlockEdge.North);
            totalEdgeTriangles += BuildEdgeIndexes(indexes, TerrainBlockEdge.South);
        }
        protected int BuildEdgeIndexes(List<ushort> indexes, TerrainBlockEdge edge)
        {
            TerrainBlock neighbor = neighbors[(int)edge];

            if (neighbor == null || neighbor.DetailLevel <= DetailLevel)
                return BuildBasicEdgeIndexes(indexes, edge);
            else
                return BuildStitchedEdgeIndexes(indexes, edge);
        }
        protected int BuildBasicEdgeIndexes(List<ushort> indexes, TerrainBlockEdge edge)
        {
            int triangles = 0;
            int stepping = 1 << DetailLevel;
            int total = BlockSize;

            int startX = 0;
            int endX = 0;
            int startZ = 0;
            int endZ = 0;

            switch (edge)
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

            for (int ix = startX; ix <= endX; ix += stepping)
            {
                for (int iz = startZ; iz <= endZ; iz += stepping)
                {
                    if (edge == TerrainBlockEdge.West)
                    {
                        if (iz == startZ)
                        {
                            indexes.Add(IndexFromCoords(ix, iz));
                            indexes.Add(IndexFromCoords(ix + stepping, iz + stepping));
                            indexes.Add(IndexFromCoords(ix, iz + stepping));
                            triangles++;

                            continue;
                        }

                        if (iz == endZ)
                        {
                            indexes.Add(IndexFromCoords(ix, iz));
                            indexes.Add(IndexFromCoords(ix + stepping, iz));
                            indexes.Add(IndexFromCoords(ix, iz + stepping));
                            triangles++;

                            continue;
                        }
                    }
                    else if (edge == TerrainBlockEdge.East)
                    {
                        if (iz == startZ)
                        {
                            indexes.Add(IndexFromCoords(ix + stepping, iz));
                            indexes.Add(IndexFromCoords(ix + stepping, iz + stepping));
                            indexes.Add(IndexFromCoords(ix, iz + stepping));
                            triangles++;

                            continue;
                        }

                        if (iz == endZ)
                        {
                            indexes.Add(IndexFromCoords(ix, iz));
                            indexes.Add(IndexFromCoords(ix + stepping, iz));
                            indexes.Add(IndexFromCoords(ix + stepping, iz + stepping));
                            triangles++;

                            continue;
                        }
                    }
                    else if (edge == TerrainBlockEdge.North)
                    {
                        if (ix == startX)
                        {
                            indexes.Add(IndexFromCoords(ix, iz));
                            indexes.Add(IndexFromCoords(ix + stepping, iz));
                            indexes.Add(IndexFromCoords(ix + stepping, iz + stepping));
                            triangles++;

                            continue;
                        }

                        if (ix == endX)
                        {
                            indexes.Add(IndexFromCoords(ix, iz));
                            indexes.Add(IndexFromCoords(ix + stepping, iz));
                            indexes.Add(IndexFromCoords(ix, iz + stepping));
                            triangles++;

                            continue;
                        }
                    }

                    else if (edge == TerrainBlockEdge.South)
                    {
                        if (ix == startX)
                        {
                            indexes.Add(IndexFromCoords(ix + stepping, iz));
                            indexes.Add(IndexFromCoords(ix + stepping, iz + stepping));
                            indexes.Add(IndexFromCoords(ix, iz + stepping));
                            triangles++;

                            continue;
                        }

                        if (ix == endX)
                        {
                            indexes.Add(IndexFromCoords(ix, iz));
                            indexes.Add(IndexFromCoords(ix + stepping, iz + stepping));
                            indexes.Add(IndexFromCoords(ix, iz + stepping));
                            triangles++;

                            continue;
                        }
                    }

                    indexes.Add(IndexFromCoords(ix, iz));
                    indexes.Add(IndexFromCoords(ix + stepping, iz));
                    indexes.Add(IndexFromCoords(ix, iz + stepping));
                    triangles++;

                    indexes.Add(IndexFromCoords(ix + stepping, iz));
                    indexes.Add(IndexFromCoords(ix + stepping, iz + stepping));
                    indexes.Add(IndexFromCoords(ix, iz + stepping));
                    triangles++;
                }
            }

            return triangles;
        }
        protected int BuildStitchedEdgeIndexes(List<ushort> indexes, TerrainBlockEdge edge)
        {
            TerrainBlock neighbor = neighbors[(int)edge];
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

            switch (edge)
            {
                case TerrainBlockEdge.West:
                    startPos = terrainGeomipmapRenderData.BlockSize;
                    insideStep = sourceStep;
                    sourceStep = -sourceStep;
                    destStep = -destStep;
                    destHalfStep = -destHalfStep;
                    break;

                case TerrainBlockEdge.East:
                    endPos = terrainGeomipmapRenderData.BlockSize;
                    insidePos = terrainGeomipmapRenderData.BlockSize;
                    insideStep = -sourceStep;
                    break;

                case TerrainBlockEdge.North:
                    endPos = terrainGeomipmapRenderData.BlockSize;
                    insideStep = sourceStep;
                    horizontal = true;
                    break;

                case TerrainBlockEdge.South:
                    startPos = terrainGeomipmapRenderData.BlockSize;
                    insidePos = terrainGeomipmapRenderData.BlockSize;
                    insideStep = -sourceStep;
                    sourceStep = -sourceStep;
                    destStep = -destStep;
                    destHalfStep = -destHalfStep;
                    horizontal = true;
                    break;
            };

            for (int pos1 = startPos; pos1 != endPos; pos1 += destStep)
            {
                for (int pos2 = 0; pos2 != destHalfStep; pos2 += sourceStep)
                {
                    if (pos1 != startPos || pos2 != 0)
                    {
                        if (horizontal)
                        {
                            indexes.Add(IndexFromCoords(pos1, insidePos));
                            indexes.Add(IndexFromCoords(pos1 + pos2 + sourceStep, insidePos + insideStep));
                            indexes.Add(IndexFromCoords(pos1 + pos2, insidePos + insideStep));
                            triangles++;
                        }
                        else
                        {
                            indexes.Add(IndexFromCoords(insidePos, pos1));
                            indexes.Add(IndexFromCoords(insidePos + insideStep, pos1 + pos2 + sourceStep));
                            indexes.Add(IndexFromCoords(insidePos + insideStep, pos1 + pos2));
                            triangles++;
                        }
                    }
                }

                if (horizontal)
                {
                    indexes.Add(IndexFromCoords(pos1, insidePos));
                    indexes.Add(IndexFromCoords(pos1 + destStep, insidePos));
                    indexes.Add(IndexFromCoords(pos1 + destHalfStep, insidePos + insideStep));
                    triangles++;
                }
                else
                {
                    indexes.Add(IndexFromCoords(insidePos, pos1));
                    indexes.Add(IndexFromCoords(insidePos, pos1 + destStep));
                    indexes.Add(IndexFromCoords(insidePos + insideStep, pos1 + destHalfStep));
                    triangles++;
                }

                for (int pos2 = destHalfStep; pos2 != destStep; pos2 += sourceStep)
                {
                    if (pos1 != endPos - destStep || pos2 != destStep - sourceStep)
                    {
                        if (horizontal)
                        {
                            indexes.Add(IndexFromCoords(pos1 + destStep, insidePos));
                            indexes.Add(IndexFromCoords(pos1 + pos2 + sourceStep, insidePos + insideStep));
                            indexes.Add(IndexFromCoords(pos1 + pos2, insidePos + insideStep));
                            triangles++;
                        }
                        else
                        {
                            indexes.Add(IndexFromCoords(insidePos, pos1 + destStep));
                            indexes.Add(IndexFromCoords(insidePos + insideStep, pos1 + pos2 + sourceStep));
                            indexes.Add(IndexFromCoords(insidePos + insideStep, pos1 + pos2));
                            triangles++;
                        }
                    }
                }
            }

            return triangles;
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



        public void BuildIndexBuffer(GraphicsDevice device)
        {
            if (IndexBuffer != null)
                IndexBuffer.Dispose();

            // allocate index buffer for maximum amount of room needed
            IndexBuffer = new IndexBuffer(device, typeof(ushort),
                                           (BlockSize + 1) * (BlockSize + 1) * 6, BufferUsage.None);

            List<ushort> indexes = new List<ushort>();

            BuildBaseIndexes(indexes);
            BuildEdgeIndexes(indexes);

            if (indexes.Count > 0)
                IndexBuffer.SetData<ushort>(indexes.ToArray());
        }


        public void BuildBaseIndexes()
        {
            List<ushort> indexes = new List<ushort>();

            BuildBaseIndexes(indexes);

            if (indexes.Count > 0)
                IndexBuffer.SetData<ushort>(indexes.ToArray());
        }
        public void BuildEdgeIndexes()
        {
            if (IndexBuffer == null) return;
            List<ushort> indexes = new List<ushort>();

            BuildEdgeIndexes(indexes);

            if (indexes.Count > 0)
            {
                IndexBuffer.SetData<ushort>(totalEdgeOffset * sizeof(ushort),
                                             indexes.ToArray(), 0, indexes.Count);

            }
        }




        public void ChangeDetailLevel(int mipLevel, bool force)
        {
            if (mipLevel > MaxDetailLevel)
                mipLevel = MaxDetailLevel;

            if (mipLevel == detailLevel && force != true)
                return;

            detailLevel = mipLevel;

            BuildBaseIndexes();
            BuildEdgeIndexes();

            if (West != null)
                West.BuildEdgeIndexes();

            if (East != null)
                East.BuildEdgeIndexes();

            if (North != null)
                North.BuildEdgeIndexes();

            if (South != null)
                South.BuildEdgeIndexes();
        }

        public int MaxDetailLevel
        {
            get { return (BlockSize >> 2) - 1; }
        }


        public VertexBuffer VertexBuffer;
        public TerrainMaterial Material;
        public int TotalVertices;

        public bool IsLoaded { get { return VertexBuffer != null && IndexBuffer != null; } }



        public TerrainBlock()
        {

        }


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
        public void WritePreProcessedData(ByteWriter BW, TerrainHeightMap map, Matrix projectionMatrix, int materialIndex)
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

            float[] localMinDistancesSquared = CalculateMinDistances(projectionMatrix, map);

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
        public VertexMultitextured[] GenerateVerticesFromHeightmapSpecialWeightmap(TerrainHeightMap map, out Vector3 min, out Vector3 max)
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

                    vert.Normal = CalculateAveragedNormal(map, cx, cz);// terrain.GetAveragedNormal( cx, cz );

                    min = Vector3.Min(min, vert.Position);
                    max = Vector3.Max(max, vert.Position);

                    vertexes[IndexFromCoords(ix, iz)] = vert;
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



        protected VertexMultitextured[] GenerateVerticesFromHeightmap(TerrainHeightMap map, out Vector3 min, out Vector3 max)
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

                    vert.Normal = CalculateAveragedNormal(map, cx, cz);// terrain.GetAveragedNormal( cx, cz );

                    min = Vector3.Min(min, vert.Position);
                    max = Vector3.Max(max, vert.Position);

                    vertexes[IndexFromCoords(ix, iz)] = vert;
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


        public static Vector3 CalculateAveragedNormal(TerrainHeightMap map, int x, int z)
        {
            Vector3 normal = new Vector3();

            // top left
            if (x > 0 && z > 0)
                normal += CalculateNormal(map, x - 1, z - 1);

            // top center
            if (z > 0)
                normal += CalculateNormal(map, x, z - 1);

            // top right
            if (x < map.Width && z > 0)
                normal += CalculateNormal(map, x + 1, z - 1);

            // middle left
            if (x > 0)
                normal += CalculateNormal(map, x - 1, z);

            // middle center
            normal += CalculateNormal(map, x, z);

            // middle right
            if (x < map.Width)
                normal += CalculateNormal(map, x + 1, z);

            // lower left
            if (x > 0 && z < map.Length)
                normal += CalculateNormal(map, x - 1, z + 1);

            // lower center
            if (z < map.Length)
                normal += CalculateNormal(map, x, z + 1);

            // lower right
            if (x < map.Width && z < map.Length)
                normal += CalculateNormal(map, x + 1, z + 1);

            return Vector3.Normalize(normal);
        }

        public static Vector3 CalculateNormal(TerrainHeightMap map, int x, int z)
        {
            float scale = 1;
            float heightScale = 1;
            Vector3 v1 = new Vector3(x * scale, map[x, z + 1] * heightScale, (z + 1) * scale);
            Vector3 v2 = new Vector3(x * scale, map[x, z - 1] * heightScale, (z - 1) * scale);
            Vector3 v3 = new Vector3((x + 1) * scale, map[x + 1, z] * heightScale, z * scale);
            Vector3 v4 = new Vector3((x - 1) * scale, map[x - 1, z] * heightScale, z * scale);

            return Vector3.Normalize(Vector3.Cross(v1 - v2, v3 - v4));
        }



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


        public void GenerateLightmap()
        {
            //TerrainLightmapGenerator gen = new TerrainLightmapGenerator( terrain );

            //GenerateLightmap( gen );

        }

        public void GenerateLightmap(TerrainLightmapGenerator gen)
        {
            byte[] data = gen.Generate(x, z);
            TerrainGeomipmapRenderData.LightMap.SetData<byte>(0, new Rectangle(x, z, terrainGeomipmapRenderData.BlockSize + 1, terrainGeomipmapRenderData.BlockSize + 1), data, 0, data.Length, SetDataOptions.None);

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

                    data[IndexFromCoords(tx, tz)] = new Color((byte)R, (byte)G, (byte)B, (byte)A);
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









        public void LoadHeightField()
        {
            UnloadHeightField();
            actor = CreateHeightField();
        }

        public void UnloadHeightField()
        {
            if (actor != null) actor.destroy();
            actor = null;

        }

        public NxActor CreateHeightField()
        {
            NxHeightFieldDesc heightFieldDesc = NxHeightFieldDesc.Default;
            heightFieldDesc.nbColumns = (uint)terrainGeomipmapRenderData.BlockSize + 1;
            heightFieldDesc.nbRows = (uint)terrainGeomipmapRenderData.BlockSize + 1;
            heightFieldDesc.verticalExtent = -1000;
            heightFieldDesc.convexEdgeThreshold = 0;
            heightFieldDesc.sampleStride = 3 * 4;
            heightFieldDesc.setSampleDimensions(terrainGeomipmapRenderData.BlockSize + 1, terrainGeomipmapRenderData.BlockSize + 1);

            float ox = terrainGeomipmapRenderData.HeightMap.Width * 0.5f;
            float oz = terrainGeomipmapRenderData.HeightMap.Length * 0.5f;

            for (int ix = 0; ix < terrainGeomipmapRenderData.BlockSize + 1; ix++)
            {
                for (int iz = 0; iz < terrainGeomipmapRenderData.BlockSize + 1; iz++)
                {
                    int cx = this.x + ix;
                    int cz = this.z + iz;
                    bool flag = (ix % 2) == 0;
                    if (iz % 2 != 0) flag = !flag;
                    flag = false;
                    heightFieldDesc.setSample(ix, iz, new NxHeightFieldSample((short)(terrainGeomipmapRenderData.HeightMap[cx, cz] * 5000), 0, flag, 0));
                }
            }
            NxHeightField heightField = null;//ServerClientMainOud.instance.ServerMain.PhysicsSDK.createHeightField( heightFieldDesc );

            heightFieldDesc = null;

            NxHeightFieldShapeDesc shapeDesc = new NxHeightFieldShapeDesc();

            shapeDesc.HeightField = heightField;
            shapeDesc.heightScale = terrainGeomipmapRenderData.HeightScale / 5000;
            shapeDesc.rowScale = terrainGeomipmapRenderData.Scale;
            shapeDesc.columnScale = terrainGeomipmapRenderData.Scale;
            shapeDesc.materialIndexHighBits = 0;
            shapeDesc.holeMaterial = 2;

            NxActorDesc actorDesc = new NxActorDesc();
            actorDesc.addShapeDesc(shapeDesc);
            actorDesc.globalPose = Matrix.CreateTranslation(new Vector3((this.x - ox) * terrainGeomipmapRenderData.Scale, 0, (this.z - oz) * terrainGeomipmapRenderData.Scale));
            return null;//ServerClientMainOud.instance.ServerMain.PhysicsScene.createActor( actorDesc );

        }


        //public new Terrain Terrain
        //{
        //    get { return (Terrain)terrain; }

        //}
        //public new TerrainBlock West
        //{
        //    get { return (TerrainBlock)base.West; }
        //    set { base.West = value; }
        //}

        //public new TerrainBlock East
        //{
        //    get { return (TerrainBlock)base.East; }
        //    set { base.East = value; }
        //}

        //public new TerrainBlock North
        //{
        //    get { return (TerrainBlock)base.North; }
        //    set { base.North = value; }
        //}

        //public new TerrainBlock South
        //{
        //    get { return (TerrainBlock)base.South; }
        //    set { base.South = value; }
        //}

        //public new Wereld.QuadTreeNode QuadTreeNode
        //{
        //    get { return (Wereld.QuadTreeNode)base.QuadTreeNode; }
        //    set { base.QuadTreeNode = value; }
        //}

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

        protected int totalEdgeOffset = 0;
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

        public float[] CalculateMinDistances(Matrix projection, TerrainHeightMap map)
        {
            float[] localMinDistancesSquared = new float[terrainGeomipmapRenderData.MaxDetailLevel + 1];

            for (int i = 0; i < terrainGeomipmapRenderData.MaxDetailLevel + 1; i++)
            {
                float minDist = CalculateLevelMinDistance(i, projection, map);
                localMinDistancesSquared[i] = minDist * minDist;
            }

            return localMinDistancesSquared;

        }

        public float CalculateLevelMinDistance(int level, Matrix projection, TerrainHeightMap map)
        {
            float error = CalculateLevelError(level, map);

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
            float A = n / Math.Abs(t);

            float T = (2 * threshold) / verticalResolution;

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

        public float CalculateLevelError(int level, TerrainHeightMap map)
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


            for (int quadZ = 0; quadZ < terrainGeomipmapRenderData.BlockSize; quadZ += stepping)
            {
                for (int quadX = 0; quadX < terrainGeomipmapRenderData.BlockSize; quadX += stepping)
                {

                    cx = x + quadX;
                    cz = z + quadZ;
                    tl = map.GetHeight(cx, cz);
                    tr = map.GetHeight(cx + stepping, cz);
                    bl = map.GetHeight(cx, cz + stepping);
                    br = map.GetHeight(cx + stepping, cz + stepping);


                    for (int iz = 0; iz <= stepping; iz++)
                    {
                        for (int ix = 0; ix <= stepping; ix++)
                        {
                            //We could skip the corners but the error is 0 on those points anyway
                            float lerpX = MathHelper.Lerp(tl, tr, (float)ix / (float)stepping);
                            float lerpZ = MathHelper.Lerp(bl, br, (float)iz / (float)stepping);
                            float lerp = (lerpX + lerpZ) * 0.5f;

                            e = Math.Abs(map.GetHeight(cx + ix, cz + iz) - lerp);
                            maxError = MathHelper.Max(maxError, e);


                        }
                    }

                }
            }

            return maxError;

        }











        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }







     









        public int X
        {
            get { return x; }
        }

        public int Z
        {
            get { return z; }
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