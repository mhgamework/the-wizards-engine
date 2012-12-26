using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.Generation
{
    public class VoxelGrid<T>
    {
        private T[,,] data;
        private Point3 size;
        public Point3 Size
        {
            get { return size; }
            private set { size = value; }
        }


        public VoxelGrid(Point3 size)
        {
            Size = size;
            data = new T[size.X,size.Y,size.Z];
        }


        public void ForEach(Action<int, int, int> callback,BoundingBox bb)
        {
            bb.Minimum = Vector3.Maximize(bb.Minimum, new Vector3());
            bb.Maximum = Vector3.Minimize(bb.Maximum, new Vector3(size.X, size.Y, size.Z));

            var minX = (int) bb.Minimum.X;
            var minY = (int)bb.Minimum.Y;
            var minZ = (int)bb.Minimum.Z;

            var maxX = (int)bb.Maximum.X;
            var maxY = (int)bb.Maximum.Y;
            var maxZ = (int)bb.Maximum.Z;


            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    for (int z = minZ; z < maxZ; z++)
                    {
                        callback(x, y, z);
                    }
                }
            }
        }


        public void ForEach(Action<int,int,int> callback)
        {
            for (int x = 0; x < Size.X; x++)
                for (int y = 0; y < Size.Y; y++)
                    for (int z = 0; z < Size.Z; z++)
                    {
                        callback(x, y, z);
                    }
        }

        public T this[int x,int y , int z]
        {
            get { return data[x, y, z]; }
            set { data[x, y, z] = value; }
        }

        public T[,,] ToArray()
        {
            return data;
        }

        public bool ContainsKey(int ix, int iy, int iz)
        {
            if (ix < 0) return false;
            if (iy < 0) return false;
            if (iz < 0) return false;
            if (ix >= size.X) return false;
            if (iy >= size.Y) return false;
            if (iz >= size.Z) return false;
            return true;
        }
    }
}
