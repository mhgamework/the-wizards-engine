using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Shading;
using Microsoft.Xna.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    public class PhongShader : IShader
    {
        private ITraceableScene tracer;
        private readonly ICamera cam;

        public float SpecularPower = 15;
        public float SpecularIntensity = 2;
        public Color4 Diffuse = new Color4(0.8f, 0.8f, 0.8f);
        public Color4 SpecularColor = new Color4(1, 1, 1);

        public PhongShader(ITraceableScene tracer, ICamera cam)
        {
            pointLight = new PointLight { Position = new Vector3(-5, 7, -10), Radius = 100, Intensity = 10000 };

            this.tracer = tracer;
            this.cam = cam;
        }

        Color4 ambientColor = new Color4(0.1f, 0.1f, 0.1f);
        private static PointLight pointLight;

        public Color4 Shade(GeometryInput f, RayTrace trace)
        {
            //if (f.Clip)
            //    return new Color4(0.2f, 0.2f, 1f);

            Vector3 normal = f.Normal;
            //return new Color4((f.Normal + MathHelper.One) * 0.5f);
  



            var ret = new Color4();

            ret = ambientColor;

            foreach (var light in getLights())
            {
                Color4 lightResult = new Color4();
                for (int i = 0; i < light.NumSamples; i++)
                {
                    //surface-to-light vector
                    Vector3 lightVector = light.SamplePosition() - f.Position;

                    lightResult += calcPhongLight(lightVector, normal, f.Position);
                }
                ret += lightResult * (1f/light.NumSamples);

              
            }

            return ret;

        }

        private Color4 calcPhongLight(Vector3 lightVector, Vector3 normal, Vector3 position)
        {
            ShadowsSettings.ShadowsMode = true;
            IShadeCommand cmd;
            var hit = tracer.Intersect(new RayTrace(new Ray(position, Vector3.Normalize(lightVector)), 0.0001f, lightVector.Length()), out cmd, false); // TODO: mat.sqrt
            ShadowsSettings.ShadowsMode = false;
            if (hit)
            {
                //ret += new Color4(1, 0, 0);
                return new Color4(0,0,0);
            }

            //compute attenuation based on distance - linear attenuation
            //float attenuation = MathHelper.Clamp((1.0f - lightVector.Length() / light.Radius), 0, 1);

            //normalize light vector
            lightVector.Normalize();
            //compute diffuse light
            float NdL = MathHelper.Max(0, Vector3.Dot(normal, lightVector));



            Color4 diffuseLight = NdL * Diffuse;
            //reflection vector
            Vector3 reflectionVector = Vector3.Normalize(reflect(-lightVector, normal));
            //camera-to-surface vector
            Vector3 directionToCamera = Vector3.Normalize(getCameraPosition() - position);
            //compute specular light
            float specularLight = SpecularIntensity * (float)System.Math.Pow(MathHelper.Clamp(Vector3.Dot(reflectionVector, directionToCamera), 0, 1), SpecularPower);
            //return t(saturate(dot(reflectionVector, directionToCamera)));

            //ret += attenuation * light.Intensity * new Vector4((diffuseLight.rgb, specularLight) * shadowTerm;


            return diffuseLight + specularLight * SpecularColor;
        }

        private Vector3 reflect(Vector3 i, Vector3 n)
        {
            return i - 2 * n * Vector3.Dot(i, n);
        }

        private IEnumerable<PointLight> getLights()
        {
            var ret = new List<PointLight> { pointLight };
            return ret;
        }

        private Vector3 getCameraPosition()
        {
            return cam.Position;
        }
    }
}
