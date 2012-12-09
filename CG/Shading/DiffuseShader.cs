using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.Lighting;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;
using MHGameWork.TheWizards.CG.Texturing;

namespace MHGameWork.TheWizards.CG.Shading
{
    public class DiffuseShader : IShader
    {
        private ITraceableScene tracer;
        private readonly ILightProvider lightProvider;
        public IGeometrySampler<Color4> Diffuse = new Color4(0.8f, 0.8f, 0.8f);

        public DiffuseShader(ITraceableScene tracer, ILightProvider lightProvider)
        {
            this.tracer = tracer;
            this.lightProvider = lightProvider;
        }


        public Color4 Shade(TraceResult f, RayTrace trace)
        {
            Vector3 normal = f.Normal;

            var ret = new Color4();

            var hit = trace.Ray.Position + f.Distance.Value * trace.Ray.Direction;


            foreach (var light in lightProvider.GetApplicableLights(f, trace))
            {
                Color4 lightResult = new Color4();
                for (int i = 0; i < light.NumSamples; i++)
                {
                    //surface-to-light vector
                    Vector3 lightVector = light.SamplePosition() - hit;

                    var rayTrace = new RayTrace(new Ray(hit, Vector3.Normalize(lightVector)), 0.001f, lightVector.Length()) { IsShadowRay = true, FirstHit = true };
                    //rayTrace.End = float.MaxValue;
                    TraceResult result;
                    tracer.Intersect(rayTrace, out result);
                    if (result.IsHit)
                        continue;

                    lightResult += CalculateDiffuseLight(lightVector, normal, Diffuse.Sample(f));
                }
                ret += lightResult * (1f / light.NumSamples);


            }

            return ret;

        }

        public Color4 CalculateDiffuseLight(Vector3 lightVector, Vector3 normal, Color4 diffuse)
        {
            //normalize light vector
            lightVector.Normalize();
            //compute diffuse light
            float NdL = MathHelper.Max(0, Vector3.Dot(normal, lightVector));
            float lightIntensity = 1;
            Color4 diffuseLight = NdL * diffuse * lightIntensity;



            return diffuseLight;
        }

    }
}
