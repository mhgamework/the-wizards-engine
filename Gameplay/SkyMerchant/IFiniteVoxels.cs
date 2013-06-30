using System;
using DirectX11;

namespace SkyMerchantTests
{
    public interface IFiniteVoxels
    {
        void SetVoxel(Point3 pos, VoxelMaterial dirtMaterial);

        Point3 Size { get; }

        float NodeSize { get; }

        VoxelMaterial GetVoxel(Point3 pos);

        bool IsOutside(Point3 copy);
    }
}