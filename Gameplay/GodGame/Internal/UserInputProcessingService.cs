using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.ToolSelection;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// Service responsible for converting raw key input into player commands
    /// Passes raw input to a IPlayerInputHandler
    /// Also simulates the ToolSelectionMenu
    /// </summary>
    public class UserInputProcessingService : ISimulator
    {
        private Internal.Model.World world;
        private IPlayerInputHandler inputHandler;
        private readonly WorldRenderingService renderer;
        private readonly ToolSelectionMenu toolSelectionMenu;


        public UserInputProcessingService(Internal.Model.World world, IPlayerInputHandler inputHandler, WorldRenderingService renderer,ToolSelectionMenu toolSelectionMenu)
        {
            this.world = world;
            this.inputHandler = inputHandler;
            this.renderer = renderer;
            this.toolSelectionMenu = toolSelectionMenu;
        }

        public bool UserInputDisabled { get; set; }

        private IVoxel previousTarget;
        public void Simulate()
        {
            if (UserInputDisabled) return;
            toolSelectionMenu.ProcessUserInput();
            /*if (TW.Graphics.Mouse.RelativeScrollWheel < 0 || TW.Graphics.Keyboard.IsKeyPressed(Key.UpArrow))
                inputHandler.OnPreviousTool();
            if (TW.Graphics.Mouse.RelativeScrollWheel > 0 || TW.Graphics.Keyboard.IsKeyPressed(Key.DownArrow))
                inputHandler.OnNextTool();*/

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.O))
                inputHandler.OnSave();

            if (trySimulateUIControls()) return;

            var target = GetTargetedVoxel();
            if (target == null) return;
            if (target != previousTarget)
                inputHandler.OnTargetChanged(target);
            previousTarget = target;

            if (TW.Graphics.Mouse.LeftMouseJustPressed)
                inputHandler.OnLeftClick(target);
            if (TW.Graphics.Mouse.RightMouseJustPressed)
                inputHandler.OnRightClick(target);
            foreach (var k in TW.Graphics.Keyboard.PressedKeys.Where(k => TW.Graphics.Keyboard.IsKeyPressed(k)))
            {
                inputHandler.OnKeyPressed(target, k);
            }
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
        public GameVoxel GetTargetedVoxel()
        {
            var groundPos = TW.Data.Get<CameraInfo>().GetGroundplanePosition();
            if (!groundPos.HasValue) return null;
            return world.GetVoxelAtGroundPos(groundPos.Value);
        }

    }
}