using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;

namespace MHGameWork.TheWizards.GodGame
{
    public interface IPlayerTool
    {
        string Name { get; }
        void OnLeftClick(GameVoxel voxel);
        void OnRightClick(GameVoxel voxel);

      
    }
}