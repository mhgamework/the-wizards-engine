using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Synchronization;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    [NoSync]
    public class WireframeBox : EngineModelObject
    {

        public WireframeBox()
        {
            WorldMatrix = Matrix.Identity;
            Visible = true;
        }


        public Matrix WorldMatrix { get; set; }
        public bool Visible { get; set; }
        public Color4 Color { get; set; }

    }
}
