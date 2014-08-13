using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public class PlayerInputSimulator : ISimulator
    {
        private Internal.World world;
        private IPlayerInputHandler inputHandler;
        private readonly SimpleWorldRenderer renderer;

        //public IPlayerTool ActiveHandler { get; private set; }

        public PlayerInputSimulator(Internal.World world, IPlayerInputHandler inputHandler)
        {
            this.world = world;
            this.inputHandler = inputHandler;
            //ActiveHandler = handlers.First();
            this.renderer = renderer;
        }


        public void Simulate()
        {
            if (TW.Graphics.Mouse.RelativeScrollWheel < 0 || TW.Graphics.Keyboard.IsKeyPressed(Key.UpArrow))
                inputHandler.OnPreviousTool();
            if (TW.Graphics.Mouse.RelativeScrollWheel > 0 || TW.Graphics.Keyboard.IsKeyPressed(Key.DownArrow))
                inputHandler.OnNextTool();
            
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.O))
                inputHandler.OnSave();
            
if (trySimulateUIControls()) return;
            simulateTargetingInput();
}

        private bool trySimulateUIControls()
        {
            //TODO: does not raycast closest
            foreach (var vcv in renderer.VisibleCustomRenderables.OfType<ValueControlVisualizer>())
            {
                if (vcv.TryProcessUserInput(TW.Data.Get<CameraInfo>().GetCenterScreenRay())) return true;
            }
            return false;
        }
            var target = GetTargetedVoxel();
            if (target == null) return;

            if (TW.Graphics.Mouse.LeftMouseJustPressed)
                inputHandler.OnLeftClick(target);
            if (TW.Graphics.Mouse.RightMouseJustPressed)
                inputHandler.OnRightClick(target);
        }

        public GameVoxel GetTargetedVoxel()
        {
            var groundPos = TW.Data.Get<CameraInfo>().GetGroundplanePosition();
            if (!groundPos.HasValue) return null;
            return world.GetVoxelAtGroundPos(groundPos.Value);
        }

    }
}