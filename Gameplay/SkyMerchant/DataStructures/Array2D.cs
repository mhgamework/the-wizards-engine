using DirectX11;

namespace MHGameWork.TheWizards.SkyMerchant.DataStructures
{
    /// <summary>
    /// Represents a array that can be accessed using Point3
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Array2D<T>
    {
        private T[,] arr;
        public Point2 Size { get; private set; }

        public Array2D(Point2 size)
        {
            arr = new T[size.X, size.Y];
            Size = size;
        }
        public T this[Point2 pos]
        {
            get
            {
                if (!InArray(pos)) return default(T);
                return arr[pos.X, pos.Y];
            }
            set { arr[pos.X, pos.Y] = value; }
        }

        public bool InArray(Point2 pos)
        {
            for (int i = 0; i < 2; i++)
            {
                if (pos[i] < 0) return false;
                if (pos[i] >= Size[i]) return false;
            }
            return true;
        }

        public Array2D<T> Copy()
        {
            var r = this;
            var ret = new Array2D<T>(r.Size);
            ret.arr = (T[,])r.arr.Clone();
            return ret;
        }
    }
}