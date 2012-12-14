using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.VoxelTerraining
{
    [ModelObjectChanged]
    public class VoxelTerrainChunk : EngineModelObject
    {
        public VoxelTerrainChunk()
        {
            NodeSize = 1;
        }

        public float NodeSize { get; set; }
        public Voxel[, ,] Voxels { get; set; }
        public Point3 Size { get; set; }
        public Vector3 WorldPosition { get; set; }

        public void Create()
        {
            Voxels = new Voxel[Size.X, Size.Y, Size.Z];
            for (int x = 0; x < Size.X; x++)
                for (int y = 0; y < Size.Y; y++)
                    for (int z = 0; z < Size.Z; z++)
                    {
                        Voxels[x, y, z] = new Voxel();
                    }
        }

        public Vector3 GetVoxelWorldCenter(Point3 pos)
        {
            return pos.ToVector3() * NodeSize + WorldPosition;
        }

        public Voxel GetVoxelInternal(Point3 pos)
        {
            if (pos.X >= Size.X) return null;
            if (pos.Y >= Size.Y) return null;
            if (pos.Z >= Size.Z) return null;

            if (pos.X < 0) return null;
            if (pos.Y < 0) return null;
            if (pos.Z < 0) return null;
            return Voxels[pos.X, pos.Y, pos.Z];
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

        public class Voxel
        {
            public bool Visible;
            public bool Filled;
        }

        public bool InGrid(Point3 point3)
        {
            for (int i = 0; i < 3; i++)
            {
                if (point3[i] < 0) return false;
                if (point3[i] >= Size[i]) return false;
            }
            return true;
        }


        public BoundingBox GetBoundingBox()
        {
            return new BoundingBox(WorldPosition, WorldPosition + Size.ToVector3() * NodeSize);
        }


        protected bool Equals(VoxelTerrainChunk other)
        {
            return Size.Equals(other.Size) && WorldPosition.Equals(other.WorldPosition) && NodeSize.Equals(other.NodeSize);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VoxelTerrainChunk)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Size.GetHashCode();
                hashCode = (hashCode * 397) ^ WorldPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ NodeSize.GetHashCode();
                return hashCode;
            }
        }
    }
}
