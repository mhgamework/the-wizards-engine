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
    public class RTSRendererSimulator : ISimulator
    {
        public void Simulate()
        {
            updateGoblins();
            UpdateDroppedThings();
            UpdateFactories();
        }

        private static void updateGoblins()
        {
            TW.Data.EnsureAttachment<Goblin, GoblinRenderData>(g => new GoblinRenderData(g));
            foreach (var goblin in TW.Data.GetChangedObjects<Goblin>())
                goblin.get<GoblinRenderData>().fixRendering();
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
                    ent.Solid = true;
                    ent.Static = false;
                    ent.Kinematic = false;
                    ent.Tag = t;
                    t.set(ent);
                }

                if (ent.Mesh == null)
                    ent.Mesh = t.Thing.CreateMesh();

                //t.InitialPosition = ent.WorldMatrix.xna().Translation.dx();
                ent.WorldMatrix = Matrix.Translation(t.InitialPosition);
            }
        }





        private class GoblinRenderData : IModelObjectAddon<Goblin> //: WorldRendering.Entity
        {
            private readonly Goblin goblin;

            public GoblinRenderData(Goblin goblin)
            {
                this.goblin = goblin;
            }

            public Vector3 LastPosition { get; set; }
            public Vector3 LookDirection { get; set; }


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
                  goblin.  HoldingEntity.WorldMatrix = Matrix.Translation(Vector3.UnitZ * 0.5f) * calcGoblinMatrix();
                  goblin.HoldingEntity.Mesh = g.Holding.CreateMesh();
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

            public Matrix calcGoblinMatrix()
            {
                var quat = Functions.CreateFromLookDir(-Vector3.Normalize(LookDirection).xna());

                return Microsoft.Xna.Framework.Matrix.CreateFromQuaternion(quat).dx() * /*Matrix.Scaling(0.01f, 0.01f, 0.01f) **/
                       Matrix.Translation(goblin.Position);
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
                ent.WorldMatrix = calcGoblinMatrix();

                LastPosition = goblin.Position;
                ent.Mesh = TW.Assets.LoadMesh("Core\\Barrel01");//Load("Goblin\\GoblinLowRes");
                ent.Solid = true;
                ent.Static = false;
                ent.Solid = false;

                updateHolding();
            }

            public void Dispose()
            {
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
            builder.AddBox(bb.Minimum, bb.Maximum);
            input.Mesh = builder.CreateMesh();
            input.Mesh.GetCoreData().Parts[0].MeshMaterial.DiffuseMap = factory.InputType.Texture;


            output = new Engine.WorldRendering.Entity();
            builder = new MeshBuilder();
            bb = factory.GetOutputArea();
            bb.Maximum += Vector3.UnitY * (-bb.Maximum.Y + 0.03f);
            builder.AddBox(bb.Minimum, bb.Maximum);
            output.Mesh = builder.CreateMesh();
            output.Mesh.GetCoreData().Parts[0].MeshMaterial.DiffuseMap = factory.OutputType.Texture;


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
