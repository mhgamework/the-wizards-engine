using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    public abstract class AbstractHermiteGrid
    {
        protected readonly List<Point3> cube_verts;
        private readonly int[][] edgeToVertices;
        private readonly List<Point3[]> cubeEdges;

        public abstract bool GetSign(Point3 pos);


        protected AbstractHermiteGrid()
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

        public abstract Point3 Dimensions { get; }


        public bool[] GetCubeSigns(Point3 cube)
        {
            return cube_verts.Select(offset => GetSign(cube + offset)).ToArray();
        }

        public void ForEachCube(Action<Point3> action)
        {
            for (int x = 0; x < Dimensions.X; x++)
                for (int y = 0; y < Dimensions.Y; y++)
                    for (int z = 0; z < Dimensions.Z; z++)
                        action(new Point3(x, y, z));

        }

        public int[] GetAllEdgeIds()
        {
            return Enumerable.Range(0, 12).ToArray();
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
            var offsets = GetEdgeOffsets(edgeId);

            return Vector3.Lerp(offsets[0], offsets[1], getEdgeData(cube, edgeId).W);
        }

        public Vector3 GetEdgeNormal(Point3 curr, int i)
        {
            return UtilityExtensions.TakeXYZ(getEdgeData(curr, i));
        }
        /// <summary>
        /// Returns hermite grid normal data. xyz components are the normal, w component is the lerp factor 0..1 for the intersectionpoint between start and end of the edge
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="edgeId"></param>
        /// <returns></returns>
        public abstract Vector4 getEdgeData(Point3 cube, int edgeId);

        public virtual int GetEdgeId(Point3 start, Point3 end)
        {
            if (start.X > end.X || start.Y > end.Y || start.Z > end.Z)
                throw new InvalidOperationException();

            end -= start;
            start = new Point3();

            var ret = cubeEdges.FindIndex(arr => (arr[0] == start) && (arr[1] == end));

            if (ret == -1) throw new InvalidOperationException();

            return ret;


        }

        public Point3[] GetEdgeOffsets(int edgeId)
        {
            var start = cube_verts[GetEdgeVertexIds(edgeId)[0]];
            var end = cube_verts[GetEdgeVertexIds(edgeId)[1]];

            return new Point3[] { start, end };
        }

        public bool[] GetEdgeSigns(Point3 cube, int edgeId)
        {
            var offsets = GetEdgeOffsets(edgeId);
            return new[] { GetSign(cube + offsets[0]), GetSign(cube + offsets[1]) };
        }

        public bool HasEdgeData(Point3 cube, int edgeId)
        {
            var signs = GetEdgeSigns(cube, edgeId);
            return signs[0] != signs[1];
        }

        public Vector4 getEdgeData(Point3 cube, Point3 dir)
        {
            return getEdgeData(cube, GetEdgeId(cube, cube + dir));
        }

      
        
    }
}