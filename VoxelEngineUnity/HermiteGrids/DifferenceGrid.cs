using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    public class DifferenceGrid : AbstractHermiteGrid
    {
        private readonly AbstractHermiteGrid a;
        private readonly AbstractHermiteGrid b;
        private readonly global::DirectX11.Point3_Adapter offset;

        public DifferenceGrid( AbstractHermiteGrid a, AbstractHermiteGrid b ) : this( a, b, new global::DirectX11.Point3_Adapter() )
        {
            
        }
        public DifferenceGrid( AbstractHermiteGrid a, AbstractHermiteGrid b, global::DirectX11.Point3_Adapter offset )
        {
            this.a = a;
            this.b = b;
            this.offset = offset;
        }

        public override bool GetSign(global::DirectX11.Point3_Adapter pos)
        {
            return !b.GetSign(pos-offset) && a.GetSign(pos);
        }

        public override global::DirectX11.Point3_Adapter Dimensions
        {
            get { return a.Dimensions; }
        }

        public override global::MHGameWork.TheWizards.Vector4_Adapter getEdgeData(global::DirectX11.Point3_Adapter cube, int edgeId)
        {
            global::DirectX11.Point3_Adapter offsetPos = cube - offset;
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

        public override global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter GetMaterial( global::DirectX11.Point3_Adapter cube )
        {
            if ( !b.GetSign( cube - offset ) ) return a.GetMaterial( cube );
            return null;
        }
    }
}