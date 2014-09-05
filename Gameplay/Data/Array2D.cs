using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.GodGame;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures
{
    /// <summary>
    /// Represents a array that can be accessed using Point3
    /// Allows infinite accessing, crossing boundaries returns the default value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Array2D<T>
    {
        private T[,] arr;
        public Point2 Size { get; private set; }

        public IEnumerable<T> Values
        {
            get
            {
                for (int x = 0; x < Size.X; x++)
                    for (int y = 0; y < Size.Y; y++)
                    {
                        yield return arr[x, y];
                    }
            }
        }

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

        public void ForEach(Action<T, Point2> func)
        {
            for (int x = 0; x < Size.X; x++)
                for (int y = 0; y < Size.Y; y++)
                {
                    var pos = new Point2(x, y);
                    func(this[pos], pos);
                }
        }

        public IEnumerable<T> Get8Connected(Point2 coord)
        {
            yield return this[new Point2(coord.X + 1, coord.Y + 1)];
            yield return this[new Point2(coord.X + 1, coord.Y + 0)];
            yield return this[new Point2(coord.X + 1, coord.Y - 1)];
            yield return this[new Point2(coord.X + 0, coord.Y + 1)];
            //yield return this[new Point2(coord.X + 0, coord.Y + 0)];
            yield return this[new Point2(coord.X + 0, coord.Y - 1)];
            yield return this[new Point2(coord.X - 1, coord.Y + 1)];
            yield return this[new Point2(coord.X - 1, coord.Y + 0)];
            yield return this[new Point2(coord.X - 1, coord.Y - 1)];
        }
    }
}