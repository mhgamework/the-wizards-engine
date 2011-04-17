using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Scene
{

    /// <summary>
    /// This is NOT a data class
    /// This represents a GamePlay-level entity.
    /// </summary>
    public class Entity
    {
        public Transformation Transformation { get; set; }
        public bool Visible { get; set; }
        public bool Solid { get; set; }
        public IMesh Mesh { get; set; }
        public BoundingBox BoundingBox { get; set; }

        public float? Raycast(Ray ray)
        {
            throw new NotImplementedException();
        }



        private MeshRenderElement renderElement;

        internal void UpdateRenderElement(MeshRenderer renderer)
        {

            if (renderElement != null && renderElement.Mesh != Mesh)
            {
                renderElement.Delete();
                renderElement = null;
            }
            if (renderElement == null)
                renderElement = renderer.AddMesh(Mesh);


            renderElement.WorldMatrix = Transformation.CreateMatrix();
        }

    }
}
