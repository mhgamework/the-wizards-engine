using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public interface IRenderable:ISpelObject 
    {
        void OnBeforeRender(object sender, RenderEventArgs e);

        void OnRender(object sender, RenderEventArgs e);

        void OnAfterRender(object sender, RenderEventArgs e);

    }
}
