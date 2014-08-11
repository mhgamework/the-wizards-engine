using System;
using MHGameWork.TheWizards.GodGame.Internal;

namespace MHGameWork.TheWizards.GodGame
{
    class DelegatePlayerTool : IPlayerTool
    {
        private readonly Action<GameVoxel> onLeftClick;
        private readonly Action<GameVoxel> onRightClick;
        public string Name { get; private set; }

        public DelegatePlayerTool(string name, Action<GameVoxel> onLeftClick, Action<GameVoxel> onRightClick)
        {
            this.onLeftClick = onLeftClick;
            this.onRightClick = onRightClick;
            Name = name;
        }

        public void OnLeftClick(GameVoxel voxel)
        {
            onLeftClick(voxel);
        }

        public void OnRightClick(GameVoxel voxel)
        {
            onRightClick(voxel);
        }
        public override string ToString()
        {
            return "Handler: " + Name;
        }

    }
}