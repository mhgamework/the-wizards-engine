using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing
{
    /// <summary>
    /// This is a Render component which supports mesh rendering
    /// </summary>
    public interface IMeshRenderComponent : IRenderComponent
    {
        IMesh Mesh { get; set; }
        bool Visible { get; set; }
        Matrix ObjectMatrix { get; set; }
    }
}