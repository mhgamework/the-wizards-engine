using System;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    /// <summary>
    /// Implements the TownBorderTool player-independently
    /// </summary>
    public class TownBorderTool : PerPlayerAdapterTool<TownBorderPlayerTool>
    {
        public TownBorderTool(TownCenterService townCenterService)
            : base("TownBorderTool",p => new TownBorderPlayerTool(townCenterService))
        {
        }
    }
    /// <summary>
    /// Implements the town border tool
    /// Provides binding of IPlayerTools to the town border tool
    /// </summary>
    public class TownBorderPlayerTool : IPlayerToolPerPlayer
    {
        private readonly TownCenterService townCenterService;

        private Town ActiveTown;

        public TownBorderPlayerTool(TownCenterService townCenterService)
        {
            this.townCenterService = townCenterService;
        }


        public void OnLeftClick(IVoxelHandle voxel)
        {
            var clickedTown = townCenterService.GetTownForVoxel(voxel.GetInternalVoxel());
            if (clickedTown == null) return;
            if (clickedTown.CanRemove(voxel.GetInternalVoxel()))
            {
                
                clickedTown.TownVoxels.Remove(voxel.GetInternalVoxel());
                voxel.MarkChanged();
                voxel.Get8Connected().ForEach(v => v.MarkChanged());
            }
        }

        public void OnRightClick(IVoxelHandle voxel)
        {
            var clickedTown = townCenterService.GetTownForVoxel(voxel.GetInternalVoxel());

            if (clickedTown == null && ActiveTown != null)
            {
                TryAddBorder(voxel.GetInternalVoxel(), ActiveTown);
                voxel.MarkChanged();
                voxel.Get8Connected().ForEach(v =>v.MarkChanged());
            }
            else if (clickedTown != null)
            {
                ActiveTown = clickedTown;
            }
        }

        public void OnKeypress(IVoxelHandle voxel, Key key)
        {
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