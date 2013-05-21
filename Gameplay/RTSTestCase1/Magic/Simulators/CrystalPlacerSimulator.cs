using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic.Simulators
{
    public class CrystalPlacerSimulator : ISimulator
    {
        public void Simulate()
        {

            var groundplane = new Plane(MathHelper.Up, 0);
            var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();

            var dist =  ray.xna().Intersects(groundplane.xna());

            if (!dist.HasValue) return;

            var point = ray.Position + ray.Direction * dist.Value;


            TW.Graphics.LineManager3D.AddCenteredBox(point, 0.5f, new Color4(1, 0, 0));

            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.F)) return;

            new SimpleCrystal { Position = point, Capacity = 1000, Energy = 0 };

        }
    }
}