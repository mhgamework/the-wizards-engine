using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.Building;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for storing and rendering a BuildUnit. It has a position relative to the position of the DynamicBlock it belongs to.
    /// </summary>
    public class BuildSlot
    {
        public BuildUnit BuildUnit;
        private readonly DynamicBlock block;
        public Vector3 RelativeTranslation;
        public float RelativeRotationY;
        private DeferredMeshRenderElement meshEl;
        private readonly DeferredRenderer renderer;

        public BuildSlot(DynamicBlock block, Vector3 RelativeTranslation, float RelativeRotationY, DeferredRenderer renderer)
        {
            this.block = block;
            this.RelativeTranslation = RelativeTranslation;
            this.RelativeRotationY = RelativeRotationY; 
            this.renderer = renderer;
        }

        public void SetBuildUnit(BuildUnit buildUnit)
        {
            if (meshEl != null)
            {
                meshEl.Delete();
                meshEl = null;
            }
            this.BuildUnit = buildUnit;
            if (buildUnit != null)
            {
                meshEl = renderer.CreateMeshElement(buildUnit.Mesh);
                meshEl.WorldMatrix = Matrix.RotationY(RelativeRotationY)*Matrix.Translation(RelativeTranslation)*
                                     Matrix.Translation(block.Position);
            }
        }
    }
}
