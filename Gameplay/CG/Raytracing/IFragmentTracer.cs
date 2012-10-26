using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.CG
{
    /// <summary>
    /// Responsible for tracing the world and returning fragment input
    /// </summary>
    public interface IFragmentTracer
    {
        FragmentInput TraceFragment(RayTrace rayTrace);
    }
}
