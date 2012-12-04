using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    public interface IShadeCommand
    {
        Color4 CalculateColor();
    }
}
