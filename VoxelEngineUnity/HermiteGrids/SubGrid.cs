
using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Selects a part of the decorated grid, with an offset and new size, with subsampling support
    /// TODO: subsampling doesnt work because we need a fix for normals.
    /// </summary>
    public class SubGrid : AbstractHermiteGrid
    {
        private readonly AbstractHermiteGrid fullGrid;
        private readonly global::DirectX11.Point3_Adapter offset;
        private readonly global::DirectX11.Point3_Adapter size;
        private readonly int sampleResolution;

        /// <summary>
        /// SampleResolution of 1 means same resolution, resolution of 2 means skip 1 every 2 voxels
        /// Size must be divisible by sampleResolution
        /// </summary>
        /// <param name="fullGrid"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="sampleResolution"></param>
        public SubGrid(AbstractHermiteGrid fullGrid, global::DirectX11.Point3_Adapter offset, global::DirectX11.Point3_Adapter size, int sampleResolution)
        {
            this.fullGrid = fullGrid;
            this.offset = offset;
            this.size = size;
            this.sampleResolution = sampleResolution;
            for (int i = 0; i < 3; i++)
            {
                if (size[i] % sampleResolution != 0) throw new InvalidOperationException("Subgrid has invalid sample resolution");

                if (offset[i] + size[i] > fullGrid.Dimensions[i]) throw new InvalidOperationException("Subgrid is partially or completely outside the fullgrid");
                if (offset[i] < 0) throw new InvalidOperationException("Subgrid is partially or completely outside the fullgrid");
            }
        }

        public override bool GetSign(global::DirectX11.Point3_Adapter pos)
        {
            global::DirectX11.Point3_Adapter offsetPos = (pos - offset) * sampleResolution;
            return fullGrid.GetSign(offsetPos);
        }

        public override global::DirectX11.Point3_Adapter Dimensions
        {
            get { return size; }
        }

        public override global::MHGameWork.TheWizards.Vector4_Adapter getEdgeData(global::DirectX11.Point3_Adapter cube, int edgeId)
        {
            global::DirectX11.Point3_Adapter offsetPos = (cube - offset) * sampleResolution;
            return fullGrid.getEdgeData(offsetPos, edgeId);
        }

        public override global::MHGameWork.TheWizards.DualContouring.DCVoxelMaterial_Adapter GetMaterial(global::DirectX11.Point3_Adapter cube)
        {
            global::DirectX11.Point3_Adapter offsetPos = (cube - offset) * sampleResolution;
            return fullGrid.GetMaterial(offsetPos);
        }
    }
}