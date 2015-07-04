using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    public class DifferenceGrid : AbstractHermiteGrid
    {
        private readonly AbstractHermiteGrid a;
        private readonly AbstractHermiteGrid b;
        private readonly Point3 offset;

        public DifferenceGrid( AbstractHermiteGrid a, AbstractHermiteGrid b ) : this( a, b, new Point3() )
        {
            
        }
        public DifferenceGrid( AbstractHermiteGrid a, AbstractHermiteGrid b, Point3 offset )
        {
            this.a = a;
            this.b = b;
            this.offset = offset;
        }

        public override bool GetSign(Point3 pos)
        {
            return !b.GetSign(pos-offset) && a.GetSign(pos);
        }

        public override Point3 Dimensions
        {
            get { return a.Dimensions; }
        }

        public override Vector4 getEdgeData(Point3 cube, int edgeId)
        {
            var offsetPos = cube - offset;
            if ( offsetPos.X < 0 || offsetPos.Y < 0 || offsetPos.Z < 0
                 || offsetPos.X >= b.Dimensions.X || offsetPos.Y >= b.Dimensions.Y || offsetPos.Z >= b.Dimensions.Z )
                return a.getEdgeData( cube, edgeId );

            if (!a.HasEdgeData(cube, edgeId))
                return b.getEdgeData(offsetPos, edgeId);

            if (!b.HasEdgeData(offsetPos, edgeId))
                return a.getEdgeData(cube, edgeId);

            var da = a.getEdgeData(cube, edgeId).W;
            var db = b.getEdgeData(offsetPos, edgeId).W;

            float diff;

            if (GetSign(cube))
                diff = da - db; // 0 is not air, so if diff > 0 then b is closer
            else
                diff = db - da; // 0 is air, so if diff > 0 then b is closer

            return diff > 0 ? b.getEdgeData(offsetPos, edgeId) : a.getEdgeData(cube, edgeId);

        }


    }
}