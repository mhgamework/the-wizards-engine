using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Types;

namespace MHGameWork.TheWizards.GodGame
{
    public class CreateLandInputHandler : IPlayerInputHandler
    {
        private readonly Internal.World world;

        public CreateLandInputHandler(Internal.World world)
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