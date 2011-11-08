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
        private readonly Vector3 scaling;
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
            scaling = new Vector3(1, 1, 1);
        }

        public BuildSlot(DynamicBlock block, Vector3 RelativeTranslation, float RelativeRotationY, Vector3 scaling, DeferredRenderer renderer)
        {
            this.block = block;
            this.RelativeTranslation = RelativeTranslation;
            this.RelativeRotationY = RelativeRotationY;
            this.renderer = renderer;
            this.scaling = scaling;
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
                meshEl.WorldMatrix = Matrix.Scaling(scaling) * Matrix.RotationY(RelativeRotationY)*Matrix.Translation(RelativeTranslation)*
                                     Matrix.Translation(block.Position);
            }
        }
    }
}
