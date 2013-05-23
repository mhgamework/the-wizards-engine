using System.Linq.Expressions;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Selecting;
using MHGameWork.TheWizards.RTSTestCase1._Tests;
using MHGameWork.TheWizards.Rendering.Text;
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
        public EditorConfiguration Configuration { get; private set; }

        private readonly MenuDisplayer menuDisplayer;
        private WorldPlacerUpdater selectorUpdater;
        private WorldSelector selector;
        private Vector3 pos;
        private Vector3 dir;
        private Vector3 up;


        public WorldInputtingSimulator()
        {
            Configuration = new EditorConfiguration();
            selector = new WorldSelector();

            menuDisplayer = new MenuDisplayer(Configuration.Menu, selector);



        }

        public void Simulate()
        {
            if (selectorUpdater == null && Configuration.Placer != null)
                selectorUpdater = new WorldPlacerUpdater(Configuration.Placer, selector);

            pos = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx();
            dir = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Forward.dx();
            up = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Up.dx();

            simulateMenu();

            selector.UpdateTarget(TW.Data.Get<CameraInfo>().GetCenterScreenRay());
            if (TW.Graphics.Mouse.LeftMouseJustPressed)
                selector.Select();


            selectorUpdater.Simulate();



        }

        private void simulateMenu()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.G))
                menuDisplayer.Toggle(pos + dir * 10, dir, up);

            if (menuDisplayer.Visible && Vector3.Distance(pos, menuDisplayer.Position) > 20)
                menuDisplayer.Hide();


            menuDisplayer.Simulate();
        }
    }
}