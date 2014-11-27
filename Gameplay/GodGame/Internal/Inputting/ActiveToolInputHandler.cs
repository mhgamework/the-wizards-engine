using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Inputting;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// Handles player input commands and forwards them to the active playertool
    /// </summary>
    public class ActiveToolInputHandler : IPlayerInputHandler
    {
        private Internal.Model.World world;
        private readonly WorldPersisterService worldPersister;
        private readonly PlayerState player;

        public PlayerTool ActivePlayerTool
        {
            get { return player.ActiveTool; }
            set { player.ActiveTool = value; }
        }

        public ActiveToolInputHandler(Internal.Model.World world, WorldPersisterService worldPersister, PlayerState player, NullPlayerTool nullPlayerTool)
        {
            this.world = world;
            this.worldPersister = worldPersister;
            this.player = player;
            ActivePlayerTool = nullPlayerTool;
        }

        public void OnSave()
        {
            worldPersister.Save(world, worldPersister.GetDefaultSaveFile());
        }

        public void OnRightClick(GameVoxel target)
        {
            if (tryVoxelInteract(target)) return;

            ActivePlayerTool.OnRightClick(player, target);
        }
        private bool tryVoxelInteract(GameVoxel target)
        {
            var ret = target.Type.Interact(target);

            return ret;
        }

        public void OnLeftClick(GameVoxel target)
        {
            ActivePlayerTool.OnLeftClick(player, target);
        }



        public void OnNextTool()
        {
        }

        public void OnPreviousTool()
        {
        }

        public void OnKeyPressed(GameVoxel target, Key key)
        {
            ActivePlayerTool.OnKeypress(player, target, key);
        }

        public void OnTargetChanged(GameVoxel target)
        {
            ActivePlayerTool.OnTargetChanged(player, target, TW.Graphics.Keyboard, TW.Graphics.Mouse);
        }
    }
}