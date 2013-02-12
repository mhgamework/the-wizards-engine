using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTS;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Rendering
{
    public class GoblinRenderData : IModelObjectAddon<Goblin> //: WorldRendering.Entity
    {
        private readonly Goblin goblin;

        public GoblinRenderData(Goblin goblin)
        {
            this.goblin = goblin;
        }

        public Vector3 LastPosition { get; set; }
        public Vector3 LookDirection { get { return goblin.LookDirection; } set { goblin.LookDirection = value; } }


        internal void updateHolding()
        {
            var g = goblin;
            if (g.Holding == null)
            {
                disposeHoldingEntity();
            }
            else
            {
                createHoldingEntity();
                goblin.HoldingEntity.WorldMatrix = goblin.CalculateHoldingMatrix();
                goblin.HoldingEntity.Mesh = g.Holding.CreateMesh();
                goblin.HoldingEntity.Kinematic = true;
                goblin.HoldingEntity.Solid = true;
            }
        }

        private void createHoldingEntity()
        {
            if (goblin.HoldingEntity != null) return;
            goblin.HoldingEntity = new Engine.WorldRendering.Entity();

        }

        private void disposeHoldingEntity()
        {
            if (goblin.HoldingEntity == null) return;
            TW.Data.Objects.Remove(goblin.HoldingEntity);
            goblin.HoldingEntity.Visible = false;
            goblin.HoldingEntity = null;
        }



        public void fixRendering()
        {
            if (goblin.get<Engine.WorldRendering.Entity>() == null)
            {
                if (goblin.GoblinEntity == null)
                    goblin.GoblinEntity = new Engine.WorldRendering.Entity();
                goblin.set(goblin.GoblinEntity);
            }
            var ent = goblin.GoblinEntity;
            ent.Tag = goblin;

            var diff = (LastPosition - goblin.Position);
            diff.Y = 0;
            if (diff.Length() > 0.01f)
            {
                LookDirection = -diff;
            }
            LastPosition = goblin.Position;
            ent.WorldMatrix = goblin.calcGoblinMatrix();

            LastPosition = goblin.Position;
            ent.Mesh = TW.Assets.LoadMesh("Core\\Barrel01");//Load("Goblin\\GoblinLowRes");
            ent.Solid = true;
            ent.Static = false;
            ent.Solid = false;

            updateHolding();
        }

        public void Dispose()
        {
            TW.Data.Objects.Remove(goblin.GoblinEntity);
        }
    }

}