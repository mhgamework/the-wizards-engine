using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    [Serializable]
    public class HermiteDataGrid : AbstractHermiteGrid
    {
        private Array3D<Vertex> cells;

        private List<Point3> dirs = new List<Point3>(new Point3[] { new Point3(1, 0, 0), new Point3(0, 1, 0), new Point3(0, 0, 1) });

        private HermiteDataGrid()
            : base()
        {

        }

        public override bool GetSign(Point3 pos)
        {
            var v = cells[pos];
            if (cells == null) return false;
            return v.Sign;
        }

        public override Point3 Dimensions
        {
            get { return cells.Size; }
        }

        [Serializable]
        private struct Vertex
        {
            public bool Sign;
            public Vector4[] EdgeData;
        }


        public override Vector4 getEdgeData(Point3 cube, int edgeId)
        {
            var start = cube_verts[GetEdgeVertexIds(edgeId)[0]];
            var end = cube_verts[GetEdgeVertexIds(edgeId)[1]];

            return cells[cube + start].EdgeData[dirs.IndexOf(end - start)];
        }


        public static HermiteDataGrid FromIntersectableGeometry(float gridWorldSize, int resolution, Matrix world, IIntersectableObject obj)
        {
            var numCubes = new Point3(resolution, resolution, resolution);
            var grid = new HermiteDataGrid();

            var gridToWorld = Matrix.Scaling(1f / resolution, 1f / resolution, 1f / resolution) * Matrix.Scaling(new Vector3(gridWorldSize));
            var gridToGeometry = gridToWorld * Matrix.Invert(world);
            var geometryToGrid = Matrix.Invert(gridToGeometry);


            // Fill vertices
            grid.cells = new Array3D<Vertex>(numCubes + new Point3(1, 1, 1));
            grid.ForEachCube(cube =>
                {
                    grid.cells[cube] = new Vertex()
                        {
                            Sign = obj.IsInside(Vector3.TransformCoordinate(cube.ToVector3(), gridToGeometry)),
                            EdgeData = new Vector4[3]
                        };
                });

            // Find changing edges and calculate edge info
            grid.ForEachCube(cube =>
                {
                    var sign = grid.GetSign(cube);

                    for (int i = 0; i < grid.dirs.Count; i++)
                    {
                        if (sign == grid.GetSign(cube + grid.dirs[i])) continue;
                        //sign difference
                        var vector4 = obj.GetIntersection(Vector3.TransformCoordinate(cube, gridToGeometry), Vector3.TransformCoordinate(cube + grid.dirs[i], gridToGeometry));
                        vector4 = new Vector4(Vector3.Normalize(Vector3.TransformNormal(vector4.TakeXYZ(), geometryToGrid)), vector4.W);
                        grid.cells[cube].EdgeData[i] = vector4;
                    }
                });

            return grid;
        }

        public static HermiteDataGrid CopyGrid(AbstractHermiteGrid grid)
        {
            var ret = new HermiteDataGrid();
            ret.cells = new Array3D<Vertex>(grid.Dimensions);
            ret.ForEachCube(p =>
                {
                    ret.cells[p] = new Vertex()
                        {
                            Sign = grid.GetSign(p),
                            /*EdgeData = ret.dirs.Select(dir =>
                                {
                                    var edgeId = grid.GetEdgeId(p, p + dir);
                                    return grid.HasEdgeData(p, edgeId) ? grid.getEdgeData(p, edgeId) : new Vector4();
                                }).ToArray()*/
                        };
                });

            ret.ForEachCube(p =>
            {
                ret.cells[p] = new Vertex()
                {
                    Sign = ret.cells[p].Sign,
                    EdgeData = ret.dirs.Select(dir =>
                        {
                            Point3 endPoint = p + dir;
                            var edgeId = grid.GetEdgeId(p, endPoint);
                            // Optimization: direclty read from already constructed data
                            if (ret.cells[p].Sign == ret.cells[endPoint].Sign) return new Vector4();
                            if (!ret.cells.InArray(endPoint)) return new Vector4();
                            if (!grid.HasEdgeData(p, edgeId))
                            {
                                // This can normally not happen, since we check if there is a sign difference by looking at the already evaluated density points.
                                //  If this would be true there is some problem with the manual determining of the existence of an edge.
                                //throw new InvalidOperationException();
                                return new Vector4();
                            }
                            return grid.getEdgeData(p, edgeId);
                        }).ToArray()
                };
            });

            return ret;
        }

        public static HermiteDataGrid LoadFromFile(FileInfo fi)
        {
            var ret = new HermiteDataGrid();
            ret.Load(fi);
            return ret;
        }


        public void Save(FileInfo fi)
        {
            using (var fs = new BinaryWriter(fi.Create()))
            {
                fs.Write("HermiteDataGrid format V1.00. Layout 3D array of (boolean sign, Vector4 edgedata) layout out in x,y,z for loop (so first z then y then x)");
                fs.Write(cells.Size.X);
                fs.Write(cells.Size.Y);
                fs.Write(cells.Size.Z);
                for (int x = 0; x < cells.Size.X; x++)
                    for (int y = 0; y < cells.Size.Y; y++)
                        for (int z = 0; z < cells.Size.Z; z++)
                        {
                            var val = cells[new Point3(x, y, z)];
                            fs.Write(val.Sign);
                            fs.Write(val.EdgeData.Length);
                            for (int i = 0; i < val.EdgeData.Length; i++)
                            {
                                var edge = val.EdgeData[i];
                                fs.Write(edge.X);
                                fs.Write(edge.Y);
                                fs.Write(edge.Z);
                                fs.Write(edge.W);
                            }
                        }

            }

            /*using (var fs = new StreamWriter(fi.Create()))
            {
                cells.ForEach((v, p) =>
                    {
                        fs.WriteLine("Cell: " + p);
                        fs.WriteLine(v.Sign);
                        if (v.EdgeData != null)
                        {
                            foreach (var edge in v.EdgeData)
                                fs.WriteLine(edge);

                        }
                    });
            }*/

        }

        public void Load(FileInfo fi)
        {
            using (var fs = new BinaryReader(fi.OpenRead()))
            {
                var format = fs.ReadString();
                var size = new Point3(fs.ReadInt32(), fs.ReadInt32(), fs.ReadInt32());
                cells = new Array3D<Vertex>(size);
                for (int x = 0; x < cells.Size.X; x++)
                    for (int y = 0; y < cells.Size.Y; y++)
                        for (int z = 0; z < cells.Size.Z; z++)
                        {
                            var val = new Vertex();
                            val.Sign = fs.ReadBoolean();
                            val.EdgeData = new Vector4[fs.ReadInt32()];
                            for (int i = 0; i < val.EdgeData.Length; i++)
                            {
                                val.EdgeData[i] = new Vector4(fs.ReadSingle(), fs.ReadSingle(), fs.ReadSingle(), fs.ReadSingle());
                            }
                            cells[new Point3(x, y, z)] = val;
                        }

            }
        }

    }
}