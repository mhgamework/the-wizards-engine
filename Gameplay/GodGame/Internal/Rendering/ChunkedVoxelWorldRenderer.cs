using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered._Engine;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Internal.Rendering
{
    public class ChunkedVoxelWorldRenderer : IVoxelWorldRenderer
    {
        public const int ChunkSize = 16;
        private readonly Model.World world;
        private int numChunks;

        private Array2D<Entity> chunkEntities;
        private HashSet<Point2> dirtyChunks = new HashSet<Point2>();

        public ChunkedVoxelWorldRenderer(Model.World world)
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
            flagDirtyChunks();

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
                    if (chunk.Mesh == null || dirtyChunks.Contains(p)) updateChunk(p);
                });

            visible.ForEach(e => e.Visible = true);
            chunkEntities.Values.Except(visible).ForEach(e => e.Visible = false);
        }

        [TWProfile]
        private void flagDirtyChunks()
        {
            //chunkEntities.ForEach((_, p) => changedChunks.Add(p));
            foreach (var v in world.ChangedVoxels)
            {
                dirtyChunks.Add((v.Coord.ToVector2() / ChunkSize).Floor());
            }

        }

        private void updateChunk(Point2 p)
        {
            var oldMesh = chunkEntities[p].Mesh;
            if (oldMesh != null)
                TW.Graphics.AcquireRenderer().ClearMeshCache(oldMesh);

            chunkEntities[p].Mesh = BuildChunkMesh((p.ToVector2() * ChunkSize).Round(),
                                  new Point2(ChunkSize, ChunkSize));

            dirtyChunks.Remove(p);
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
                             * Matrix.Translation(((p.ToVector2() + new Vector2(0.5f)) * world.VoxelSize.X).ToXZ(v.Data.Height));
                MeshBuilder.AppendMeshTo(getMesh(v), batch, vWorld);

            });

            var optimizer = new MeshOptimizer();
            batch = optimizer.CreateOptimized(batch);

            return batch;
        }

        private IMesh getMesh(GameVoxel gameVoxel)
        {
            if (gameVoxel.Type == null) return null;

            var handle = new IVoxelHandle((IVoxel) gameVoxel);

            return gameVoxel.Type.GetMesh(handle);

        }


    }
}