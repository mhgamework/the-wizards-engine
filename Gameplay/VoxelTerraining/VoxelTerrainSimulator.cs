using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.VoxelTerraining
{
    public class VoxelTerrainSimulator : ISimulator
    {
        public void Simulate()
        {
            foreach (var terrain in TW.Data.GetChangedObjects<VoxelTerrain>())
            {
                if (terrain.get<RenderData>() == null)
                    terrain.set(new RenderData(terrain));


                terrain.get<RenderData>().Update();
            }
        }

        public class RenderData
        {
            public VoxelTerrain Terrain;

            private WorldRendering.Entity _ent;


            public RenderData(VoxelTerrain terrain)
            {
                Terrain = terrain;
                _ent = new WorldRendering.Entity();
            }

            private bool[, ,] visited;
            private List<VoxelBlock> visibleBlocks = new List<VoxelBlock>();

            private MeshBuilder _builder;

            private void floodFill()
            {
                visited = new bool[Terrain.Size.X, Terrain.Size.Y, Terrain.Size.Z];


                var queue = new Queue<Point3>();

                queue.Enqueue(new Point3());

                while (queue.Count > 0)
                {
                    Point3 curr = queue.Dequeue();
                    if (visited[curr.X, curr.Y, curr.Z])
                        continue;
                    visited[curr.X, curr.Y, curr.Z] = true;

                    foreach (var neighbourPos in Terrain.GetNeighbourPositions(curr))
                    {
                        var block = Terrain.GetVoxel(neighbourPos);
                        if (block == null)
                            continue;

                        if (block.Filled)
                        {
                            makeBlockVisible(neighbourPos, block);
                        }
                        else
                        {
                            queue.Enqueue(neighbourPos);
                        }
                    }
                }

            }

            private void makeBlockVisible(Point3 pos, VoxelBlock block)
            {
                if (block.Visible) return;

                block.Visible = true;
                visibleBlocks.Add(block);

                var scale = 0.95f;
                _builder.AddBox(-MathHelper.One * 0.5f * scale + pos.ToVector3(), MathHelper.One * 0.5f * scale + pos.ToVector3());
                //boxMesh.GetCollisionData().Boxes.Add(new MeshCollisionData.Box { Dimensions = MathHelper.One.xna() ,Orientation = Matrix.Translation(pos).xna() });


                //entities.Add(pos,ent);
            }


            public void Update()
            {
                _builder = new MeshBuilder();

                floodFill();

                var boxMesh = _builder.CreateMesh();

                boxMesh.GetCoreData().Parts[0].MeshMaterial = new MeshCoreData.Material() { DiffuseMap = TW.Assets.LoadTexture("Core\\checker.png") };

                _ent.WorldMatrix = Matrix.Identity;
                _ent.Mesh = boxMesh;
            }
        }
    }
}
