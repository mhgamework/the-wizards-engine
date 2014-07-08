using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public class PlayerInputSimulator : ISimulator
    {
        private readonly IEnumerable<IPlayerInputHandler> handlers;
        private Internal.World world;

        public IPlayerInputHandler ActiveHandler { get; private set; }

        public PlayerInputSimulator(IEnumerable<IPlayerInputHandler> handlers, Internal.World world)
        {
            this.handlers = handlers;
            this.world = world;
            ActiveHandler = handlers.First();
        }


        public void Simulate()
        {
            scrollActiveHandler();

            var target = GetTargetedVoxel();
            if (target == null) return;

            if (tryVoxelInteract(target)) return;

            if (TW.Graphics.Mouse.LeftMouseJustPressed)
                ActiveHandler.OnLeftClick(target);
            else if (TW.Graphics.Mouse.RightMouseJustPressed)
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

        private void scrollActiveHandler()
        {
            var playerInputHandlers = handlers.Concat(handlers);
            if (TW.Graphics.Mouse.RelativeScrollWheel < 0 || TW.Graphics.Keyboard.IsKeyPressed(Key.UpArrow))
            {
                var inputHandlers = handlers.SkipWhile(el => el != ActiveHandler);
                var skipWhile = inputHandlers.Concat(handlers);
                ActiveHandler = skipWhile.Skip(1).First();
            }
            if (TW.Graphics.Mouse.RelativeScrollWheel > 0 || TW.Graphics.Keyboard.IsKeyPressed(Key.DownArrow))
            {
                var inputHandlers = playerInputHandlers.TakeWhile((el, i) => el != ActiveHandler || i == 0);
                ActiveHandler = inputHandlers.Last();
            }
        }

        public GameVoxel GetTargetedVoxel()
        {
            var groundPos = TW.Data.Get<CameraInfo>().GetGroundplanePosition();
            if (!groundPos.HasValue) return null;
            return world.GetVoxelAtGroundPos(groundPos.Value);
        }

    }
}