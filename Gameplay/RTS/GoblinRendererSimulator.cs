using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.RTS.Commands;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using Quaternion = Microsoft.Xna.Framework.Quaternion;

namespace MHGameWork.TheWizards.RTS
{
    public class GoblinRendererSimulator : ISimulator
    {
        public void Simulate()
        {
            foreach (var goblin in TW.Data.GetChangedObjects<Goblin>())
            {
                if (goblin.get<GoblinRenderData>() == null)
                    goblin.set(new GoblinRenderData { LookDirection = new Vector3(0, 0, 1) });
                var ent = goblin.get<Engine.WorldRendering.Entity>();
                fixRendering(goblin);
            }

            UpdateDroppedThings();
            UpdateFactories();

        }

        private void UpdateFactories()
        {
            TW.Data.EnsureAttachment<Factory, FactoryRenderData>(o => new FactoryRenderData(o));
            foreach (var f in TW.Data.GetChangedObjects<Factory>())
            {
                f.get<FactoryRenderData>().Update();
            }
        }

        private static void UpdateDroppedThings()
        {
            foreach (var c in TW.Data.GetChangesOfType<DroppedThing>())
            {
                var t = c.ModelObject as DroppedThing;
                var ent = t.get<Engine.WorldRendering.Entity>();

                if (c.Change == ModelChange.Removed)
                {
                    if (ent != null)
                    {
                        ent.Visible = false;
                        TW.Data.RemoveObject(ent);
                    }
                    t.set<Engine.WorldRendering.Entity>(null);
                    continue;
                }

                if (ent == null)
                {
                    ent = new Engine.WorldRendering.Entity();
                    t.set(ent);
                }

                if (ent.Mesh == null)
                {
                    var builder = new MeshBuilder();
                    builder.AddBox(MathHelper.One * -0.2f, MathHelper.One * 0.2f);
                    ent.Mesh = builder.CreateMesh();
                }

                ent.WorldMatrix = Matrix.Translation(t.Position + Vector3.UnitY * 0.2f);
            }
        }

        private static void fixRendering(Goblin goblin)
        {
            if (goblin.get<Engine.WorldRendering.Entity>() == null)
                goblin.set(new Engine.WorldRendering.Entity());
            var ent = goblin.get<Engine.WorldRendering.Entity>();

            ent.Tag = goblin;

            var renderData = goblin.get<GoblinRenderData>();
            var diff = (renderData.LastPosition - goblin.Position);
            diff.Y = 0;
            if (diff.Length() > 0.01f)
            {
                renderData.LookDirection = -diff;
            }
            renderData.LastPosition = goblin.Position;
            ent.WorldMatrix = renderData.calcGoblinMatrix(goblin);

            renderData.LastPosition = goblin.Position;
            ent.Mesh = MeshFactory.Load("Core\\Barrel01");//Load("Goblin\\GoblinLowRes");
            ent.Solid = true;
            ent.Static = false;

            renderData.updateHolding(goblin);
        }



        private class GoblinRenderData//: WorldRendering.Entity
        {
            public Vector3 LastPosition { get; set; }
            public Vector3 LookDirection { get; set; }

            public Engine.WorldRendering.Entity HoldingEntity { get; set; }

            internal void updateHolding(Goblin g)
            {
                if (g.Holding == null)
                {
                    disposeHoldingEntity();
                }
                else
                {
                    createHoldingEntity();
                    HoldingEntity.WorldMatrix = Matrix.Scaling(0.4f, 0.4f, 0.4f) * Matrix.Translation(Vector3.UnitZ * 0.5f) * calcGoblinMatrix(g);
                }
            }

            private void createHoldingEntity()
            {
                if (HoldingEntity != null) return;
                HoldingEntity = new Engine.WorldRendering.Entity();
                var builder = new MeshBuilder();
                builder.AddBox(MathHelper.One * -0.5f, MathHelper.One * 0.5f);
                HoldingEntity.Mesh = builder.CreateMesh();

            }

            private void disposeHoldingEntity()
            {
                if (HoldingEntity == null) return;
                TW.Data.Objects.Remove(HoldingEntity);
                HoldingEntity.Visible = false;
                HoldingEntity = null;
            }

            public Matrix calcGoblinMatrix(Goblin goblin)
            {
                var quat = Functions.CreateFromLookDir(-Vector3.Normalize(LookDirection).xna());

                return Microsoft.Xna.Framework.Matrix.CreateFromQuaternion(quat).dx() * /*Matrix.Scaling(0.01f, 0.01f, 0.01f) **/
                       Matrix.Translation(goblin.Position);
            }
        }
    }

    public class FactoryRenderData : IModelObjectAddon<Factory>
    {
        private readonly Factory factory;
        private Engine.WorldRendering.Entity main;
        private Engine.WorldRendering.Entity input;
        private Engine.WorldRendering.Entity output;
        public FactoryRenderData(Factory factory)
        {
            this.factory = factory;
            main = new Engine.WorldRendering.Entity();
            var builder = new MeshBuilder();
            builder.AddBox(new Vector3(-1, 0, -1), new Vector3(1, 3, 1));
            main.Mesh = builder.CreateMesh();


            input = new Engine.WorldRendering.Entity();
            builder = new MeshBuilder();
            var bb = factory.GetInputArea();
            bb.Maximum += Vector3.UnitY * (-bb.Maximum.Y + 0.03f);
            builder.AddBox(bb.Minimum,bb.Maximum);
            input.Mesh = builder.CreateMesh();


            output = new Engine.WorldRendering.Entity();
            builder = new MeshBuilder();
            bb = factory.GetOutputArea();
            bb.Maximum += Vector3.UnitY*(-bb.Maximum.Y + 0.03f);
            builder.AddBox(bb.Minimum,bb.Maximum);
            output.Mesh = builder.CreateMesh();

            
        }

        public void Dispose()
        {
            TW.Data.Objects.Remove(main);
        }

        public void Update()
        {
            main.WorldMatrix = Matrix.Translation(factory.Position);
        }
    }
}
