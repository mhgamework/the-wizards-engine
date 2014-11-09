using System;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    public class TownBorderTool : PerPlayerAdapterTool<TownBorderPlayerTool>
    {
        public TownBorderTool(Internal.Model.World world) : base(p => new TownBorderPlayerTool(null))
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Implements the town border tool
    /// Provides binding of IPlayerTools to the town border tool
    /// </summary>
    public class TownBorderPlayerTool : IPlayerToolPerPlayer
    {
        private readonly TownCenterService townCenterService;
        private readonly Internal.Model.World world;
        private readonly PlayerState playerState;

        public TownBorderPlayerTool(TownCenterService townCenterService)
        {
            this.townCenterService = townCenterService;
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

        /// <summary>
        /// Adds given voxel to the town border, and takes it from its current town if one exists
        /// Does nothing in case invalid border addition, and returns false
        /// </summary>
        public bool TryAddBorder(IVoxel voxel, Town town)
        {
            if (!town.IsAtBorder(voxel)) return false;

            var oldTown = townCenterService.GetTownForVoxel(voxel);
            if (oldTown != null)
            {
                if (oldTown.TownVoxels.Count == 1) return false;//Do not remove last
                oldTown.TownVoxels.Remove(voxel);
            }
            town.TownVoxels.Add(voxel);

            return true;
        }

    }
}