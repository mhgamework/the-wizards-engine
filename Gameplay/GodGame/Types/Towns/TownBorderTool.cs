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
    public class TownBorderPlayerTool : PlayerToolPerPlayer
    {
        private readonly TownCenterService townCenterService;

        private Town ActiveTown;

        public TownBorderPlayerTool(TownCenterService townCenterService)
        {
            this.townCenterService = townCenterService;
        }

        public override void OnLeftClick(IVoxelHandle voxel)
        {
            doRemove(voxel, townCenterService.GetTownForVoxel(voxel.GetInternalVoxel()));
        }

        public override void OnRightClick(IVoxelHandle voxel)
        {
            doAdd(voxel, townCenterService.GetTownForVoxel(voxel.GetInternalVoxel()));
        }

        public override void OnTargetChanged(IVoxelHandle voxel, DirectX11.Input.TWKeyboard keyboard, DirectX11.Input.TWMouse mouse)
        {
            var clickedTown = townCenterService.GetTownForVoxel(voxel.GetInternalVoxel());

            if (mouse.LeftMousePressed)
                doRemove(voxel, clickedTown);
            else if (mouse.RightMousePressed)
                doAdd(voxel, clickedTown);
        }

        private void doAdd(IVoxelHandle voxel, Town clickedTown)
        {
            if (clickedTown != null)
            {
                ActiveTown = clickedTown;
                return;
            }
            if (ActiveTown != null)
            {
                tryAdd(voxel, ActiveTown);
            }
        }

        private static void doRemove(IVoxelHandle voxel, Town clickedTown)
        {
            if (clickedTown == null) return;
            if (clickedTown.CanRemove(voxel.GetInternalVoxel()))
            {
                clickedTown.TownVoxels.Remove(voxel.GetInternalVoxel());
                voxel.MarkChanged();
                voxel.Get8Connected().ForEach(v => v.MarkChanged());
            }
        }

        private void tryAdd(IVoxelHandle voxel, Town town)
        {
            TryAddBorder(voxel.GetInternalVoxel(), town);
            voxel.MarkChanged();
            voxel.Get8Connected().ForEach(v => v.MarkChanged());
        }

        private static void tryRemove(IVoxelHandle voxel, Town clickedTown)
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