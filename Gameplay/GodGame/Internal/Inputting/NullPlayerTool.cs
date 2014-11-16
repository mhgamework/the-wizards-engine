using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame.Internal.Inputting
{
    public class NullPlayerTool : IPlayerTool
    {
        public string Name { get { return "NullPlayerTool"; } }

        public void OnLeftClick(PlayerState player, IVoxelHandle voxel)
        {

        }

        public void OnRightClick(PlayerState player, IVoxelHandle voxel)
        {
        }

        public void OnKeypress(PlayerState player, IVoxelHandle voxel, Key key)
        {
        }
    }
}