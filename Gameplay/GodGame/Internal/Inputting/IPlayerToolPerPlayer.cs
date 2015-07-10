using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Input;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public interface IPlayerToolPerPlayer
    {
        void OnLeftClick(IVoxelHandle voxel);
        void OnRightClick(IVoxelHandle voxel);
        void OnKeypress(IVoxelHandle voxel, Key key);
        void OnTargetChanged(IVoxelHandle voxel, TWKeyboard keyboard, TWMouse mouse);

    }
    public class PlayerToolPerPlayer:IPlayerToolPerPlayer
    {
        public virtual void OnLeftClick(IVoxelHandle voxel)
        {
            
        }

        public virtual void OnRightClick(IVoxelHandle voxel)
        {
        }

        public virtual void OnKeypress(IVoxelHandle voxel, Key key)
        {
        }

        public virtual void OnTargetChanged(IVoxelHandle voxel, TWKeyboard keyboard, TWMouse mouse)
        {
            
        }
    }
}