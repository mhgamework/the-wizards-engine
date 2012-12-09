using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.Lighting
{
    public class MultipleLightProvider :ILightProvider
    {
        private List<PointLight> lights = new List<PointLight>();

        public MultipleLightProvider()
        {
            
        }
        public void AddLight(PointLight light)
        {
            lights.Add(light);
        }

        public IEnumerable<PointLight> GetApplicableLights(TraceResult input, RayTrace trace)
        {
            return lights;
        }
    }
}
