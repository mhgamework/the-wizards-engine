using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public class PlayerInputHandler : IPlayerInputHandler
    {
        private readonly IEnumerable<IPlayerTool> handlers;
        private Internal.World world;
        private readonly WorldPersister worldPersister;
        private readonly PlayerState player;

        public IPlayerTool ActiveHandler { get { return player.ActiveTool; } private set { player.ActiveTool = value; } }

        public PlayerInputHandler(IEnumerable<IPlayerTool> handlers, Internal.World world, WorldPersister worldPersister, PlayerState player)
        {
            this.handlers = handlers.ToArray();
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

            ActiveHandler.OnRightClick(target);
        }
        private bool tryVoxelInteract(GameVoxel target)
        {
            var handle = new IVoxelHandle(world);
            handle.CurrentVoxel = target;

            var ret = target.Type.Interact(handle);

            handle.CurrentVoxel = null;

            return ret;
        }

        public void OnLeftClick(GameVoxel target)
        {
            ActiveHandler.OnLeftClick(target);
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

    }
}