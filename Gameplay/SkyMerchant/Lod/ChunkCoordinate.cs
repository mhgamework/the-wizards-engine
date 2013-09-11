using System;
using System.Collections.Generic;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Lod
{
    /// <summary>
    /// Represents a chunk at a depth and position
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
        }

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

            if (Depth == 0) // is root
            {
                xmod = -1;
                ymod = -1;
                zmod = -1;
            }

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
            if (Depth == 0)
                return new BoundingBox(rootCenter - rootSize * 0.5f, rootCenter + rootSize * 0.5f);
            var size = GetChunkSize(rootSize);
            var min = rootCenter + Vector3.Modulate(Position.ToVector3(), size);
            return new BoundingBox(min, min + size);
        }
        public Vector3 GetChunkSize(Vector3 rootSize)
        {
            var divide = 1 << Depth;
            return rootSize / divide;
        }

        public override string ToString()
        {
            return string.Format("Depth: {0}, {1}", Depth, Position);
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

        /// <summary>
        /// Returns an index with given properties
        ///     the map 'index to chunkcoord' is a bijection
        ///     index >= 0
        ///     index1 smaller than index2 => depth1 smaller or equal than depth2
        /// </summary>
        /// <returns></returns>
        public int GetUnrolledIndex()
        {
            if (Depth == 0) return 0;
            // sum previous depths (geometric series)
            var a = 1;
            var r = 8;
            var r_tothe_n = 1 << (Depth * 3); // 8^Depth
            var start = a * (1 - r_tothe_n) / (1 - r);

            var x = Position.X + (1 << (Depth - 1));
            var y = Position.Y + (1 << (Depth - 1));
            var z = Position.Z + (1 << (Depth - 1));


            return start + z * (1 << (Depth * 2)) + y * (1 << Depth) + x;
        }
    }
}