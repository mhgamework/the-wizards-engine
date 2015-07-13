using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Input;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public interface IPlayerTool
    {
        string Name { get; }
        void OnLeftClick(PlayerState player, IVoxelHandle voxel);
        void OnRightClick(PlayerState player, IVoxelHandle voxel);
        void OnKeypress(PlayerState player, IVoxelHandle voxel, Key key);
        void OnTargetChanged(PlayerState player, IVoxelHandle voxel, TWKeyboard keyboard, TWMouse mouse);
    }
    public abstract class PlayerTool : IPlayerTool
    {
        public PlayerTool(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }
        public virtual void OnLeftClick(PlayerState player, IVoxelHandle voxel)
        {
            
        }
        public virtual void OnRightClick(PlayerState player, IVoxelHandle voxel)
        {
            
        }
        public virtual void OnKeypress(PlayerState player, IVoxelHandle voxel, Key key)
        {
            
        }

        public virtual void OnTargetChanged(PlayerState player, IVoxelHandle voxel, TWKeyboard keyboard, TWMouse mouse)
        {
            
        }
    }
}