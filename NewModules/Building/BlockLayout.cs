using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    public struct BlockLayout
    {
        public uint Layout;

        public static Dictionary<Point3, uint> Masks = new Dictionary<Point3, uint>();
        public static List<Point3> Axes = new List<Point3>();
        static BlockLayout()
        {
          

            var axes = new List<Point3>();
            axes = Axes;
            axes.Add(new Point3(-1, -1, -1));
            axes.Add(new Point3(-1, -1, 0));
            axes.Add(new Point3(-1, -1, 1));
            axes.Add(new Point3(-1, 0, -1));
            axes.Add(new Point3(-1, 0, 0));
            axes.Add(new Point3(-1, 0, 1));
            axes.Add(new Point3(-1, 1, -1));
            axes.Add(new Point3(-1, 1, 0));
            axes.Add(new Point3(-1, 1, 1));

            axes.Add(new Point3(0, -1, -1));
            axes.Add(new Point3(0, -1, 0));
            axes.Add(new Point3(0, -1, 1));
            axes.Add(new Point3(0, 0, -1));
            //axes.Add(new Point3(0, 0, 0));
            axes.Add(new Point3(0, 0, 1));
            axes.Add(new Point3(0, 1, -1));
            axes.Add(new Point3(0, 1, 0));
            axes.Add(new Point3(0, 1, 1));
            
            axes.Add(new Point3(1, -1, -1));
            axes.Add(new Point3(1, -1, 0));
            axes.Add(new Point3(1, -1, 1));
            axes.Add(new Point3(1, 0, -1));
            axes.Add(new Point3(1, 0, 0));
            axes.Add(new Point3(1, 0, 1));
            axes.Add(new Point3(1, 1, -1));
            axes.Add(new Point3(1, 1, 0));
            axes.Add(new Point3(1, 1, 1));

            
            for (int i = 0; i < axes.Count; i++)
            {
                Masks.Add(axes[i],(uint)( 1 << i));
            }
    Masks.Add(new Point3(0, 0, 0), 0);
          Axes.Add(new Point3(0, 0, 0));
            
        }

        public BlockLayout(Point3[] points) : this()
        {
            Layout = 0;
            for (int i = 0; i < points.Length; i++)
            {
                var p = points[i];
                Layout |= GetMask(p);
            }
        }

        public BlockLayout(uint layout) : this()
        {
            Layout = layout;
        }

        public static uint GetMask(Point3 offset)
        {
            return Masks[offset];
        }

        public override string ToString()
        {
            return Convert.ToString(Layout, 2);
        }
    }
}
