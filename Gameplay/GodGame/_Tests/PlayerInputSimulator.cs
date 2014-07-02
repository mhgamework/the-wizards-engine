using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.Model;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    public class PlayerInputSimulator : ISimulator
    {
        private readonly PlayerInputHandler handler;
        private World world;

        public PlayerInputSimulator(PlayerInputHandler handler, World world)
        {
            this.handler = handler;
            this.world = world;
        }


        public void Simulate()
        {
            var target = GetTargetedVoxel();
            if (target == null) return;

            if (TW.Graphics.Mouse.LeftMouseJustPressed)
                handler.OnLeftClick(target);
            if (TW.Graphics.Mouse.RightMouseJustPressed)
                handler.OnRightClick(target);
        }

        public GameVoxel GetTargetedVoxel()
        {
            var groundPos = TW.Data.Get<CameraInfo>().GetGroundplanePosition();
            if (!groundPos.HasValue) return null;
            return world.GetVoxelAtGroundPos(groundPos.Value);
        }

    }
}