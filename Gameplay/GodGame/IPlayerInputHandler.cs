namespace MHGameWork.TheWizards.GodGame._Tests
{
    public interface IPlayerInputHandler
    {
        string Name { get; }
        void OnLeftClick(GameVoxel voxel);
        void OnRightClick(GameVoxel voxel);

      
    }
}