using System;
using System.Collections.Generic;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.Simulation.Spatial
{
    /// <summary>
    /// Represents a chunk at a depth and position
    /// 
    /// The coordinate system for the chunks grows outwards from 0,0,0 (only positive numbers)
    /// At each depth the number of nodes is doubled on each axis, the root stays in position.
    /// </summary>
    public struct ChunkCoordinate
    {
        public int Depth;
        public Point3 Position;

        public ChunkCoordinate(int depth, Point3 position)
            : this()
        {
            Depth = depth;
            Position = position;
        }
        public ChunkCoordinate(int depth, int x, int y, int z)
        {
            Depth = depth;
            Position = new Point3(x, y, z);
        }

        static ChunkCoordinate()
        {
            Root = new ChunkCoordinate(0, new Point3());
            Empty = new ChunkCoordinate(-1, new Point3());
        }

        public static ChunkCoordinate Empty { get; private set; }
        public static ChunkCoordinate Root { get; private set; }

        public IEnumerable<ChunkCoordinate> GetChildren()
        {
            var cDepth = Depth + 1;
            var cX = Position.X << 1;
            var cY = Position.Y << 1;
            var cZ = Position.Z << 1;
            var xmod = 1;//cX < 0 ? -1 : 1;
            var ymod = 1;//cY < 0 ? -1 : 1;
            var zmod = 1;//cZ < 0 ? -1 : 1;

            //if (Depth == 0) // is root
            //{
            //    xmod = -1;
            //    ymod = -1;
            //    zmod = -1;
            //}
            if (IsEmtpy) throw new NotImplementedException();

            yield return new ChunkCoordinate(cDepth, cX, cY, cZ);
            yield return new ChunkCoordinate(cDepth, cX, cY, cZ + zmod);
            yield return new ChunkCoordinate(cDepth, cX, cY + ymod, cZ);
            yield return new ChunkCoordinate(cDepth, cX, cY + ymod, cZ + zmod);
            yield return new ChunkCoordinate(cDepth, cX + xmod, cY, cZ);
            yield return new ChunkCoordinate(cDepth, cX + xmod, cY, cZ + zmod);
            yield return new ChunkCoordinate(cDepth, cX + xmod, cY + ymod, cZ);
            yield return new ChunkCoordinate(cDepth, cX + xmod, cY + ymod, cZ + zmod);
        }

        public BoundingBox GetBoundingBox(Vector3 rootCenter, Vector3 rootSize)
        {
            if (IsEmtpy) throw new NotImplementedException();

            var rootMin = rootCenter - rootSize * 0.5f;

            if (IsRoot)
                return new BoundingBox(rootMin, rootMin + rootSize);

            var size = GetChunkSize(rootSize);
            var min = rootMin + Vector3.Modulate(Position.ToVector3(), size);
            return new BoundingBox(min, min + size);
        }
        public Vector3 GetChunkSize(Vector3 rootSize)
        {
            if (Depth < 0) throw new NotImplementedException();

            var divide = 1 << Depth;
            return rootSize / divide;
        }

        public override string ToString()
        {
            if (Depth < 0) return "Chunk - Empty";
            return string.Format("Chunk - Depth: {0}, {1}", Depth, Position);
        }

        public bool Equals(ChunkCoordinate other)
        {
            return Depth == other.Depth && Position.Equals(other.Position);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ChunkCoordinate && Equals((ChunkCoordinate)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Depth * 397) ^ Position.GetHashCode();
            }
        }

        private sealed class DepthPositionEqualityComparer : IEqualityComparer<ChunkCoordinate>
        {
            public bool Equals(ChunkCoordinate x, ChunkCoordinate y)
            {
                return x.Depth == y.Depth && x.Position.Equals(y.Position);
            }

            public int GetHashCode(ChunkCoordinate obj)
            {
                unchecked
                {
                    return (obj.Depth * 397) ^ obj.Position.GetHashCode();
                }
            }
        }

        private static readonly IEqualityComparer<ChunkCoordinate> DepthPositionComparerInstance = new DepthPositionEqualityComparer();

        public static IEqualityComparer<ChunkCoordinate> DepthPositionComparer
        {
            get { return DepthPositionComparerInstance; }
        }

        public bool IsEmtpy { get { return Depth < 0; } }
        public bool IsRoot { get { return Depth == 0; } }

        /// <summary>
        /// Returns an index with given properties
        ///     the map 'index to chunkcoord' is a bijection
        ///     index >= 0
        ///     index1 smaller than index2 => depth1 smaller or equal than depth2
        /// </summary>
        /// <returns></returns>
        public int GetUnrolledIndex()
        {
            if (IsEmtpy) return -1;
            if (IsRoot) return 0;
            // sum previous depths (geometric series)
            var start = GetCumulativeNbChunks(Depth);

            var x = Position.X;
            var y = Position.Y;
            var z = Position.Z;


            return start + z * (1 << (Depth * 2)) + y * (1 << Depth) + x;
        }

        public static int GetCumulativeNbChunks(int depth)
        {
            var a = 1;
            var r = 8;
            var r_tothe_n = GetNbChunksAtDepth(depth); // 8^Depth
            var start = a*(1 - r_tothe_n)/(1 - r);
            return start;
        }

        public static int GetNbChunksAtDepth(int depth)
        {
            return 1 << (depth * 3);
        }
     

        public ChunkCoordinate GetParent()
        {
            if (IsRoot) return Empty;
            if (IsRoot) throw new InvalidOperationException();

            //var sX = Math.Sign(Position.X);
            //var sY = Math.Sign(Position.Y);
            //var sZ = Math.Sign(Position.Z);

            //var x = ((Position.X * sX) >> 1) * sX;
            //var y = ((Position.Y * sY) >> 1) * sY;
            //var z = ((Position.Z * sZ) >> 1) * sZ;

            var x = Position.X / 2;
            var y = Position.Y / 2;
            var z = Position.Z / 2;

            return new ChunkCoordinate(Depth - 1, x, y, z);
        }
    }
}