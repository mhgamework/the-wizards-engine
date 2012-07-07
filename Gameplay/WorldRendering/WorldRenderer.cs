using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards._XNA.World.Rendering
{
    /// <summary>
    /// This class is responsible for rendering a world. It will manage all states on the renderer(s). 
    /// For inputs it requires a (dynamic)world and some kind of camera info(more than just camera, it can contain info about probable movement etc)
    /// </summary>
    public class WorldRenderer : ISimulator
    {
        private readonly ModelContainer.ModelContainer world;
        private readonly DeferredRenderer renderer;
        private Dictionary<WorldRendering.Entity, EntityRenderData> entityRenderDataMap = new Dictionary<WorldRendering.Entity, EntityRenderData>();
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


                if (!(change.ModelObject is WorldRendering.Entity))
                    continue;

                var ent = (WorldRendering.Entity)change.ModelObject;

                switch (change.ChangeType)
                {
                    case ModelContainer.ModelContainer.WorldChangeType.None:
                        //Huh?
                        throw new InvalidOperationException();
                    case ModelContainer.ModelContainer.WorldChangeType.Added:
                        if (entityRenderDataMap.ContainsKey(ent))
                            throw new InvalidOperationException("Invalid change, entity is already in the renderer");

                        var renderData = new EntityRenderData(ent, renderer);
                        entityRenderDataMap[ent] = renderData;

                        renderData.UpdateRenderData();

                        break;
                    case ModelContainer.ModelContainer.WorldChangeType.Modified:
                        if (!entityRenderDataMap.ContainsKey(ent))
                            throw new InvalidOperationException("Invalid change, entity is not in the renderer");
                        entityRenderDataMap[ent].UpdateRenderData();
                        break;
                    case ModelContainer.ModelContainer.WorldChangeType.Removed:
                        if (!entityRenderDataMap.ContainsKey(ent))
                            throw new InvalidOperationException("Invalid change, entity is not in the renderer");
                        entityRenderDataMap[ent].RemoveRenderData();
                        entityRenderDataMap.Remove(ent);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }

        public void Simulate()
        {
            ProcessWorldChanges();
        }
    }
}
