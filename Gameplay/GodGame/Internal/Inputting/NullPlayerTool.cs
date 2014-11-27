using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame.Internal.Inputting
{
    public class NullPlayerTool : PlayerTool
    {
        public NullPlayerTool()
            : base("NullPlayerTool")
        {
        }


        public override void OnLeftClick(PlayerState player, IVoxelHandle voxel)
        {

        }

        public override void OnRightClick(PlayerState player, IVoxelHandle voxel)
        {
        }

        public override void OnKeypress(PlayerState player, IVoxelHandle voxel, Key key)
        {
        }
    }
}