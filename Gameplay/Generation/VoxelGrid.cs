using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;

namespace MHGameWork.TheWizards.Generation
{
    public class VoxelGrid<T>
    {
        private T[,,] data;
        public Point3 Size { get; private set; }



        public VoxelGrid(Point3 size)
        {
            Size = size;
            data = new T[size.X,size.Y,size.Z];
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
    }
}
