using MHGameWork.TheWizards.GodGame.Internal;

namespace MHGameWork.TheWizards.GodGame
{
    public interface IPlayerInputHandler
    {
        string Name { get; }
        void OnLeftClick(GameVoxel voxel);
        void OnRightClick(GameVoxel voxel);

      
    }
}