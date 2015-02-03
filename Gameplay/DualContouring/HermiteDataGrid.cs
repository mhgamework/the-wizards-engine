using System;
using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
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
                            EdgeData = ret.dirs.Select(dir =>
                                {
                                    var edgeId = grid.GetEdgeId(p, p + dir);
                                    return grid.HasEdgeData(p, edgeId) ? grid.getEdgeData(p, edgeId) : new Vector4();
                                }).ToArray()
                        };
                });

            return ret;
        }
    }
}