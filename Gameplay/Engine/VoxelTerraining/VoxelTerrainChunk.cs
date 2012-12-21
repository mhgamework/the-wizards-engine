using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Serialization;
using SlimDX;

namespace MHGameWork.TheWizards.Engine.VoxelTerraining
{
    /// <summary>
    /// Responsible for storing a part of the voxel terrain in a chunk
    /// </summary>
    [ModelObjectChanged]
    public class VoxelTerrainChunk : EngineModelObject
    {
        private Point3 size;

        public VoxelTerrainChunk()
        {
            NodeSize = 1;
        }

        public float NodeSize { get; set; }
        [CustomStringSerializer(typeof(VoxelStringSerializer))]
        public Voxel[, ,] Voxels { get; set; }
        public Point3 Size
        {
            get { return size; }
            set { size = value; }
        }

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
            return GetVoxelInternal(ref pos);
        }
        public Voxel GetVoxelInternal(ref Point3 pos)
        {
            if (pos.X >= size.X) return null;
            if (pos.Y >= size.Y) return null;
            if (pos.Z >= size.Z) return null;

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


        public class VoxelStringSerializer : IConditionalSerializer
        {
            public bool CanOperate(Type type)
            {
                return type == typeof(Voxel[, ,]);
            }

            public string Serialize(object obj, Type type, StringSerializer stringSerializer)
            {
                var builder = new StringBuilder();

                var grid = (Voxel[, ,])obj;
                for (int x = 0; x <= grid.GetUpperBound(0); x++)
                {
                    builder.AppendLine("Slice");
                    for (int y = 0; y <= grid.GetUpperBound(1); y++)
                    {
                        for (int z = 0; z <= grid.GetUpperBound(2); z++)
                        {
                            builder.Append(stringSerializer.Serialize(grid[x, y, z].Filled ? 1 : 0, typeof(int)));
                        }
                        builder.AppendLine();
                    }
                }



                return builder.ToString();
            }

            public object Deserialize(string value, Type type, StringSerializer stringSerializer)
            {
                var reader = new StringReader(value);
                int x = -1;
                int y = 0;
                List<List<String>> slices = new List<List<string>>();

                List<string> currentSlice = null;
                while (reader.Peek() != -1)
                {
                    var line = reader.ReadLine();
                    if (line == "Slice")
                    {
                        x++;
                        currentSlice = new List<string>();
                        slices.Add(currentSlice);
                        continue;
                    }

                    currentSlice.Add(line);
                }

                var grid = new Voxel[slices.Count, slices[0].Count, slices[0][0].Length];
                for (x = 0; x <= grid.GetUpperBound(0); x++)
                    for (y = 0; y <= grid.GetUpperBound(1); y++)
                    {
                        var line = slices[x][y];
                        for (int z = 0; z <= grid.GetUpperBound(2); z++)
                        {

                            var filled = (int)stringSerializer.Deserialize(line.Substring(z, 1), typeof(int)) == 1;
                            grid[x, y, z] = new Voxel { Filled = filled };

                        }
                    }
                return grid;

            }
        }
    }
}
