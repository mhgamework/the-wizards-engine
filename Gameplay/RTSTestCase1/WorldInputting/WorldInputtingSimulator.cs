using System.Linq.Expressions;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Placing;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Selecting;
using MHGameWork.TheWizards.RTSTestCase1._Tests;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldInputting
{
    /// <summary>
    /// Simulates the engine editor functionalities, through the use of the EditorConfiguration
    /// 
    /// TODO: current this is not hotloading compatible
    /// </summary>
    public class WorldInputtingSimulator : ISimulator
    {
        private readonly EditorConfiguration controller;


        private MenuDisplayer menuDisplayer;
        private WorldPlacerUpdater worldPlacerUpdater;
        private WorldSelector selector;
        private Vector3 pos;
        private Vector3 dir;
        private Vector3 up;


        public WorldInputtingSimulator(EditorConfiguration controller)
        {
            this.controller = controller;
            selector = new WorldSelector();





        }

        public void Simulate()
        {


            updateCameraInfo();

            simulateMenu();

            selector.UpdateTarget(TW.Data.Get<CameraInfo>().GetCenterScreenRay());
            if (TW.Graphics.Mouse.LeftMouseJustPressed)
                selector.Select();

            simulateWorldPlacer();

            // Less dirty than before, but still dirty
            selector.ClearProviders();
            if (controller.SelectableProvider != null)
                selector.AddProvider(controller.SelectableProvider);


            selector.RenderSelection();
        }

        private void simulateWorldPlacer()
        {
            if (controller.Placer == null)
            {
                if (worldPlacerUpdater != null)
                    worldPlacerUpdater.Disable();
                worldPlacerUpdater = null;
            }
            if (worldPlacerUpdater == null && controller.Placer != null)
                worldPlacerUpdater = new WorldPlacerUpdater(controller.Placer, selector);
            if (worldPlacerUpdater != null)
                worldPlacerUpdater.Simulate();
        }

        private void updateCameraInfo()
        {
            pos = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx();
            dir = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Forward.dx();
            up = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Up.dx();
        }

        private void simulateMenu()
        {
            if (menuDisplayer == null || menuDisplayer.Config != controller.Menu)
            {
                menuDisplayer = new MenuDisplayer(controller.Menu, selector);
            }

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.G))
                menuDisplayer.Toggle(pos + dir * 10, dir, up);

            if (menuDisplayer.Visible && Vector3.Distance(pos, menuDisplayer.Position) > 20)
                menuDisplayer.Hide();


            menuDisplayer.Simulate();
        }
    }
}