
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;

namespace MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Culling
{
    /// <summary>
    /// A culler that doesnt cull 
    /// for testing , DUH!
    /// </summary>
    public class CullerNoCull : ICuller
    {
        public CullerNoCull()
        {

        }

        public ICamera CullCamera
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public void AddCullable(ICullable cullable)
        {
            cullable.VisibleReferenceCount++;
        }
        public void RemoveCullable(ICullable cullable)
        {
            cullable.VisibleReferenceCount--;
        }

        public void UpdateVisibility()
        {
        }

        public void UpdateCullable(ICullable cullable)
        {
        }

    }
}