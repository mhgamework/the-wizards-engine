using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.SkyMerchant._Engine.Spatial;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Lod
{
    public interface IRenderable : IBoundingBox
    {
        bool Visible { get; set; }
    }
}
