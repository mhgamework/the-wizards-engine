using System;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    class DelegatePlayerTool : PlayerTool
    {
        private readonly Action<IVoxelHandle> onLeftClick;
        private readonly Action<IVoxelHandle> onRightClick;
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

        }

        public override string ToString()
        {
            return "Handler: " + Name;
        }

    }
}