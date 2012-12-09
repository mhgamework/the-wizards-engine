using System.Collections.Generic;
using MHGameWork.TheWizards.CG.Cameras;
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


        public PhongShader(ITraceableScene tracer, ICamera cam)
            : this(tracer, cam, new SimpleLightProvider())
        {

        }
        public PhongShader(ITraceableScene tracer, ICamera cam, ILightProvider lightProvider)
        {
            this.tracer = tracer;
            this.cam = cam;
            this.lightProvider = lightProvider;
        }


        public Color4 Shade(GeometryInput f, RayTrace trace)
        {
            //if (f.Clip)
            //    return new Color4(0.2f, 0.2f, 1f);

            Vector3 normal = f.Normal;
            //return new Color4((f.Normal + MathHelper.One) * 0.5f);




            var ret = new Color4();

            ret = AmbientColor.Sample(f);

            foreach (var light in lightProvider.GetApplicableLights(f, trace))
            {
                Color4 lightResult = new Color4();
                for (int i = 0; i < light.NumSamples; i++)
                {
                    //surface-to-light vector
                    Vector3 lightVector = light.SamplePosition() - f.Position;

                    lightResult += calcPhongLight(lightVector, normal, f.Position, Diffuse.Sample(f), Specular.Sample(f), SpecularPower.Sample(f), SpecularIntensity.Sample(f));
                }
                ret += lightResult * (1f / light.NumSamples);


            }

            return ret;

        }

        private Color4 calcPhongLight(Vector3 lightVector, Vector3 normal, Vector3 position, Color4 diffuse, Color4 specular, float specularPower, float specularIntensity)
        {
            TraceResult result;
            tracer.Intersect(new RayTrace(new Ray(position, Vector3.Normalize(lightVector)), 0.0001f, lightVector.Length()) { IsShadowRay = true, FirstHit = true }, out result); // TODO: mat.sqrt
            if (result.IsHit)
            {
                //ret += new Color4(1, 0, 0);
                return new Color4(0, 0, 0);
            }

            //compute attenuation based on distance - linear attenuation
            //float attenuation = MathHelper.Clamp((1.0f - lightVector.Length() / light.Radius), 0, 1);

            //normalize light vector
            lightVector.Normalize();
            //compute diffuse light
            float NdL = MathHelper.Max(0, Vector3.Dot(normal, lightVector));



            Color4 diffuseLight = NdL * diffuse;
            //reflection vector
            Vector3 reflectionVector = Vector3.Normalize(reflect(-lightVector, normal));
            //camera-to-surface vector
            Vector3 directionToCamera = Vector3.Normalize(getCameraPosition() - position);
            //compute specular light
            float specularLight = specularIntensity * (float)System.Math.Pow(MathHelper.Clamp(Vector3.Dot(reflectionVector, directionToCamera), 0, 1), specularPower);
            //return t(saturate(dot(reflectionVector, directionToCamera)));

            //ret += attenuation * light.Intensity * new Vector4((diffuseLight.rgb, specularLight) * shadowTerm;


            return diffuseLight + specularLight * specular;
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
