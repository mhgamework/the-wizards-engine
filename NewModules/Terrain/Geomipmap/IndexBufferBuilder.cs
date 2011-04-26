using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Common.GeoMipMap;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Terrain.Geomipmap
{
    public class IndexBufferBuilder
    {
        // Real variables
        private readonly IXNAGame game;
        public int BlockSize { get; set; }
        public int MaxDetailLevel
        {
            get { return CalculateMaxDetailLevel(BlockSize); }
        }

        public static int CalculateMaxDetailLevel(int blockSize)
        {
            // The -1 can be removed, that detail lvl seems to be supported?
            return (blockSize >> 2) - 1;
        }

        public IndexBufferBuilder(IXNAGame game)
        {
            this.game = game;
        }

        public void BuildBaseIndexes(ITerrainBlock block)
        {
            currentBlock = block;
            if (block.IndexBuffer == null)
                CreateIndexBuffer(block);

            List<ushort> indexes = new List<ushort>();

            BuildBaseIndexes(indexes);


            block.TriangleCount = getTriangleCount();

            if (indexes.Count == 0) return;

            block.IndexBuffer.SetData<ushort>(indexes.ToArray());
        }
        public void BuildEdgeIndexes(ITerrainBlock block)
        {
            currentBlock = block;

            if (block.IndexBuffer == null) return;
            List<ushort> indexes = new List<ushort>();

            BuildEdgeIndexes(indexes);

            block.TriangleCount = getTriangleCount();

            if (indexes.Count == 0) return;
            block.IndexBuffer.SetData<ushort>(getTotalEdgeOffset() * sizeof(ushort),
                                         indexes.ToArray(), 0, indexes.Count);

        }

        public void ChangeDetailLevel(ITerrainBlock block, int mipLevel)
        {
            currentBlock = block;
            currentBlock.DetailLevel = mipLevel;
            if (mipLevel > MaxDetailLevel)
                throw new InvalidOperationException("mipLevel > MaxDetailLevel");

            BuildBaseIndexes(block);
            BuildEdgeIndexes(block);

            var west = block.GetNeighbour(TerrainBlockEdge.West);
            var east = block.GetNeighbour(TerrainBlockEdge.East);
            var north = block.GetNeighbour(TerrainBlockEdge.North);
            var south = block.GetNeighbour(TerrainBlockEdge.South);

            if (west != null)
                BuildEdgeIndexes(west);

            if (east != null)
                BuildEdgeIndexes(east);

            if (north != null)
                BuildEdgeIndexes(north);

            if (south != null)
                BuildEdgeIndexes(south);
        }

        public void CreateIndexBuffer(ITerrainBlock block)
        {
            currentBlock = block;

            if (block.IndexBuffer != null)
                block.IndexBuffer.Dispose();

            // allocate index buffer for maximum amount of room needed
            block.IndexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(ushort),
                                           (BlockSize + 1) * (BlockSize + 1) * 6, BufferUsage.None);

            block.TriangleCount = 0;

        }




        // State functions
        private ITerrainBlock currentBlock;
        private int DetailLevel { get { return currentBlock.DetailLevel; } }
        private int totalEdgeTriangles;
        private int getTriangleCount()
        {
            return getTotalBaseTriangles() + totalEdgeTriangles;
        }
        private int getTotalEdgeOffset()
        {
            return getTotalBaseTriangles() * 3;
        }
        private int getTotalBaseTriangles()
        {
            var x = (BlockSize >> DetailLevel) - 2;
            if (x < 0) return 0; // This is in the case of the lowest detail lvl, note that this lvl was disabled at some point (or still is)
            return x * x * 2;
        }


        protected void BuildBaseIndexes(List<ushort> indexes)
        {
            //totalBaseTriangles = 0;

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
                    //totalBaseTriangles++;

                    // triangle 2
                    indexes.Add(IndexFromCoords(ix + stepping, iz));
                    indexes.Add(IndexFromCoords(ix + stepping, iz + stepping));
                    indexes.Add(IndexFromCoords(ix, iz + stepping));
                    //totalBaseTriangles++;
                }
            }

            if (getTotalEdgeOffset() != indexes.Count) throw new Exception("Error in algorithm, this should be equal!");
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
            if (getNeightbourDetailLevel(edge) <= DetailLevel)
                return BuildBasicEdgeIndexes(indexes, edge);
            else
                return BuildStitchedEdgeIndexes(indexes, edge);
        }

        /// <summary>
        /// Returns MaxDetailLevel when no neighbour exists!
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        private int getNeightbourDetailLevel(TerrainBlockEdge edge)
        {
            var b =currentBlock.GetNeighbour(edge);
            if (b == null) return MaxDetailLevel;
            return b.DetailLevel;
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
            int sourceStep = 1 << DetailLevel;
            int destStep = 1 << getNeightbourDetailLevel(edge);
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
                    startPos = BlockSize;
                    insideStep = sourceStep;
                    sourceStep = -sourceStep;
                    destStep = -destStep;
                    destHalfStep = -destHalfStep;
                    break;

                case TerrainBlockEdge.East:
                    endPos = BlockSize;
                    insidePos = BlockSize;
                    insideStep = -sourceStep;
                    break;

                case TerrainBlockEdge.North:
                    endPos = BlockSize;
                    insideStep = sourceStep;
                    horizontal = true;
                    break;

                case TerrainBlockEdge.South:
                    startPos = BlockSize;
                    insidePos = BlockSize;
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






        public ushort IndexFromCoords(int x, int z)
        {
            return (ushort)(z * (BlockSize + 1) + x);
        }



    }
}
