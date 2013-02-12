using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.RTSTestCase1.Cannons;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Rendering
{
    public class CannonRenderData : IModelObjectAddon<Cannon>
    {
        private readonly Cannon cannon;

        private Entity ent = new Entity();

        public CannonRenderData(Cannon cannon)
        {
            this.cannon = cannon;
            ent = new Entity();
        }

        public void Update()
        {
            ent.WorldMatrix = Matrix.RotationY(cannon.Angle) * Matrix.Translation(cannon.Position);
            ent.Mesh = TW.Assets.LoadMesh("RTS\\Cannon");
        }

        public void Dispose()
        {
            TW.Data.RemoveObject(ent);
        }
    }
}