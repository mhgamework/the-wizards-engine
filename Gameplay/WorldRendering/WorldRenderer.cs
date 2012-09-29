using System;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// This class is responsible for rendering a world. It will manage all states on the renderer(s). 
    /// For inputs it requires a (dynamic)world and some kind of camera info(more than just camera, it can contain info about probable movement etc)
    /// TODO: add 'Scenes' to allow for creating multiple simultaneous rendering environments
    /// </summary>
    public class WorldRenderer
    {
        private readonly Data.ModelContainer world;
        private readonly DeferredRenderer renderer;
        public WorldRenderer(Data.ModelContainer world, DeferredRenderer renderer)
        {
            this.world = world;
            this.renderer = renderer;
        }

        public void ProcessWorldChanges()
        {
            int length;
            Data.ModelContainer.ObjectChange[] objectChanges;
            world.GetObjectChanges(out objectChanges, out length);


            for (int i = 0; i < length; i++)
            {
                var change = objectChanges[i];


                if ((change.ModelObject is WorldRendering.Entity))
                    updateEntity(change);
                if ((change.ModelObject is WorldRendering.WireframeBox))
                    updateWireframeBox(change);
            }

        }

        private void updateEntity(Data.ModelContainer.ObjectChange change)
        {
            var ent = (WorldRendering.Entity)change.ModelObject;

            if (change.Change == ModelChange.Added)
            {
                if (ent.get<EntityRenderData>() != null)
                    throw new InvalidOperationException("Invalid change, entity is already in the renderer");

                var renderData = new EntityRenderData(ent, renderer);
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
        private void updateWireframeBox(Data.ModelContainer.ObjectChange change)
        {
            var ent = (WireframeBox)change.ModelObject;

            if (change.Change == ModelChange.Added)
            {
                var el = renderer.CreateLinesElement();

                ent.set(el);

                change.Change = ModelChange.Modified; // also modified :P
            }
            if (change.Change == ModelChange.Modified)
            {
                var el = ent.get<DeferredLinesElement>();
                el.Lines.ClearAllLines();

                if (ent.Visible)
                    el.Lines.AddAABB(new BoundingBox(MathHelper.One * -0.5f, MathHelper.One * 0.5f), ent.WorldMatrix, ent.Color);
            }
            if (change.Change == ModelChange.Removed)
            {
                ent.get<DeferredLinesElement>().Delete();
                ent.set<DeferredLinesElement>(null);
            }
        }

        public void Simulate()
        {
            ProcessWorldChanges();
        }
    }
}
