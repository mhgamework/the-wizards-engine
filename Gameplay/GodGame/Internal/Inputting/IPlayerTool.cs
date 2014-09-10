using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public interface IPlayerTool
    {
        string Name { get; }
        void OnLeftClick(PlayerState player, IVoxelHandle voxel);
        void OnRightClick(PlayerState player, IVoxelHandle voxel);
        void OnKeypress(PlayerState player, IVoxelHandle voxel, Key key);


    }
}