using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    public interface ISurface
    {
        void Intersects(ref RayTrace trace, out float? result, out IShadeCommand shadeCommand, bool generateShadeCommand);
    }
}
