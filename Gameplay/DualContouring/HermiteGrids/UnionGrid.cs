using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// The first grid is the source grid, the second grid is added with given offset
    /// </summary>
    public class UnionGrid : AbstractHermiteGrid
    {
        private readonly Point3 _offset;
        private readonly AbstractHermiteGrid a;
        private readonly AbstractHermiteGrid b;

        public UnionGrid(AbstractHermiteGrid a, AbstractHermiteGrid b, Point3 offset)
        {
            _offset = offset;
            this.a = a;
            this.b = b;
        }

        public UnionGrid(AbstractHermiteGrid a, AbstractHermiteGrid b) : this(a, b, new Point3()) { }

        public override bool GetSign(Point3 pos)
        {
            return a.GetSign(pos) || b.GetSign(pos + _offset);
        }

        public override Point3 Dimensions
        {
            get { return a.Dimensions; }
        }

        public override Vector4 getEdgeData(Point3 cube, int edgeId)
        {
            var signs = a.GetEdgeSigns(cube, edgeId);
            if (signs[0] != signs[1])
                return a.getEdgeData(cube, edgeId);
            signs = b.GetEdgeSigns(cube + _offset, edgeId);
            if (signs[0] != signs[1])
                return b.getEdgeData(cube + _offset, edgeId);


            throw new InvalidOperationException("No crossing edge here!");
        }


    }
}