using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.WorldRendering
{
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
    }
}
