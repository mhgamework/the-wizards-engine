using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.World.Rendering
{
    /// <summary>
    /// This class is responsible for rendering a world. It will manage all states on the renderer(s). 
    /// For inputs it requires a (dynamic)world and some kind of camera info(more than just camera, it can contain info about probable movement etc)
    /// </summary>
    public class WorldRenderer
    {
        private readonly WorldNoSectors world;
        private readonly DeferredRenderer renderer;
        private Dictionary<Entity, EntityRenderData> entityRenderDataMap = new Dictionary<Entity, EntityRenderData>();
        public WorldRenderer(WorldNoSectors world, DeferredRenderer renderer)
        {
            this.world = world;
            this.renderer = renderer;
        }

        public void ProcessWorldChanges()
        {
            int length;
            WorldNoSectors.EntityChange[] entityChanges;
            world.GetEntityChanges(out entityChanges, out length);


            for (int i = 0; i < length; i++)
            {
                var change = entityChanges[i];
                switch (change.ChangeType)
                {
                    case WorldNoSectors.WorldChangeType.None:
                        //Huh?
                        throw new InvalidOperationException();
                        break;
                    case WorldNoSectors.WorldChangeType.Added:
                        if (entityRenderDataMap.ContainsKey(change.Entity))
                            throw new InvalidOperationException("Invalid change, entity is already in the renderer");

                        var renderData = new EntityRenderData(change.Entity, renderer);
                        entityRenderDataMap[change.Entity] = renderData;

                        renderData.UpdateRenderData();

                        break;
                    case WorldNoSectors.WorldChangeType.Modified:
                        if (!entityRenderDataMap.ContainsKey(change.Entity))
                            throw new InvalidOperationException("Invalid change, entity is not in the renderer");
                        entityRenderDataMap[change.Entity].UpdateRenderData();
                        break;
                    case WorldNoSectors.WorldChangeType.Removed:
                        if (!entityRenderDataMap.ContainsKey(change.Entity))
                            throw new InvalidOperationException("Invalid change, entity is not in the renderer");
                        entityRenderDataMap[change.Entity].RemoveRenderData();
                        entityRenderDataMap.Remove(change.Entity);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }
    }
}
