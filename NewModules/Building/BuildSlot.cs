using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Model;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Tests.Building;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for storing and rendering a BuildUnit. It has a position relative to the position of the DynamicBlock it belongs to.
    /// </summary>
    [ModelObjectChanged]
    public class BuildSlot : IModelObject
    {
        public Vector3 Scaling
        {
            get { return scaling; }
        }

        public DynamicBlock Block
        {
            get { return block; }
        }

        public Vector3 RelativeTranslation1
        {
            get { return RelativeTranslation; }
        }

        public float RelativeRotationY1
        {
            get { return RelativeRotationY; }
        }

        private readonly Vector3 scaling;
        public BuildUnit BuildUnit { get; set; }
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
            BuildUnit = buildUnit;
            return;
            
        }

        public ModelContainer Container { get; private set; }
        public void Initialize(ModelContainer container)
        {
            Container = container;
        }
    }
}
