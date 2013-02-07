using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Responsible for provides access to the Graphics subsystem.
    /// Currently also input, and not really a wrapper :P
    /// </summary>
    public class GraphicsWrapper : DX11Game
    {
        private DeferredRenderer renderer;

        public DeferredRenderer AcquireRenderer()
        {
            if (renderer == null)
                renderer = new DeferredRenderer(TW.Graphics);

            return renderer;
        }

    }
}
