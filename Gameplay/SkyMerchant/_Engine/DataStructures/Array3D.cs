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
            for (int i = 0; i < 3; i++)
            {
                if (pos[i] < 0) return false;
                if (pos[i] >= Size[i]) return false;
            }
            return true;
        }
    }
}