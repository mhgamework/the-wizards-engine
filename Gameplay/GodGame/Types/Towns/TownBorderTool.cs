using System;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    public class TownBorderTool : PerPlayerAdapterTool<TownBorderPlayerTool>
    {
        public TownBorderTool(Internal.Model.World world) : base(p => new TownBorderPlayerTool(world,p))
        {
        }
    }
    public class TownBorderPlayerTool : IPlayerToolPerPlayer
    {
        private readonly Internal.Model.World world;
        private readonly PlayerState playerState;

        public TownBorderPlayerTool(Internal.Model.World world, PlayerState playerState)
        {
            this.world = world;
            this.playerState = playerState;
        }

        public void OnLeftClick(IVoxelHandle voxel)
        {
            throw new System.NotImplementedException();
        }

        public void OnRightClick(IVoxelHandle voxel)
        {
            throw new System.NotImplementedException();
        }

        public void OnKeypress(IVoxelHandle voxel, Key key)
        {
            throw new System.NotImplementedException();
        }
    }
}