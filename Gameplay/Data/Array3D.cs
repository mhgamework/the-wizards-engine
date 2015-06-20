using System;
using DirectX11;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures
{
    /// <summary>
    /// Represents a array that can be accessed using Point3
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Array3D<T>
    {
        private T[, ,] arr;
        public Point3 Size { get; private set; }

        public Array3D(Point3 size)
        {
            arr = new T[size.X, size.Y, size.Z];
            Size = size;
        }
        public T this[Point3 pos]
        {
            get
            {
                if (!InArray(pos)) return default(T);
                return arr[pos.X, pos.Y, pos.Z];
            }
            set { arr[pos.X, pos.Y, pos.Z] = value; }
        }

        public bool InArray(Point3 pos)
        {
            if (pos.X < 0 || pos.Y < 0 || pos.Z < 0) return false;
            if (pos.X >= Size.X || pos.Y >= Size.Y || pos.Z >= Size.Z) return false;

            return true;
        }

        public void ForEach(Action<T, Point3> func)
        {
            for (int x = 0; x < Size.X; x++)
                for (int y = 0; y < Size.Y; y++)
                    for (int z = 0; z < Size.Z; z++)
                    {
                        var pos = new Point3(x, y, z);
                        func(this[pos], pos);
                    }
        }

        public T GetTiled(Point3 pos)
        {
            // Dont call the this[] directly, its about 20% slower when i tested it. Probably because of the out of bounds check
            pos.X = TWMath.nfmod(pos.X, Size.X);
            pos.Y = TWMath.nfmod(pos.Y, Size.Y);
            pos.Z = TWMath.nfmod(pos.Z, Size.Z);
            return arr[pos.X,pos.Y,pos.Z];
        }
       
    }
}