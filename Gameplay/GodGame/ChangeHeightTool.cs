using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public class ChangeHeightTool : IPlayerTool
    {
        public int Size { get; private set; }
        private Internal.Model.World world;

        public ChangeHeightTool(Internal.Model.World world)
        {
            this.world = world;
        }

        public string Name { get { return "ChangeHeight"; } }
        public void OnLeftClick(GameVoxel voxel)
        {
            foreach (var v in GetVoxelsInRange(voxel))
            {
                v.Data.Height++;
                world.NotifyVoxelChanged(v);
            }
        }

        public void OnRightClick(GameVoxel voxel)
        {
            foreach (var v in GetVoxelsInRange(voxel))
            {
                v.Data.Height--;
                world.NotifyVoxelChanged(v);
            }
        }

        public void OnKeypress(GameVoxel voxel, Key key)
        {
            if (key == Key.NumberPadPlus)
                Size++;
            if (key == Key.NumberPadMinus)
                Size--;

            if (Size < 0)
                Size = 0;
        }

        private List<GameVoxel> GetVoxelsInRange(GameVoxel centerVoxel)
        {
            return world.GetRange(centerVoxel, Size).ToList();
        }
    }
}
