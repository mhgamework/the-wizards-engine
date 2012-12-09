using System.Collections.Generic;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.Lighting
{
    public class SimpleLightProvider : ILightProvider
    {
        private PointLight pointLight;

        public SimpleLightProvider()
        {
            pointLight = new PointLight { Position = new Vector3(-5, 7, 10), Radius = 100, Intensity = 10000 };
        }
        public IEnumerable<PointLight> GetApplicableLights(TraceResult input, RayTrace trace)
        {
            yield return pointLight;
        }
    }
}
