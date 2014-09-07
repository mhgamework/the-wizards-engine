using System.Drawing;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.Scattered._Engine;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers
{
    public class RangeVisualizer : IRenderable
    {
        private readonly GameVoxel handle;
        private readonly int range;

        public RangeVisualizer(IVoxelHandle handle, int range)
        {
            // WARNING: cannot keep handle because it can be shared across voxels!!
            // IDEA: should autoconvert between the gameplay voxel type and the rendering voxel type
            this.handle = handle.GetInternalVoxel();
            this.range = range;
        }

        public void Show()
        {

        }

        public void Update()
        {
            var size = handle.GetBoundingBox().Maximum - handle.GetBoundingBox().Minimum;
            var bb = handle.GetBoundingBox().GetCenter().ToBoundingBox(new Vector3((range+0.5f) * size.X, 1, (range+0.5f) * size.Z));
            TW.Graphics.LineManager3D.AddBox(bb, Color.Red);
        }

        public void Hide()
        {
        }
    }
}