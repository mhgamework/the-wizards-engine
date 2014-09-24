using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// Handles player input commands and applies them onto the game state
    /// </summary>
    public class PlayerInputHandler : IPlayerInputHandler
    {
        private readonly IEnumerable<IPlayerTool> handlers;
        private Internal.Model.World world;
        private readonly WorldPersisterService worldPersister;
        private readonly PlayerState player;

        public IPlayerTool ActiveHandler { get { return player.ActiveTool; } private set { player.ActiveTool = value; } }

        public PlayerInputHandler(PlayerToolsFactory factory, Internal.Model.World world, WorldPersisterService worldPersister, PlayerState player)
        {
            this.handlers = factory.Tools.ToArray();
            this.world = world;
            this.worldPersister = worldPersister;
            this.player = player;
            ActiveHandler = this.handlers.First();
        }

        public void OnSave()
        {
            worldPersister.Save(world, worldPersister.GetDefaultSaveFile());
        }

        public void OnRightClick(GameVoxel target)
        {
            if (tryVoxelInteract(target)) return;

            ActiveHandler.OnRightClick(player, target);
        }
        private bool tryVoxelInteract(GameVoxel target)
        {
            var ret = target.Type.Interact(target);

            return ret;
        }

        public void OnLeftClick(GameVoxel target)
        {
            ActiveHandler.OnLeftClick(player, target);
        }



        public void OnNextTool()
        {
            var playerInputHandlers = handlers.Concat(handlers);
            var inputHandlers = playerInputHandlers.TakeWhile((el, i) => el != ActiveHandler || i == 0);
            ActiveHandler = inputHandlers.Last();
        }

        public void OnPreviousTool()
        {
            var inputHandlers = handlers.SkipWhile(el => el != ActiveHandler);
            var skipWhile = inputHandlers.Concat(handlers);
            ActiveHandler = skipWhile.Skip(1).First();
        }

        public void OnKeyPressed(GameVoxel target, Key key)
        {
            ActiveHandler.OnKeypress(player, target, key);
        }
    }
}