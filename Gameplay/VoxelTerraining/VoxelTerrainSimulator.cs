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
            foreach (var terrain in TW.Data.GetChangedObjects<VoxelTerrainChunk>())
            {
                if (terrain.get<RenderData>() == null)
                    terrain.set(new RenderData(terrain));

                terrain.get<RenderData>().Update();

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

            private bool[, ,] visited;
            private List<VoxelBlock> visibleBlocks = new List<VoxelBlock>();


            private MeshBuilder _builder;

            private void floodFill()
            {
                visited = new bool[TerrainChunk.Size.X, TerrainChunk.Size.Y, TerrainChunk.Size.Z];

                for (int x = 0; x < TerrainChunk.Size.X; x++)
                {
                    for (int y = 0; y < TerrainChunk.Size.Y; y++)
                    {
                        for (int z = 0; z < TerrainChunk.Size.Z; z++)
                        {
                            var pos = new Point3(x, y, z);
                            var node = TerrainChunk.GetVoxel(pos);
                            if (node.Filled) continue;
                            if (visited[x, y, z]) continue;
                            floodFillFromNode(pos);


                        }
                    }
                }
            }

            private void floodFillFromNode(Point3 point3)
            {
                var queue = new Queue<Point3>();

                queue.Enqueue(point3);

                while (queue.Count > 0)
                {
                    Point3 curr = queue.Dequeue();
                    if (visited[curr.X, curr.Y, curr.Z])
                        continue;
                    visited[curr.X, curr.Y, curr.Z] = true;

                    foreach (var neighbourPos in TerrainChunk.GetNeighbourPositions(curr))
                    {
                        var block = TerrainChunk.GetVoxel(neighbourPos);
                        if (block == null)
                            continue;

                        if (block.Filled)
                        {
                            makeBlockVisible(block);
                        }
                        else
                        {
                            queue.Enqueue(neighbourPos);
                        }
                    }
                }
            }

            private void makeBlockVisible(VoxelBlock block)
            {
                if (block.Visible) return;

                block.Visible = true;
                visibleBlocks.Add(block);

            }
            private void makeSidesVisible()
            {
                for (int x = 0; x < TerrainChunk.Size.X; x++)
                {
                    for (int y = 0; y < TerrainChunk.Size.Y; y++)
                    {
                        for (int z = 0; z < TerrainChunk.Size.Z; z++)
                        {
                            if (x != 0 && x != TerrainChunk.Size.X - 1
                                //&& y != 0 && y != TerrainChunk.Size.Y - 1
                                && z != 0 && z != TerrainChunk.Size.Z - 1) continue;
                            var voxelBlock = TerrainChunk.GetVoxel(new Point3(x, y, z));
                            if (voxelBlock.Filled)
                                makeBlockVisible(voxelBlock);
                        }
                    }
                }
            }

            public void Update()
            {
                _builder = new MeshBuilder();

                clearVisible();

                //makeSidesVisible(); //This is epic cheat :))))
                floodFill();

                var boxMesh = createMesh();


                _ent.WorldMatrix = Matrix.Translation(TerrainChunk.WorldPosition);
                _ent.Mesh = boxMesh;
                _ent.Solid = true;
                _ent.Static = true;




            }

            private IMesh createMesh()
            {
                var relativeCorePath = "Core\\checker.png";
                //relativeCorePath = "GrassGreenTexture0006.jpg";
                relativeCorePath = "dryvalley.jpg";


                foreach (var block in visibleBlocks)
                {
                    var border = new Vector3(1, 1, 1) * 0.05f;
                    border = new Vector3();

                    var pos = block.Position;
                    _builder.AddBox(pos.ToVector3() * TerrainChunk.NodeSize + border, (MathHelper.One + pos.ToVector3()) * TerrainChunk.NodeSize - border);
                }






                //entities.Add(pos,ent);

                var boxMesh = _builder.CreateMesh();

                boxMesh.GetCoreData().Parts[0].MeshMaterial = new MeshCoreData.Material()
                {
                    DiffuseMap = TW.Assets.LoadTexture(relativeCorePath)
                };

                var boxes = new List<MeshCollisionData.Box>();

                foreach (var block in visibleBlocks)
                {
                    var pos = block.Position;
                    boxes.Add(new MeshCollisionData.Box { Dimensions = MathHelper.One.xna() * TerrainChunk.NodeSize, Orientation = Matrix.Translation((pos + MathHelper.One * 0.5f) * TerrainChunk.NodeSize).xna() });
                }

                boxMesh.GetCollisionData().Boxes.AddRange(boxes);

                return boxMesh;
            }

            private void clearVisible()
            {
                foreach (var b in visibleBlocks)
                {
                    b.Visible = false;
                }
                visibleBlocks.Clear();
            }
        }
    }
}
