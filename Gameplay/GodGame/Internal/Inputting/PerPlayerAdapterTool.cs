using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// Adapter for mapping per-player tool instances to a global tool instance
    /// </summary>
    public class PerPlayerAdapterTool<T> : IPlayerTool where T : IPlayerToolPerPlayer
    {
        private readonly Func<PlayerState, T> createPlayerTool;

        private Dictionary<PlayerState, T> tools =
            new Dictionary<PlayerState, T>();

        public PerPlayerAdapterTool(Func<PlayerState, T> createPlayerTool)
        {
            this.createPlayerTool = createPlayerTool;
        }

        public string Name { get { return "ChangeHeight"; } }
        public void OnLeftClick(PlayerState player, IVoxelHandle voxel)
        {
            getTool(player).OnLeftClick(voxel);
        }

        public void OnRightClick(PlayerState player, IVoxelHandle voxel)
        {
            getTool(player).OnRightClick(voxel);
        }

        public void OnKeypress(PlayerState player, IVoxelHandle voxel, Key key)
        {
            getTool(player).OnKeypress(voxel, key);
        }

        private T getTool(PlayerState player)
        {
            return tools.GetOrCreate(player, () => createPlayerTool(player));
        }
    }
}