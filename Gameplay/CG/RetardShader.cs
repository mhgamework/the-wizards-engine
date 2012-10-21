using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerGraphics;
using DirectX11;
using MHGameWork.TheWizards.Raycasting;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    public class RetardShader
    {
        private SceneRaytracer tracer = new SceneRaytracer();

        Color4 ambientColor;

        public Color4 CalculateRayColor(Ray r, float min, float max)
        {
            var result = new RaycastResult();
            var f = tracer.TraceFragment(r, min, max, result);
            if (f == null)
                return new Color4(0.2f, 0.2f, 1f);

            Vector3 normal = f.Normal;
            float specularPower = f.SpecularPower;
            float specularIntensity = f.SpecularIntensity;

            

            var ret = new Color4();

            ret = ambientColor;

            foreach (var light in getLights())
            {
                //surface-to-light vector
                Vector3 lightVector = light.Position - f.Position;
                //compute attenuation based on distance - linear attenuation
                float attenuation = MathHelper.Clamp((1.0f - lightVector.Length() / light.Radius), 0, 1);

                //normalize light vector
                lightVector.Normalize();
                //compute diffuse light
                float NdL = MathHelper.Max(0, Vector3.Dot(normal, lightVector));


                Color4 diffuseLight = NdL * f.Diffuse;
                //reflection vector
                Vector3 reflectionVector = Vector3.Normalize(reflect(-lightVector, normal));
                //camera-to-surface vector
                Vector3 directionToCamera = Vector3.Normalize(getCameraPosition() - f.Position);
                //compute specular light
                float specularLight = specularIntensity * (float)Math.Pow(MathHelper.Clamp(Vector3.Dot(reflectionVector, directionToCamera), 0, 1), specularPower);
                //return t(saturate(dot(reflectionVector, directionToCamera)));

                //ret += attenuation * light.Intensity * new Vector4((diffuseLight.rgb, specularLight) * shadowTerm;

                var lightResult = diffuseLight + specularLight * f.SpecularColor;
                ret += lightResult;
            }

            return ret;

        }

        private Vector3 reflect(Vector3 i, Vector3 n)
        {
            return i - 2 * n * Vector3.Dot(i, n);
        }

        private IEnumerable<PointLight> getLights()
        {
            throw new NotImplementedException();
        }

        private Vector3 getCameraPosition()
        {
            throw new NotImplementedException();
        }
    }
}
