using System;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    class DelegatePlayerInputHandler : IPlayerInputHandler
    {
        private readonly Action<GameVoxel> onLeftClick;
        private readonly Action<GameVoxel> onRightClick;
        public string Name { get; private set; }

        public DelegatePlayerInputHandler(string name, Action<GameVoxel> onLeftClick, Action<GameVoxel> onRightClick)
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