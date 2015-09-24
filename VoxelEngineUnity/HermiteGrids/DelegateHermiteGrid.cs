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
        private Func<global::DirectX11.Point3_Adapter, bool> getSignDel;
        private Func<global::DirectX11.Point3_Adapter, int, global::MHGameWork.TheWizards.Vector4_Adapter> getEdgeDataDel;
        private global::DirectX11.Point3_Adapter dimensions;
        private readonly Func<global::DirectX11.Point3_Adapter, global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter> getMaterial;

        public DelegateHermiteGrid(Func<global::DirectX11.Point3_Adapter, bool> getSignDel, Func<global::DirectX11.Point3_Adapter, int, global::MHGameWork.TheWizards.Vector4_Adapter> getEdgeDataDel, global::DirectX11.Point3_Adapter dimensions)
        {
            this.getSignDel = getSignDel;
            this.getEdgeDataDel = getEdgeDataDel;
            this.dimensions = dimensions;
            global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter mat = new global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter() { Texture = global::MHGameWork.TheWizards.DualContouring.DCFiles_Adapter.UVCheckerMap10_512 };
            this.getMaterial = _ => mat;
        }
        public DelegateHermiteGrid(Func<global::DirectX11.Point3_Adapter, bool> getSignDel, Func<global::DirectX11.Point3_Adapter, int, global::MHGameWork.TheWizards.Vector4_Adapter> getEdgeDataDel, global::DirectX11.Point3_Adapter dimensions, Func<global::DirectX11.Point3_Adapter, global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter> getMaterial)
        {
            this.getSignDel = getSignDel;
            this.getEdgeDataDel = getEdgeDataDel;
            this.dimensions = dimensions;
            this.getMaterial = getMaterial;
        }

        public override bool GetSign(global::DirectX11.Point3_Adapter pos)
        {
            return getSignDel(pos);
        }

        public override global::DirectX11.Point3_Adapter Dimensions { get { return dimensions; } }

        public override global::MHGameWork.TheWizards.Vector4_Adapter getEdgeData(global::DirectX11.Point3_Adapter cube, int edgeId)
        {
            return getEdgeDataDel(cube, edgeId);
        }

        public override global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter GetMaterial(global::DirectX11.Point3_Adapter cube)
        {
            return getMaterial(cube);
        }
    }
}