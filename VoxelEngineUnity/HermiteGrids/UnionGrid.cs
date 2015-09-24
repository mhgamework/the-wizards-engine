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
        private readonly global::DirectX11.Point3_Adapter _offset;
        private readonly AbstractHermiteGrid a;
        private readonly AbstractHermiteGrid b;

        public UnionGrid(AbstractHermiteGrid a, AbstractHermiteGrid b, global::DirectX11.Point3_Adapter offset)
        {
            _offset = offset;
            this.a = a;
            this.b = b;
        }

        public UnionGrid(AbstractHermiteGrid a, AbstractHermiteGrid b) : this(a, b, new global::DirectX11.Point3_Adapter()) { }

        public override bool GetSign(global::DirectX11.Point3_Adapter pos)
        {
            global::DirectX11.Point3_Adapter offsetPos = pos - _offset;
            if (offsetPos.X < 0 || offsetPos.Y < 0 || offsetPos.Z < 0) return a.GetSign(pos);
            return a.GetSign(pos) || b.GetSign(pos - _offset);
        }

        public override global::DirectX11.Point3_Adapter Dimensions
        {
            get { return a.Dimensions; }
        }

        public override global::MHGameWork.TheWizards.Vector4_Adapter getEdgeData(global::DirectX11.Point3_Adapter cube, int edgeId)
        {

            var signs = a.GetEdgeSigns(cube, edgeId);
            if (signs[0] != signs[1])
                return a.getEdgeData(cube, edgeId);



            signs = b.GetEdgeSigns(cube - _offset, edgeId);
            if (signs[0] != signs[1])
                return b.getEdgeData(cube - _offset, edgeId);


            throw new InvalidOperationException("No crossing edge here!");
        }

        public override global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter GetMaterial(global::DirectX11.Point3_Adapter cube)
        {
            if (a.GetSign(cube)) return a.GetMaterial(cube);
            if (b.GetSign(cube - _offset)) return b.GetMaterial(cube - _offset);
            return null;
        }
    }
}