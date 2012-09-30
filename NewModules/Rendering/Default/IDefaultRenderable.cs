using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.Rendering.Default
{
    public interface IDefaultRenderable : IXNAObject
    {
        DefaultRenderElement CreateRenderElement();
    }
}
