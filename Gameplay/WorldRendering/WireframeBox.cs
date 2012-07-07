using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Synchronization;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    [NoSync]
    public class WireframeBox : BaseModelObject
    {
        public WireframeBox()
        {
            BoundingBox = new BoundingBox(Vector3.Zero, MathHelper.One);
            WorldMatrix = Matrix.Identity;
            Visible = true;
        }


        public BoundingBox BoundingBox { get; set; }
        public Matrix WorldMatrix { get; set; }
        public bool Visible { get; set; }
    }
}
