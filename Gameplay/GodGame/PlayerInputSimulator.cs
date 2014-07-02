using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    public class PlayerInputSimulator : ISimulator
    {
        private readonly IEnumerable<IPlayerInputHandler> handlers;
        private World world;

        public IPlayerInputHandler ActiveHandler { get; private set; }

        public PlayerInputSimulator(IEnumerable<IPlayerInputHandler> handlers, World world)
        {
            this.handlers = handlers;
            this.world = world;
            ActiveHandler = handlers.First();
        }


        public void Simulate()
        {
            var playerInputHandlers = handlers.Concat(handlers);
            if (TW.Graphics.Mouse.RelativeScrollWheel < 0)
            {
                var inputHandlers = handlers.SkipWhile(el => el != ActiveHandler);
                var skipWhile = inputHandlers.Concat(handlers);
                ActiveHandler = skipWhile.Skip(1).First();


            }
            if (TW.Graphics.Mouse.RelativeScrollWheel > 0)
            {
                var inputHandlers = playerInputHandlers.TakeWhile((el, i) => el != ActiveHandler || i == 0);
                ActiveHandler = inputHandlers.Last();
            }

            var target = GetTargetedVoxel();
            if (target == null) return;

            if (TW.Graphics.Mouse.LeftMouseJustPressed)
                ActiveHandler.OnLeftClick(target);
            if (TW.Graphics.Mouse.RightMouseJustPressed)
                ActiveHandler.OnRightClick(target);
        }

        public GameVoxel GetTargetedVoxel()
        {
            var groundPos = TW.Data.Get<CameraInfo>().GetGroundplanePosition();
            if (!groundPos.HasValue) return null;
            return world.GetVoxelAtGroundPos(groundPos.Value);
        }

    }
}