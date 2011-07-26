using System;
using SlimDX;
using BoundingBox = Microsoft.Xna.Framework.BoundingBox;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    public class DeferredMeshRenderElement : ICullable
    {
        public DeferredMeshRenderer Renderer { get; private set; }
        public IMesh Mesh { get; private set; }
        private Matrix worldMatrix;
        private BoundingBox boundingBox;

        public Matrix WorldMatrix
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


        public DeferredMeshRenderElement(DeferredMeshRenderer renderer, IMesh mesh)
        {
            Renderer = renderer;
            Mesh = mesh;
            worldMatrix = Matrix.Identity;
            updateBoundingBox();
        }

        private void updateBoundingBox()
        {
            throw new NotImplementedException();
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

        #region ICullable Members

        Microsoft.Xna.Framework.BoundingBox ICullable.BoundingBox
        {
            get { return boundingBox; }
        }

        int ICullable.VisibleReferenceCount { get; set; }

        #endregion
    }
}
