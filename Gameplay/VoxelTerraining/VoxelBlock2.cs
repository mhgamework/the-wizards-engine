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
                TW.Data.NotifyObjectModified(terrain);
            }
        }
        private VoxelTerrain.Voxel block;
        private VoxelTerrain terrain;
        private Point3 position;

        public VoxelBlock(Point3 position, VoxelTerrain terrain, VoxelTerrain.Voxel block)
        {
            this.position = position;
            this.terrain = terrain;
            this.block = block;
        }
    }
}
