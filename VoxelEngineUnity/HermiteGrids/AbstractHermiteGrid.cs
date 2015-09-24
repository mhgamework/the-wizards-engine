using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Represents a hermite grid.
    /// Note: the last edges in +x,+y and +z do NOT have to be available in the grid. Each grid cube only needs to have its 3 (0,0,0) adjacing edge info
    /// </summary>
    public abstract class AbstractHermiteGrid
    {
        protected readonly List<global::DirectX11.Point3_Adapter> cube_verts;
        private readonly int[][] edgeToVertices;
        private readonly List<global::DirectX11.Point3_Adapter[]> cubeEdges;

        public abstract bool GetSign(global::DirectX11.Point3_Adapter pos);

        public class startend
        {
            private readonly global::DirectX11.Point3_Adapter start;
            private readonly global::DirectX11.Point3_Adapter end;

            public global::DirectX11.Point3_Adapter Start
            {
                get { return start; }
            }

            public global::DirectX11.Point3_Adapter End
            {
                get { return end; }
            }

            public startend( global::DirectX11.Point3_Adapter start, global::DirectX11.Point3_Adapter end )
            {
                this.start = start;
                this.end = end;
            }

            protected bool Equals( startend other )
            {
                return start.Equals( other.start ) && end.Equals( other.end );
            }

            public override bool Equals( object obj )
            {
                if ( ReferenceEquals( null, obj ) ) return false;
                if ( ReferenceEquals( this, obj ) ) return true;
                if ( obj.GetType() != this.GetType() ) return false;
                return Equals( (startend) obj );
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ( start.GetHashCode()*397 ) ^ end.GetHashCode();
                }
            }
        }

        protected AbstractHermiteGrid()
        {
            cube_verts = (from x in Enumerable.Range(0, 2)
                          from y in Enumerable.Range(0, 2)
                          from z in Enumerable.Range(0, 2)
                          select new global::DirectX11.Point3_Adapter(x, y, z)).ToList();


            cubeEdges = (from v in cube_verts
                         from offset in new[] { new global::DirectX11.Point3_Adapter(1, 0, 0), new global::DirectX11.Point3_Adapter(0, 1, 0), new global::DirectX11.Point3_Adapter(0, 0, 1) }
                         where (v + offset).X < 1.5
                         where (v + offset).Y < 1.5
                         where (v + offset).Z < 1.5
                         select new startend( v, v + offset )).Distinct().Select(e => new[] { e.Start, e.End }).ToList();

            edgeToVertices = cubeEdges.Select(edge => new int[] { cube_verts.IndexOf(edge[0]), cube_verts.IndexOf(edge[1]) }).ToArray();
        }

        public abstract global::DirectX11.Point3_Adapter Dimensions { get; }

        public void GetCubeSigns(global::DirectX11.Point3_Adapter cube,bool[] output)
        {
            for ( int i = 0; i < 8; i++ )
            {
                output[ i ] = GetSign( cube_verts[ i ] + cube );
            }
        }
        public bool[] GetCubeSigns(global::DirectX11.Point3_Adapter cube)
        {
            var ret = new bool[8];
            GetCubeSigns(cube, ret);
            return ret;
        }

        /// <summary>
        /// Enumerates each gridpoint. This does one extra point compared to ForEachCube
        /// </summary>
        /// <param name="action"></param>
        public void ForEachGridPoint(Action<global::DirectX11.Point3_Adapter> action)
        {
            int maxX = Dimensions.X + 1;
            int maxY = Dimensions.Y + 1;
            int maxZ = Dimensions.Z + 1;
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    for (int z = 0; z < maxZ; z++)
                        action(new global::DirectX11.Point3_Adapter(x, y, z));
                }
            }
        }

        /// <summary>
        /// Enumerates each cube in the hermite grid, this is equal to Dimensions
        /// </summary>
        /// <param name="action"></param>
        public void ForEachCube(Action<global::DirectX11.Point3_Adapter> action)
        {
            for (int x = 0; x < Dimensions.X; x++)
                for (int y = 0; y < Dimensions.Y; y++)
                    for (int z = 0; z < Dimensions.Z; z++)
                        action(new global::DirectX11.Point3_Adapter(x, y, z));

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
        public global::MHGameWork.TheWizards.Vector3_Adapter GetEdgeIntersectionCubeLocal(global::DirectX11.Point3_Adapter cube, int edgeId)
        {
            var offsets = GetEdgeOffsets(edgeId);

            return global::MHGameWork.TheWizards.Vector3_Adapter.Lerp(offsets[0].ToVector3(), offsets[1].ToVector3(), getEdgeData(cube, edgeId).W);
        }

        public global::MHGameWork.TheWizards.Vector3_Adapter GetEdgeNormal(global::DirectX11.Point3_Adapter curr, int i)
        {
            return global::MHGameWork.TheWizards.UtilityExtensions_Adapter.TakeXYZ(getEdgeData(curr, i));
        }
        /// <summary>
        /// Returns hermite grid normal data. xyz components are the normal, w component is the lerp factor 0..1 for the intersectionpoint between start and end of the edge
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="edgeId"></param>
        /// <returns></returns>
        public abstract global::MHGameWork.TheWizards.Vector4_Adapter getEdgeData(global::DirectX11.Point3_Adapter cube, int edgeId);

        public virtual int GetEdgeId(global::DirectX11.Point3_Adapter start, global::DirectX11.Point3_Adapter end)
        {
            if (start.X > end.X || start.Y > end.Y || start.Z > end.Z)
                throw new InvalidOperationException();

            end -= start;
            start = new global::DirectX11.Point3_Adapter();

            var ret = cubeEdges.FindIndex(arr => (arr[0] == start) && (arr[1] == end));

            if (ret == -1) throw new InvalidOperationException();

            return ret;


        }

        public global::DirectX11.Point3_Adapter[] GetEdgeOffsets(int edgeId)
        {
            global::DirectX11.Point3_Adapter start = cube_verts[GetEdgeVertexIds(edgeId)[0]];
            global::DirectX11.Point3_Adapter end = cube_verts[GetEdgeVertexIds(edgeId)[1]];

            return new global::DirectX11.Point3_Adapter[] { start, end };
        }

        public bool[] GetEdgeSigns(global::DirectX11.Point3_Adapter cube, int edgeId)
        {
            var offsets = GetEdgeOffsets(edgeId);
            return new[] { GetSign(cube + offsets[0]), GetSign(cube + offsets[1]) };
        }

        public bool HasEdgeData(global::DirectX11.Point3_Adapter cube, int edgeId)
        {
            var signs = GetEdgeSigns(cube, edgeId);
            return signs[0] != signs[1];
        }

        public global::MHGameWork.TheWizards.Vector4_Adapter getEdgeData(global::DirectX11.Point3_Adapter cube, global::DirectX11.Point3_Adapter dir)
        {
            return getEdgeData(cube, GetEdgeId(cube, cube + dir));
        }

        public abstract global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter GetMaterial( global::DirectX11.Point3_Adapter cube );
    }
}