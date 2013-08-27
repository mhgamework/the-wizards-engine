using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant.Voxels;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Engine.VoxelTerraining
{
    /// <summary>
    /// Renders all VoxelTerrainChunks
    /// </summary>
    public class VoxelTerrainSimulator : ISimulator
    {
        private List<RenderData> chunksToUpdate = new List<RenderData>();

        private List<RenderData> renderDatas = new List<RenderData>();

        public void Simulate()
        {
            foreach (var change in TW.Data.GetChangesOfType<VoxelTerrainChunk>())
            {
                var terrain = (VoxelTerrainChunk)change.ModelObject;

                if (change.Change == ModelChange.Removed)
                {

                    if (terrain.get<RenderData>() != null)
                    {
                        chunksToUpdate.Remove(terrain.get<RenderData>());
                        renderDatas.Remove(terrain.get<RenderData>());
                        terrain.get<RenderData>().Dispose();

                    }
                    continue;
                }

                if (terrain.get<RenderData>() == null)
                {
                    terrain.set(new RenderData(terrain));
                    renderDatas.Add(terrain.get<RenderData>());
                }

                if (!chunksToUpdate.Contains(terrain.get<RenderData>()))
                    chunksToUpdate.Add(terrain.get<RenderData>());
            }
            foreach (var data in renderDatas)
            {
                data.UpdateVisibility();
            }
            var sorted = chunksToUpdate.OrderBy(t => t.camDistance);
            for (int i = 0; i < 1; i++)
            {
                if (!sorted.Any()) break;
                var first = sorted.First();
                chunksToUpdate.Remove(first);
                first.Update();
            }

        }

        public class RenderData
        {
            public VoxelTerrainChunk TerrainChunk;

            private WorldRendering.Entity _ent;


            public RenderData(VoxelTerrainChunk terrainChunk)
            {
                TerrainChunk = terrainChunk;
                _ent = new WorldRendering.Entity();
            }

            private List<Point3> visibleBlocks = new List<Point3>();


            private MeshBuilder _builder;
            public float camDistance;



            public void Update()
            {
                var b = new VoxelMeshBuilder();
                var boxMesh = b.BuildMesh(new VoxelAdapter(TerrainChunk));

                _builder = new MeshBuilder();

                clearVisible();

                _ent.WorldMatrix = Matrix.Translation(TerrainChunk.WorldPosition);
                _ent.Mesh = boxMesh;
                _ent.Solid = true;
                _ent.Static = true;




            }

            private void clearVisible()
            {
                for (int index = 0; index < visibleBlocks.Count; index++)
                {
                    var b = visibleBlocks[index];
                    TerrainChunk.GetVoxelInternal(ref b).Visible = false;
                }

                visibleBlocks.Clear();
            }

            public void Dispose()
            {
                if (_ent != null)
                    TW.Data.RemoveObject(_ent);
            }

            public void UpdateVisibility()
            {
                camDistance = Vector3.DistanceSquared(TerrainChunk.WorldPosition,
                                                      TW.Data.GetSingleton<CameraInfo>()
                                                        .ActiveCamera.ViewInverse.xna()
                                                        .Translation.dx());
            }
        }
    }

    public class VoxelAdapter : IFiniteVoxels
    {
        private readonly VoxelTerrainChunk terrainChunk;
        private VoxelMaterial mat = new VoxelMaterial() { Texture = TW.Assets.LoadTexture("GrassGreenTexture0006.jpg") };
        public VoxelAdapter(VoxelTerrainChunk terrainChunk)
        {
            this.terrainChunk = terrainChunk;
        }

        public void SetVoxel(Point3 pos, VoxelMaterial dirtMaterial)
        {
            throw new NotImplementedException();
        }

        public Point3 Size { get { return terrainChunk.Size; } }
        public float NodeSize { get { return terrainChunk.NodeSize; } }
        public VoxelMaterial GetVoxel(Point3 pos)
        {
            if (IsOutside(pos)) return null;
            return terrainChunk.GetVoxelInternal(pos).Filled ? mat : null;
        }

        public bool IsOutside(Point3 copy)
        {
            for (int i = 0; i < 3; i++)
            {
                if (copy[i] < 0) return true;
                if (copy[i] >= Size[i]) return true;
            }
            return false;
        }
    }
}
