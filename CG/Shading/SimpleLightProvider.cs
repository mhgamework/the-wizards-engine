using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;

namespace MHGameWork.TheWizards.CG.Shading
{
    public class SimpleLightProvider : ILightProvider
    {
        private PointLight pointLight;

        public SimpleLightProvider()
        {
            pointLight = new PointLight { Position = new Vector3(-5, 7, -10), Radius = 100, Intensity = 10000 };
        }
        public IEnumerable<PointLight> GetApplicableLights(GeometryInput input, RayTrace trace)
        {
            yield return pointLight;
        }
    }
}
