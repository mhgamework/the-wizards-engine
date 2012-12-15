using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// Responsible for updating the renderer with information from Entity
    /// </summary>
    public class EntitySimulator : ISimulator
    {
        public void Simulate()
        {
            foreach (var change in TW.Data.GetChangesOfType<Entity>())
            {
                updateEntity(change);
            }
        }
        private void updateEntity(Data.ModelContainer.ObjectChange change)
        {
            var ent = (WorldRendering.Entity)change.ModelObject;

            if (change.Change == ModelChange.Added)
            {
                if (ent.get<EntityRenderData>() != null)
                    throw new InvalidOperationException("Invalid change, entity is already in the renderer");

                var renderData = new EntityRenderData(ent, TW.Graphics.AcquireRenderer());
                ent.set<EntityRenderData>(renderData);

                renderData.UpdateRenderData();

            }
            if (change.Change == ModelChange.Modified || change.Change == ModelChange.Added)
            {
                if (ent.get<EntityRenderData>() == null)
                    throw new InvalidOperationException("Invalid change, entity is not in the renderer");
                ent.get<EntityRenderData>().UpdateRenderData();
            }
            if (change.Change == ModelChange.Removed)
            {
                if (ent.get<EntityRenderData>() == null)
                    throw new InvalidOperationException("Invalid change, entity is not in the renderer");
                ent.get<EntityRenderData>().RemoveRenderData();
                ent.set<EntityRenderData>(null);
            }
        }
        public class EntityRenderData
        {
            private readonly WorldRendering.Entity entity;
            private readonly DeferredRenderer renderer;

            private DeferredMeshRenderElement element;

            public EntityRenderData(WorldRendering.Entity entity, DeferredRenderer renderer)
            {
                this.entity = entity;
                this.renderer = renderer;
            }

            public void UpdateRenderData()
            {
                createElement();
                updateElement();
            }

            private void createElement()
            {
                if (element != null && element.Mesh == entity.Mesh) return; // already ok
                RemoveRenderData();
                if (entity.Mesh == null) return;
                element = renderer.CreateMeshElement(entity.Mesh);
            }

            private void updateElement()
            {
                if (element == null) return;
                element.WorldMatrix = entity.WorldMatrix;
                element.Visible = entity.Visible && !entity.Batched;
            }

            public void RemoveRenderData()
            {
                if (element != null)
                    element.Delete();
                element = null;
            }
        }
    }
}
