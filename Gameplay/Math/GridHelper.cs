using System.Collections.Generic;
using DirectX11;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// Helper class for working with grids 2D and 3D
    /// </summary>
    public static class GridHelper
    {
        /// <summary>
        /// x,y,z
        /// </summary>
        public static readonly Point3[] Axes3D = new[] { new Point3(1, 0, 0), new Point3(0, 1, 0), new Point3(0, 0, 1) };
        /// <summary>
        /// x,y,z,-x,-y,-z
        /// </summary>
        public static readonly Point3[] OrthogonalDirections3D = new[] { new Point3(1, 0, 0), new Point3(0, 1, 0), new Point3(0, 0, 1),
                                                                new Point3(-1, 0, 0), new Point3(0, -1, 0), new Point3(0, 0, -1)};

        public static readonly Point3[] UnitCubeCorners = new[] { new Point3(0, 0, 0), new Point3(1, 0, 0), new Point3(0, 1, 0), new Point3(0, 0, 1),
                                                                       new Point3(1,1,0),new Point3(0,1,1),new Point3(1,0,1),new Point3(1,1,1),};
    }
}