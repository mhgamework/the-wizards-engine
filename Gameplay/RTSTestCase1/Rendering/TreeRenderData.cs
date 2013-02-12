using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Rendering
{
    public class TreeRenderData : IModelObjectAddon<Tree>
    {
        private readonly Tree tree;
        private Entity entity;

        public TreeRenderData(Tree tree)
        {
            this.tree = tree;
            entity = new Entity();
        }

        public void Update()
        {
            entity.Solid = true;
            entity.Static = true;
            entity.Mesh = TW.Assets.LoadMesh("RTS\\Tree");
            entity.WorldMatrix = Matrix.Scaling(1 + tree.Size*0.1f, 1 + tree.Size*0.1f, 1 + tree.Size*0.1f) * Matrix.Scaling(1f,3,1f)* Matrix.Translation(tree.Position);
        }
        public void Dispose()
        {
            
        }
    }
}
