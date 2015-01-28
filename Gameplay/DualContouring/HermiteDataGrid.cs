using System;
using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    public class HermiteDataGrid
    {
        private List<Point3> cube_verts;

        private Array3D<Vertex> cells;
        private int[][] edgeToVertices;

        private List<Point3> dirs = new List<Point3>(new Point3[] { new Point3(1, 0, 0), new Point3(0, 1, 0), new Point3(0, 0, 1) });
        private List<Point3[]> cubeEdges;

        private HermiteDataGrid()
        {
            cube_verts = (from x in Enumerable.Range(0, 2)
                          from y in Enumerable.Range(0, 2)
                          from z in Enumerable.Range(0, 2)
                          select new Point3(x, y, z)).ToList();


            cubeEdges = (from v in cube_verts
                         from offset in new[] { new Point3(1, 0, 0), new Point3(0, 1, 0), new Point3(0, 0, 1) }
                         where (v + offset).X < 1.5
                         where (v + offset).Y < 1.5
                         where (v + offset).Z < 1.5
                         select new { Start = v, End = v + offset }).Distinct().Select(e => new[] { e.Start, e.End }).ToList();

            edgeToVertices = cubeEdges.Select(edge => new int[] { cube_verts.IndexOf(edge[0]), cube_verts.IndexOf(edge[1]) }).ToArray();
        }

        public bool GetSign(Point3 pos)
        {
            var v = cells[pos];
            if (cells == null) return false;
            return v.Sign;
        }

        public bool[] GetCubeSigns(Point3 cube)
        {
            return cube_verts.Select(offset => GetSign(cube + offset)).ToArray();
        }

        private struct Vertex
        {
            public bool Sign;
            public Vector4[] EdgeData;
        }


        public void ForEachCube(Action<Point3> action)
        {
            for (int x = 0; x < cells.Size.X; x++)
                for (int y = 0; y < cells.Size.Y; y++)
                    for (int z = 0; z < cells.Size.Z; z++)
                        action(new Point3(x, y, z));

        }

        public int[] GetAllEdgeIds()
        {
            return Enumerable.Range(0, 11).ToArray();
        }
        public int[] GetEdgeVertexIds(int edgeId)
        {
            return edgeToVertices[edgeId];
        }

        /// <summary>
        /// Returns the intersection point on given edge, in cube local space
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Vector3 GetEdgeIntersectionCubeLocal(Point3 cube, int edgeId)
        {
            var start = cube_verts[GetEdgeVertexIds(edgeId)[0]];
            var end = cube_verts[GetEdgeVertexIds(edgeId)[1]];

            return Vector3.Lerp(start, end, getEdgeData(cube, edgeId).W);
        }
        public Vector3 GetEdgeNormal(Point3 curr, int i)
        {
            return getEdgeData(curr, i).TakeXYZ();
        }

        private Vector4 getEdgeData(Point3 cube, int edgeId)
        {
            var start = cube_verts[GetEdgeVertexIds(edgeId)[0]];
            var end = cube_verts[GetEdgeVertexIds(edgeId)[1]];

            return cells[cube + start].EdgeData[dirs.IndexOf(end - start)];
        }


        public static HermiteDataGrid FromIntersectableGeometry(float gridWorldSize, int resolution, Matrix world, Func<Vector3, bool> isInside, Func<Vector3, Vector3, Vector4> intersect)
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
                            Sign = isInside(Vector3.TransformCoordinate(cube.ToVector3(), gridToGeometry)),
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
                        var vector4 = intersect(Vector3.TransformCoordinate(cube, gridToGeometry), Vector3.TransformCoordinate(cube + grid.dirs[i], gridToGeometry));
                        vector4 = new Vector4(Vector3.Normalize(Vector3.TransformNormal(vector4.TakeXYZ(), geometryToGrid)), vector4.W);
                        grid.cells[cube].EdgeData[i] = vector4;
                    }
                });

            return grid;
        }


        public int GetEdgeId(Point3 start, Point3 end)
        {
            if (start.X > end.X || start.Y > end.Y || start.Z > end.Z)
                throw new InvalidOperationException();

            end -= start;
            start = new Point3();

            var ret = cubeEdges.FindIndex(arr => (arr[0] == start) && (arr[1] == end));

            if (ret == -1) throw new InvalidOperationException();

            return ret;


        }
    }
}