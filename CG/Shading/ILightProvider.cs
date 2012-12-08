using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Raytracing;

namespace MHGameWork.TheWizards.CG.Shading
{
    public interface ILightProvider
    {
        IEnumerable<PointLight> GetApplicableLights(GeometryInput input, RayTrace trace);
    }

    }
