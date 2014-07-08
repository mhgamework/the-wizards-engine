namespace MHGameWork.TheWizards.GodGame._Tests
{
    public class CreateLandInputHandler : IPlayerInputHandler
    {
        private readonly World world;

        public CreateLandInputHandler(World world)
        {
            this.world = world;
        }

        public string Name { get { return "CreateLand"; } }

        public void OnLeftClick(GameVoxel voxel)
        {
            voxel.ChangeType(GameVoxelType.Air);
        }
        public void OnRightClick(GameVoxel voxel)
        {
            voxel.ChangeType(GameVoxelType.Land);
        }
        public override string ToString()
        {
            return "Handler: " + Name;
        }

    }
}