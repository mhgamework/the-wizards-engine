using System;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

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

        public void OnKeypress(GameVoxel voxel, Key key)
        {
            
        }

        public override string ToString()
        {
            return "Handler: " + Name;
        }

    }
}