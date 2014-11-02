using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public interface IPlayerToolPerPlayer
    {
        void OnLeftClick(IVoxelHandle voxel);
        void OnRightClick(IVoxelHandle voxel);
        void OnKeypress(IVoxelHandle voxel, Key key);
    }
}