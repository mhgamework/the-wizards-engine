using DirectX11;
using MHGameWork.TheWizards.Engine.Persistence;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Engine.PhysX
{
    public class BarrelShooterSimulator : ISimulator
    {
        private IMesh mesh;

        public BarrelShooterSimulator()
        {
            mesh = TW.Assets.LoadMesh("Core\\Barrel01");
        }


        public void Simulate()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.B))
                return;

            var ent = new WorldRendering.Entity
                {
                    WorldMatrix =
                        Matrix.Translation(TW.Data.GetSingleton<CameraInfo>().ActiveCamera.ViewInverse.GetTranslation()),
                    Mesh = mesh,
                    Solid = true,
                    Static = false
                };


            TW.Data.GetSingleton<Datastore>().Persist(ent);


        }
    }
}
