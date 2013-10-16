using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.GameObjects
{
    /// <summary>
    /// Implements the IMeshRenderComponent using the Physical object
    /// </summary>
    public class PhysicalMeshRenderComponent:IMeshRenderComponent
    {
        private readonly SkyPhysical ph;

        public PhysicalMeshRenderComponent(SkyPhysical ph)
        {
            this.ph = ph;
        }

        public IMesh Mesh
        {
            get { return ph.Mesh; }
            set { ph.Mesh = value; }
        }

        public bool Visible
        {
            get { return ph.Visible; }
            set { ph.Visible = value; }
        }

        public Matrix ObjectMatrix
        {
            get { return ph.ObjectMatrix; }
            set { ph.ObjectMatrix = value; }
        }
    }
}