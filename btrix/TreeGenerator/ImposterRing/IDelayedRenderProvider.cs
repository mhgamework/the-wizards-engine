using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;

namespace TreeGenerator.ImposterRing
{
    public interface IDelayedRenderProvider
    {
        void StartDelayedRendering(IXNAGame game, ICamera camera);

        bool CanRender();

        void SingleRender();

        bool IsRenderingComplete();
    }
}
