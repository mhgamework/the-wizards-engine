using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;

namespace MHGameWork.TheWizards.VoxelTerraining
{
    public class VoxelBlock
    {
        public bool Visible
        {
            get
            {
                return block.Visible;
            }
            set
            {
                block.Visible = value;
            }
        }
        public bool Filled
        {
            get { return block.Filled; }
            set
            {
                if (block.Filled == value)
                    return;
                block.Filled = value;
                TW.Data.NotifyObjectModified(TerrainChunk);
            }
        }

        public Point3 Position { get; private set; }

        public VoxelTerrainChunk TerrainChunk { get; private set; }

        private VoxelTerrainChunk.Voxel block;

        public VoxelBlock(Point3 position, VoxelTerrainChunk terrainChunk, VoxelTerrainChunk.Voxel block)
        {
            this.Position = position;
            this.TerrainChunk = terrainChunk;
            this.block = block;
        }
    }
}
