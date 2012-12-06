using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    public interface IShadeCommand
    {
        Color4 CalculateColor();
    }
}
