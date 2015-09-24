namespace MHGameWork.TheWizards.DualContouring
{
    public abstract class AbstractHermiteGrid_Adapter
    {
        public virtual System.Int32 GetEdgeId(global::DirectX11.Point3_Adapter start, global::DirectX11.Point3_Adapter end)
        {
            throw new System.InvalidOperationException();
        }

        public global::DirectX11.Point3_Adapter[] GetEdgeOffsets(System.Int32 edgeId)
        {
            throw new System.InvalidOperationException();
        }

        public void ForEachCube(global::System.Action<global::DirectX11.Point3_Adapter> action)
        {
            throw new System.InvalidOperationException();
        }

        public System.Boolean HasEdgeData(global::DirectX11.Point3_Adapter cube, System.Int32 edgeId)
        {
            throw new System.InvalidOperationException();
        }

        public System.Boolean[] GetEdgeSigns(global::DirectX11.Point3_Adapter cube, System.Int32 edgeId)
        {
            throw new System.InvalidOperationException();
        }

        public abstract global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial GetMaterial(global::DirectX11.Point3_Adapter cube);
        public System.Int32[] GetAllEdgeIds()
        {
            throw new System.InvalidOperationException();
        }

        public System.Int32[] GetEdgeVertexIds(System.Int32 edgeId)
        {
            throw new System.InvalidOperationException();
        }

        public void GetCubeSigns(global::DirectX11.Point3_Adapter cube, System.Boolean[] output)
        {
            throw new System.InvalidOperationException();
        }

        public abstract global::MHGameWork.TheWizards.Vector4_Adapter getEdgeData(global::DirectX11.Point3_Adapter cube, System.Int32 edgeId);
        public global::MHGameWork.TheWizards.Vector3_Adapter GetEdgeIntersectionCubeLocal(global::DirectX11.Point3_Adapter cube, System.Int32 edgeId)
        {
            throw new System.InvalidOperationException();
        }

        public global::MHGameWork.TheWizards.Vector3_Adapter GetEdgeNormal(global::DirectX11.Point3_Adapter curr, System.Int32 i)
        {
            throw new System.InvalidOperationException();
        }
    }
}