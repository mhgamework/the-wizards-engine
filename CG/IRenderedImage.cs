using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG
{
    public interface IRenderedImage
    {
        Color4 GetPixel(Vector2 pos);
    }
}
