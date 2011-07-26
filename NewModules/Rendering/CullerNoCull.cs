using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.Rendering
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