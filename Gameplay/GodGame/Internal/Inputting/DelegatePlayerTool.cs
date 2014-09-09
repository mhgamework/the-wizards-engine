using System;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    class DelegatePlayerTool : IPlayerTool
    {
        private readonly Action<IVoxelHandle> onLeftClick;
        private readonly Action<IVoxelHandle> onRightClick;
        public string Name { get; private set; }

        public DelegatePlayerTool(string name, Action<IVoxelHandle> onLeftClick, Action<IVoxelHandle> onRightClick)
        {
            this.onLeftClick = onLeftClick;
            this.onRightClick = onRightClick;
            Name = name;
        }

        public void OnLeftClick(IVoxelHandle voxel)
        {
            onLeftClick(voxel);
        }

        public void OnRightClick(IVoxelHandle voxel)
        {
            onRightClick(voxel);
        }

        public void OnKeypress(IVoxelHandle voxel, Key key)
        {
            
        }

        public override string ToString()
        {
            return "Handler: " + Name;
        }

    }
}