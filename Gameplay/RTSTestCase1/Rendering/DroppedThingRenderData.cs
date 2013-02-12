using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTS.Commands;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Rendering
{
    public class DroppedThingRenderData : IModelObjectAddon<DroppedThing>
    {
        private DroppedThing t;
        private Entity ent;
        public DroppedThingRenderData(DroppedThing t)
        {
            this.t = t;
            
            ent = new Engine.WorldRendering.Entity();
            ent.Solid = true;
            ent.Static = false;
            ent.Kinematic = false;
            ent.Tag = t;
            t.set(ent);
        }

        public void Update()
        {
            if (ent.Mesh == null)
                ent.Mesh = t.Thing.CreateMesh();

            //t.InitialPosition = ent.WorldMatrix.xna().Translation.dx();
            ent.WorldMatrix = Matrix.Translation(t.InitialPosition);
        }

        public void Dispose()
        {
            if (ent == null) return;

            ent.Visible = false;
            TW.Data.RemoveObject(ent);
            ent = null;
        }
    }
}