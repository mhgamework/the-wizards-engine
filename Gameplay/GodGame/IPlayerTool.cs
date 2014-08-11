using MHGameWork.TheWizards.GodGame.Internal;

namespace MHGameWork.TheWizards.GodGame
{
    public interface IPlayerTool
    {
        string Name { get; }
        void OnLeftClick(GameVoxel voxel);
        void OnRightClick(GameVoxel voxel);

      
    }
}