using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for representing a block in the world. It keeps information about this block and makes sure it is rendered.
    /// </summary>
    public class Block
    {
        private readonly DeferredRenderer renderer;
        private BlockType type;
        private DeferredMeshRenderElement meshEl;
        public Point3 Position { get; private set; }

        public Block(DeferredRenderer renderer, BlockType type, Point3 position)
        {
            this.renderer = renderer;
            this.type = type;
            Position = position;
            meshEl = renderer.CreateMeshElement(type.Mesh);
            meshEl.WorldMatrix = Matrix.Translation(position+ new Vector3(0.5f,0,0.5f));
            
        }

        public void DeleteFromRenderer()
        {
            meshEl.Delete();
        }

        public void ChangeTypeTo(BlockType type)
        {
            meshEl.Delete();
            this.type = type;
            meshEl = renderer.CreateMeshElement(type.Mesh);
            meshEl.WorldMatrix = type.Transformation * Matrix.Translation(Position + new Vector3(0.5f, 0, 0.5f));
        }
    }
}
