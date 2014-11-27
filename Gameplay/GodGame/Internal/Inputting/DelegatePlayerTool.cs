using System;
using MHGameWork.TheWizards.DirectX11.Input;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    class DelegatePlayerTool : PlayerTool
    {
        public Action<IVoxelHandle> onLeftClick { get; private set; }
        public Action<IVoxelHandle> onRightClick { get; private set; }
        public Action<PlayerState, IVoxelHandle, Key> onKeypress { get; set; }
        public Action<PlayerState, IVoxelHandle, TWKeyboard, TWMouse> onTargetChanged { get; set; }

        public string Name { get; private set; }

        public DelegatePlayerTool(string name, Action<IVoxelHandle> onLeftClick, Action<IVoxelHandle> onRightClick)
            : base(name)
        {
            this.onLeftClick = onLeftClick;
            this.onRightClick = onRightClick;
            Name = name;
        }

        public override void OnLeftClick(PlayerState player, IVoxelHandle voxel)
        {
            onLeftClick(voxel);
        }

        public override void OnRightClick(PlayerState player, IVoxelHandle voxel)
        {
            onRightClick(voxel);
        }

        public override void OnKeypress(PlayerState player, IVoxelHandle voxel, Key key)
        {
            if (onKeypress != null)
                onKeypress(player, voxel, key);
        }
        public override void OnTargetChanged(PlayerState player, IVoxelHandle voxel, DirectX11.Input.TWKeyboard keyboard, DirectX11.Input.TWMouse mouse)
        {
            if (onTargetChanged != null)
                onTargetChanged(player, voxel, TW.Graphics.Keyboard, TW.Graphics.Mouse);
        }

        public override string ToString()
        {
            return "Handler: " + Name;
        }

    }
}