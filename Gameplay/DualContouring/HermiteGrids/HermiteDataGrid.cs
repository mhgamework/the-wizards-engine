using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// //TODO: serious optimization idea: store all signs for each cube in each cube, this is the limiting factor in the QEF calculation
    /// </summary>
    [Serializable]
    public class HermiteDataGrid : AbstractHermiteGrid
    {
        /// <summary>
        /// There are Dimensions + 1 cells (for holding the normals on the boundaries)
        /// </summary>
        private Array3D<Vertex> cells;

        private Point3[] dirs = new Point3[] { new Point3(1, 0, 0), new Point3(0, 1, 0), new Point3(0, 0, 1) };
        private OffsetInfo[] offsetInfos;
        private HermiteDataGrid()
            : base()
        {
            // Construct offset info
            offsetInfos = GetAllEdgeIds().Select(edgeId =>
                {
                    var offsets = GetEdgeOffsets(edgeId);

                    var dir = offsets[1] - offsets[0];
                    if (dir.X == 1 && dir.Y == 0 && dir.Z == 0)
                        edgeId = 0;
                    else if (dir.X == 0 && dir.Y == 1 && dir.Z == 0)
                        edgeId = 1;
                    else if (dir.X == 0 && dir.Y == 0 && dir.Z == 1)
                        edgeId = 2;
                    else throw new InvalidOperationException();
                    return new OffsetInfo() { Offset = offsets[0], dataEdgeId = edgeId };
                }).ToArray();
        }

        public override bool GetSign(Point3 pos)
        {
            var v = cells[pos];
            if (cells == null) throw new InvalidOperationException("Outside of cell bounds!!");
            return v.Sign;
        }

        public override Point3 Dimensions
        {
            get { return cells.Size - new Point3(1, 1, 1); }
        }

        [Serializable]
        private struct Vertex
        {
            public bool Sign;
            public DCVoxelMaterial Material;
            public Vector4[] EdgeData;
        }
        /// <summary>
        /// THe offset to apply on the given (cube+edgeID), to get the cube on which we can read the data by reading 'dataedgeid'
        /// </summary>
        private struct OffsetInfo
        {

            public Point3 Offset;
            public int dataEdgeId;
        }


        public override Vector4 getEdgeData(Point3 cube, int edgeId)
        {
            var offsetInfo = offsetInfos[edgeId];
            return cells[cube + offsetInfo.Offset].EdgeData[offsetInfo.dataEdgeId];
            /*var start = cube_verts[GetEdgeVertexIds(edgeId)[0]];
            var end = cube_verts[GetEdgeVertexIds(edgeId)[1]];

            return cells[cube + start].EdgeData[dirs.IndexOf(end - start)];*/
        }

        public override DCVoxelMaterial GetMaterial(Point3 cube)
        {
            return cells[cube].Material;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridWorldSize">Size of the grid in world space</param>
        /// <param name="resolution">Aka the subdivision of the grid</param>
        /// <param name="world">Worldmatrix of the object</param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HermiteDataGrid FromIntersectableGeometry(float gridWorldSize, int resolution, Matrix world, IIntersectableObject obj)
        {
            var numCubes = new Point3(resolution, resolution, resolution);
            var grid = new HermiteDataGrid();

            var gridToWorld = Matrix.Scaling(1f / resolution, 1f / resolution, 1f / resolution) * Matrix.Scaling(new Vector3(gridWorldSize));
            var gridToGeometry = gridToWorld * Matrix.Invert(world);
            var geometryToGrid = Matrix.Invert(gridToGeometry);


            // Fill vertices
            grid.cells = new Array3D<Vertex>(numCubes + new Point3(1, 1, 1));
            grid.ForEachGridPoint(cube =>
                {
                    grid.cells[cube] = new Vertex()
                        {
                            Sign = obj.IsInside(Vector3.TransformCoordinate(cube.ToVector3(), gridToGeometry)),
                            EdgeData = new Vector4[3]
                        };
                });

            // Find changing edges and calculate edge info
            grid.ForEachGridPoint(cube =>
                {
                    var sign = grid.GetSign(cube);

                    for (int i = 0; i < grid.dirs.Length; i++)
                    {
                        Point3 start = cube;
                        Point3 end = cube + grid.dirs[i];
                        if (sign == grid.GetSign(end)) continue;
                        //sign difference


                        Vector3 startT = Vector3.TransformCoordinate(start, gridToGeometry);
                        Vector3 endT = Vector3.TransformCoordinate(end, gridToGeometry);
                        var vector4 = obj.GetIntersection(startT, endT);
                        var intersectionPoint = Vector3.TransformCoordinate(Vector3.Lerp(startT, endT, vector4.W), geometryToGrid);
                        var realLerp = Vector3.Distance(start, intersectionPoint); // /1
                        Debug.Assert(Math.Abs(Vector3.Distance(start, end) - 1) < 0.001); // Algo check
                        vector4 = new Vector4(Vector3.Normalize(Vector3.TransformNormal(vector4.TakeXYZ(), geometryToGrid)), realLerp);
                        grid.cells[cube].EdgeData[i] = vector4;
                    }
                });

            return grid;
        }

        public static HermiteDataGrid CopyGrid(AbstractHermiteGrid grid)
        {
            var ret = new HermiteDataGrid();
            Point3 storageSize = grid.Dimensions + new Point3(1, 1, 1);
            ret.cells = new Array3D<Vertex>(storageSize);

            for (int x = 0; x < storageSize.X; x++)
                for (int y = 0; y < storageSize.Y; y++)
                    for (int z = 0; z < storageSize.Z; z++)
                    {
                        var p = new Point3(x, y, z);
                        ret.cells[p] = new Vertex()
                        {
                            Sign = grid.GetSign(p),
                            Material = grid.GetMaterial(p)
                            /*EdgeData = ret.dirs.Select(dir =>
                                {
                                    var edgeId = grid.GetEdgeId(p, p + dir);
                                    return grid.HasEdgeData(p, edgeId) ? grid.getEdgeData(p, edgeId) : new Vector4();
                                }).ToArray()*/
                        };
                    }

            //ret.ForEachGridPoint(p =>
            //    {
            //        ret.cells[p] = new Vertex()
            //            {
            //                Sign = grid.GetSign(p),
            //                /*EdgeData = ret.dirs.Select(dir =>
            //                    {
            //                        var edgeId = grid.GetEdgeId(p, p + dir);
            //                        return grid.HasEdgeData(p, edgeId) ? grid.getEdgeData(p, edgeId) : new Vector4();
            //                    }).ToArray()*/
            //            };
            //    });


            var dirs = ret.dirs;
            var dirEdges = dirs.Select(i => ret.GetEdgeId(new Point3(), i)).ToArray();

            ret.ForEachGridPoint(p =>
                {
                    var gridPointP = ret.cells.GetFast(p.X, p.Y, p.Z);
                    gridPointP.EdgeData = new Vector4[3];
                    for (int i = 0; i < 3; i++)
                    {
                        var dir = dirs[i];
                        var edgeId = dirEdges[i];

                        Point3 endPoint = p + dir;
                        // Optimization: direclty read from already constructed data
                        if (!ret.cells.InArray(endPoint)) continue;
                        if (gridPointP.Sign == ret.cells.GetFast(endPoint.X, endPoint.Y, endPoint.Z).Sign) continue;
                        if (!grid.HasEdgeData(p, edgeId))
                        {
                            // This can normally not happen, since we check if there is a sign difference by looking at the already evaluated density points.
                            //  If this would be true there is some problem with the manual determining of the existence of an edge.
                            //throw new InvalidOperationException();
                            continue;
                        }
                        gridPointP.EdgeData[i] = grid.getEdgeData(p, edgeId);
                    }
                    /*val.EdgeData = ret.dirs.Select( dir =>
                        {
                         
                        } ).ToArray();*/

                    ret.cells[p] = gridPointP;
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

        public static AbstractHermiteGrid Empty(Point3 dimensions)
        {
            var ret = new HermiteDataGrid();
            Point3 storageSize = dimensions + new Point3(1, 1, 1);
            ret.cells = new Array3D<Vertex>(storageSize);

            for (int x = 0; x < storageSize.X; x++)
                for (int y = 0; y < storageSize.Y; y++)
                    for (int z = 0; z < storageSize.Z; z++)
                    {
                        var p = new Point3(x, y, z);
                        ret.cells[p] = new Vertex()
                        {
                            Sign = false,
                            EdgeData = new Vector4[3]
                        };
                    }
            return ret;
        }
    }
}