using System;
using System.Collections.Generic;
using System.Drawing;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.Scattered._Engine;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class HighlightVoxelsVisualizer : IRenderable
    {
        private readonly Func<IVoxelHandle, IEnumerable<IVoxelHandle>> getHighlights;
        private readonly GameVoxel handle;

        public HighlightVoxelsVisualizer(IVoxelHandle handle, Func<IVoxelHandle, IEnumerable<IVoxelHandle>> getHighlights)
        {
            this.getHighlights = getHighlights;
            // WARNING: cannot keep handle because it can be shared across voxels!!
            // IDEA: should autoconvert between the gameplay voxel type and the rendering voxel type
            this.handle = handle.GetInternalVoxel();
        }

        public void Show()
        {

        }

        public void Update()
        {
            foreach (var highlight in getHighlights(new IVoxelHandle(handle.World, handle)))
            {
                var t = highlight.GetInternalVoxel();
                TW.Graphics.LineManager3D.AddBox(t.GetBoundingBox(), Color.Red.dx());
            }
        }

        public void Hide()
        {
        }
    }
}