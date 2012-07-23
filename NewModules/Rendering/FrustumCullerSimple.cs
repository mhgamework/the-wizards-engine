using System;
using System.Diagnostics;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering
{
    public interface ICuller
    {
        ICamera CullCamera { get; set; }
        void AddCullable(ICullable cullable);
        void RemoveCullable(ICullable cullable);
        void UpdateVisibility();
        void UpdateCullable(ICullable cullable);
    }

    /// <summary>
    /// TODO: the height of the treenodes should be updated to encapsulate the cullables height
    /// </summary>
    [Obsolete("Only for backwards compatibility")]
    public class FrustumCullerSimple : ICuller
    {
        private FrustumCuller culler;
        public FrustumCullerView View { get; private set; }

        public FrustumCullerSimple(BoundingBox quadtreeBounding, int numberSplits)
        {
            culler = new FrustumCuller(quadtreeBounding, numberSplits + 1);
            View = culler.CreateView();
        }

        public ICamera CullCamera { get; set; }

        public FrustumCuller.CullNode RootNode       
        {
            get { return culler.RootNode; }
        }

        public void AddCullable(ICullable cullable)
        {
            culler.AddCullable(cullable);

        }
        public void RemoveCullable(ICullable cullable)
        {
            culler.RemoveCullable(cullable);
        }

        public void UpdateVisibility()
        {
            View.UpdateVisibility(CullCamera.ViewProjection);
        }

        public void UpdateCullable(ICullable cullable)
        {
            culler.UpdateCullable(cullable);
        }

    }
}