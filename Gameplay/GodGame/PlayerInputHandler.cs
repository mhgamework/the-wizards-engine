namespace MHGameWork.TheWizards.GodGame._Tests
{
    public class PlayerInputHandler
    {
        private readonly World world;

        public PlayerInputHandler(World world)
        {
            this.world = world;
        }

        public void OnLeftClick(GameVoxel voxel)
        {
            voxel.ChangeType(GameVoxelType.Air);
        }
        public void OnRightClick(GameVoxel voxel)
        {
            voxel.ChangeType(GameVoxelType.Land);
        }
    }
}