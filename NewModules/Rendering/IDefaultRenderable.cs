using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.Rendering
{
    public interface IDefaultRenderable : IXNAObject
    {
        DefaultRenderElement CreateRenderElement();
    }
}
