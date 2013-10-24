using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects
{
    /// <summary>
    /// This component adds rendering of a mesh to the component.
    /// </summary>
    public interface IMeshRenderComponent : IGameObjectComponent
    {
        IMesh Mesh { get; set; }
        bool Visible { get; set; }
        Matrix ObjectMatrix { get; set; }
    }
}