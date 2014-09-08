using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal.Model;

namespace MHGameWork.TheWizards.GodGame
{
    public class ChangeHeightTool : IPlayerTool
    {
        public string Name { get { return "ChangeHeight"; }}
        public void OnLeftClick(GameVoxel voxel)
        {
            voxel.Data.Height++;
            voxel.World.NotifyVoxelChanged(voxel);
        }

        public void OnRightClick(GameVoxel voxel)
        {
            voxel.Data.Height--;
            voxel.World.NotifyVoxelChanged(voxel);
        }
    }
}
