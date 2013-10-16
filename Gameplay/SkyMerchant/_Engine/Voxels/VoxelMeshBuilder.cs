using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.Voxels
{
    public class VoxelMeshBuilder
    {
        public IMesh BuildMesh(IFiniteVoxels voxels)
        {
            this.voxels = voxels;
            _builder = new MeshBuilder();

            clearVisible();

            makeSidesVisible();
            floodFill();

            var boxMesh = createMesh();

            return boxMesh;
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
                    var neighbour = voxels.GetVoxel(point3);
                    if (neighbour != null)
                        continue; // face not visible

                    var halfSize = voxels.NodeSize * 0.5f;

                    {
                        var center = block.ToVector3() * voxels.NodeSize + halfSize * MathHelper.One;
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
                boxes.Add(new MeshCollisionData.Box { Dimensions = MathHelper.One.xna() * voxels.NodeSize, Orientation = Matrix.Translation((pos + MathHelper.One * 0.5f) * voxels.NodeSize).xna() });
            }

            boxMesh.GetCollisionData().Boxes.AddRange(boxes);

            return boxMesh;
        }

        private void floodFill()
        {
            visited = new bool[voxels.Size.X, voxels.Size.Y, voxels.Size.Z];

            for (int x = 0; x < voxels.Size.X; x++)
            {
                for (int y = 0; y < voxels.Size.Y; y++)
                {
                    for (int z = 0; z < voxels.Size.Z; z++)
                    {
                        var pos = new Point3(x, y, z);
                        var node = voxels.GetVoxel(pos);
                        if (node != null) continue;
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


                foreach (Point3 neighbourPos in GetNeighbourPositions(curr))
                {
                    var copy = neighbourPos;
                    var block = voxels.GetVoxel(copy);
                    if (voxels.IsOutside(copy))
                    {
                        /*// we have reached the sides!!
                        var otherBlock = TW.Data.GetSingleton<VoxelTerrain>()
                          .GetVoxelAt(TerrainChunk.GetVoxelWorldCenter(neighbourPos));

                        if (otherBlock == null) continue; // end of all teraain!

                            

                            makeBlockVisible(block, neighbourPos); // dont recurse, do make visible*/
                        continue;
                    }

                    if (block != null)
                    {
                        makeBlockVisible(neighbourPos);
                    }
                    else
                    {
                        queue.Enqueue(neighbourPos);
                    }
                }
            }
        }

        private void makeBlockVisible(Point3 pos)
        {
            if (visible[pos.X, pos.Y, pos.Z]) return;
            visible[pos.X, pos.Y, pos.Z] = true;
            visibleBlocks.Add(pos);

        }
        private void makeSidesVisible()
        {
            for (int x = 0; x < voxels.Size.X; x++)
            {
                for (int y = 0; y < voxels.Size.Y; y++)
                {
                    for (int z = 0; z < voxels.Size.Z; z++)
                    {
                        if (x != 0 && x != voxels.Size.X - 1
                            && y != 0 && y != voxels.Size.Y - 1
                            && z != 0 && z != voxels.Size.Z - 1) continue;
                        var pos = new Point3(x, y, z);
                        var material = voxels.GetVoxel(pos);
                        if (material != null)
                            makeBlockVisible(pos);
                    }
                }
            }
        }

        private IFiniteVoxels voxels;



        private bool[, ,] visited;
        private bool[, ,] visible;
        private List<Point3> visibleBlocks = new List<Point3>();


        private MeshBuilder _builder;

        private void clearVisible()
        {
            visible = new bool[voxels.Size.X,voxels.Size.Y,voxels.Size.Z];
            visibleBlocks.Clear();
        }

        public IEnumerable<Point3> GetNeighbourPositions(Point3 curr)
        {
            yield return new Point3(curr.X + 1, curr.Y, curr.Z);
            yield return new Point3(curr.X - 1, curr.Y, curr.Z);
            yield return new Point3(curr.X, curr.Y - 1, curr.Z);
            yield return new Point3(curr.X, curr.Y + 1, curr.Z);
            yield return new Point3(curr.X, curr.Y, curr.Z + 1);
            yield return new Point3(curr.X, curr.Y, curr.Z - 1);
        }

    }
}