using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    public class DifferenceGrid : AbstractHermiteGrid
    {
        private readonly AbstractHermiteGrid a;
        private readonly AbstractHermiteGrid b;

        public DifferenceGrid(AbstractHermiteGrid a, AbstractHermiteGrid b)
        {
            if (a.Dimensions != b.Dimensions) throw new InvalidOperationException("Should have same dimensions");
            this.a = a;
            this.b = b;

        }

        public override bool GetSign(Point3 pos)
        {
            return !b.GetSign(pos) && a.GetSign(pos);
        }

        public override Point3 Dimensions
        {
            get { return a.Dimensions; }
        }

        public override Vector4 getEdgeData(Point3 cube, int edgeId)
        {
            if (!a.HasEdgeData(cube, edgeId))
                return b.getEdgeData(cube, edgeId);

            if (!b.HasEdgeData(cube, edgeId))
                return a.getEdgeData(cube, edgeId);

            var da = a.getEdgeData(cube, edgeId).W;
            var db = b.getEdgeData(cube, edgeId).W;

            float diff;

            if (GetSign(cube))
                diff = da - db; // 0 is not air, so if diff > 0 then b is closer
            else
                diff = db - da; // 0 is air, so if diff > 0 then b is closer

            return diff > 0 ? b.getEdgeData(cube, edgeId) : a.getEdgeData(cube, edgeId);

        }


    }
}