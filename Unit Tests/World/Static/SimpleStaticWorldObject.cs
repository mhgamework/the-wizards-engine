using System;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.World;
using MHGameWork.TheWizards.World.Static;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Tests.World.Static
{
    public class SimpleStaticWorldObject : IStaticWorldObject
    {
        public bool Change { get; set; }
        public int ID { get; set; }


        private Matrix worldMatrix;
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set
            {
                worldMatrix = value;
                if (renderElement != null) renderElement.WorldMatrix = value;
            }
        }

        private IMesh mesh;
        public IMesh Mesh
        {
            get { return mesh; }
            private set { mesh = value; }
        }

        private readonly MeshRenderer renderer;
        private MeshRenderElement renderElement;

        public SimpleStaticWorldObject(MeshRenderer renderer)
        {
            this.renderer = renderer;
            WorldMatrix = Matrix.Identity;
        }

        public void Delete()
        {
            deleteRenderElement();
        }

        private void deleteRenderElement()
        {
            if (renderElement == null) return;
            renderElement.WorldMatrix = new Matrix();
            renderElement = null;


        }
        public void ChangeMesh(IMesh newMesh)
        {
            deleteRenderElement();
            renderElement = renderer.AddMesh(newMesh);
        }
    }
}
