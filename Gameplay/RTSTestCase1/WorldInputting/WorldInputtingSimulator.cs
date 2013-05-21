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
        private WorldSelector selector;



        public WorldInputtingSimulator()
        {
            Configuration = new EditorConfiguration();
            selector = new WorldSelector();

            menuDisplayer = new MenuDisplayer(Configuration.Menu, selector);



        }

        public void Simulate()
        {
            var pos = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx();
            var dir = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Forward.dx();
            var up = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Up.dx();
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.G))
                menuDisplayer.Toggle(pos + dir * 10, dir,up);

            if (menuDisplayer.Visible && Vector3.Distance(pos, menuDisplayer.Position) > 20)
                menuDisplayer.Hide();


            menuDisplayer.Simulate();

            selector.UpdateTarget(TW.Data.Get<CameraInfo>().GetCenterScreenRay());
            if (TW.Graphics.Mouse.LeftMouseJustPressed)
                selector.Select();



            

        }
    }
}