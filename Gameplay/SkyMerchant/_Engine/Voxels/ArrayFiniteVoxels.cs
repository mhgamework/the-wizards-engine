using DirectX11;
using MHGameWork.TheWizards.SkyMerchant.DataStructures;
using MHGameWork.TheWizards.SkyMerchant.Voxels;

namespace MHGameWork.TheWizards.SkyMerchant.Generation
{
    /// <summary>
    /// Represents a 3D voxel set
    /// </summary>
    public class ArrayFiniteVoxels : IFiniteVoxels
    {
        private Array3D<VoxelMaterial> arr;

        public ArrayFiniteVoxels(Point3 size)
        {
            arr = new Array3D<VoxelMaterial>(size);
            NodeSize = 1;
        }
        public void SetVoxel(Point3 pos, VoxelMaterial dirtMaterial)
        {
            arr[pos] = dirtMaterial;
        }

        public Point3 Size { get { return arr.Size; } }
        public float NodeSize { get; set; }
        public VoxelMaterial GetVoxel(Point3 pos)
        {
            return arr[pos];
        }

        public bool IsOutside(Point3 copy)
        {
            return !arr.InArray(copy);
        }
    }
}