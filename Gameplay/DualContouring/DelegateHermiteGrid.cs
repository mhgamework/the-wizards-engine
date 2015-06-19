using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Hermite grid implemented using delegate functions
    /// IDEA: auto implement delegate wrappers for all abstract classes/interfaces
    /// </summary>
    public class DelegateHermiteGrid : AbstractHermiteGrid
    {
        private Func<Point3,bool> getSignDel;
        private Func<Point3,int,Vector4> getEdgeDataDel;
        private Point3 dimensions;

        public DelegateHermiteGrid(Func<Point3, bool> getSignDel, Func<Point3, int, Vector4> getEdgeDataDel, Point3 dimensions)
        {
            this.getSignDel = getSignDel;
            this.getEdgeDataDel = getEdgeDataDel;
            this.dimensions = dimensions;
        }

        public override bool GetSign(Point3 pos)
        {
            return getSignDel(pos);
        }

        public override Point3 Dimensions { get { return dimensions; } }

        public override Vector4 getEdgeData(Point3 cube, int edgeId)
        {
            return getEdgeDataDel(cube, edgeId);
        }
    }
}