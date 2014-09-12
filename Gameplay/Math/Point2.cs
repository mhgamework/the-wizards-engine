using System;
using SlimDX;

namespace DirectX11
{
    /// <summary>
    /// This struct represents a discrete vector
    /// </summary>
    [Serializable]
    public struct Point2
    {
        public int X;
        public int Y;



        public Point2(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// This uses a vector3 as a Point2, note that this uses Math.round to convert the coords
        /// </summary>
        /// <param name="v"></param>
        public Point2(Vector2 v)
        {
            X = (int)Math.Round(v.X);
            Y = (int)Math.Round(v.Y);
        }

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    default:
                        throw new ArgumentException("index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    default:
                        throw new ArgumentException("index");
                }
            }
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
        public static implicit operator Vector2(Point2 p)
        {
            return p.ToVector2();
        }
        public static Boolean operator ==(Point2 p, Point2 p2)
        {
            return p.hasSameValue(p2);
        }
        public static Boolean operator !=(Point2 p, Point2 p2)
        {
            return !p.hasSameValue(p2);
        }

        public Boolean hasSameValue(Point2 pos)
        {
            if (pos.X == X && pos.Y == Y)
                return true;
            return false;
        }
        public static Point2 operator -(Point2 p, Point2 p2)
        {
            return p + -p2;
        }
        public static Point2 operator +(Point2 p, Point2 p2)
        {
            return new Point2(p.X + p2.X, p.Y + p2.Y);
        }
        public static Point2 operator -(Point2 p)
        {
            return new Point2(-p.X, -p.Y);
        }

        public override string ToString()
        {
            return string.Format("X: {0},Y: {1}", X, Y);
        }

        public static Point2 UnitX()
        {
            return new Point2(1, 0);
        }
        public static Point2 UnitY()
        {
            return new Point2(0, 1);
        }

        public static Point2 Floor(Vector2 v)
        {
            return new Point2((int)Math.Floor(v.X), (int)Math.Floor(v.Y));
        }
        public static Point2 Ceiling(Vector2 v)
        {
            return new Point2((int)Math.Ceiling(v.X), (int)Math.Ceiling(v.Y));
        }


        public bool Equals(Point2 other)
        {
            return other.X == X && other.Y == Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Point2)) return false;
            return Equals((Point2)obj);
        }



        public override int GetHashCode()
        {
            unchecked
            {
                int result = X;
                result = (result * 397) ^ Y;
                return result;
            }
        }

        public float GetLength()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }


        public void ForEach(Action<Point2> func)
        {
            for (int x = 0; x < X; x++)
                for (int y = 0; y < Y; y++)
                {
                    var pos = new Point2(x, y);
                    func(pos);
                }
        }
    }
}
