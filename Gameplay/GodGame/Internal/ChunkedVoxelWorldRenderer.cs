using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered._Engine;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class ChunkedVoxelWorldRenderer : IVoxelWorldRenderer
    {
        public const int ChunkSize = 16;
        private readonly World world;
        private int numChunks;

        private Array2D<Entity> chunkEntities;

        public ChunkedVoxelWorldRenderer(World world)
        {
            this.world = world;

            numChunks = world.WorldSize / ChunkSize + 1;

            chunkEntities = new Array2D<Entity>(new Point2(numChunks, numChunks));
            chunkEntities.ForEach((_, p) =>
                {
                    chunkEntities[p] = new Entity();
                    chunkEntities[p].WorldMatrix = Matrix.Translation((p.ToVector2() * ChunkSize * world.VoxelSize.X).ToXZ());
                });
        }

        [TWProfile]
        public void UpdateWindow(Point2 offset, Vector3 worldTranslation, Point2 windowSize)
        {
            updateChangedChunks();

            updateVisibleChunks(offset, windowSize);
        }

        [TWProfile]
        private void updateVisibleChunks(Point2 offset, Point2 windowSize)
        {
            windowSize = ((offset + windowSize).ToVector2() / ChunkSize).Ceiling();
            offset = (offset.ToVector2() / ChunkSize).Floor();
            windowSize -= offset;

            chunkEntities.ForEach((e, _) => e.Visible = false);
            var visible = new List<Entity>();
            windowSize.ForEach(p =>
                {
                    p += offset;
                    var chunk = chunkEntities[p];
                    if (chunk == null) return;
                    visible.Add(chunkEntities[p]);
                    if (chunk.Mesh == null) updateChunk(p);
                });

            visible.ForEach(e => e.Visible = true);
            chunkEntities.Values.Except(visible).ForEach(e => e.Visible = false);
        }

        [TWProfile]
        private void updateChangedChunks()
        {
            var changedChunks = new HashSet<Point2>();

            //chunkEntities.ForEach((_, p) => changedChunks.Add(p));
            foreach (var v in world.ChangedVoxels)
            {
                changedChunks.Add((v.Coord.ToVector2() / ChunkSize).Floor());
            }

            changedChunks.ForEach(updateChunk);
        }

        private void updateChunk(Point2 p)
        {
            var oldMesh = chunkEntities[p].Mesh;
            if (oldMesh != null)
                TW.Graphics.AcquireRenderer().ClearMeshCache(oldMesh);

            chunkEntities[p].Mesh = BuildChunkMesh((p.ToVector2() * ChunkSize).Round(),
                                  new Point2(ChunkSize, ChunkSize));
        }

        [TWProfile]
        private IMesh BuildChunkMesh(Point2 offset, Point2 size)
        {
            IMesh batch = new RAMMesh();
            size.ForEach(p =>
            {
                var v = world.GetVoxel(p + new Point2(offset));
                if (v == null) return;
                var mesh = getMesh(v);
                if (mesh == null) return;
                var vWorld = Matrix.Scaling(new Vector3(world.VoxelSize.X))
                    * Matrix.Translation(world.GetBoundingBox(p).GetCenter());
                MeshBuilder.AppendMeshTo(getMesh(v), batch, vWorld);

            });

            var optimizer = new MeshOptimizer();
            batch = optimizer.CreateOptimized(batch);

            return batch;
        }

        private IMesh getMesh(GameVoxel gameVoxel)
        {
            if (gameVoxel.Type == null) return null;

            var handle = new IVoxelHandle(world, gameVoxel);

            return gameVoxel.Type.GetMesh(handle);

        }


    }
}