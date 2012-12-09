﻿using System.Collections.Generic;
using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.Lighting;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;
using MHGameWork.TheWizards.CG.Texturing;

namespace MHGameWork.TheWizards.CG.Shading
{
    public class SpecularShader : IShader
    {
        private ITraceableScene tracer;
        private readonly ICamera cam;
        private readonly ILightProvider lightProvider;

        public IGeometrySampler<float> SpecularPower = 15f;
        public IGeometrySampler<float> SpecularIntensity = 2;
        public IGeometrySampler<Color4> Specular = new Color4(1, 1, 1);
        private DiffuseShader diffuseShader;


        public SpecularShader(ITraceableScene tracer, ICamera cam)
            : this(tracer, cam, new SimpleLightProvider())
        {

        }
        public SpecularShader(ITraceableScene tracer, ICamera cam, ILightProvider lightProvider)
        {
            this.tracer = tracer;
            this.cam = cam;
            this.lightProvider = lightProvider;

            diffuseShader = new DiffuseShader(tracer, lightProvider);
        }


        public Color4 Shade(TraceResult f, RayTrace trace)
        {
            //if (f.Clip)
            //    return new Color4(0.2f, 0.2f, 1f);

            Vector3 normal = f.Normal;
            //return new Color4((f.Normal + MathHelper.One) * 0.5f);


            var ret = new Color4();


            var hit = trace.Ray.Position + f.Distance.Value * trace.Ray.Direction;


            foreach (var light in lightProvider.GetApplicableLights(f, trace))
            {
                Color4 lightResult = new Color4();
                for (int i = 0; i < light.NumSamples; i++)
                {
                    //TODO: shadows
                    //surface-to-light vector
                    Vector3 lightVector = light.SamplePosition() - hit;

                    lightResult += CalculateSpecularLight(lightVector, normal, hit, Specular.Sample(f), SpecularPower.Sample(f), SpecularIntensity.Sample(f));
                }
                ret += lightResult * (1f / light.NumSamples);


            }

            return ret;

        }

        public Color4 CalculateSpecularLight(Vector3 lightVector, Vector3 normal, Vector3 position, Color4 specular, float specularPower, float specularIntensity)
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

            //TODO
            var lightIntensity = specularIntensity;

            //reflection vector
            Vector3 reflectionVector = Vector3.Normalize(reflect(-lightVector, normal));
            //camera-to-surface vector
            Vector3 directionToCamera = Vector3.Normalize(getCameraPosition() - position);
            //compute specular light
            return specular * lightIntensity * (float)System.Math.Pow(MathHelper.Max(0, Vector3.Dot(reflectionVector, directionToCamera)), specularPower);
            //return specular * lightIntensity * (float)System.Math.Pow(MathHelper.Clamp(Vector3.Dot(reflectionVector, directionToCamera), 0, 1), specularPower);
        }

        private Vector3 reflect(Vector3 i, Vector3 n)
        {
            return i - 2 * n * Vector3.Dot(i, n);
        }

        private Vector3 getCameraPosition()
        {
            return cam.Position;
        }
    }
}
