using System;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Rendering
{
    public class SimpleMeshRenderElement : IMeshRenderElement
    {
        public SimpleMeshRenderer Renderer { get; private set; }
        public IMesh Mesh { get; private set; }
        private Matrix worldMatrix;
        public Microsoft.Xna.Framework.Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set
            {
                worldMatrix = value;
                Renderer.UpdateWorldMatrix(this);
            }
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        internal int ElementNumber { get; set; }


        public SimpleMeshRenderElement(SimpleMeshRenderer renderer, IMesh mesh)
        {
            Renderer = renderer;
            Mesh = mesh;
            worldMatrix = Matrix.Identity;
        }


        /// <summary>
        /// Removes this element from the renderer
        /// </summary>
        public void Delete()
        {
            Renderer.DeleteMesh(this);
            Renderer = null;
            Mesh = null;

        }

        public bool IsDeleted { get { return Renderer == null; } }
    }
}
