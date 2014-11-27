using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public class CreateLandTool : PlayerTool
    {
        private readonly Internal.Model.World world;
        private readonly AirType air;
        private readonly LandType land;

        public CreateLandTool(Internal.Model.World world, AirType air, LandType land):base("CreateLand")
        {
            this.world = world;
            this.air = air;
            this.land = land;
        }

        public override void OnLeftClick(PlayerState player, IVoxelHandle voxel)
        {
            voxel.ChangeType(air);
        }
        public override void OnRightClick(PlayerState player, IVoxelHandle voxel)
        {
            voxel.ChangeType(land);
        }

        public override void OnKeypress(PlayerState player, IVoxelHandle voxel, Key key)
        {

        }

        public override string ToString()
        {
            return "Handler: " + Name;
        }

    }
}