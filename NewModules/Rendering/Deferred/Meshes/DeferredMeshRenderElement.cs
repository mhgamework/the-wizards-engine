using System;
using System.Linq;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    /// <summary>
    /// Responsible for providing a facade to a mesh element in the deferred renderer
    /// </summary>
    public class DeferredMeshRenderElement : ICullable
    {
        public DeferredMeshRenderElement(DeferredMeshRenderer renderer, IMesh mesh)
        {
            Renderer = renderer;
            Mesh = mesh;
            worldMatrix = Matrix.Identity;
            visible = true;
            CastsShadows = true;
            updateBoundingBox();
        }

        public DeferredMeshRenderer Renderer { get; private set; }
        public bool CastsShadows { get; set; }
        public IMesh Mesh { get; private set; }
        private Matrix worldMatrix;
        private BoundingBox boundingBox;
        private bool visible;

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

            //TODO: optimize!
            //Lol!
            // EDIT: MOAR LOL!
            boundingBox = Mesh.GetCoreData().Parts.Select(part => (part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position).Length == 0 ? new Microsoft.Xna.Framework.BoundingBox() :
                Microsoft.Xna.Framework.BoundingBox.CreateFromPoints(
                part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position))))
                .Aggregate(new Microsoft.Xna.Framework.BoundingBox(), (current, t) => current.MergeWith(t)).dx();

            boundingBox = SlimDX.BoundingBox.FromPoints(Vector3.TransformCoordinate(boundingBox.GetCorners(), ref worldMatrix));


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
