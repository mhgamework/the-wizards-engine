using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.Engine.WorldRendering
{
    /// <summary>
    /// Responsible for batching entities that are static into one big entity
    /// </summary>
    public class EntityBatcherSimulator : ISimulator
    {
        private Batch superBatch;

        private float time;
        private float countdown = 0;
        private int count = 0;
        public void Simulate()
        {
            time += TW.Graphics.Elapsed;
            countdown -= TW.Graphics.Elapsed;
            count++;

            TW.Data.EnsureAttachment<Entity, BatchInfo>(e => new BatchInfo());

            foreach (var ent in TW.Data.GetChangedObjects<Entity>())
            {
                ent.get<BatchInfo>().LastChange = time;

                // Batched object was changed, needs unbatch
                if (ent.Batched)
                {
                    destroySuperBatch();

                }

            }
            if (count == 10)
            {
                foreach (var ent in TW.Data.Objects.Where(t => t is Entity).Select(t => (Entity)t))
                {
                    ent.get<BatchInfo>().LastChange = -100;
                }
            }
            if (countdown < 0 || superBatch == null)
            {
                countdown = 5;
                scanForBatches();
            }


        }

        private void scanForBatches()
        {
            // Adding batches changes the entity count, cache this list.
            var buffer = TW.Data.Objects.Where(i => i is Entity).Select(o => (Entity)o).ToArray();
            bool needsSuperbatchUpdate = false;
            foreach (var ent in buffer)
            {
                if (!ent.get<BatchInfo>().ShouldBatch(time) || ent.get<BatchInfo>().Batch != null) continue;

                needsSuperbatchUpdate = true;
                break;
            }
            if (needsSuperbatchUpdate)
                updateSuperBatch();


        }

        private void updateSuperBatch()
        {
            int oldCount = 0;
            if (superBatch != null) oldCount = superBatch.EntityCount;

            var entities = new List<Entity>();

            foreach (var ent in TW.Data.Objects.Where(o => o is Entity).Select(o => (Entity)o))
            {
                if (ent.Mesh == null) continue;
                if (!ent.Visible) continue;
                if (!ent.get<BatchInfo>().ShouldBatch(time)) continue;

                entities.Add(ent);
            }

            if (entities.Count - oldCount <= 20) return;

            destroySuperBatch();
            superBatch = new Batch();
            foreach (var ent in entities)
                superBatch.AddEntity(ent);
            superBatch.Build();

        }

        private void destroySuperBatch()
        {
            if (superBatch == null) return;

            superBatch.Dispose();
            superBatch = null;
        }

        private class Batch : IDisposable
        {
            private List<Entity> entities = new List<Entity>();
            private DeferredMeshRenderElement element;
            public int EntityCount { get { return entities.Count; } }

            public Batch()
            {

            }

            public void AddEntity(Entity ent)
            {
                entities.Add(ent);
            }
            public void RemoveEntity(Entity ent)
            {
                entities.Remove(ent);
            }

            public void Build()
            {
                IMesh mesh = new RAMMesh();

                foreach (var ent in entities)
                {
                    MeshBuilder.AppendMeshTo(ent.Mesh, mesh, ent.WorldMatrix);
                    ent.Batched = true;
                    ent.get<BatchInfo>().Batch = this;
                }
                var optimizer = new MeshOptimizer();
                mesh = optimizer.CreateOptimized(mesh);

                element = TW.Graphics.AcquireRenderer().CreateMeshElement(mesh);

            }

            public void Dispose()
            {
                foreach (var ent in entities)
                {
                    ent.Batched = false;
                    ent.get<BatchInfo>().Batch = null;
                }
                element.Delete();
                element = null;
            }
        }

        private class BatchInfo : IModelObjectAddon<Entity>
        {

            /// <summary>
            /// The batch this entity is in
            /// </summary>
            public Batch Batch;


            public float LastChange = float.MinValue;

            public bool ShouldBatch(float time)
            {
                return time - LastChange > 5;
            }

            public void Dispose()
            {
            }
        }
    }
}
