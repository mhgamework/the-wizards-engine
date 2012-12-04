using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Raytracing;

namespace MHGameWork.TheWizards.CG
{
    /// <summary>
    /// Responsible for tracing the world and returning fragment input
    /// </summary>
    public interface ITraceableScene
    {
        bool Intersect(RayTrace rayTrace, out IShadeCommand command, bool generateShadeCommand);
    }
}
