using System.Collections.Generic;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.Lighting
{
    public interface ILightProvider
    {
        IEnumerable<PointLight> GetApplicableLights(TraceResult input, RayTrace trace);
    }

    }
