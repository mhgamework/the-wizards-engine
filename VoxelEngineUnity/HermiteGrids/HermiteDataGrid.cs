using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using MHGameWork.TheWizards.VoxelEngine.Persistence;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// //TODO: serious optimization idea: store all signs for each cube in each cube, this is the limiting factor in the QEF calculation
    /// </summary>
    [Serializable]
    public class HermiteDataGrid : AbstractHermiteGrid, IHermiteData
    {
        /// <summary>
        /// There are Dimensions + 1 cells (for holding the normals on the boundaries)
        /// </summary>
        private Array3D<Vertex> cells;

        private global::DirectX11.Point3_Adapter[] dirs = new global::DirectX11.Point3_Adapter[] { new global::DirectX11.Point3_Adapter(1, 0, 0), new global::DirectX11.Point3_Adapter(0, 1, 0), new global::DirectX11.Point3_Adapter(0, 0, 1) };
        private OffsetInfo[] offsetInfos;
        private HermiteDataGrid()
            : base()
        {
            // Construct offset info
            offsetInfos = GetAllEdgeIds().Select(edgeId =>
                {
                    var offsets = GetEdgeOffsets(edgeId);

                    global::DirectX11.Point3_Adapter dir = offsets[1] - offsets[0];
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

        public override bool GetSign(global::DirectX11.Point3_Adapter pos)
        {
            var v = cells[pos];
            if (cells == null) throw new InvalidOperationException("Outside of cell bounds!!");
            return v.Sign;
        }

        public override global::DirectX11.Point3_Adapter Dimensions
        {
            get { return cells.Size - new global::DirectX11.Point3_Adapter(1, 1, 1); }
        }

        [Serializable]
        [DebuggerDisplay("VoxelCell({Sign})")]
        private struct Vertex
        {
            public bool Sign;
            public global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter Material;
            public global::MHGameWork.TheWizards.Vector4_Adapter[] EdgeData;
        }
        /// <summary>
        /// THe offset to apply on the given (cube+edgeID), to get the cube on which we can read the data by reading 'dataedgeid'
        /// </summary>
        private struct OffsetInfo
        {

            public global::DirectX11.Point3_Adapter Offset;
            public int dataEdgeId;
        }


        public override global::MHGameWork.TheWizards.Vector4_Adapter getEdgeData(global::DirectX11.Point3_Adapter cube, int edgeId)
        {
            var offsetInfo = offsetInfos[edgeId];
            return cells[cube + offsetInfo.Offset].EdgeData[offsetInfo.dataEdgeId];
            /*var start = cube_verts[GetEdgeVertexIds(edgeId)[0]];
            var end = cube_verts[GetEdgeVertexIds(edgeId)[1]];

            return cells[cube + start].EdgeData[dirs.IndexOf(end - start)];*/
        }

        public override global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter GetMaterial(global::DirectX11.Point3_Adapter cube)
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
        public static HermiteDataGrid FromIntersectableGeometry(float gridWorldSize, int resolution, global::MHGameWork.TheWizards.Matrix_Adapter world, IIntersectableObject obj)
        {
            global::DirectX11.Point3_Adapter numCubes = new global::DirectX11.Point3_Adapter(resolution, resolution, resolution);
            var grid = new HermiteDataGrid();

            global::MHGameWork.TheWizards.Matrix_Adapter gridToWorld = global::MHGameWork.TheWizards.Matrix_Adapter.Scaling(1f / resolution, 1f / resolution, 1f / resolution) * global::MHGameWork.TheWizards.Matrix_Adapter.Scaling(new global::MHGameWork.TheWizards.Vector3_Adapter(gridWorldSize));
            global::MHGameWork.TheWizards.Matrix_Adapter gridToGeometry = gridToWorld * global::MHGameWork.TheWizards.Matrix_Adapter.Invert(world);
            global::MHGameWork.TheWizards.Matrix_Adapter geometryToGrid = global::MHGameWork.TheWizards.Matrix_Adapter.Invert(gridToGeometry);


            // Fill vertices
            grid.cells = new Array3D<Vertex>(numCubes + new global::DirectX11.Point3_Adapter(1, 1, 1));
            grid.ForEachGridPoint(cube =>
                {
                    grid.cells[cube] = new Vertex()
                        {
                            Sign = obj.IsInside(global::MHGameWork.TheWizards.Vector3_Adapter.TransformCoordinate(cube.ToVector3(), gridToGeometry)),
                            EdgeData = new global::MHGameWork.TheWizards.Vector4_Adapter[3]
                        };
                });

            // Find changing edges and calculate edge info
            grid.ForEachGridPoint(cube =>
                {
                    var sign = grid.GetSign(cube);

                    for (int i = 0; i < grid.dirs.Length; i++)
                    {
                        global::DirectX11.Point3_Adapter start = cube;
                        global::DirectX11.Point3_Adapter end = cube + grid.dirs[i];
                        if (sign == grid.GetSign(end)) continue;
                        //sign difference


                        global::MHGameWork.TheWizards.Vector3_Adapter startT = global::MHGameWork.TheWizards.Vector3_Adapter.TransformCoordinate(start, gridToGeometry);
                        global::MHGameWork.TheWizards.Vector3_Adapter endT = global::MHGameWork.TheWizards.Vector3_Adapter.TransformCoordinate(end, gridToGeometry);
                        global::MHGameWork.TheWizards.Vector4_Adapter vector4 = obj.GetIntersection(startT, endT);
                        global::MHGameWork.TheWizards.Vector3_Adapter intersectionPoint = global::MHGameWork.TheWizards.Vector3_Adapter.TransformCoordinate(global::MHGameWork.TheWizards.Vector3_Adapter.Lerp(startT, endT, vector4.W), geometryToGrid);
                        var realLerp = global::MHGameWork.TheWizards.Vector3_Adapter.Distance(start, intersectionPoint); // /1
                        Debug.Assert(Math.Abs(global::MHGameWork.TheWizards.Vector3_Adapter.Distance(start, end) - 1) < 0.001); // Algo check
                        vector4 = new global::MHGameWork.TheWizards.Vector4_Adapter(global::MHGameWork.TheWizards.Vector3_Adapter.Normalize(global::MHGameWork.TheWizards.Vector3_Adapter.TransformNormal(vector4.TakeXYZ(), geometryToGrid)), realLerp);
                        grid.cells[cube].EdgeData[i] = vector4;
                    }
                });

            return grid;
        }

        public static HermiteDataGrid CopyGrid(AbstractHermiteGrid grid)
        {
            var ret = new HermiteDataGrid();
            global::DirectX11.Point3_Adapter storageSize = grid.Dimensions + new global::DirectX11.Point3_Adapter(1, 1, 1);
            ret.cells = new Array3D<Vertex>(storageSize);

            for (int x = 0; x < storageSize.X; x++)
                for (int y = 0; y < storageSize.Y; y++)
                    for (int z = 0; z < storageSize.Z; z++)
                    {
                        global::DirectX11.Point3_Adapter p = new global::DirectX11.Point3_Adapter(x, y, z);
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
            var dirEdges = dirs.Select(i => ret.GetEdgeId(new global::DirectX11.Point3_Adapter(), i)).ToArray();

            ret.ForEachGridPoint(p =>
                {
                    var gridPointP = ret.cells.GetFast(p.X, p.Y, p.Z);
                    gridPointP.EdgeData = new global::MHGameWork.TheWizards.Vector4_Adapter[3];
                    for (int i = 0; i < 3; i++)
                    {
                        global::DirectX11.Point3_Adapter dir = dirs[i];
                        var edgeId = dirEdges[i];

                        global::DirectX11.Point3_Adapter endPoint = p + dir;
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
                            var val = cells[new global::DirectX11.Point3_Adapter(x, y, z)];
                            fs.Write(val.Sign);
                            fs.Write(val.EdgeData.Length);
                            for (int i = 0; i < val.EdgeData.Length; i++)
                            {
                                global::MHGameWork.TheWizards.Vector4_Adapter edge = val.EdgeData[i];
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
                global::DirectX11.Point3_Adapter size = new global::DirectX11.Point3_Adapter(fs.ReadInt32(), fs.ReadInt32(), fs.ReadInt32());
                cells = new Array3D<Vertex>(size);
                for (int x = 0; x < cells.Size.X; x++)
                    for (int y = 0; y < cells.Size.Y; y++)
                        for (int z = 0; z < cells.Size.Z; z++)
                        {
                            var val = new Vertex();
                            val.Sign = fs.ReadBoolean();
                            val.EdgeData = new global::MHGameWork.TheWizards.Vector4_Adapter[fs.ReadInt32()];
                            for (int i = 0; i < val.EdgeData.Length; i++)
                            {
                                val.EdgeData[i] = new global::MHGameWork.TheWizards.Vector4_Adapter(fs.ReadSingle(), fs.ReadSingle(), fs.ReadSingle(), fs.ReadSingle());
                            }
                            cells[new global::DirectX11.Point3_Adapter(x, y, z)] = val;
                        }

            }
        }

        public static HermiteDataGrid Empty(global::DirectX11.Point3_Adapter dimensions)
        {
            var ret = new HermiteDataGrid();
            global::DirectX11.Point3_Adapter storageSize = dimensions + new global::DirectX11.Point3_Adapter(1, 1, 1);
            ret.cells = new Array3D<Vertex>(storageSize);

            for (int x = 0; x < storageSize.X; x++)
                for (int y = 0; y < storageSize.Y; y++)
                    for (int z = 0; z < storageSize.Z; z++)
                    {
                        global::DirectX11.Point3_Adapter p = new global::DirectX11.Point3_Adapter(x, y, z);
                        ret.cells[p] = new Vertex()
                        {
                            Sign = false,
                            EdgeData = new global::MHGameWork.TheWizards.Vector4_Adapter[3]
                        };
                    }
            return ret;
        }



        global::DirectX11.Point3_Adapter IHermiteData.NumCells
        {
            get { return Dimensions; }
        }

        float IHermiteData.GetIntersection(global::DirectX11.Point3_Adapter cell, int dir)
        {
            return getEdgeData(cell, dirs[dir]).W;
        }

        global::MHGameWork.TheWizards.Vector3_Adapter IHermiteData.GetNormal(global::DirectX11.Point3_Adapter cell, int dir)
        {
            return getEdgeData(cell, dirs[dir]).TakeXYZ();
        }

        public static global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter DefaultMaterial = new global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter();
        object IHermiteData.GetMaterial(global::DirectX11.Point3_Adapter cell)
        {
            if ( !GetSign( cell ) ) return null;
            global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter mat = GetMaterial(cell);
            if ( mat == null ) return DefaultMaterial;
            return mat;
        }
        public void SetMaterial(global::DirectX11.Point3_Adapter cell, object material)
        {
            var v = cells[ cell ];
            cells[cell] = new Vertex()
                {
                    EdgeData = v.EdgeData,
                    Material = (global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter)material,
                    Sign = material != null
                };
        }



        void IHermiteData.SetIntersection(global::DirectX11.Point3_Adapter cell, int dir, float intersect)
        {
            var v = cells[cell];
            throw new NotImplementedException();
            cells[cell] = new Vertex()
            {
                EdgeData = v.EdgeData,
                Material = v.Material,
                Sign = v.Sign
            };
        }

        void IHermiteData.SetNormal(global::DirectX11.Point3_Adapter flattenedCoord, int dir, global::MHGameWork.TheWizards.Vector3_Adapter normal)
        {
            throw new NotImplementedException();
        }
    }
}