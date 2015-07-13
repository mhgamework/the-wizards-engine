using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Input;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// Adapter for mapping per-player tool instances to a global tool instance
    /// </summary>
    public class PerPlayerAdapterTool<T> : PlayerTool where T : IPlayerToolPerPlayer
    {
        private readonly string name;
        private readonly Func<PlayerState, T> createPlayerTool;

        private Dictionary<PlayerState, T> tools =
            new Dictionary<PlayerState, T>();

        public PerPlayerAdapterTool(string name, Func<PlayerState, T> createPlayerTool)
            : base(name)
        {
            this.name = name;
            this.createPlayerTool = createPlayerTool;
        }
        public override void OnLeftClick(PlayerState player, IVoxelHandle voxel)
        {
            getTool(player).OnLeftClick(voxel);
        }

        public override void OnRightClick(PlayerState player, IVoxelHandle voxel)
        {
            getTool(player).OnRightClick(voxel);
        }

        public override void OnKeypress(PlayerState player, IVoxelHandle voxel, Key key)
        {
            getTool(player).OnKeypress(voxel, key);
        }
        public override void OnTargetChanged(PlayerState player, IVoxelHandle voxel, TWKeyboard keyboard, TWMouse mouse)
        {
            getTool(player).OnTargetChanged(voxel, keyboard, mouse);
        }

        private T getTool(PlayerState player)
        {
            return tools.GetOrCreate(player, () => createPlayerTool(player));
        }
    }
}