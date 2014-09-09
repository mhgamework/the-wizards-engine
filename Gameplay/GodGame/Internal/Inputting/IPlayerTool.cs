using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public interface IPlayerTool
    {
        string Name { get; }
        void OnLeftClick(GameVoxel voxel);
        void OnRightClick(GameVoxel voxel);
        void OnKeypress(GameVoxel voxel, Key key);


    }
}