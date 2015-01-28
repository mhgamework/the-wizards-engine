using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    public class UnionGrid : AbstractHermiteGrid
    {
        private readonly AbstractHermiteGrid a;
        private readonly AbstractHermiteGrid b;

        public UnionGrid(AbstractHermiteGrid a, AbstractHermiteGrid b)
        {
            if (a.Dimensions != b.Dimensions) throw new InvalidOperationException("Should have same dimensions");
            this.a = a;
            this.b = b;

        }

        public override bool GetSign(Point3 pos)
        {
            return a.GetSign(pos) || b.GetSign(pos);
        }

        public override Point3 Dimensions
        {
            get { return a.Dimensions; }
        }

        public override Vector4 getEdgeData(Point3 cube, int edgeId)
        {
            var signs = a.GetEdgeSigns(cube,edgeId);
            if (signs[0] != signs[1])
                return a.getEdgeData(cube, edgeId);
            signs = b.GetEdgeSigns(cube, edgeId);
            if (signs[0] != signs[1])
                return b.getEdgeData(cube, edgeId);


            throw new InvalidOperationException("No crossing edge here!");
        }

        
    }
}