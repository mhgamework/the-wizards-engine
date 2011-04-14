using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public interface IRenderPrimitives
    {
        void RenderPrimitives();
        Microsoft.Xna.Framework.Matrix WorldMatrix { get;}
    }
}
