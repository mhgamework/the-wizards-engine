using System;
using System.Linq;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    public class DeferredMeshRenderElement : ICullable
    {
        public DeferredMeshRenderElement(DeferredMeshRenderer renderer, IMesh mesh, MeshBoundingBoxFactory boundingBoxFactory)
        {
            Renderer = renderer;
            Mesh = mesh;
            worldMatrix = Matrix.Identity;
            visible = true;
            CastsShadows = true;
            cachedMeshCorners = boundingBoxFactory.GetCorners(mesh);
            updateBoundingBox();
        }

        public DeferredMeshRenderer Renderer { get; private set; }
        public bool CastsShadows { get; set; }
        public IMesh Mesh { get; private set; }
        private Matrix worldMatrix;
        private BoundingBox boundingBox;
        private bool visible;

        private Vector3[] cachedMeshCorners;

        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set
            {
                worldMatrix = value;
                updateBoundingBox();
                Renderer.UpdateWorldMatrix(this);
            }
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        internal int ElementNumber { get; set; }



        private void updateBoundingBox()
        {
            boundingBox = SlimDX.BoundingBox.FromPoints(Vector3.TransformCoordinate(cachedMeshCorners, ref worldMatrix));
        }



        /// <summary>
        /// Removes this element from the renderer
        /// </summary>
        public void Delete()
        {
            if (Renderer != null)
                Renderer.DeleteMesh(this);
            Renderer = null;
            Mesh = null;

        }

        public bool IsDeleted { get { return Renderer == null; } }

        #region ICullable Members

        public Microsoft.Xna.Framework.BoundingBox BoundingBox
        {
            get { return boundingBox.xna(); }
        }

        private int visibleReferenceCount;
        int ICullable.VisibleReferenceCount
        {
            get { return visibleReferenceCount; }
            set
            {
                if (visibleReferenceCount == value) return; visibleReferenceCount = value;
            }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                if (value == visible) return;
                visible = value;
            }
        }

        #endregion
    }
}
