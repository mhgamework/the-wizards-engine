using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.Shading
{
    public interface ILightProvider
    {
        IEnumerable<PointLight> GetApplicableLights(TraceResult input, RayTrace trace);
    }

    }
