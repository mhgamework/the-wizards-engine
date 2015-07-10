using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;

namespace MHGameWork.TheWizards.Rendering.Default
{
    public interface IDefaultRenderable : IXNAObject
    {
        DefaultRenderElement CreateRenderElement();
    }
}
