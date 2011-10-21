using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    public struct Point3
    {
        public int X;
        public int Y;
        public int Z;

      

        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3(Vector3 v)
        {
            X = (int)Math.Floor(v.X);
            Y = (int)Math.Floor(v.Y);
            Z = (int)Math.Floor(v.Z);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X,Y,Z);
        }
        public static implicit operator Vector3(Point3 p)
        {
            return p.ToVector3();
        }
        public static Boolean operator ==(Point3 p, Point3 p2)
        {
            return p.hasSameValue(p2);
        }
        public static Boolean operator !=(Point3 p, Point3 p2)
        {
            return !p.hasSameValue(p2);
        }

        public Boolean hasSameValue(Point3 pos)
        {
            if (pos.X == X && pos.Y == Y && pos.Z == Z)
                return true;
            return false;
        }
    }               
}                   
                    