﻿using System.Collections.Generic;
using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.Lighting;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;
using MHGameWork.TheWizards.CG.Texturing;

namespace MHGameWork.TheWizards.CG.Shading
{
    public class PhongShader : IShader
    {
        private ITraceableScene tracer;
        private readonly ICamera cam;
        private readonly ILightProvider lightProvider;

        public IGeometrySampler<float> SpecularPower = 15f;
        public IGeometrySampler<float> SpecularIntensity = 2;
        public IGeometrySampler<Color4> Diffuse = new Color4(0.8f, 0.8f, 0.8f);
        public IGeometrySampler<Color4> Specular = new Color4(1, 1, 1);
        public IGeometrySampler<Color4> AmbientColor = new Color4(0.1f, 0.1f, 0.1f);
        private DiffuseShader diffuseShader;
        private SpecularShader specularShader;


        public PhongShader(ITraceableScene tracer, ICamera cam)
            : this(tracer, cam, new SimpleLightProvider())
        {

        }
        public PhongShader(ITraceableScene tracer, ICamera cam, ILightProvider lightProvider)
        {
            this.tracer = tracer;
            this.cam = cam;
            this.lightProvider = lightProvider;

            diffuseShader = new DiffuseShader(tracer, lightProvider);
            specularShader = new SpecularShader(tracer, cam, lightProvider);
        }


        public Color4 Shade(TraceResult f, RayTrace trace)
        {
            Vector3 normal = f.Normal;

            var ret = new Color4();

            ret = AmbientColor.Sample(f);

            var hit = trace.Ray.Position + f.Distance.Value * trace.Ray.Direction;


            foreach (var light in lightProvider.GetApplicableLights(f, trace))
            {
                Color4 lightResult = new Color4();
                for (int i = 0; i < light.NumSamples; i++)
                {
                    //surface-to-light vector
                    Vector3 lightVector = light.SamplePosition() - hit;

                    lightResult += calcPhongLight(lightVector, normal, hit, Diffuse.Sample(f), Specular.Sample(f), SpecularPower.Sample(f), SpecularIntensity.Sample(f));
                }
                ret += lightResult * (1f / light.NumSamples);


            }

            return ret;

        }

        private Color4 calcPhongLight(Vector3 lightVector, Vector3 normal, Vector3 position, Color4 diffuse, Color4 specular, float specularPower, float specularIntensity)
        {
            TraceResult result;
            var rayTrace = new RayTrace(new Ray(position, Vector3.Normalize(lightVector)), 0.001f, lightVector.Length()) { IsShadowRay = true, FirstHit = true };
            //rayTrace.End = float.MaxValue;
            tracer.Intersect(rayTrace, out result); // TODO: mat.sqrt
            if (result.IsHit)
            {
                //ret += new Color4(1, 0, 0);
                return new Color4(0, 0, 0);
            }

            //compute attenuation based on distance - linear attenuation
            //float attenuation = MathHelper.Clamp((1.0f - lightVector.Length() / light.Radius), 0, 1);

            //normalize light vector
            lightVector.Normalize();

            Color4 diffuseLight = diffuseShader.CalculateDiffuseLight(lightVector, normal, diffuse);
            //compute specular light
            Color4 specularLight = specularShader.CalculateSpecularLight(lightVector, normal, position, specular,
                                                                         specularPower, specularIntensity);

            //return t(saturate(dot(reflectionVector, directionToCamera)));

            //ret += attenuation * light.Intensity * new Vector4((diffuseLight.rgb, specularLight) * shadowTerm;


            return diffuseLight + specularLight * specular;
        }

    }
}
