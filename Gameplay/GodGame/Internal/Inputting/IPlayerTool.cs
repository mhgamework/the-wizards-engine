using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public interface IPlayerTool
    {
        string Name { get; }
        void OnLeftClick(IVoxelHandle voxel);
        void OnRightClick(IVoxelHandle voxel);
        void OnKeypress(IVoxelHandle voxel, Key key);


    }
}