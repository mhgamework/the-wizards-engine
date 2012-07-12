using System;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// This class is responsible for rendering a world. It will manage all states on the renderer(s). 
    /// For inputs it requires a (dynamic)world and some kind of camera info(more than just camera, it can contain info about probable movement etc)
    /// </summary>
    public class WorldRenderer 
    {
        private readonly ModelContainer.ModelContainer world;
        private readonly DeferredRenderer renderer;
        public WorldRenderer(ModelContainer.ModelContainer world, DeferredRenderer renderer)
        {
            this.world = world;
            this.renderer = renderer;
        }

        public void ProcessWorldChanges()
        {
            int length;
            ModelContainer.ModelContainer.ObjectChange[] objectChanges;
            world.GetEntityChanges(out objectChanges, out length);


            for (int i = 0; i < length; i++)
            {
                var change = objectChanges[i];


                if ((change.ModelObject is WorldRendering.Entity))
                    updateEntity(change);
                if ((change.ModelObject is WorldRendering.WireframeBox))
                    updateWireframeBox(change);
            }

        }

        private void updateEntity(ModelContainer.ModelContainer.ObjectChange change)
        {
            var ent = (WorldRendering.Entity) change.ModelObject;

                if (change.Change ==  ModelChange.Added)
                {
                    if (ent.get<EntityRenderData>() != null)
                        throw new InvalidOperationException("Invalid change, entity is already in the renderer");

                    var renderData = new EntityRenderData(ent, renderer);
                    ent.set<EntityRenderData>(renderData);

                    renderData.UpdateRenderData();

                }
            if(change.Change ==  ModelChange.Modified || change.Change == ModelChange.Added)
            {
                if (ent.get<EntityRenderData>() == null)
                    throw new InvalidOperationException("Invalid change, entity is not in the renderer");
                ent.get<EntityRenderData>().UpdateRenderData();
            }
            if (change.Change ==  ModelChange.Removed)
            {
                if (ent.get<EntityRenderData>() == null)
                    throw new InvalidOperationException("Invalid change, entity is not in the renderer");
                ent.get<EntityRenderData>().RemoveRenderData();
                ent.set<EntityRenderData>(null);
            }
        }
        private void updateWireframeBox(ModelContainer.ModelContainer.ObjectChange change)
        {
            var ent = (WorldRendering.Entity) change.ModelObject;

                if (change.Change ==  ModelChange.Added)
                {
                    if (ent.get<EntityRenderData>() != null)
                        throw new InvalidOperationException("Invalid change, entity is already in the renderer");

                    var renderData = new EntityRenderData(ent, renderer);
                    ent.set<EntityRenderData>(renderData);

                    renderData.UpdateRenderData();

                }
            if(change.Change ==  ModelChange.Modified || change.Change == ModelChange.Added)
            {
                if (ent.get<EntityRenderData>() == null)
                    throw new InvalidOperationException("Invalid change, entity is not in the renderer");
                ent.get<EntityRenderData>().UpdateRenderData();
            }
            if (change.Change ==  ModelChange.Removed)
            {
                if (ent.get<EntityRenderData>() == null)
                    throw new InvalidOperationException("Invalid change, entity is not in the renderer");
                ent.get<EntityRenderData>().RemoveRenderData();
                ent.set<EntityRenderData>(null);
            }
        }

        public void Simulate()
        {
            ProcessWorldChanges();
        }
    }
}
