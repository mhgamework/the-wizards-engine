using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
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

            private bool[, ,] visited;
            private List<Point3> visibleBlocks = new List<Point3>();


            private MeshBuilder _builder;
            public float camDistance;

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
                            var node = TerrainChunk.GetVoxelInternal(ref pos);
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


                    foreach (Point3 neighbourPos in TerrainChunk.GetNeighbourPositions(curr))
                    {
                        var copy = neighbourPos;
                        var block = TerrainChunk.GetVoxelInternal(ref copy);
                        if (block == null)
                        {
                            /*// we have reached the sides!!
                            var otherBlock = TW.Data.GetSingleton<VoxelTerrain>()
                              .GetVoxelAt(TerrainChunk.GetVoxelWorldCenter(neighbourPos));

                            if (otherBlock == null) continue; // end of all teraain!

                            

                                makeBlockVisible(block, neighbourPos); // dont recurse, do make visible*/
                            continue;
                        }

                        if (block.Filled)
                        {
                            makeBlockVisible(block, neighbourPos);
                        }
                        else
                        {
                            queue.Enqueue(neighbourPos);
                        }
                    }
                }
            }

            private void makeBlockVisible(VoxelTerrainChunk.Voxel block, Point3 pos)
            {
                if (block.Visible) return;

                block.Visible = true;
                visibleBlocks.Add(pos);

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
                            var pos = new Point3(x, y, z);
                            var voxelBlock = TerrainChunk.GetVoxelInternal(ref pos);
                            if (voxelBlock.Filled)
                                makeBlockVisible(voxelBlock, pos);
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
                relativeCorePath = "GrassGreenTexture0006.jpg";
                //relativeCorePath = "dryvalley.jpg";


                var dirs = new List<Vector3>();
                var ups = new List<Vector3>();
                var rights = new List<Vector3>();
                dirs.Add(Vector3.UnitX);
                dirs.Add(Vector3.UnitY);
                dirs.Add(Vector3.UnitZ);
                dirs.Add(-Vector3.UnitX);
                dirs.Add(-Vector3.UnitY);
                dirs.Add(-Vector3.UnitZ);

                ups.Add(Vector3.UnitY);
                ups.Add(Vector3.UnitX);
                ups.Add(Vector3.UnitY);
                ups.Add(Vector3.UnitY);
                ups.Add(Vector3.UnitX);
                ups.Add(Vector3.UnitY);


                for (int i = 0; i < dirs.Count; i++)
                {
                    rights.Add(Vector3.Cross(ups[i], dirs[i]));
                }


                var nPos = new Vector3[6];
                var nNormals = new Vector3[6];
                var nUvs = new Vector2[6];

                foreach (var block in visibleBlocks)
                {
                    var border = new Vector3(1, 1, 1) * 0.05f;
                    border = new Vector3();

                    var pos = block;
                    for (int i = 0; i < dirs.Count; i++)
                    {
                        var dir = dirs[i];
                        var point3 = block + new Point3(dir);
                        var neighbour = TerrainChunk.GetVoxelInternal(ref point3);
                        if (neighbour != null && neighbour.Filled)
                            continue; // face not visible

                        var halfSize = TerrainChunk.NodeSize * 0.5f;

                        {
                            var center = block.ToVector3() + halfSize * MathHelper.One;
                            dir *= halfSize;
                            var right = rights[i] * halfSize;
                            var up = ups[i] * halfSize;

                            var j = 0;

                            nPos[j] = center + dir + up + right;
                            j++;
                            nPos[j] = center + dir + up - right;
                            j++;
                            nPos[j] = center + dir - up - right;
                            j++;

                            nPos[j] = center + dir - up - right;
                            j++;
                            nPos[j] = center + dir - up + right;
                            j++;
                            nPos[j] = center + dir + up + right;
                            j++;
                        }


                        {
                            var j = 0;
                            var center = new Vector2(0.5f, 0.5f);
                            var up = new Vector2(0.5f, 0);
                            var right = new Vector2(0, 0.5f);
                            nUvs[j] = center + up + right; j++;
                            nUvs[j] = center + up - right; j++;
                            nUvs[j] = center - up - right; j++;

                            nUvs[j] = center - up - right; j++;
                            nUvs[j] = center - up + right; j++;
                            nUvs[j] = center + up + right; j++;
                        }
                        for (int k = 0; k < dirs.Count; k++)
                        {
                            nNormals[k] = dirs[i];
                        }

                        _builder.AddCustom(nPos, nNormals, nUvs);
                    }
                    //_builder.AddBox(pos.ToVector3() * TerrainChunk.NodeSize + border, (MathHelper.One + pos.ToVector3()) * TerrainChunk.NodeSize - border);
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
                    var pos = block;
                    boxes.Add(new MeshCollisionData.Box { Dimensions = MathHelper.One.xna() * TerrainChunk.NodeSize, Orientation = Matrix.Translation((pos + MathHelper.One * 0.5f) * TerrainChunk.NodeSize).xna() });
                }

                boxMesh.GetCollisionData().Boxes.AddRange(boxes);

                return boxMesh;
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
}
